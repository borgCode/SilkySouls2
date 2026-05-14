using System.Collections.Generic;
using System.Numerics;
using SilkySouls2.enums;
using SilkySouls2.Interfaces;
using SilkySouls2.Memory;
using SilkySouls2.Models.Target;
using SilkySouls2.Utilities;
using static SilkySouls2.Memory.Offsets;

namespace SilkySouls2.Services
{
    public class TargetService(IMemoryService memoryService, HookManager hookManager, IChrCtrlService chrCtrlService)
        : ITargetService
    {
        private readonly List<nint> _disabledEntities = new();
        private bool _disableTargetHookInstalled;

        public void ToggleTargetHook(bool isEnabled)
        {
            var saveTargetPtrCode = CustomCodeOffsets.Base + CustomCodeOffsets.SaveLockedTarget;

            if (!isEnabled)
            {
                hookManager.UninstallHook(saveTargetPtrCode);
                return;
            }

            var saveTargetHook = Hooks.LockedTarget;
            var savedTargetPtr = CustomCodeOffsets.Base + CustomCodeOffsets.LockedTargetPtr;
            if (PatchManager.IsScholar())
            {
                InstallScholarTargetHook(saveTargetPtrCode, saveTargetHook, savedTargetPtr);
            }
            else
            {
                InstallVanillaTargetHook(saveTargetPtrCode, saveTargetHook, savedTargetPtr);
            }
        }

        private void InstallScholarTargetHook(nint saveTargetPtrCode, nint saveTargetHook, nint savedTargetPtr)
        {
            var saveTargetPtrBytes = AsmLoader.GetAsmBytes(AsmScript.SaveTargetPtr64);
            AsmHelper.WriteRelativeOffset(saveTargetPtrBytes, saveTargetPtrCode, savedTargetPtr, 7, 0x3);
            AsmHelper.WriteRelativeOffset(saveTargetPtrBytes, saveTargetPtrCode + 0xE, saveTargetHook + 0x7, 5, 0xE + 1);

            memoryService.WriteBytes(saveTargetPtrCode, saveTargetPtrBytes);
            hookManager.InstallHook(saveTargetPtrCode, saveTargetHook,
                [0x48, 0x89, 0xBB, 0xC0, 0x00, 0x00, 0x00]);
        }

        private void InstallVanillaTargetHook(nint saveTargetPtrCode, nint saveTargetHook, nint savedTargetPtr)
        {
            var saveTargetPtrBytes = AsmLoader.GetAsmBytes(AsmScript.SaveTargetPtr32);
            AsmHelper.WriteAbsoluteAddress32(saveTargetPtrBytes, savedTargetPtr, 0x6 + 2);
            AsmHelper.WriteRelativeOffset(saveTargetPtrBytes, saveTargetPtrCode + 0xC, saveTargetHook + 0x6, 5, 0xC + 1);
            memoryService.WriteBytes(saveTargetPtrCode, saveTargetPtrBytes);
            hookManager.InstallHook(saveTargetPtrCode, saveTargetHook,
                [0x89, 0xB7, 0xB8, 0x00, 0x00, 0x00]);
        }

        public nint GetTargetChrCtrl() =>
            memoryService.ReadPointer(CustomCodeOffsets.Base + CustomCodeOffsets.LockedTargetPtr);

        public int GetTargetHp() => chrCtrlService.GetHp(GetTargetChrCtrl());
        public int GetTargetMaxHp() => chrCtrlService.GetMaxHp(GetTargetChrCtrl());
        public void SetTargetHp(int health) => chrCtrlService.SetHp(GetTargetChrCtrl(), health);
        public Vector3 GetTargetPos() => chrCtrlService.GetPos(GetTargetChrCtrl());
        public void SetTargetSpeed(float value) => chrCtrlService.SetSpeed(GetTargetChrCtrl(), value);
        public float GetTargetSpeed() => chrCtrlService.GetSpeed(GetTargetChrCtrl());
        
        
        public int GetLastAct() =>
            memoryService.Read<int>(CustomCodeOffsets.Base + (int)CustomCodeOffsets.RepeatAct.AttackId);

        public (bool PoisonToxic, bool Bleed) GetImmunities()
        {
            var targetPtr = memoryService.ReadPointer(CustomCodeOffsets.Base + CustomCodeOffsets.LockedTargetPtr);
            var chrParamPtr = memoryService.ReadPointer(targetPtr + ChrCtrl.ChrParam);

            return (
                memoryService.Read<int>(chrParamPtr + ChrCtrl.ChrParamOffsets.PoisonToxicResist) == 100,
                memoryService.Read<int>(chrParamPtr + ChrCtrl.ChrParamOffsets.BleedResist) == 100
            );
        }

        
        public void ToggleCurrentActHook(bool isEnabled)
        {
            var code = CustomCodeOffsets.Base + (int)CustomCodeOffsets.RepeatAct.Code;

            if (!isEnabled)
            {
                hookManager.UninstallHook(code);
                return;
            }

            var repeatFlagLoc = CustomCodeOffsets.Base + (int)CustomCodeOffsets.RepeatAct.RepeatFlag;
            var attackId = CustomCodeOffsets.Base + (int)CustomCodeOffsets.RepeatAct.AttackId;
            var lockedTargetPtr = CustomCodeOffsets.Base + CustomCodeOffsets.LockedTargetPtr;
            var currentActOrigin = Hooks.SetCurrentAct;

            if (PatchManager.IsScholar())
            {
                InstallScholarRepeatActHook(code, repeatFlagLoc, attackId, lockedTargetPtr, currentActOrigin);
            }
            else
            {
                InstallVanillaRepeatActHook(code, repeatFlagLoc, attackId, lockedTargetPtr, currentActOrigin);
            }
        }
        
        private void InstallScholarRepeatActHook(nint code, nint repeatFlagLoc, nint attackId, nint lockedTargetPtr, nint currentActOrigin)
        {
            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.RepeatAct64);

            AsmHelper.WriteRelativeOffsets(codeBytes, [
                (code + 0x8, lockedTargetPtr, 7, 0x8 + 3),
                (code + 0x28, repeatFlagLoc, 7, 0x28 + 2),
                (code + 0x31, attackId, 6, 0x31 + 2),
                (code + 0x3F, attackId, 6, 0x3F + 2)
            ]);

            memoryService.WriteBytes(code, codeBytes);

            hookManager.InstallHook(code, currentActOrigin, [0x83, 0x89, 0x50, 0x03, 0x00, 0x00, 0x01]);
        }

        private void InstallVanillaRepeatActHook(nint code, nint repeatFlagLoc, nint attackId, nint lockedTargetPtr, nint currentActOrigin)
        {
            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.RepeatAct32);
            AsmHelper.WriteAbsoluteAddresses32(codeBytes, [
                (lockedTargetPtr, 0x1 + 2),
                (repeatFlagLoc, 0x1B + 2),
                (attackId, 0x24 + 1),
                (attackId, 0x35 + 1)
            ]);
            AsmHelper.WriteJumpOffsets(codeBytes, [
                (currentActOrigin, 6, code + 0x30, 0x30 + 1),
                (currentActOrigin, 6, code + 0x41, 0x41 + 1)
            ]);
            memoryService.WriteBytes(code, codeBytes);

            hookManager.InstallHook(code, currentActOrigin, [0x89, 0x81, 0x5C, 0x02, 0x00, 0x00]);
        }

        public void ToggleRepeatAct(bool isRepeatActEnabled) =>
            memoryService.Write(CustomCodeOffsets.Base + (int)CustomCodeOffsets.RepeatAct.RepeatFlag, isRepeatActEnabled);

        
        public void ClearLockedTarget()
        {
            var size = PatchManager.IsScholar() ? 8 : 4;
            memoryService.WriteBytes(CustomCodeOffsets.Base + CustomCodeOffsets.LockedTargetPtr, new byte[size]);
        }

        public void ToggleTargetAi(bool isDisableTargetAiEnabled)
        {
            var chrAi = GetChrAi(GetTargetChrCtrl());
            var arrayLoc = CustomCodeOffsets.Base + (int)CustomCodeOffsets.DisableTargetAi.Array;
            var countLoc = CustomCodeOffsets.Base + (int)CustomCodeOffsets.DisableTargetAi.Count;
            var codeLoc = CustomCodeOffsets.Base + (int)CustomCodeOffsets.DisableTargetAi.Code;
            if (PatchManager.IsScholar())
                Process64BitDisable(isDisableTargetAiEnabled, chrAi, arrayLoc, countLoc, codeLoc);
            else
                Process32BitDisable(isDisableTargetAiEnabled, chrAi, arrayLoc, countLoc, codeLoc);
        }

        private void Process64BitDisable(bool isDisableTargetAiEnabled, nint chrAi, nint arrayLoc, nint countLoc,
            nint codeLoc)
        {
            if (isDisableTargetAiEnabled)
            {
                if (_disabledEntities.Count >= 20) ClearDisableEntities();

                _disabledEntities.Add(chrAi);
                memoryService.Write(arrayLoc + (_disabledEntities.Count - 1) * 8, (long)chrAi);
                memoryService.Write(countLoc, _disabledEntities.Count);
                if (_disableTargetHookInstalled) return;
                var origin = Hooks.DisableTargetAi;

                var bytes = AsmLoader.GetAsmBytes(AsmScript.DisableTargetAi64);

                AsmHelper.WriteRelativeOffsets(bytes, [
                    (codeLoc + 0x6, countLoc, 7, 0x6 + 2),
                    (codeLoc + 0xD, origin + 6, 6, 0xD + 2),
                    (codeLoc + 0x15, countLoc, 6, 0x15 + 2),
                    (codeLoc + 0x39, origin + 6, 5, 0x39 + 1),
                    (codeLoc + 0x40, origin + 9, 5, 0x40 + 1)
                ]);

                AsmHelper.WriteAbsoluteAddress64(bytes, arrayLoc, 0x1B + 2);

                memoryService.WriteBytes(codeLoc, bytes);
                hookManager.InstallHook(codeLoc, origin, [0x48, 0x89, 0xFA, 0x48, 0x89, 0xD9]);
                _disableTargetHookInstalled = true;
            }
            else
            {
                _disabledEntities.Remove(chrAi);
                for (int i = 0; i < _disabledEntities.Count; i++)
                {
                    memoryService.Write(arrayLoc + i * 8, (long)_disabledEntities[i]);
                }

                memoryService.Write(countLoc, _disabledEntities.Count);
                if (_disabledEntities.Count != 0) return;
                hookManager.UninstallHook(codeLoc);
                ClearDisableEntities();
                _disableTargetHookInstalled = false;
            }
        }

        private void Process32BitDisable(bool isDisableTargetAiEnabled, nint chrAi, nint arrayLoc, nint countLoc,
            nint codeLoc)
        {
            if (isDisableTargetAiEnabled)
            {
                if (_disabledEntities.Count >= 20) ClearDisableEntities();
                _disabledEntities.Add(chrAi);
                memoryService.Write(arrayLoc + (_disabledEntities.Count - 1) * 4, (int)chrAi);
                memoryService.Write(countLoc, _disabledEntities.Count);
                if (_disableTargetHookInstalled) return;
                var origin = Hooks.DisableTargetAi;
                var bytes = AsmLoader.GetAsmBytes(AsmScript.DisableTargetAi32);
                AsmHelper.WriteAbsoluteAddresses32(bytes, [
                    (countLoc, 0x5 + 2),
                    (countLoc, 0x14 + 2),
                    (arrayLoc, 0x1A + 1)
                ]);

                AsmHelper.WriteRelativeOffsets(bytes, [
                    (codeLoc + 0xC, origin + 5, 6, 0xC + 2),
                    (codeLoc + 0x30, origin + 5, 5, 0x30 + 1),
                    (codeLoc + 0x37, origin + 0xA, 5, 0x37 + 1)
                ]);

                memoryService.WriteBytes(codeLoc, bytes);
                hookManager.InstallHook(codeLoc, origin, [0x8B, 0x06, 0x8B, 0x50, 0x1C]);
                _disableTargetHookInstalled = true;
            }
            else
            {
                _disabledEntities.Remove(chrAi);
                for (int i = 0; i < _disabledEntities.Count; i++)
                {
                    memoryService.Write(arrayLoc + i * 4, (int)_disabledEntities[i]);
                }

                memoryService.Write(countLoc, _disabledEntities.Count);
                if (_disabledEntities.Count != 0) return;
                hookManager.UninstallHook(codeLoc);
                ClearDisableEntities();
                _disableTargetHookInstalled = false;
            }
        }

        public bool IsAiDisabled(nint targetId) => _disabledEntities.Contains(GetChrAi(targetId));

        private nint GetChrAi(nint targetId)
        {
            var operatorPtr = memoryService.ReadPointer(targetId + ChrCtrl.Operator);
            var chrAiManPtr = memoryService.ReadPointer(operatorPtr + ChrCtrl.OperatorOffsets.ChrAiManipulator);
            return memoryService.ReadPointer(chrAiManPtr + ChrCtrl.ChrAiManipulatorOffsets.ChrAi);
        }

        public void ClearDisableEntities()
        {
            memoryService.WriteBytes(CustomCodeOffsets.Base + (int)CustomCodeOffsets.DisableTargetAi.Array,
                new byte[0x100]);
            memoryService.Write(CustomCodeOffsets.Base + (int)CustomCodeOffsets.DisableTargetAi.Count, 0);
            _disabledEntities.Clear();
        }

        public bool IsLightPoiseImmune()
        {
            var targetPtr = memoryService.ReadPointer(CustomCodeOffsets.Base + CustomCodeOffsets.LockedTargetPtr);
            var poiseStruct = memoryService.ReadPointer(targetPtr + ChrCtrl.PoiseImmunityPtr);

            return memoryService.Read<byte>(poiseStruct + ChrCtrl.PoiseStuff.LightStaggerImmuneFlag) == 1;
        }

        public int GetChrParam(int chrParamOffset)
        {
            var targetPtr = memoryService.ReadPointer(CustomCodeOffsets.Base + CustomCodeOffsets.LockedTargetPtr);
            var chrParamBase = memoryService.ReadPointer(targetPtr + ChrCtrl.ChrParam);

            return memoryService.Read<int>(chrParamBase + chrParamOffset);
        }

        public float GetChrCommonParam(int chrCommonOffset)
        {
            var targetPtr = memoryService.ReadPointer(CustomCodeOffsets.Base + CustomCodeOffsets.LockedTargetPtr);
            var chrParamBase = memoryService.ReadPointer(targetPtr + ChrCtrl.ChrCommon);

            return memoryService.Read<float>(chrParamBase + chrCommonOffset);
        }

        public TargetState GetTargetState()
        {
            int memStart = ChrCtrl.Hp;
            int blockSize = (ChrCtrl.Speed + 4) - ChrCtrl.Hp;
            var ptr = GetTargetChrCtrl() + memStart;
            var block = new MemoryBlock(memoryService.ReadBytes(ptr, blockSize));

            return new TargetState
            {
                CurrentHp = block.Get<int>(ChrCtrl.Hp - memStart),
                MaxHp = block.Get<int>(ChrCtrl.MaxHp - memStart),
                CurrentHeavyPoise = block.Get<float>(ChrCtrl.HeavyPoiseCurrent - memStart),
                CurrentLightPoise = block.Get<float>(ChrCtrl.LightPoiseCurrent - memStart),
                CurrentPoison = block.Get<float>(ChrCtrl.PoisonCurrent - memStart),
                CurrentToxic = block.Get<float>(ChrCtrl.ToxicCurrent - memStart),
                CurrentBleed = block.Get<float>(ChrCtrl.BleedCurrent - memStart),
                Speed = block.Get<float>(ChrCtrl.Speed - memStart),
            };
        }

        public TargetMaxValues GetMaxValues()
        {
            int memStart = ChrCtrl.HeavyPoiseMax;
            int blockSize = (ChrCtrl.LightPoiseMax + 4) - ChrCtrl.HeavyPoiseMax;
            var ptr = GetTargetChrCtrl() + memStart;
            var block = new MemoryBlock(memoryService.ReadBytes(ptr, blockSize));

            return new TargetMaxValues
            {
                MaxHeavyPoise = block.Get<float>(ChrCtrl.HeavyPoiseMax - memStart),
                MaxLightPoise = block.Get<float>(ChrCtrl.LightPoiseMax - memStart),
                PoisonMax = block.Get<float>(ChrCtrl.PoisonMax - memStart),
                ToxicMax = block.Get<float>(ChrCtrl.ToxicMax - memStart),
                BleedMax = block.Get<float>(ChrCtrl.BleedMax - memStart),
            };
        }

        public void ToggleDistHook(bool isEnabled)
        {
            var code = CustomCodeOffsets.Base + CustomCodeOffsets.GetDistCode;
            if (isEnabled)
            {
                if (PatchManager.IsScholar()) InstallScholarGetDist(code);
                else InstallVanillaGetDist(code);
            }
            else
            {
                hookManager.UninstallHook(code);
            }
        }

        private void InstallScholarGetDist(nint code)
        {
            var bytes = AsmLoader.GetAsmBytes(AsmScript.GetDist64);
            var savedDist = CustomCodeOffsets.Base + CustomCodeOffsets.TargetDist;
            var lockedTarget = CustomCodeOffsets.Base + CustomCodeOffsets.LockedTargetPtr;
            AsmHelper.WriteRelativeOffsets(bytes, [
            (code + 0x4, lockedTarget, 7, 0x4 + 3),
            (code + 0x11, GameManagerImp.Base, 7, 0x11 + 3),
            (code + 0x6D, Functions.ResolveTargetCtrlFromHandle, 5, 0x6D + 1),
            (code + 0x92, savedDist, 8, 0x92 + 4),
            (code + 0xBA, Hooks.PreAiEzState + 7, 5, 0xBA + 1)
            ]);
            
            memoryService.WriteBytes(code, bytes);
            hookManager.InstallHook(code, Hooks.PreAiEzState, [0x48, 0x8B, 0x4B, 0x60, 0x0F, 0x28, 0xCE]);
        }

        private void InstallVanillaGetDist(nint code)
        {
            var bytes = AsmLoader.GetAsmBytes(AsmScript.GetDist32);
            var savedDist = CustomCodeOffsets.Base + CustomCodeOffsets.TargetDist;
            var lockedTarget = CustomCodeOffsets.Base + CustomCodeOffsets.LockedTargetPtr;
            
            AsmHelper.WriteAbsoluteAddresses32(bytes, [
            (lockedTarget, 0x3 + 2),
            (GameManagerImp.Base, 0xB + 1),
            (savedDist, 0x49 + 2)
            ]);
            
            AsmHelper.WriteRelativeOffset(bytes, code + 0x5A, Hooks.PreAiEzState + 5, 5, 0x5A + 1);
            
            memoryService.WriteBytes(code, bytes);
            hookManager.InstallHook(code, Hooks.PreAiEzState, [0x8B, 0x4E, 0x3C, 0x8B, 0x11]);
        }

        public float GetDist() => memoryService.Read<float>(CustomCodeOffsets.Base + CustomCodeOffsets.TargetDist);
    }
}