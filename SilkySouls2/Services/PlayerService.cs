using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SilkySouls2.Memory;
using SilkySouls2.Utilities;
using static SilkySouls2.Memory.Offsets;

namespace SilkySouls2.Services
{
    public class PlayerService
    {
        private readonly MemoryIo _memoryIo;
        private readonly HookManager _hookManager;
        private readonly NopManager _nopManager;


        public PlayerService(MemoryIo memoryIo, HookManager hookManager, NopManager nopManager)
        {
            _memoryIo = memoryIo;
            _hookManager = hookManager;
            _nopManager = nopManager;
        }

        public int GetHp() =>
            _memoryIo.ReadInt32(GetPlayerCtrlField(GameManagerImp.ChrCtrlOffsets.Hp));

        public int GetMaxHp() =>
            _memoryIo.ReadInt32(GetPlayerCtrlField(GameManagerImp.ChrCtrlOffsets.MaxHp));

        public void SetHp(int hp) =>
            _memoryIo.WriteInt32(GetPlayerCtrlField(GameManagerImp.ChrCtrlOffsets.Hp), hp);


        public void SetFullHp()
        {
            var full = _memoryIo.ReadInt32(GetPlayerCtrlField(GameManagerImp.ChrCtrlOffsets.FullHpWithHollowing));
            _memoryIo.WriteInt32(GetPlayerCtrlField(GameManagerImp.ChrCtrlOffsets.Hp), full);
        }

        public void SetRtsr()
        {
            var full = _memoryIo.ReadInt32(GetPlayerCtrlField(GameManagerImp.ChrCtrlOffsets.FullHpWithHollowing));
            _memoryIo.WriteInt32(GetPlayerCtrlField(GameManagerImp.ChrCtrlOffsets.Hp), (full * 30) / 100);
        }

        public int GetSp() =>
            _memoryIo.ReadInt32(GetPlayerCtrlField(GameManagerImp.ChrCtrlOffsets.Stamina));

        public void SetSp(int sp) =>
            _memoryIo.WriteInt32(GetPlayerCtrlField(GameManagerImp.ChrCtrlOffsets.Stamina), sp);

        private IntPtr GetPlayerCtrlField(int fieldOffset) =>
            _memoryIo.FollowPointers(GameManagerImp.Base, new[] { GameManagerImp.Offsets.PlayerCtrl, fieldOffset },
                false);


        public void ToggleNoDeath(bool isNoDeathEnabled) =>
            _memoryIo.WriteInt32(GetPlayerCtrlField(GameManagerImp.ChrCtrlOffsets.MinHp),
                isNoDeathEnabled ? 1 : -99999);

        public void ToggleNoDamage(bool isNoDamageEnabled)
        {
            var hookLoc = Hooks.HpWrite;
            var code = CodeCaveOffsets.Base + CodeCaveOffsets.PlayerNoDamage;

            if (isNoDamageEnabled)
            {
                var codeBytes = AsmLoader.GetAsmBytes("PlayerNoDamage");
                var bytes = BitConverter.GetBytes(GameManagerImp.Base.ToInt64());
                Array.Copy(bytes, 0, codeBytes, 0x1 + 2, 8);
                bytes = AsmHelper.GetJmpOriginOffsetBytes(hookLoc, 6, code + 0x2C);
                Array.Copy(bytes, 0, codeBytes, 0x27 + 1, 4);
                _memoryIo.WriteBytes(code, codeBytes);

                _hookManager.InstallHook(code.ToInt64(), hookLoc, new byte[] { 0x89, 0x83, 0x68, 0x01, 0x00, 0x00 });
            }
            else
            {
                _hookManager.UninstallHook(code.ToInt64());
            }
        }


        public void ToggleInfiniteStamina(bool isInfiniteStaminaEnabled) =>
            _memoryIo.WriteByte(Patches.InfiniteStam + 1, isInfiniteStaminaEnabled ? 0x82 : 0x83);

        public int GetPlayerStat(int statOffset) => _memoryIo.ReadUInt8(GetStatPtr(statOffset));

        private IntPtr GetStatPtr(int statOffset)
        {
            return _memoryIo.FollowPointers(GameManagerImp.Base, new[]
            {
                GameManagerImp.Offsets.PlayerCtrl,
                GameManagerImp.ChrCtrlOffsets.StatsPtr,
                statOffset
            }, false);
        }

        public void SetPlayerStat(int statOffset, byte val)
        {
            var currentStatVal = GetPlayerStat(statOffset);
            _memoryIo.WriteUInt8(GetStatPtr(statOffset), val);
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

            var giveSouls = Funcs.GiveSouls;
            var levelLookUp = Funcs.LevelLookUp;
            var levelUp = Funcs.LevelUp;

            var statsEntity = _memoryIo.FollowPointers(GameManagerImp.Base, new[]
            {
                GameManagerImp.Offsets.PlayerCtrl,
                GameManagerImp.ChrCtrlOffsets.StatsPtr
            }, true);

            var currentStatBytes = _memoryIo.ReadBytes(GetStatPtr(GameManagerImp.ChrCtrlOffsets.Stats.Vigor), 22);
            var currentLevel = _memoryIo.ReadInt32(GetStatPtr(GameManagerImp.ChrCtrlOffsets.Stats.SoulLevel));

            if (numOfLevels <= 0)
            {
                _memoryIo.WriteBytes(Patches.NegativeLevel + 1, new byte[] { 0x85 });
            }

            _memoryIo.WriteByte(negativeFlag, numOfLevels <= 0 ? 1 : 0);

            _memoryIo.WriteBytes(buffer, currentStatBytes);
            _memoryIo.WriteByte(numOfLevelsShortLoc, numOfLevels);
            _memoryIo.WriteInt32(numOfLevelsIntLoc, numOfLevels);
            _memoryIo.WriteInt32(currentLevelLoc, currentLevel);
            _memoryIo.WriteInt32(newLevelLoc, currentLevel + numOfLevels);
            var currentSouls = _memoryIo.ReadInt32(GetStatPtr(GameManagerImp.ChrCtrlOffsets.Stats.CurrentSouls));
            _memoryIo.WriteInt32(currentSoulsLoc, currentSouls);

            var bytes = AsmLoader.GetAsmBytes("LevelUp");

            AsmHelper.WriteAbsoluteAddresses(bytes, new[]
            {
                (levelLookUp, 0x18 + 2),
                (statsEntity.ToInt64(), 0x4D + 2),
                (giveSouls, 0x57 + 2),
                (statsEntity.ToInt64(), 0x6C + 2),
                (statsEntity.ToInt64(), 0x90 + 2),
                (levelUp, 0xA8 + 2)
            });

            AsmHelper.WriteRelativeOffsets(bytes, new[]
            {
                (code.ToInt64(), currentLevelLoc.ToInt64(), 6, 0x0 + 2),
                (code.ToInt64() + 0xD, negativeFlag.ToInt64(), 7, 0xD + 2),
                (code.ToInt64() + 0x33, newLevelLoc.ToInt64(), 6, 0x33 + 2),
                (code.ToInt64() + 0x3B, requiredSouls.ToInt64(), 6, 0x3B + 2),
                (code.ToInt64() + 0x41, currentSoulsLoc.ToInt64(), 6, 0x41 + 2),
                (code.ToInt64() + 0x7C, currentSoulsLoc.ToInt64(), 6, 0x7C + 2),
                (code.ToInt64() + 0x82, requiredSouls.ToInt64(), 6, 0x82 + 2),
                (code.ToInt64() + 0x8A, soulsAfterLevelUp.ToInt64(), 6, 0x8A + 2),
                (code.ToInt64() + 0x9A, buffer.ToInt64(), 7, 0x9A + 3)
            });

            _memoryIo.WriteBytes(code, bytes);
            _memoryIo.RunThreadAndWaitForCompletion(code);
            if (numOfLevels <= 0) _memoryIo.WriteBytes(Patches.NegativeLevel + 1, new byte[] { 0x84 });

            var newSouls = _memoryIo.ReadInt32(GetStatPtr(GameManagerImp.ChrCtrlOffsets.Stats.CurrentSouls));
            GiveSouls(currentSouls - newSouls);
            
        }
        
        public int GetSoulLevel() => _memoryIo.ReadInt32(GetStatPtr(GameManagerImp.ChrCtrlOffsets.Stats.SoulLevel));

        public float GetPlayerSpeed() => _memoryIo.ReadFloat(GetSpeedPtr());

        public void SetPlayerSpeed(float speed) => _memoryIo.WriteFloat(GetSpeedPtr(), speed);

        private IntPtr GetSpeedPtr()
        {
            return _memoryIo.FollowPointers(GameManagerImp.Base, new[]
            {
                GameManagerImp.Offsets.PlayerCtrl,
                GameManagerImp.ChrCtrlOffsets.Speed
            }, false);
        }

        public void ToggleNoGoodsConsume(bool isNoGoodsConsumeEnabled) =>
            _memoryIo.WriteBytes(Patches.InfiniteGoods,
                isNoGoodsConsumeEnabled
                    ? new byte[] { 0x90, 0x90, 0x90, 0x90 }
                    : new byte[] { 0x66, 0x29, 0x73, 0x20 });

        public void ToggleInfiniteCasts(bool isInfiniteCastsEnabled) =>
            _memoryIo.WriteBytes(Patches.InfiniteCasts,
                isInfiniteCastsEnabled
                    ? new byte[] { 0x90, 0x90, 0x90 }
                    : new byte[] { 0x88, 0x4D, 0x20 });

        public void ToggleInfiniteDurability(bool isInfiniteDuraEnabled) =>
            _memoryIo.WriteBytes(Patches.InfiniteDurability,
                isInfiniteDuraEnabled
                    ? new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90 }
                    : new byte[] { 0xF3, 0x0F, 0x11, 0xB4, 0xC3, 0x94, 0x00, 0x00, 0x00 });

        public void SavePos(int index)
        {
            byte[] positionBytes = _memoryIo.ReadBytes(GetPositionPtr(), 0x40);
            if (index == 0)
                _memoryIo.WriteBytes(CodeCaveOffsets.Base + (int)CodeCaveOffsets.SavedPos.Pos1, positionBytes);
            else _memoryIo.WriteBytes(CodeCaveOffsets.Base + (int)CodeCaveOffsets.SavedPos.Pos2, positionBytes);
        }

        public void RestorePos(int index)
        {
            byte[] positionBytes;
            if (index == 0)
                positionBytes = _memoryIo.ReadBytes(CodeCaveOffsets.Base + (int)CodeCaveOffsets.SavedPos.Pos1, 0x40);
            else positionBytes = _memoryIo.ReadBytes(CodeCaveOffsets.Base + (int)CodeCaveOffsets.SavedPos.Pos2, 0x40);
            _memoryIo.WriteBytes(GetPositionPtr(), positionBytes);
        }

        private IntPtr GetPositionPtr() =>
            _memoryIo.FollowPointers(HkHardwareInfo.Base, new[]
            {
                HkHardwareInfo.HkpWorld,
                HkHardwareInfo.HkpChrRigidBodyPtr,
                HkHardwareInfo.HkpChrRigidBody,
                HkHardwareInfo.HkpRigidBodyPtr,
                HkHardwareInfo.HkpRigidBody.PlayerCoords
            }, false);

        public (float x, float y, float z) GetCoords()
        {
            var xyzPtr = _memoryIo.FollowPointers(GameManagerImp.Base, new[]
            {
                GameManagerImp.Offsets.PlayerCtrl,
                GameManagerImp.ChrCtrlOffsets.ChrPhysicsCtrlPtr,
                GameManagerImp.ChrCtrlOffsets.ChrPhysicsCtrl.Xyz
            }, false);

            var coordBytes = _memoryIo.ReadBytes(xyzPtr, 12);
            float x = BitConverter.ToSingle(coordBytes, 0);
            float z = BitConverter.ToSingle(coordBytes, 4);
            float y = BitConverter.ToSingle(coordBytes, 8);
            return (x, y, z);
        }

        public void SetNewGame(int value) => _memoryIo.WriteByte(GetNewGamePtr(), value);

        public int GetNewGame() => _memoryIo.ReadUInt8(GetNewGamePtr());

        private IntPtr GetNewGamePtr() =>
            _memoryIo.FollowPointers(GameManagerImp.Base, new[]
            {
                GameManagerImp.Offsets.GameDataManager,
                GameManagerImp.GameDataManagerOffsets.NewGamePtr,
                GameManagerImp.GameDataManagerOffsets.NewGame
            }, false);

        public void GiveSouls(int souls)
        {
            var codeBytes = AsmLoader.GetAsmBytes("GiveSouls");
            var giveSoulsFunc = Funcs.GiveSouls;
            var statsEntity = _memoryIo.FollowPointers(GameManagerImp.Base, new[]
            {
                GameManagerImp.Offsets.PlayerCtrl,
                GameManagerImp.ChrCtrlOffsets.StatsPtr
            }, true);

            AsmHelper.WriteAbsoluteAddresses(codeBytes, new[]
            {
                (statsEntity.ToInt64(), 0x0 + 2),
                (souls, 0x0A + 2),
                (giveSoulsFunc, 0x18 + 2)
            });
            _memoryIo.AllocateAndExecute(codeBytes);
        }

        public int GetSoulMemory() =>
            _memoryIo.ReadInt32(GetStatPtr(GameManagerImp.ChrCtrlOffsets.Stats.SoulMemory));

        public void RestoreSpellcasts()
        {
            var inventoryBag = _memoryIo.FollowPointers(GameManagerImp.Base, new[]
            {
                GameManagerImp.Offsets.GameDataManager,
                GameManagerImp.GameDataManagerOffsets.InventoryPtr,
                GameManagerImp.GameDataManagerOffsets.Inventory.ItemInventory2BagListForSpells
            }, true);
            var func = Funcs.RestoreSpellcasts;

            var codeBytes = AsmLoader.GetAsmBytes("RestoreSpellcasts");
            AsmHelper.WriteAbsoluteAddresses(codeBytes, new[]
            {
                (inventoryBag.ToInt64(), 0x0 + 2),
                (func, 0x20 + 2)
            });

            _memoryIo.AllocateAndExecute(codeBytes);
        }

        public void ToggleSilent(bool isSilentEnabled)
        {
            if (isSilentEnabled) _nopManager.InstallNop(Patches.Silent.ToInt64(), 5);
            else _nopManager.RestoreNop(Patches.Silent.ToInt64());
        }

        public void ToggleHidden(bool isHiddenEnabled) =>
            _memoryIo.WriteBytes(Patches.Hidden + 1, isHiddenEnabled ? new byte[] { 0x85 } : new byte[] { 0x84 });

        public void ToggleInfinitePoise(bool isInfinitePoiseEnabled)
        {
            var code = CodeCaveOffsets.Base + CodeCaveOffsets.InfinitePoise;

            if (isInfinitePoiseEnabled)
            {
                var origin = Hooks.InfinitePoise;
                var gameMan = GameManagerImp.Base;
                var codeBytes = AsmLoader.GetAsmBytes("InfinitePoise");

                var bytes = BitConverter.GetBytes(gameMan.ToInt64());
                Array.Copy(bytes, 0, codeBytes, 0x1 + 2, 8);
                bytes = AsmHelper.GetJmpOriginOffsetBytes(origin, 6, code + 0x2C);
                Array.Copy(bytes, 0, codeBytes, 0x27 + 1, 4);
                _memoryIo.WriteBytes(code, codeBytes);
                _hookManager.InstallHook(code.ToInt64(), origin, new byte[]
                    { 0x39, 0x9D, 0xEC, 0x05, 0x00, 0x00 });
            }
            else
            {
                _hookManager.UninstallHook(code.ToInt64());
            }
        }

        public void ToggleAutoSetNg7(bool isAutoSetNewGameSevenEnabled) =>
            _memoryIo.WriteByte(Patches.Ng7 + 3, isAutoSetNewGameSevenEnabled ? 9 : 1);

        public void SetSpEffect(GameIds.SpEffects.SpEffectData restoreHumanity)
        {
            var spEffectParams = CodeCaveOffsets.Base + CodeCaveOffsets.SpEffectParams;
            var code = CodeCaveOffsets.Base + CodeCaveOffsets.SpEffectCode;

            var chrSpEffectCtrl = _memoryIo.FollowPointers(GameManagerImp.Base, new[]
            {
                GameManagerImp.Offsets.PlayerCtrl,
                GameManagerImp.ChrCtrlOffsets.ChrSpEffectCtrl
            }, true);

            var setEffectFunc = Funcs.SetSpEffect;

            _memoryIo.WriteInt32(spEffectParams, restoreHumanity.EffectId);
            _memoryIo.WriteInt32(spEffectParams + 0x4, restoreHumanity.Quantity);
            _memoryIo.WriteFloat(spEffectParams + 0x8, restoreHumanity.FloatValue);
            _memoryIo.WriteByte(spEffectParams + 0xC, restoreHumanity.EffectType);
            _memoryIo.WriteByte(spEffectParams + 0xD, restoreHumanity.Param1);
            _memoryIo.WriteByte(spEffectParams + 0xE, restoreHumanity.Param2);
            _memoryIo.WriteByte(spEffectParams + 0xF, restoreHumanity.Param3);

            var codeBytes = AsmLoader.GetAsmBytes("SetSpEffect");
            var bytes = BitConverter.GetBytes(chrSpEffectCtrl.ToInt64());
            Array.Copy(bytes, 0, codeBytes, 0x7 + 2, 8);
            AsmHelper.WriteRelativeOffsets(codeBytes, new[]
            {
                (code.ToInt64(), spEffectParams.ToInt64(), 7, 0x0 + 3),
                (code.ToInt64() + 0x15, setEffectFunc, 5, 0x15 + 1)
            });

            _memoryIo.WriteBytes(code, codeBytes);
            _memoryIo.RunThread(code);
        }

        public void ToggleNoSoulGain(bool isEnabled)
        {
            if (isEnabled) _nopManager.InstallNop(Patches.NoSoulGain.ToInt64(), 5);
            else _nopManager.RestoreNop(Patches.NoSoulGain.ToInt64());
        }

        public void ToggleNoHollowing(bool isEnabled) =>
            _memoryIo.WriteBytes(Patches.NoHollowing,
                isEnabled
                    ? new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90, 0x90 }
                    : new byte[] { 0x88, 0x81, 0xAC, 0x01, 0x00, 0x00 }
            );

        public void ToggleNoSoulLoss(bool isEnabled) =>
            _memoryIo.WriteBytes(Patches.NoSoulLoss,
                isEnabled
                    ? new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90, 0x90 }
                    : new byte[] { 0x89, 0x90, 0xEC, 0x00, 0x00, 0x00 } 
            );
    }
}