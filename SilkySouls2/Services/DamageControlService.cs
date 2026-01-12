using System;
using SilkySouls2.enums;
using SilkySouls2.Interfaces;
using SilkySouls2.Memory;
using SilkySouls2.Utilities;
using static SilkySouls2.Memory.Offsets;

namespace SilkySouls2.Services
{
    public class DamageControlService(IMemoryService memoryService, HookManager hookManager)
    {
        private IntPtr _oneShotFlag;
        private IntPtr _dealNoDamageFlag;
        private IntPtr _freezeTargetFlag;
        private IntPtr _damageControlCode;
        
        public bool IsOneShotEnabled => memoryService.Read<byte>(_oneShotFlag) == 1;
        public bool IsDealNoDamageEnabled => memoryService.Read<byte>(_dealNoDamageFlag) == 1;
        public bool IsTargetHpFrozenEnabled => memoryService.Read<byte>(_freezeTargetFlag) == 1;

        public void WriteDamageControlCode()
        {
            _damageControlCode = CodeCaveOffsets.Base + (int)CodeCaveOffsets.DamageControl.DamageControlCode;
            _oneShotFlag = CodeCaveOffsets.Base + (int)CodeCaveOffsets.DamageControl.OneShotFlag;
            _dealNoDamageFlag = CodeCaveOffsets.Base + (int)CodeCaveOffsets.DamageControl.DealNoDamageFlag;
            _freezeTargetFlag = CodeCaveOffsets.Base + (int)CodeCaveOffsets.DamageControl.FreezeTargetHpFlag;
            
            
            var lockedTarget = CodeCaveOffsets.Base + CodeCaveOffsets.LockedTargetPtr;
            var hookLoc = Hooks.DamageControl;

            if (PatchManager.Current.Edition == GameEdition.Scholar)
            {
                var codeBytes = AsmLoader.GetAsmBytes(AsmScript.DamageControl64);
                
                AsmHelper.WriteRelativeOffsets(codeBytes, new []
                {
                    (_damageControlCode.ToInt64() + 0xD, GameManagerImp.Base.ToInt64(), 7, 0xD + 3),
                    (_damageControlCode.ToInt64() + 0x26, _freezeTargetFlag.ToInt64(), 7, 0x26 + 2),
                    (_damageControlCode.ToInt64() + 0x2F, lockedTarget.ToInt64(), 7, 0x2F + 3),
                    (_damageControlCode.ToInt64() + 0x45, _oneShotFlag.ToInt64(), 7, 0x45 + 2),
                    (_damageControlCode.ToInt64() + 0x58, _dealNoDamageFlag.ToInt64(), 7, 0x58 + 2),
                    (_damageControlCode.ToInt64() + 0x6A, hookLoc + 0x5, 5, 0x6A + 1)
                });
            
                memoryService.WriteBytes(_damageControlCode, codeBytes);
            }
            else
            {
                var codeBytes = AsmLoader.GetAsmBytes(AsmScript.DamageControl32);
                
                AsmHelper.WriteAbsoluteAddresses32(codeBytes, new []
                {
                    (GameManagerImp.Base.ToInt64(), 0xC + 1),
                    (_freezeTargetFlag.ToInt64(), 0x1E + 2),
                    (lockedTarget.ToInt64(), 0x27 + 1),
                    (_oneShotFlag.ToInt64(), 0x3A + 2),
                    (_dealNoDamageFlag.ToInt64(), 0x4C + 2)
                });
                var jmpBytes = AsmHelper.GetJmpOriginOffsetBytes(hookLoc, 5, _damageControlCode + 0x62);
                Array.Copy(jmpBytes, 0, codeBytes, 0x5D + 1, 4);
                memoryService.WriteBytes(_damageControlCode, codeBytes);
            }
        }
        
        public void ToggleOneShot(bool enabled)
        {
            if (enabled)
            {
                EnsureHookInstalled();
                memoryService.Write(_oneShotFlag, (byte)1);
            }
            else
            {
                memoryService.Write(_oneShotFlag, (byte)0);
                CheckAndRemoveHookIfNeeded();
            }
        }
        
        public void ToggleDealNoDamage(bool enabled)
        {
            if (enabled)
            {
                EnsureHookInstalled();
                memoryService.Write(_dealNoDamageFlag, (byte)1);
            }
            else
            {
                memoryService.Write(_dealNoDamageFlag, (byte)0);
                CheckAndRemoveHookIfNeeded();
            }
        }
        
        public void ToggleFreezeTargetHp(bool enabled)
        {
            if (enabled)
            {
                EnsureHookInstalled();
                memoryService.Write(_freezeTargetFlag, (byte)1);
            }
            else
            {
                memoryService.Write(_freezeTargetFlag, (byte)0);
                CheckAndRemoveHookIfNeeded();
            }
        }
        private void EnsureHookInstalled()
        {
            if (hookManager.IsHookInstalled(_damageControlCode.ToInt64())) return;
            hookManager.InstallHook(_damageControlCode.ToInt64(), Hooks.DamageControl,
                PatchManager.Current.Edition == GameEdition.Scholar
                    ? new byte[] { 0x0F, 0x29, 0x74, 0x24, 0x20 }
                    : new byte[] { 0x53, 0x56, 0x8B, 0x75, 0x0C });
        }
        
        private void CheckAndRemoveHookIfNeeded()
        {
            bool allFlagsOff = 
                !IsOneShotEnabled && 
                !IsDealNoDamageEnabled && 
                !IsTargetHpFrozenEnabled;
                
            if (allFlagsOff && hookManager.IsHookInstalled(_damageControlCode.ToInt64()))
            {
                hookManager.UninstallHook(_damageControlCode.ToInt64());
            }
        }
        
    }
}