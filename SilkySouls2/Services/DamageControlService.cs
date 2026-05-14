using System;
using SilkySouls2.enums;
using SilkySouls2.Interfaces;
using SilkySouls2.Memory;
using SilkySouls2.Utilities;
using static SilkySouls2.Memory.Offsets;

namespace SilkySouls2.Services
{
    public class DamageControlService : IDamageControlService
    {
        private readonly IMemoryService _memoryService;
        private readonly HookManager _hookManager;

        private IntPtr _oneShotFlag;
        private IntPtr _dealNoDamageFlag;
        private IntPtr _freezeTargetFlag;
        private IntPtr _damageControlCode;

        public DamageControlService(IMemoryService memoryService, HookManager hookManager, IStateService stateService)
        {
            _memoryService = memoryService;
            _hookManager = hookManager;
            stateService.Subscribe(State.Attached, WriteDamageControlCode);
        }

        public bool IsOneShotEnabled => _memoryService.Read<byte>(_oneShotFlag) == 1;
        public bool IsDealNoDamageEnabled => _memoryService.Read<byte>(_dealNoDamageFlag) == 1;
        public bool IsTargetHpFrozenEnabled => _memoryService.Read<byte>(_freezeTargetFlag) == 1;

        private void WriteDamageControlCode()
        {
            _damageControlCode = CustomCodeOffsets.Base + (int)CustomCodeOffsets.DamageControl.DamageControlCode;
            _oneShotFlag = CustomCodeOffsets.Base + (int)CustomCodeOffsets.DamageControl.OneShotFlag;
            _dealNoDamageFlag = CustomCodeOffsets.Base + (int)CustomCodeOffsets.DamageControl.DealNoDamageFlag;
            _freezeTargetFlag = CustomCodeOffsets.Base + (int)CustomCodeOffsets.DamageControl.FreezeTargetHpFlag;


            var lockedTarget = CustomCodeOffsets.Base + CustomCodeOffsets.LockedTargetPtr;
            var hookLoc = Hooks.DamageControl;

            if (PatchManager.Current.Edition == GameEdition.Scholar)
            {
                var codeBytes = AsmLoader.GetAsmBytes(AsmScript.DamageControl64);

                AsmHelper.WriteRelativeOffsets(codeBytes, [
                    (_damageControlCode + 0xD, GameManagerImp.Base, 7, 0xD + 3),
                    (_damageControlCode + 0x26, _freezeTargetFlag, 7, 0x26 + 2),
                    (_damageControlCode + 0x2F, lockedTarget, 7, 0x2F + 3),
                    (_damageControlCode + 0x45, _oneShotFlag, 7, 0x45 + 2),
                    (_damageControlCode + 0x58, _dealNoDamageFlag, 7, 0x58 + 2),
                    (_damageControlCode + 0x6A, hookLoc + 0x5, 5, 0x6A + 1)
                ]);

                _memoryService.WriteBytes(_damageControlCode, codeBytes);
            }
            else
            {
                var codeBytes = AsmLoader.GetAsmBytes(AsmScript.DamageControl32);

                AsmHelper.WriteAbsoluteAddresses32(codeBytes, [
                    (GameManagerImp.Base, 0xC + 1),
                    (_freezeTargetFlag, 0x1E + 2),
                    (lockedTarget, 0x27 + 1),
                    (_oneShotFlag, 0x3A + 2),
                    (_dealNoDamageFlag, 0x4C + 2)
                ]);
                var jmpBytes = AsmHelper.GetJmpOriginOffsetBytes(hookLoc, 5, _damageControlCode + 0x62);
                Array.Copy(jmpBytes, 0, codeBytes, 0x5D + 1, 4);
                _memoryService.WriteBytes(_damageControlCode, codeBytes);
            }
        }

        public void ToggleOneShot(bool enabled)
        {
            if (enabled)
            {
                EnsureHookInstalled();
                _memoryService.Write(_oneShotFlag, (byte)1);
            }
            else
            {
                _memoryService.Write(_oneShotFlag, (byte)0);
                CheckAndRemoveHookIfNeeded();
            }
        }

        public void ToggleDealNoDamage(bool enabled)
        {
            if (enabled)
            {
                EnsureHookInstalled();
                _memoryService.Write(_dealNoDamageFlag, (byte)1);
            }
            else
            {
                _memoryService.Write(_dealNoDamageFlag, (byte)0);
                CheckAndRemoveHookIfNeeded();
            }
        }

        public void ToggleFreezeTargetHp(bool enabled)
        {
            if (enabled)
            {
                EnsureHookInstalled();
                _memoryService.Write(_freezeTargetFlag, (byte)1);
            }
            else
            {
                _memoryService.Write(_freezeTargetFlag, (byte)0);
                CheckAndRemoveHookIfNeeded();
            }
        }
        private void EnsureHookInstalled()
        {
            if (_hookManager.IsHookInstalled(_damageControlCode)) return;
            _hookManager.InstallHook(_damageControlCode, Hooks.DamageControl,
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

            if (allFlagsOff && _hookManager.IsHookInstalled(_damageControlCode))
            {
                _hookManager.UninstallHook(_damageControlCode);
            }
        }

    }
}
