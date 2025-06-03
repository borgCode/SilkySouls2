using System;
using System.Collections.Generic;
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
            _memoryIo.ReadInt32(GetPlayerCtrlField(GameManagerImp.PlayerCtrlOffsets.Hp));

        public int GetMaxHp() =>
            _memoryIo.ReadInt32(GetPlayerCtrlField(GameManagerImp.PlayerCtrlOffsets.MaxHp));

        public void SetHp(int hp) =>
            _memoryIo.WriteInt32(GetPlayerCtrlField(GameManagerImp.PlayerCtrlOffsets.Hp), hp);
        
        
        public int GetSp() =>
            _memoryIo.ReadInt32(GetPlayerCtrlField(GameManagerImp.PlayerCtrlOffsets.Stamina));

        public void SetSp(int sp)=>
            _memoryIo.WriteInt32(GetPlayerCtrlField(GameManagerImp.PlayerCtrlOffsets.Stamina), sp);

        private IntPtr GetPlayerCtrlField(int fieldOffset) =>
            _memoryIo.FollowPointers(GameManagerImp.Base, new[] { GameManagerImp.Offsets.PlayerCtrl, fieldOffset },
                false);
        
        
        
        //        
        //                public int GetNewGame() =>
        //                    _memoryIo.ReadInt32((IntPtr)_memoryIo.ReadInt64(GameDataMan.Base) + GameDataMan.NewGame);
        //        
        //                public void SetNewGame(int value) =>
        //                    _memoryIo.WriteInt32((IntPtr)_memoryIo.ReadInt64(GameDataMan.Base) + GameDataMan.NewGame, value);
        //        
        //                public int GetPlayerStat(GameDataMan.Stats stat)
        //                {
        //                    var statsBasePtr = (IntPtr) _memoryIo.ReadInt64((IntPtr)_memoryIo.ReadInt64(GameDataMan.Base) + GameDataMan.PlayerGameData);
        //                    return _memoryIo.ReadInt32(statsBasePtr + (int)stat);
        //                }
        //        
        //                public void SetPlayerStat(GameDataMan.Stats stat, int newValue)
        //                {
        //                    var statsBasePtr = (IntPtr) _memoryIo.ReadInt64((IntPtr)_memoryIo.ReadInt64(GameDataMan.Base) + GameDataMan.PlayerGameData);
        //                    int currentVal = _memoryIo.ReadInt32(statsBasePtr + (int)stat);
        //                    if (currentVal == newValue) return;
        //                    
        //                    if (stat == GameDataMan.Stats.Souls) HandleSoulEdit(statsBasePtr, newValue, currentVal);
        //                    else HandleStatEdit(statsBasePtr, stat, newValue, currentVal);
        //                }
        //        
        //                private void HandleSoulEdit(IntPtr statsBasePtr, int newValue, int currentVal)
        //                {
        //                    if (newValue < currentVal)
        //                    {
        //                        _memoryIo.WriteInt32(statsBasePtr + (int) GameDataMan.Stats.Souls, newValue);
        //                        return;
        //                    }
        //        
        //                    int difference = newValue - currentVal;
        //                    int currentTotalSouls = _memoryIo.ReadInt32(statsBasePtr + (int)GameDataMan.Stats.TotalSouls);
        //                    _memoryIo.WriteInt32(statsBasePtr + (int)GameDataMan.Stats.TotalSouls, difference + currentTotalSouls);
        //                    _memoryIo.WriteInt32(statsBasePtr + (int) GameDataMan.Stats.Souls, newValue);
        //                }
        //        
        //                private void HandleStatEdit(IntPtr statsBasePtr, GameDataMan.Stats stat, int newValue, int currentVal)
        //                {
        //                    var validatedStat = newValue;
        //                    if (validatedStat < 1) validatedStat = 1;
        //                    if (validatedStat > 99) validatedStat = 99;
        //                    if (validatedStat == currentVal) return;
        //                    _memoryIo.WriteInt32(statsBasePtr + (int) stat, newValue);
        //                    
        //                    int currentSoulLevel = _memoryIo.ReadInt32(statsBasePtr + (int)GameDataMan.Stats.SoulLevel);
        //                    int newLevel = currentSoulLevel + (validatedStat - currentVal);
        //                    
        //                    _memoryIo.WriteInt32(statsBasePtr + (int)GameDataMan.Stats.SoulLevel, newLevel);
        //                    if (newLevel < currentSoulLevel) return;
        //                    
        //                    int totalSoulsRequired = CalculateTotalSoulsRequired(currentSoulLevel, newLevel);
        //                    int currentTotalSouls = _memoryIo.ReadInt32(statsBasePtr + (int)GameDataMan.Stats.TotalSouls);
        //                    _memoryIo.WriteInt32(statsBasePtr + (int)GameDataMan.Stats.TotalSouls, totalSoulsRequired + currentTotalSouls);
        //                }
        //        
        //                public void GiveSouls()
        //                {
        //                    var statsBasePtr = (IntPtr) _memoryIo.ReadInt64((IntPtr)_memoryIo.ReadInt64(GameDataMan.Base) + GameDataMan.PlayerGameData);
        //                    int currentVal = _memoryIo.ReadInt32(statsBasePtr + (int)GameDataMan.Stats.Souls);
        //                    HandleSoulEdit(statsBasePtr, currentVal + 10000, currentVal);
        //                }
        //        
        //                private int CalculateTotalSoulsRequired(int startLevel, int endLevel)
        //                {
        //                    startLevel = Math.Max(1, startLevel);
        //                    double totalSouls = 0;
        //                    for (int level = startLevel + 1; level <= endLevel; level++)
        //                    {
        //                        if (level <= 11)
        //                        {
        //                            totalSouls += _lowLevelSoulRequirements[level];
        //                        }
        //                        else
        //                        {
        //                            double x = level;
        //                            double levelCost = 0.02 * Math.Pow(x, 3) + 3.06 * Math.Pow(x, 2) + 105.6 * x - 895;
        //                            totalSouls += Math.Round(levelCost);
        //                        }
        //                    }
        //        
        //                    return (int)totalSouls;
        //                }
        //        
        //                public float GetPlayerSpeed() => _memoryIo.ReadFloat(GetPlayerSpeedPtr());
        //        
        //                public void SetPlayerSpeed(float speed) => _memoryIo.WriteFloat(GetPlayerSpeedPtr(), speed);
        //        
        //                private IntPtr GetPlayerSpeedPtr()
        //                {
        //                    return _memoryIo.FollowPointers(WorldChrMan.Base,
        //                        new[]
        //                        {
        //                            WorldChrMan.PlayerIns,
        //                            (int)WorldChrMan.PlayerInsOffsets.Modules,
        //                            (int)WorldChrMan.Modules.ChrBehaviorModule,
        //                            (int)WorldChrMan.ChrBehaviorModule.AnimSpeed
        //                        }, false);
        //                }
        //        
        //        
        //                public void ToggleNoRoll(bool isNoRollEnabled)
        //                {
        //                    if (isNoRollEnabled)
        //                    {
        //                        _memoryIo.WriteByte(Offsets.Patches.NoRoll + 0x6, 0);
        //                        _memoryIo.WriteByte(Offsets.Patches.NoRoll + 0x15, 0);
        //                    }
        //                    else
        //                    {
        //                        _memoryIo.WriteByte(Offsets.Patches.NoRoll + 0x6, 1);
        //                        _memoryIo.WriteByte(Offsets.Patches.NoRoll + 0x15, 1);
        //                    }
        //                }
        //        
        //                public int GetMp() => _memoryIo.ReadInt32(GetChrDataFieldPtr((int)WorldChrMan.ChrDataModule.Mp));
        //                public int GetSp() => _memoryIo.ReadInt32(GetChrDataFieldPtr((int)WorldChrMan.ChrDataModule.Stam));
        //        
        //                public void SetMp(int val) => _memoryIo.WriteInt32(GetChrDataFieldPtr((int)WorldChrMan.ChrDataModule.Mp), val);
        //
        // public void SetSp(int val) => _memoryIo.WriteInt32(GetChrDataFieldPtr((int)WorldChrMan.ChrDataModule.Stam), val);

        public void ToggleNoDeath(bool isNoDeathEnabled) =>
            _memoryIo.WriteInt32(GetPlayerCtrlField(GameManagerImp.PlayerCtrlOffsets.MinHp),
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
                GameManagerImp.PlayerCtrlOffsets.StatsPtr,
                statOffset
            }, false);
        }

        public void SetPlayerStat(int statOffset, byte val)
        {
            var currentStatVal = GetPlayerStat(statOffset);
            _memoryIo.WriteUInt8(GetStatPtr(statOffset), val);
            var numOfLevels = val - currentStatVal;

            if (numOfLevels <= 0)
            {
                
            }
            
            var buffer = CodeCaveOffsets.Base + (int)CodeCaveOffsets.LevelUp.Buffer;
            var code = CodeCaveOffsets.Base + (int)CodeCaveOffsets.LevelUp.Code;
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
                GameManagerImp.PlayerCtrlOffsets.StatsPtr
            }, true);
            
            var currentStatBytes = _memoryIo.ReadBytes(GetStatPtr(GameManagerImp.PlayerCtrlOffsets.Stats.Vig), 22);
            var currentLevel = _memoryIo.ReadInt32(GetStatPtr(GameManagerImp.PlayerCtrlOffsets.Stats.SoulLevel));
            
            
            _memoryIo.WriteBytes(buffer, currentStatBytes);
            _memoryIo.WriteByte(numOfLevelsShortLoc, numOfLevels);
            _memoryIo.WriteInt32(numOfLevelsIntLoc, numOfLevels);
            _memoryIo.WriteInt32(currentLevelLoc, currentLevel);
            _memoryIo.WriteInt32(newLevelLoc, currentLevel + numOfLevels);
            var currentSouls = _memoryIo.ReadInt32(GetStatPtr(GameManagerImp.PlayerCtrlOffsets.Stats.CurrentSouls));
            _memoryIo.WriteInt32(currentSoulsLoc, currentSouls);
            
            var bytes = AsmLoader.GetAsmBytes("LevelUp");
            
            AsmHelper.WriteAbsoluteAddresses(bytes, new []
            {
                (levelLookUp, 0xF + 2),
                (statsEntity.ToInt64(), 0x44 + 2),
                (giveSouls, 0x4E + 2),
                (statsEntity.ToInt64(), 0x63 + 2),
                (statsEntity.ToInt64(), 0x87 + 2),
                (levelUp, 0x9F + 2)
            });
            
            AsmHelper.WriteRelativeOffsets(bytes, new []
            {
                (code.ToInt64(), currentLevelLoc.ToInt64(), 6, 0x0 + 2),
                (code.ToInt64() + 0x2A, newLevelLoc.ToInt64(), 6, 0x2A + 2),
                (code.ToInt64() + 0x32, requiredSouls.ToInt64(), 6, 0x32 + 2),
                (code.ToInt64() + 0x38, currentSoulsLoc.ToInt64(), 6, 0x38 + 2),
                (code.ToInt64() + 0x73, currentSoulsLoc.ToInt64(), 6, 0x73 + 2),
                (code.ToInt64() + 0x79, requiredSouls.ToInt64(), 6, 0x79 +2),
                (code.ToInt64() + 0x81, soulsAfterLevelUp.ToInt64(), 6, 0x81 + 2),
                (code.ToInt64() + 0x91, buffer.ToInt64(), 7, 0x91 + 3)
            });
            
            _memoryIo.WriteBytes(code, bytes);
            _memoryIo.RunThread(code);
        }
        

        public int GetSoulLevel() => _memoryIo.ReadInt32(GetStatPtr(GameManagerImp.PlayerCtrlOffsets.Stats.SoulLevel));

        public float GetPlayerSpeed() => _memoryIo.ReadFloat(GetSpeedPtr());

        public void SetPlayerSpeed(float speed) => _memoryIo.WriteFloat(GetSpeedPtr(), speed);

        private IntPtr GetSpeedPtr()
        {
            return _memoryIo.FollowPointers(GameManagerImp.Base, new[]
            {
                GameManagerImp.Offsets.PlayerCtrl,
                GameManagerImp.PlayerCtrlOffsets.Speed
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

        public void ToggleInfiniteDurability(bool isInfiniteDuraEnabled)=>
            _memoryIo.WriteBytes(Patches.InfiniteDurability,
                isInfiniteDuraEnabled
                    ? new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90 }
                    : new byte[] { 0xF3, 0x0F, 0x11, 0xB4, 0xC3, 0x94, 0x00, 0x00, 0x00 });

        public void SavePos(int index)
        {
            byte[] positionBytes = _memoryIo.ReadBytes(GetPositionPtr(), 0x40);
            if (index == 0) _memoryIo.WriteBytes(CodeCaveOffsets.Base + (int)CodeCaveOffsets.SavedPos.Pos1, positionBytes);
            else _memoryIo.WriteBytes(CodeCaveOffsets.Base + (int)CodeCaveOffsets.SavedPos.Pos2, positionBytes);
        }

        public void RestorePos(int index)
        {
            byte[] positionBytes;
            if (index == 0) positionBytes = _memoryIo.ReadBytes(CodeCaveOffsets.Base + (int)CodeCaveOffsets.SavedPos.Pos1, 0x40);
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
                GameManagerImp.PlayerCtrlOffsets.ChrPhysicsCtrlPtr,
                GameManagerImp.PlayerCtrlOffsets.ChrPhysicsCtrl.Xyz
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
                GameManagerImp.PlayerCtrlOffsets.StatsPtr
            }, true);
            
            AsmHelper.WriteAbsoluteAddresses(codeBytes, new []
            {
                (statsEntity.ToInt64(), 0x0 + 2),
                (souls, 0x0A + 2),
                (giveSoulsFunc, 0x18 + 2)
            });
            _memoryIo.AllocateAndExecute(codeBytes);
        }

        public int GetSoulMemory() => _memoryIo.ReadInt32(GetStatPtr(GameManagerImp.PlayerCtrlOffsets.Stats.SoulMemory));

        public void RestoreSpellcasts()
        {
            var inventoryBag = _memoryIo.FollowPointers(GameManagerImp.Base, new[]
            {
                GameManagerImp.Offsets.GameDataManager,
                GameManagerImp.GameDataManagerOffsets.InventoryPtr,
                GameManagerImp.GameDataManagerOffsets.Inventory.ItemInventory2BagList
            }, true);
            var func = Funcs.RestoreSpellcasts;

            var codeBytes = AsmLoader.GetAsmBytes("RestoreSpellcasts");
            AsmHelper.WriteAbsoluteAddresses(codeBytes, new []
            {
                (inventoryBag.ToInt64(), 0x0 + 2),
                (func, 0x20 + 2)
            });
            
            _memoryIo.AllocateAndExecute(codeBytes);
        }

        public void ToggleSilent(bool isSilentEnabled)
        {
            if (isSilentEnabled) _nopManager.InstallNOP(Patches.Silent.ToInt64(), 5);
            else _nopManager.RestoreNOP(Patches.Silent.ToInt64());
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
    }
}