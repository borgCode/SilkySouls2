using System;
using SilkySouls2.Memory;
using SilkySouls2.Utilities;

namespace SilkySouls2.Services
{
    public class DamageControlService
    {
        private readonly MemoryIo _memoryIo;
        private readonly HookManager _hookManager;
        
        private IntPtr _oneShotFlag;
        private IntPtr _dealNoDamageFlag;
        private IntPtr _freezeTargetFlag;
        private IntPtr _damageControlCode;
        
        public bool IsOneShotEnabled => _memoryIo.ReadUInt8(_oneShotFlag) == 1;
        public bool IsDealNoDamageEnabled => _memoryIo.ReadUInt8(_dealNoDamageFlag) == 1;
        public bool IsTargetHpFrozenEnabled => _memoryIo.ReadUInt8(_freezeTargetFlag) == 1;

        
        public DamageControlService(MemoryIo memoryIo, HookManager hookManager)
        {
            _memoryIo = memoryIo;
            _hookManager = hookManager;
        }
        
        public void WriteDamageControlCode()
        {
            _damageControlCode = CodeCaveOffsets.Base + (int)CodeCaveOffsets.DamageControl.DamageControlCode;
            _oneShotFlag = CodeCaveOffsets.Base + (int)CodeCaveOffsets.DamageControl.OneShotFlag;
            _dealNoDamageFlag = CodeCaveOffsets.Base + (int)CodeCaveOffsets.DamageControl.DealNoDamageFlag;
            _freezeTargetFlag = CodeCaveOffsets.Base + (int)CodeCaveOffsets.DamageControl.FreezeTargetHpFlag;
            
            
            var lockedTarget = CodeCaveOffsets.Base + CodeCaveOffsets.LockedTargetPtr;
            var hookLoc = Offsets.Hooks.DamageControl;
            
            var codeBytes = AsmLoader.GetAsmBytes("DamageControl");
            
            var bytes = BitConverter.GetBytes(Offsets.GameManagerImp.Base.ToInt64());
            Array.Copy(bytes, 0, codeBytes, 0xD + 2, 8);
            AsmHelper.WriteRelativeOffsets(codeBytes, new []
            {
                (_damageControlCode.ToInt64() + 0x32, _freezeTargetFlag.ToInt64(), 7, 0x32 + 2),
                (_damageControlCode.ToInt64() + 0x3B, lockedTarget.ToInt64(), 7, 0x3B + 3),
                (_damageControlCode.ToInt64() + 0x51, _oneShotFlag.ToInt64(), 7, 0x51 + 2),
                (_damageControlCode.ToInt64() + 0x64, _dealNoDamageFlag.ToInt64(), 7, 0x64 + 2),
                (_damageControlCode.ToInt64() + 0x76, hookLoc + 0x5, 5, 0x76 + 1)
            });
            
            _memoryIo.WriteBytes(_damageControlCode, codeBytes);
        }
        
        public void ToggleOneShot(bool enabled)
        {
            if (enabled)
            {
                EnsureHookInstalled();
                _memoryIo.WriteByte(_oneShotFlag, 1);
            }
            else
            {
                _memoryIo.WriteByte(_oneShotFlag, 0);
                CheckAndRemoveHookIfNeeded();
            }
        }
        
        public void ToggleDealNoDamage(bool enabled)
        {
            if (enabled)
            {
                EnsureHookInstalled();
                _memoryIo.WriteByte(_dealNoDamageFlag, 1);
            }
            else
            {
                _memoryIo.WriteByte(_dealNoDamageFlag, 0);
                CheckAndRemoveHookIfNeeded();
            }
        }
        
        public void ToggleFreezeTargetHp(bool enabled)
        {
            if (enabled)
            {
                EnsureHookInstalled();
                _memoryIo.WriteByte(_freezeTargetFlag, 1);
            }
            else
            {
                _memoryIo.WriteByte(_freezeTargetFlag, 0);
                CheckAndRemoveHookIfNeeded();
            }
        }
        private void EnsureHookInstalled()
        {
            if (!_hookManager.IsHookInstalled(_damageControlCode.ToInt64()))
            {
                _hookManager.InstallHook(_damageControlCode.ToInt64(), Offsets.Hooks.DamageControl,
                    new byte[] { 0x0F, 0x29, 0x74, 0x24, 0x20});
            }
        }
        
        private void CheckAndRemoveHookIfNeeded()
        {
            bool allFlagsOff = 
                !IsOneShotEnabled && 
                !IsDealNoDamageEnabled && 
                !IsTargetHpFrozenEnabled;
                
            if (allFlagsOff && _hookManager.IsHookInstalled(_damageControlCode.ToInt64()))
            {
                _hookManager.UninstallHook(_damageControlCode.ToInt64());
            }
        }
        
    }
}