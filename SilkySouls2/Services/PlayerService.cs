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
        IChrCtrlService chrCtrlService,
        IReminderService reminderService)
        : IPlayerService
    {
        public int GetHp() =>
            memoryService.Read<int>(GetPlayerCtrlField(ChrCtrl.Hp));

        public int GetMaxHp() =>
            memoryService.Read<int>(GetPlayerCtrlField(ChrCtrl.FullHpWithHollowing));

        public void SetHp(int hp) =>
            memoryService.Write(GetPlayerCtrlField(ChrCtrl.Hp), hp);

        public void SetFullHp()
        {
            var full = memoryService.Read<int>(GetPlayerCtrlField(ChrCtrl.FullHpWithHollowing));
            memoryService.Write(GetPlayerCtrlField(ChrCtrl.Hp), full);
        }

        public void SetRtsr()
        {
            var full = memoryService.Read<int>(GetPlayerCtrlField(ChrCtrl.FullHpWithHollowing));
            memoryService.Write(GetPlayerCtrlField(ChrCtrl.Hp), (full * 30) / 100 - 1);
        }

        public int GetSp() =>
            memoryService.Read<int>(GetPlayerCtrlField(ChrCtrl.Stamina));

        public void SetSp(int sp) =>
            memoryService.Write(GetPlayerCtrlField(ChrCtrl.Stamina), sp);

        private nint GetPlayerCtrlField(int fieldOffset) =>
            memoryService.FollowPointers(GameManagerImp.Base, [GameManagerImp.PlayerCtrl, fieldOffset],
                false);

        public void ToggleNoDeath(bool isNoDeathEnabled) =>
            memoryService.Write(GetPlayerCtrlField(ChrCtrl.MinHp),
                isNoDeathEnabled ? 1 : -99999);

        public void ToggleNoDamage(bool isNoDamageEnabled)
        {
            var code = CustomCodeOffsets.Base + CustomCodeOffsets.PlayerNoDamage;

            if (!isNoDamageEnabled)
            {
                hookManager.UninstallHook(code);
                return;
            }
            
            if (PatchManager.IsScholar()) InstallScholarNoDamageHook(code);
            else InstallVanillaNoDamageHook(code);
        }

        private void InstallScholarNoDamageHook(nint code)
        {
            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.PlayerNoDamage64);
            AsmHelper.WriteAbsoluteAddress64(codeBytes, GameManagerImp.Base, 0x1 + 2);
            var jmpBytes = AsmHelper.GetJmpOriginOffsetBytes(Hooks.PlayerNoDamage, 6, code + 0x2C);
            Array.Copy(jmpBytes, 0, codeBytes, 0x27 + 1, 4);
            memoryService.WriteBytes(code, codeBytes);

            hookManager.InstallHook(code, Hooks.PlayerNoDamage, [0x89, 0x83, 0x68, 0x01, 0x00, 0x00]);
        }

        private void InstallVanillaNoDamageHook(nint code)
        {
            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.PlayerNoDamage32);
            AsmHelper.WriteAbsoluteAddress32(codeBytes, GameManagerImp.Base, 0x1 + 1);
            var jmpBytes = AsmHelper.GetJmpOriginOffsetBytes(Hooks.PlayerNoDamage, 6, code + 0x1F);
            Array.Copy(jmpBytes, 0, codeBytes, 0x1A + 1, 4);
            memoryService.WriteBytes(code, codeBytes);
            hookManager.InstallHook(code, Hooks.PlayerNoDamage, [0x89, 0x8E, 0xFC, 0x00, 0x00, 0x00]);
        }

        public void ToggleInfiniteStamina(bool isInfiniteStaminaEnabled) =>
            memoryService.Write(Patches.InfiniteStam + 1, isInfiniteStaminaEnabled ? (byte)0x82 : (byte)0x83);

        public int GetPlayerStat(int statOffset) => memoryService.Read<byte>(GetStatPtr(statOffset));

        private nint GetStatPtr(int statOffset)
        {
            return memoryService.FollowPointers(GameManagerImp.Base, [
                GameManagerImp.PlayerCtrl,
                ChrCtrl.StatsPtr,
                statOffset
            ], false);
        }

        public void SetPlayerStat(int statOffset, byte val)
        {
            var currentStatVal = GetPlayerStat(statOffset);
            memoryService.Write(GetStatPtr(statOffset), val);
            var numOfLevels = val - currentStatVal;

            var buffer = CustomCodeOffsets.Base + (int)CustomCodeOffsets.LevelUp.Buffer;
            var code = CustomCodeOffsets.Base + (int)CustomCodeOffsets.LevelUp.Code;
            var negativeFlag = CustomCodeOffsets.Base + (int)CustomCodeOffsets.LevelUp.NegativeFlag;
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

            var statsEntity = memoryService.FollowPointers(GameManagerImp.Base, [
                GameManagerImp.PlayerCtrl,
                ChrCtrl.StatsPtr
            ], true);

            var currentStatBytes = memoryService.ReadBytes(GetStatPtr(ChrCtrl.Stats.Vigor), 22);
            var currentLevel = memoryService.Read<int>(GetStatPtr(ChrCtrl.Stats.SoulLevel));

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
            var currentSouls = memoryService.Read<int>(GetStatPtr(ChrCtrl.Stats.CurrentSouls));
            memoryService.Write(currentSoulsLoc, currentSouls);

            if (PatchManager.IsScholar())
            {
                var bytes = AsmLoader.GetAsmBytes(AsmScript.LevelUp64);

                AsmHelper.WriteAbsoluteAddresses64(bytes, [
                    (levelLookUp, 0x18 + 2),
                    (statsEntity, 0x4D + 2),
                    (giveSouls, 0x57 + 2),
                    (statsEntity, 0x6C + 2),
                    (statsEntity, 0x90 + 2),
                    (levelUp, 0xA8 + 2)
                ]);

                AsmHelper.WriteRelativeOffsets(bytes, [
                    (code, currentLevelLoc, 6, 0x0 + 2),
                    (code + 0xD, negativeFlag, 7, 0xD + 2),
                    (code + 0x33, newLevelLoc, 6, 0x33 + 2),
                    (code + 0x3B, requiredSouls, 6, 0x3B + 2),
                    (code + 0x41, currentSoulsLoc, 6, 0x41 + 2),
                    (code + 0x7C, currentSoulsLoc, 6, 0x7C + 2),
                    (code + 0x82, requiredSouls, 6, 0x82 + 2),
                    (code + 0x8A, soulsAfterLevelUp, 6, 0x8A + 2),
                    (code + 0x9A, buffer, 7, 0x9A + 3)
                ]);

                memoryService.WriteBytes(code, bytes);
            }
            else
            {
                var bytes = AsmLoader.GetAsmBytes(AsmScript.LevelUp32);
                AsmHelper.WriteAbsoluteAddresses32(bytes, [
                    (currentLevelLoc, 2),
                    (negativeFlag, 0xC + 2),
                    (levelLookUp, 0x15 + 1),
                    (newLevelLoc, 0x23 + 2),
                    (requiredSouls, 0x2B + 2),
                    (currentSoulsLoc, 0x31 + 2),
                    (statsEntity, 0x3E + 1),
                    (giveSouls, 0x43 + 1),
                    (statsEntity, 0x4A + 1),
                    (currentSoulsLoc, 0x55 + 2),
                    (requiredSouls, 0x5B + 2),
                    (soulsAfterLevelUp, 0x63 + 2),
                    (buffer, 0x6F + 2),
                    (statsEntity, 0x76 + 1),
                    (levelUp, 0x7B + 1)
                ]);
                memoryService.WriteBytes(code, bytes);
            }
            
            memoryService.RunThreadAndWaitForCompletion(code);
            if (numOfLevels <= 0) memoryService.WriteBytes(Patches.NegativeLevel + 1, [0x84]);

            var newSouls = memoryService.Read<int>(GetStatPtr(ChrCtrl.Stats.CurrentSouls));
            GiveSouls(currentSouls - newSouls);
        }

        public int GetSoulLevel() =>
            memoryService.Read<int>(GetStatPtr(ChrCtrl.Stats.SoulLevel));

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

        private nint GetNewGamePtr() =>
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
                ChrCtrl.StatsPtr
            ], true);

            if (PatchManager.IsScholar()) ScholarGiveSouls(statsEntity, souls, giveSoulsFunc);
            else VanillaGiveSouls(statsEntity, souls, giveSoulsFunc);
        }

        private void ScholarGiveSouls(nint statsEntity, int souls, nint giveSoulsFunc)
        {
            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.GiveSouls64);
            AsmHelper.WriteAbsoluteAddresses64(codeBytes, [
                (statsEntity, 0x0 + 2),
                (souls, 0x0A + 2),
                (giveSoulsFunc, 0x18 + 2)
            ]);
            memoryService.AllocateAndExecute(codeBytes);
        }

        private void VanillaGiveSouls(nint statsEntity, int souls, nint giveSoulsFunc)
        {
            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.GiveSouls32);
            AsmHelper.WriteAbsoluteAddresses32(codeBytes, [
                (souls, 0x0 + 1),
                (statsEntity, 0x06 + 1),
                (giveSoulsFunc, 0xb + 1)
            ]);
            memoryService.AllocateAndExecute(codeBytes);
        }

        public int GetSoulMemory() =>
            memoryService.Read<int>(GetStatPtr(ChrCtrl.Stats.SoulMemory));

        public void RestoreSpellcasts()
        {
            var scholar = PatchManager.IsScholar();

            var chainTail = scholar
                ? GameManagerImp.GameDataManagerOffsets.Inventory.ItemInventory2BagListForSpells
                : GameManagerImp.GameDataManagerOffsets.Inventory.InventoryLists;

            var inventoryBag = memoryService.FollowPointers(GameManagerImp.Base, [
                GameManagerImp.GameDataManager,
                GameManagerImp.GameDataManagerOffsets.InventoryPtr,
                chainTail
            ], true);

            if (scholar) ScholarRestoreSpells(inventoryBag);
            else VanillaRestoreSpells(inventoryBag);
        }

        private void ScholarRestoreSpells(nint inventoryBag)
        {
            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.RestoreSpellcasts64);
            AsmHelper.WriteAbsoluteAddresses64(codeBytes, [
                (inventoryBag, 0x0 + 2),
                (Functions.RestoreSpellcasts, 0x20 + 2)
            ]);
            memoryService.AllocateAndExecute(codeBytes);
        }

        private void VanillaRestoreSpells(nint inventoryBag)
        {
            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.RestoreSpellcasts32);
            AsmHelper.WriteAbsoluteAddresses32(codeBytes, [
                (inventoryBag, 1),
                (Functions.RestoreSpellcasts, 0x1E + 1)
            ]);
            memoryService.AllocateAndExecute(codeBytes);
        }

        public void ToggleSilent(bool isSilentEnabled)
        {
            var scholar = PatchManager.IsScholar();
            var length = scholar ? 5 : 16;

            byte[] bytes;
            if (isSilentEnabled)
            {
                bytes = new byte[length];
                for (var i = 0; i < length; i++) bytes[i] = 0x90;
            }
            else if (scholar)
            {
                bytes = AsmHelper.BuildNearCall(Patches.Silent, Functions.OriginalMakeSound);
            }
            else
            {
                byte[] prefix = [0x51, 0xF3, 0x0F, 0x11, 0x04, 0x24, 0x52, 0x50, 0x53, 0x8B, 0xCF];
                var call = AsmHelper.BuildNearCall(Patches.Silent + 0xB, Functions.OriginalMakeSound);
                bytes = [..prefix, ..call];
            }
            memoryService.WriteBytes(Patches.Silent, bytes);
        }

        public void ToggleHidden(bool isHiddenEnabled) =>
            memoryService.WriteBytes(Patches.Hidden + 1, isHiddenEnabled ? [0x85] : [0x84]);

        public void ToggleInfinitePoise(bool isInfinitePoiseEnabled)
        {
            var code = CustomCodeOffsets.Base + CustomCodeOffsets.InfinitePoise;

            if (!isInfinitePoiseEnabled)
            {
                hookManager.UninstallHook(code);
                return;
            }

            if (PatchManager.IsScholar()) InstallScholarInfinitePoise(code);
            else InstallVanillaInfinitePoise(code);
        }

        private void InstallScholarInfinitePoise(nint code)
        {
            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.InfinitePoise64);
            AsmHelper.WriteAbsoluteAddress64(codeBytes, GameManagerImp.Base, 0x1 + 2);
            var jmpBytes = AsmHelper.GetJmpOriginOffsetBytes(Hooks.InfinitePoise, 6, code + 0x2C);
            Array.Copy(jmpBytes, 0, codeBytes, 0x27 + 1, 4);
            memoryService.WriteBytes(code, codeBytes);
            hookManager.InstallHook(code, Hooks.InfinitePoise, [0x39, 0x9D, 0xEC, 0x05, 0x00, 0x00]);
        }

        private void InstallVanillaInfinitePoise(nint code)
        {
            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.InfinitePoise32);
            AsmHelper.WriteAbsoluteAddress32(codeBytes, GameManagerImp.Base, 0x1 + 1);
            var jmpBytes = AsmHelper.GetJmpOriginOffsetBytes(Hooks.InfinitePoise, 7, code + 0x21);
            Array.Copy(jmpBytes, 0, codeBytes, 0x1C + 1, 4);
            memoryService.WriteBytes(code, codeBytes);
            hookManager.InstallHook(code, Hooks.InfinitePoise, [0x83, 0xBB, 0xEC, 0x05, 0x00, 0x00, 0x00]);
        }

        public void SetSpEffect(SpEffect spEffect)
        {
            var spEffectParams = CustomCodeOffsets.Base + CustomCodeOffsets.SpEffectParams;
            var code = CustomCodeOffsets.Base + CustomCodeOffsets.SpEffectCode;

            var chrSpEffectCtrl = memoryService.FollowPointers(GameManagerImp.Base, [
                GameManagerImp.PlayerCtrl,
                ChrCtrl.ChrSpEffectCtrl
            ], true);
            
            memoryService.WriteBytes(spEffectParams, spEffect.ToBytes());

            if (PatchManager.IsScholar()) ScholarSetSpEffect(code, chrSpEffectCtrl, spEffectParams);
            else VanillaSetSpEffect(code, chrSpEffectCtrl, spEffectParams);

            memoryService.RunThread(code);
        }

        private void ScholarSetSpEffect(nint code, nint chrSpEffectCtrl, nint spEffectParams)
        {
            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.SetSpEffect64);
            AsmHelper.WriteAbsoluteAddress64(codeBytes, chrSpEffectCtrl, 0x7 + 2);
            AsmHelper.WriteRelativeOffsets(codeBytes, [
                (code, spEffectParams, 7, 0x0 + 3),
                (code + 0x15, Functions.SetSpEffect, 5, 0x15 + 1)
            ]);

            memoryService.WriteBytes(code, codeBytes);
        }

        private void VanillaSetSpEffect(nint code, nint chrSpEffectCtrl, nint spEffectParams)
        {
            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.SetSpEffect32);
            AsmHelper.WriteAbsoluteAddress32(codeBytes, spEffectParams, 0x3 + 2);
            AsmHelper.WriteAbsoluteAddress32(codeBytes, chrSpEffectCtrl, 0xA + 1);
            AsmHelper.WriteRelativeOffsets(codeBytes, [
                (code + 0xF, Functions.SetSpEffect, 5, 0xF + 1)
            ]);

            memoryService.WriteBytes(code, codeBytes);
        }

        public void ToggleNoSoulGain(bool isEnabled)
        {
            var bytes = isEnabled
                ? [0x90, 0x90, 0x90, 0x90, 0x90]
                : AsmHelper.BuildNearCall(Patches.NoSoulGain, Functions.OriginalSoulGain);
            memoryService.WriteBytes(Patches.NoSoulGain, bytes);
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

        public void ToggleNoHit(bool isEnabled)
        {
            var flags = memoryService.FollowPointers(GameManagerImp.Base, [
                GameManagerImp.PlayerCtrl,
                ChrCtrl.ChrFlags
            ], true);

            if (isEnabled) reminderService.TrySetReminder();
            memoryService.SetBitValue(flags + ChrCtrl.NoHit.Offset, ChrCtrl.NoHit.Mask, isEnabled);
            memoryService.WriteBytes(Patches.NoHitPatch, PatchDefinitions.NoHit.Get(isEnabled));
        }

        public void BreakWeapon(ChrAsmSlotSelector slotSelector)
        {
            if (PatchManager.IsScholar()) BreakWeaponScholar(slotSelector);
            else BreakWeaponVanilla(slotSelector);
        }

        private void BreakWeaponScholar(ChrAsmSlotSelector slotSelector)
        {
            var bytes = AsmLoader.GetAsmBytes(AsmScript.BreakWeapon64);
            var equipBrokenActionCtrl = memoryService.FollowPointers(GameManagerImp.Base, [
                GameManagerImp.PlayerCtrl,
                ChrCtrl.ChrActionCtrl,
                ChrActionCtrl.ChrEquipBrokenActionCtrl
            ], true);
            
            AsmHelper.WriteAbsoluteAddresses64(bytes, [
            (equipBrokenActionCtrl, 0x4 + 2),
            (Functions.ApplyDurabilityDamage, 0x1F + 2)
            ]);
            
            AsmHelper.WriteImmediateDword(bytes, (int)slotSelector, 0xE + 1);
            memoryService.AllocateAndExecute(bytes);
        }

        private void BreakWeaponVanilla(ChrAsmSlotSelector slotSelector)
        {
            var bytes = AsmLoader.GetAsmBytes(AsmScript.BreakWeapon32);
            var equipBrokenActionCtrl = memoryService.FollowPointers(GameManagerImp.Base, [
                GameManagerImp.PlayerCtrl,
                ChrCtrl.ChrActionCtrl,
                ChrActionCtrl.ChrEquipBrokenActionCtrl
            ], true);
            
            AsmHelper.WriteAbsoluteAddresses32(bytes, [
                (equipBrokenActionCtrl, 0xC + 1),
                (Functions.ApplyDurabilityDamage, 0x11 + 1)
            ]);
            
            AsmHelper.WriteImmediateDword(bytes, (int)slotSelector, 0x7 + 1);
            memoryService.AllocateAndExecute(bytes);
            
        }

        private nint GetPositionPtr() =>
            memoryService.FollowPointers(GameManagerImp.Base,
            [
                GameManagerImp.PxWorld, ..GameManagerImp.PxWorldOffsets.PlayerCoordsChain
            ], false);

        private nint GetPlayerCtrl() => memoryService.FollowPointers(GameManagerImp.Base,
            [GameManagerImp.PlayerCtrl], true);
    }
}