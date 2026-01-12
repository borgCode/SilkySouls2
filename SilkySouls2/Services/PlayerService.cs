using System;
using System.Numerics;
using SilkySouls2.enums;
using SilkySouls2.GameIds;
using SilkySouls2.Interfaces;
using SilkySouls2.Memory;
using SilkySouls2.Utilities;
using static SilkySouls2.Memory.Offsets;

namespace SilkySouls2.Services
{
    public class PlayerService(
        IMemoryService memoryService,
        HookManager hookManager,
        NopManager nopManager,
        IChrCtrlService chrCtrlService)
        : IPlayerService
    {
        public int GetHp() =>
            memoryService.Read<int>(GetPlayerCtrlField(GameManagerImp.ChrCtrlOffsets.Hp));

        public int GetMaxHp() =>
            memoryService.Read<int>(GetPlayerCtrlField(GameManagerImp.ChrCtrlOffsets.FullHpWithHollowing));

        public void SetHp(int hp) =>
            memoryService.Write(GetPlayerCtrlField(GameManagerImp.ChrCtrlOffsets.Hp), hp);

        public void SetFullHp()
        {
            var full = memoryService.Read<int>(GetPlayerCtrlField(GameManagerImp.ChrCtrlOffsets.FullHpWithHollowing));
            memoryService.Write(GetPlayerCtrlField(GameManagerImp.ChrCtrlOffsets.Hp), full);
        }

        public void SetRtsr()
        {
            var full = memoryService.Read<int>(GetPlayerCtrlField(GameManagerImp.ChrCtrlOffsets.FullHpWithHollowing));
            memoryService.Write(GetPlayerCtrlField(GameManagerImp.ChrCtrlOffsets.Hp), (full * 30) / 100 - 1);
        }

        public int GetSp() =>
            memoryService.Read<int>(GetPlayerCtrlField(GameManagerImp.ChrCtrlOffsets.Stamina));

        public void SetSp(int sp) =>
            memoryService.Write(GetPlayerCtrlField(GameManagerImp.ChrCtrlOffsets.Stamina), sp);

        private IntPtr GetPlayerCtrlField(int fieldOffset) =>
            memoryService.FollowPointers(GameManagerImp.Base, [GameManagerImp.PlayerCtrl, fieldOffset],
                false);

        public void ToggleNoDeath(bool isNoDeathEnabled) =>
            memoryService.Write(GetPlayerCtrlField(GameManagerImp.ChrCtrlOffsets.MinHp),
                isNoDeathEnabled ? 1 : -99999);

        public void ToggleNoDamage(bool isNoDamageEnabled)
        {
            var code = CodeCaveOffsets.Base + CodeCaveOffsets.PlayerNoDamage;

            if (!isNoDamageEnabled)
            {
                hookManager.UninstallHook(code.ToInt64());
                return;
            }

            var hookLoc = Hooks.PlayerNoDamage;
            if (PatchManager.IsScholar())
            {
                InstallScholarNoDamageHook(hookLoc, code);
            }
            else
            {
                InstallVanillaNoDamageHook(hookLoc, code);
            }
        }

        private void InstallScholarNoDamageHook(long hookLoc, IntPtr code)
        {
            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.PlayerNoDamage64);
            var bytes = BitConverter.GetBytes(GameManagerImp.Base.ToInt64());
            Array.Copy(bytes, 0, codeBytes, 0x1 + 2, 8);
            bytes = AsmHelper.GetJmpOriginOffsetBytes(hookLoc, 6, code + 0x2C);
            Array.Copy(bytes, 0, codeBytes, 0x27 + 1, 4);
            memoryService.WriteBytes(code, codeBytes);

            hookManager.InstallHook(code.ToInt64(), hookLoc, [0x89, 0x83, 0x68, 0x01, 0x00, 0x00]);
        }

        private void InstallVanillaNoDamageHook(long hookLoc, IntPtr code)
        {
            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.PlayerNoDamage32);
            var bytes = BitConverter.GetBytes(GameManagerImp.Base.ToInt32());
            Array.Copy(bytes, 0, codeBytes, 0x1 + 1, 4);
            bytes = AsmHelper.GetJmpOriginOffsetBytes(hookLoc, 6, code + 0x1F);
            Array.Copy(bytes, 0, codeBytes, 0x1A + 1, 4);
            memoryService.WriteBytes(code, codeBytes);
            hookManager.InstallHook(code.ToInt64(), hookLoc, [0x89, 0x8E, 0xFC, 0x00, 0x00, 0x00]);
        }

        public void ToggleInfiniteStamina(bool isInfiniteStaminaEnabled) =>
            memoryService.Write(Patches.InfiniteStam + 1, isInfiniteStaminaEnabled ? (byte)0x82 : (byte)0x83);

        public int GetPlayerStat(int statOffset) => memoryService.Read<byte>(GetStatPtr(statOffset));

        private IntPtr GetStatPtr(int statOffset)
        {
            return memoryService.FollowPointers(GameManagerImp.Base, [
                GameManagerImp.PlayerCtrl,
                GameManagerImp.ChrCtrlOffsets.StatsPtr,
                statOffset
            ], false);
        }

        public void SetPlayerStat(int statOffset, byte val)
        {
            var currentStatVal = GetPlayerStat(statOffset);
            memoryService.Write(GetStatPtr(statOffset), val);
            var numOfLevels = val - currentStatVal;

            var buffer = CodeCaveOffsets.Base + (int)CodeCaveOffsets.LevelUp.Buffer;
            var code = CodeCaveOffsets.Base + (int)CodeCaveOffsets.LevelUp.Code;
            var negativeFlag = CodeCaveOffsets.Base + (int)CodeCaveOffsets.LevelUp.NegativeFlag;
            var numOfLevelsShortLoc = buffer + 0xE2;
            var numOfLevelsIntLoc = buffer + 0xE8;
            var currentLevelLoc = buffer + 0xEC;
            var newLevelLoc = buffer + 0xF0;
            var currentSoulsLoc = buffer + 0xF4;
            var requiredSouls = buffer + 0xFC;
            var soulsAfterLevelUp = buffer + 0xF8;

            var giveSouls = Functions.GiveSouls;
            var levelLookUp = Functions.LevelLookup;
            var levelUp = Functions.LevelUp;

            var statsEntity = memoryService.FollowPointers(GameManagerImp.Base, new[]
            {
                GameManagerImp.PlayerCtrl,
                GameManagerImp.ChrCtrlOffsets.StatsPtr
            }, true);

            var currentStatBytes = memoryService.ReadBytes(GetStatPtr(GameManagerImp.ChrCtrlOffsets.Stats.Vigor), 22);
            var currentLevel = memoryService.ReadInt32(GetStatPtr(GameManagerImp.ChrCtrlOffsets.Stats.SoulLevel));

            if (numOfLevels <= 0)
            {
                memoryService.WriteBytes(Patches.NegativeLevel + 1, [0x85]);
            }

            memoryService.Write(negativeFlag, numOfLevels <= 0);

            memoryService.WriteBytes(buffer, currentStatBytes);
            memoryService.Write(numOfLevelsShortLoc, (byte)numOfLevels);
            memoryService.Write(numOfLevelsIntLoc, numOfLevels);
            memoryService.Write(currentLevelLoc, currentLevel);
            memoryService.Write(newLevelLoc, currentLevel + numOfLevels);
            var currentSouls = memoryService.Read<int>(GetStatPtr(GameManagerImp.ChrCtrlOffsets.Stats.CurrentSouls));
            memoryService.Write(currentSoulsLoc, currentSouls);

            if (PatchManager.IsScholar())
            {
                var bytes = AsmLoader.GetAsmBytes(AsmScript.LevelUp64);

                AsmHelper.WriteAbsoluteAddresses64(bytes, [
                    (levelLookUp, 0x18 + 2),
                    (statsEntity.ToInt64(), 0x4D + 2),
                    (giveSouls, 0x57 + 2),
                    (statsEntity.ToInt64(), 0x6C + 2),
                    (statsEntity.ToInt64(), 0x90 + 2),
                    (levelUp, 0xA8 + 2)
                ]);

                AsmHelper.WriteRelativeOffsets(bytes, [
                    (code.ToInt64(), currentLevelLoc.ToInt64(), 6, 0x0 + 2),
                    (code.ToInt64() + 0xD, negativeFlag.ToInt64(), 7, 0xD + 2),
                    (code.ToInt64() + 0x33, newLevelLoc.ToInt64(), 6, 0x33 + 2),
                    (code.ToInt64() + 0x3B, requiredSouls.ToInt64(), 6, 0x3B + 2),
                    (code.ToInt64() + 0x41, currentSoulsLoc.ToInt64(), 6, 0x41 + 2),
                    (code.ToInt64() + 0x7C, currentSoulsLoc.ToInt64(), 6, 0x7C + 2),
                    (code.ToInt64() + 0x82, requiredSouls.ToInt64(), 6, 0x82 + 2),
                    (code.ToInt64() + 0x8A, soulsAfterLevelUp.ToInt64(), 6, 0x8A + 2),
                    (code.ToInt64() + 0x9A, buffer.ToInt64(), 7, 0x9A + 3)
                ]);

                memoryService.WriteBytes(code, bytes);
            }
            else
            {
                var bytes = AsmLoader.GetAsmBytes(AsmScript.LevelUp32);
                AsmHelper.WriteAbsoluteAddresses32(bytes, [
                    (currentLevelLoc.ToInt64(), 2),
                    (negativeFlag.ToInt64(), 0xC + 2),
                    (levelLookUp, 0x15 + 1),
                    (newLevelLoc.ToInt64(), 0x23 + 2),
                    (requiredSouls.ToInt64(), 0x2B + 2),
                    (currentSoulsLoc.ToInt64(), 0x31 + 2),
                    (statsEntity.ToInt64(), 0x3E + 1),
                    (giveSouls, 0x43 + 1),
                    (statsEntity.ToInt64(), 0x4A + 1),
                    (currentSoulsLoc.ToInt64(), 0x55 + 2),
                    (requiredSouls.ToInt64(), 0x5B + 2),
                    (soulsAfterLevelUp.ToInt64(), 0x63 + 2),
                    (buffer.ToInt64(), 0x6F + 2),
                    (statsEntity.ToInt64(), 0x76 + 1),
                    (levelUp, 0x7B + 1)
                ]);
                memoryService.WriteBytes(code, bytes);
            }


            memoryService.RunThreadAndWaitForCompletion(code);
            if (numOfLevels <= 0) memoryService.WriteBytes(Patches.NegativeLevel + 1, [0x84]);

            var newSouls = memoryService.Read<int>(GetStatPtr(GameManagerImp.ChrCtrlOffsets.Stats.CurrentSouls));
            GiveSouls(currentSouls - newSouls);
        }

        public int GetSoulLevel() =>
            memoryService.Read<int>(GetStatPtr(GameManagerImp.ChrCtrlOffsets.Stats.SoulLevel));

        public float GetPlayerSpeed() => chrCtrlService.GetSpeed(GetPlayerCtrl());

        public void SetPlayerSpeed(float speed) => chrCtrlService.SetSpeed(GetPlayerCtrl(), speed);

        public void ToggleNoGoodsConsume(bool enabled) =>
            memoryService.WriteBytes(Patches.InfiniteGoods, PatchDefinitions.InfiniteGoods.Get(enabled));

        public void ToggleInfiniteCasts(bool enabled) =>
            memoryService.WriteBytes(Patches.InfiniteCasts, PatchDefinitions.InfiniteCasts.Get(enabled));

        public void ToggleInfiniteDurability(bool enabled) =>
            memoryService.WriteBytes(Patches.InfiniteDurability, PatchDefinitions.InfiniteDurability.Get(enabled));

        private byte[] _savedPos1Bytes;
        private byte[] _savedPos2Bytes;

        public void SavePos(int index)
        {
            byte[] positionBytes = memoryService.ReadBytes(GetPositionPtr(), 0x40);
            if (index == 0) _savedPos1Bytes = positionBytes;
            else _savedPos2Bytes = positionBytes;
        }

        public void RestorePos(int index)
        {
            byte[] positionBytes;
            if (index == 0) positionBytes = _savedPos1Bytes;
            else positionBytes = _savedPos2Bytes;
            memoryService.WriteBytes(GetPositionPtr(), positionBytes);
        }

        public Vector3 GetCoords() => chrCtrlService.GetPos(GetPlayerCtrl());
        
        public void SetNewGame(int value) => memoryService.Write(GetNewGamePtr(), (byte)value);

        public int GetNewGame() => memoryService.Read<byte>(GetNewGamePtr());

        private IntPtr GetNewGamePtr() =>
            memoryService.FollowPointers(GameManagerImp.Base, [
                GameManagerImp.GameDataManager,
                GameManagerImp.GameDataManagerOffsets.NewGamePtr,
                GameManagerImp.GameDataManagerOffsets.NewGame
            ], false);

        public void GiveSouls(int souls)
        {
            var giveSoulsFunc = Functions.GiveSouls;
            var statsEntity = memoryService.FollowPointers(GameManagerImp.Base, [
                GameManagerImp.PlayerCtrl,
                GameManagerImp.ChrCtrlOffsets.StatsPtr
            ], true);

            if (PatchManager.IsScholar())
            {
                ScholarGiveSouls(statsEntity, souls, giveSoulsFunc);
            }
            else
            {
                VanillaGiveSouls(statsEntity, souls, giveSoulsFunc);
            }
        }

        private void ScholarGiveSouls(IntPtr statsEntity, int souls, long giveSoulsFunc)
        {
            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.GiveSouls64);
            AsmHelper.WriteAbsoluteAddresses64(codeBytes, [
                (statsEntity.ToInt64(), 0x0 + 2),
                (souls, 0x0A + 2),
                (giveSoulsFunc, 0x18 + 2)
            ]);
            memoryService.AllocateAndExecute(codeBytes);
        }

        private void VanillaGiveSouls(IntPtr statsEntity, int souls, long giveSoulsFunc)
        {
            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.GiveSouls32);
            AsmHelper.WriteAbsoluteAddresses32(codeBytes, [
                (souls, 0x0 + 1),
                (statsEntity.ToInt64(), 0x06 + 1),
                (giveSoulsFunc, 0xb + 1)
            ]);
            memoryService.AllocateAndExecute(codeBytes);
        }

        public int GetSoulMemory() =>
            memoryService.Read<int>(GetStatPtr(GameManagerImp.ChrCtrlOffsets.Stats.SoulMemory));

        public void RestoreSpellcasts()
        {
            var func = Functions.RestoreSpellcasts;
            if (PatchManager.IsScholar())

                ScholarRestoreSpells(func);

            else
            {
                VanillaRestoreSpells(func);
            }
        }

        private void ScholarRestoreSpells(long func)
        {
            var inventoryBag = memoryService.FollowPointers(GameManagerImp.Base, new[]
            {
                GameManagerImp.GameDataManager,
                GameManagerImp.GameDataManagerOffsets.InventoryPtr,
                GameManagerImp.GameDataManagerOffsets.Inventory.ItemInventory2BagListForSpells
            }, true);

            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.RestoreSpellcasts64);
            AsmHelper.WriteAbsoluteAddresses64(codeBytes, [
                (inventoryBag.ToInt64(), 0x0 + 2),
                (func, 0x20 + 2)
            ]);

            memoryService.AllocateAndExecute(codeBytes);
        }

        private void VanillaRestoreSpells(long func)
        {
            var inventoryBag = memoryService.FollowPointers(GameManagerImp.Base, [
                GameManagerImp.GameDataManager,
                GameManagerImp.GameDataManagerOffsets.InventoryPtr,
                GameManagerImp.GameDataManagerOffsets.Inventory.InventoryLists
            ], true);

            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.RestoreSpellcasts32);
            AsmHelper.WriteAbsoluteAddresses32(codeBytes, [
                (inventoryBag.ToInt64(), 1),
                (func, 0x1E + 1)
            ]);
            memoryService.AllocateAndExecute(codeBytes);
        }

        public void ToggleSilent(bool isSilentEnabled)
        {
            //TODO get rid of nop manager
            if (isSilentEnabled)
            {
                nopManager.InstallNop(Patches.Silent.ToInt64(),
                    PatchManager.Current.Edition == GameEdition.Scholar ? 5 : 16);
            }
            else nopManager.RestoreNop(Patches.Silent.ToInt64());
        }

        public void ToggleHidden(bool isHiddenEnabled) =>
            memoryService.WriteBytes(Patches.Hidden + 1, isHiddenEnabled ? [0x85] : [0x84]);

        public void ToggleInfinitePoise(bool isInfinitePoiseEnabled)
        {
            var code = CodeCaveOffsets.Base + CodeCaveOffsets.InfinitePoise;

            if (!isInfinitePoiseEnabled)
            {
                hookManager.UninstallHook(code.ToInt64());
                return;
            }

            var origin = Hooks.InfinitePoise;
            var gameMan = GameManagerImp.Base;

            if (PatchManager.IsScholar())
            {
                InstallScholarInfinitePoise(code, origin, gameMan);
            }
            else
            {
                InstallVanillaInfinitePoise(code, origin, gameMan);
            }
        }

        private void InstallScholarInfinitePoise(IntPtr code, long origin, IntPtr gameMan)
        {
            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.InfinitePoise64);

            var bytes = BitConverter.GetBytes(gameMan.ToInt64());
            Array.Copy(bytes, 0, codeBytes, 0x1 + 2, 8);
            bytes = AsmHelper.GetJmpOriginOffsetBytes(origin, 6, code + 0x2C);
            Array.Copy(bytes, 0, codeBytes, 0x27 + 1, 4);
            memoryService.WriteBytes(code, codeBytes);
            hookManager.InstallHook(code.ToInt64(), origin, [0x39, 0x9D, 0xEC, 0x05, 0x00, 0x00]);
        }

        private void InstallVanillaInfinitePoise(IntPtr code, long origin, IntPtr gameMan)
        {
            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.InfinitePoise32);
            var bytes = BitConverter.GetBytes(gameMan.ToInt32());
            Array.Copy(bytes, 0, codeBytes, 0x1 + 1, 4);
            bytes = AsmHelper.GetJmpOriginOffsetBytes(origin, 7, code + 0x21);
            Array.Copy(bytes, 0, codeBytes, 0x1C + 1, 4);
            memoryService.WriteBytes(code, codeBytes);
            hookManager.InstallHook(code.ToInt64(), origin, [0x83, 0xBB, 0xEC, 0x05, 0x00, 0x00, 0x00]);
        }

        public void SetSpEffect(SpEffect restoreHumanity)
        {
            var spEffectParams = CodeCaveOffsets.Base + CodeCaveOffsets.SpEffectParams;
            var code = CodeCaveOffsets.Base + CodeCaveOffsets.SpEffectCode;

            var chrSpEffectCtrl = memoryService.FollowPointers(GameManagerImp.Base, [
                GameManagerImp.PlayerCtrl,
                GameManagerImp.ChrCtrlOffsets.ChrSpEffectCtrl
            ], true);

            var setEffectFunc = Functions.SetSpEffect;

            memoryService.Write(spEffectParams, restoreHumanity.EffectId);
            memoryService.Write(spEffectParams + 0x4, restoreHumanity.Quantity);
            memoryService.Write(spEffectParams + 0x8, restoreHumanity.FloatValue);
            memoryService.Write(spEffectParams + 0xC, restoreHumanity.EffectType);
            memoryService.Write(spEffectParams + 0xD, restoreHumanity.Param1);
            memoryService.Write(spEffectParams + 0xE, restoreHumanity.Param2);
            memoryService.Write(spEffectParams + 0xF, restoreHumanity.Param3);

            if (PatchManager.IsScholar())
            {
                ScholarSetSpEffect(code, chrSpEffectCtrl, spEffectParams, setEffectFunc);
            }
            else
            {
                VanillaSetSpEffect(code, chrSpEffectCtrl, spEffectParams, setEffectFunc);
            }

            memoryService.RunThread(code);
        }

        private void ScholarSetSpEffect(IntPtr code, IntPtr chrSpEffectCtrl, IntPtr spEffectParams, long setEffectFunc)
        {
            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.SetSpEffect64);
            var bytes = BitConverter.GetBytes(chrSpEffectCtrl.ToInt64());
            Array.Copy(bytes, 0, codeBytes, 0x7 + 2, 8);
            AsmHelper.WriteRelativeOffsets(codeBytes, new[]
            {
                (code.ToInt64(), spEffectParams.ToInt64(), 7, 0x0 + 3),
                (code.ToInt64() + 0x15, setEffectFunc, 5, 0x15 + 1)
            });

            memoryService.WriteBytes(code, codeBytes);
        }

        private void VanillaSetSpEffect(IntPtr code, IntPtr chrSpEffectCtrl, IntPtr spEffectParams, long setEffectFunc)
        {
            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.SetSpEffect32);

            var bytes = BitConverter.GetBytes(spEffectParams.ToInt32());
            Array.Copy(bytes, 0, codeBytes, 0x3 + 2, 4);
            bytes = BitConverter.GetBytes(chrSpEffectCtrl.ToInt32());
            Array.Copy(bytes, 0, codeBytes, 0xA + 1, 4);

            AsmHelper.WriteRelativeOffsets(codeBytes, [
                (code.ToInt64() + 0xF, setEffectFunc, 5, 0xF + 1)
            ]);

            memoryService.WriteBytes(code, codeBytes);
        }

        public void ToggleNoSoulGain(bool isEnabled)
        {
            if (isEnabled) nopManager.InstallNop(Patches.NoSoulGain.ToInt64(), 5);
            else nopManager.RestoreNop(Patches.NoSoulGain.ToInt64());
        }

        public void ToggleNoHollowing(bool enabled) =>
            memoryService.WriteBytes(Patches.NoHollowing, PatchDefinitions.NoHollowing.Get(enabled));

        public void ToggleNoSoulLoss(bool enabled) =>
            memoryService.WriteBytes(Patches.NoSoulLoss, PatchDefinitions.NoSoulLoss.Get(enabled));

        public void ToggleSoulMemWrite(bool enabled)
        {
            memoryService.WriteBytes(Patches.SoulMemWrite1, PatchDefinitions.SoulMemWrite1.Get(enabled));
            memoryService.WriteBytes(Patches.SoulMemWrite2, PatchDefinitions.SoulMemWrite2.Get(enabled));
        }

        private IntPtr GetPositionPtr() =>
            memoryService.FollowPointers(GameManagerImp.Base,
            [
                GameManagerImp.PxWorld, ..GameManagerImp.PxWorldOffsets.PlayerCoordsChain
            ], false);
        

        private nint GetPlayerCtrl() => memoryService.FollowPointers(GameManagerImp.Base,
            [GameManagerImp.PlayerCtrl], true);
    }
}