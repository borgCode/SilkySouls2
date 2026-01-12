using System;
using System.Collections.Generic;
using System.Numerics;
using SilkySouls2.enums;
using SilkySouls2.Interfaces;
using SilkySouls2.Memory;
using SilkySouls2.Models.Target;
using SilkySouls2.Utilities;
using static SilkySouls2.Memory.Offsets;
using static SilkySouls2.Memory.Offsets.GameManagerImp;

namespace SilkySouls2.Services
{
    public class TargetService(IMemoryService memoryService, HookManager hookManager, IChrCtrlService chrCtrlService)
    {
        private readonly List<long> _disabledEntities = new();
        private bool _disableTargetHookInstalled;

        public void ToggleTargetHook(bool isEnabled)
        {
            var saveTargetPtrCode = CodeCaveOffsets.Base + CodeCaveOffsets.SaveLockedTarget;

            if (!isEnabled)
            {
                hookManager.UninstallHook(saveTargetPtrCode.ToInt64());
                return;
            }

            var saveTargetHook = Hooks.LockedTarget;
            var savedTargetPtr = CodeCaveOffsets.Base + CodeCaveOffsets.LockedTargetPtr;
            if (PatchManager.IsScholar())
            {
                InstallScholarTargetHook(saveTargetPtrCode, saveTargetHook, savedTargetPtr);
            }
            else
            {
                InstallVanillaTargetHook(saveTargetPtrCode, saveTargetHook, savedTargetPtr);
            }
        }

        private void InstallScholarTargetHook(IntPtr saveTargetPtrCode, long saveTargetHook, IntPtr savedTargetPtr)
        {
            byte[] saveTargetPtrBytes = AsmLoader.GetAsmBytes(AsmScript.SaveTargetPtr64);
            byte[] bytes =
                AsmHelper.GetRelOffsetBytes(saveTargetPtrCode.ToInt64(), savedTargetPtr.ToInt64(), 7);
            Array.Copy(bytes, 0, saveTargetPtrBytes, 0x3, 4);
            bytes = AsmHelper.GetRelOffsetBytes(saveTargetPtrCode.ToInt64() + 0xE, saveTargetHook + 0x7, 5);
            Array.Copy(bytes, 0, saveTargetPtrBytes, 0xE + 1, 4);

            memoryService.WriteBytes(saveTargetPtrCode, saveTargetPtrBytes);
            hookManager.InstallHook(saveTargetPtrCode.ToInt64(), saveTargetHook,
                [0x48, 0x89, 0xBB, 0xC0, 0x00, 0x00, 0x00]);
        }

        private void InstallVanillaTargetHook(IntPtr saveTargetPtrCode, long saveTargetHook, IntPtr savedTargetPtr)
        {
            byte[] saveTargetPtrBytes = AsmLoader.GetAsmBytes(AsmScript.SaveTargetPtr32);
            var bytes = BitConverter.GetBytes(savedTargetPtr.ToInt32());
            Array.Copy(bytes, 0, saveTargetPtrBytes, 0x6 + 2, 4);
            bytes = AsmHelper.GetRelOffsetBytes(saveTargetPtrCode.ToInt64() + 0xC, saveTargetHook + 0x6, 5);
            Array.Copy(bytes, 0, saveTargetPtrBytes, 0xC + 1, 4);
            memoryService.WriteBytes(saveTargetPtrCode, saveTargetPtrBytes);
            hookManager.InstallHook(saveTargetPtrCode.ToInt64(), saveTargetHook,
                [0x89, 0xB7, 0xB8, 0x00, 0x00, 0x00]);
        }

        public nint GetTargetChrCtrl()
        {
            return PatchManager.IsScholar()
                ? memoryService.Read<nint>(CodeCaveOffsets.Base + CodeCaveOffsets.LockedTargetPtr)
                : memoryService.Read<int>(CodeCaveOffsets.Base + CodeCaveOffsets.LockedTargetPtr);
        }

        public int GetTargetHp() => chrCtrlService.GetHp(GetTargetChrCtrl());
        public int GetTargetMaxHp() => chrCtrlService.GetMaxHp(GetTargetChrCtrl());
        public void SetTargetHp(int health) => chrCtrlService.SetHp(GetTargetChrCtrl(), health);
        public Vector3 GetTargetPos() => chrCtrlService.GetPos(GetTargetChrCtrl());
        public void SetTargetSpeed(float value) => chrCtrlService.SetSpeed(GetTargetChrCtrl(), value);
        public float GetTargetSpeed() => chrCtrlService.GetSpeed(GetTargetChrCtrl());
        
        
        public int GetLastAct() =>
            memoryService.Read<int>(CodeCaveOffsets.Base + (int)CodeCaveOffsets.RepeatAct.AttackId);

        public (bool PoisonToxic, bool Bleed) GetImmunities()
        {
            var chrParamPtr = PatchManager.IsScholar()
                ? (IntPtr)memoryService.ReadInt64(
                    (IntPtr)memoryService.ReadInt64(CodeCaveOffsets.Base + CodeCaveOffsets.LockedTargetPtr) +
                    ChrCtrlOffsets.ChrParam)
                : (IntPtr)memoryService.ReadInt32(
                    (IntPtr)memoryService.ReadInt32(CodeCaveOffsets.Base + CodeCaveOffsets.LockedTargetPtr) +
                    ChrCtrlOffsets.ChrParam);

            return (
                memoryService.Read<int>(chrParamPtr + ChrCtrlOffsets.ChrParamOffsets.PoisonToxicResist) == 100,
                memoryService.Read<int>(chrParamPtr + ChrCtrlOffsets.ChrParamOffsets.BleedResist) == 100
            );
        }

        
        public void ToggleCurrentActHook(bool isEnabled)
        {
            var code = CodeCaveOffsets.Base + (int)CodeCaveOffsets.RepeatAct.Code;

            if (!isEnabled)
            {
                hookManager.UninstallHook(code.ToInt64());
                return;
            }
            
            var repeatFlagLoc = CodeCaveOffsets.Base + (int)CodeCaveOffsets.RepeatAct.RepeatFlag;
            var attackId = CodeCaveOffsets.Base + (int)CodeCaveOffsets.RepeatAct.AttackId;
            var lockedTargetPtr = CodeCaveOffsets.Base + CodeCaveOffsets.LockedTargetPtr;
            var currectActOrigin = Hooks.SetCurrentAct;

            if (PatchManager.IsScholar())
            {
                InstallScholarRepeatActHook(code, repeatFlagLoc, attackId, lockedTargetPtr, currectActOrigin);
            }
            else
            {
                InstallVanillaRepeatActHook(code, repeatFlagLoc, attackId, lockedTargetPtr, currectActOrigin);
            }
        }
        
        private void InstallScholarRepeatActHook(IntPtr code, IntPtr repeatFlagLoc, IntPtr attackId, IntPtr lockedTargetPtr, long currectActOrigin)
        {
            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.RepeatAct64);

            AsmHelper.WriteRelativeOffsets(codeBytes, [
                (code.ToInt64() + 0x8, lockedTargetPtr.ToInt64(), 7, 0x8 + 3),
                (code.ToInt64() + 0x28, repeatFlagLoc.ToInt64(), 7, 0x28 + 2),
                (code.ToInt64() + 0x31, attackId.ToInt64(), 6, 0x31 + 2),
                (code.ToInt64() + 0x3F, attackId.ToInt64(), 6, 0x3F + 2)
            ]);

            memoryService.WriteBytes(code, codeBytes);

            hookManager.InstallHook(code.ToInt64(), currectActOrigin, [0x83, 0x89, 0x50, 0x03, 0x00, 0x00, 0x01]);
        }
        
        private void InstallVanillaRepeatActHook(IntPtr code, IntPtr repeatFlagLoc, IntPtr attackId, IntPtr lockedTargetPtr, long currectActOrigin)
        {
            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.RepeatAct32);
            AsmHelper.WriteAbsoluteAddresses32(codeBytes, [
                (lockedTargetPtr.ToInt64(), 0x1 + 2),
                (repeatFlagLoc.ToInt64(), 0x1B + 2),
                (attackId.ToInt64(), 0x24 + 1),
                (attackId.ToInt64(), 0x35 + 1)
            ]);
            AsmHelper.WriteJumpOffsets(codeBytes, [
                (currectActOrigin, 6, code + 0x30, 0x30 + 1),
                (currectActOrigin, 6, code + 0x41, 0x41 + 1)
            ]);
            memoryService.WriteBytes(code, codeBytes);

            hookManager.InstallHook(code.ToInt64(), currectActOrigin, [0x89, 0x81, 0x5C, 0x02, 0x00, 0x00]);
        }

        public void ToggleRepeatAct(bool isRepeatActEnabled) =>
            memoryService.Write(CodeCaveOffsets.Base + (int)CodeCaveOffsets.RepeatAct.RepeatFlag, isRepeatActEnabled);

        
        public void ClearLockedTarget()
        {
            if (PatchManager.IsScholar())
            {
                memoryService.WriteBytes(CodeCaveOffsets.Base + CodeCaveOffsets.LockedTargetPtr, new byte[]
                    { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
            }
            else
            {
                memoryService.WriteBytes(CodeCaveOffsets.Base + CodeCaveOffsets.LockedTargetPtr, new byte[]
                    { 0x00, 0x00, 0x00, 0x00 });
            }
        }

        public void ToggleTargetAi(bool isDisableTargetAiEnabled)
        {
            var chrAi = GetChrAi(GetTargetChrCtrl());
            var arrayLoc = CodeCaveOffsets.Base + (int)CodeCaveOffsets.DisableTargetAi.Array;
            var countLoc = CodeCaveOffsets.Base + (int)CodeCaveOffsets.DisableTargetAi.Count;
            var codeLoc = CodeCaveOffsets.Base + (int)CodeCaveOffsets.DisableTargetAi.Code;
            if (PatchManager.IsScholar())
                Process64BitDisable(isDisableTargetAiEnabled, chrAi, arrayLoc, countLoc, codeLoc);
            else
                Process32BitDisable(isDisableTargetAiEnabled, chrAi, arrayLoc, countLoc, codeLoc);
        }

        private void Process64BitDisable(bool isDisableTargetAiEnabled, long chrAi, IntPtr arrayLoc, IntPtr countLoc,
            IntPtr codeLoc)
        {
            if (isDisableTargetAiEnabled)
            {
                if (_disabledEntities.Count >= 20) ClearDisableEntities();

                _disabledEntities.Add(chrAi);
                memoryService.WriteInt64(arrayLoc + (_disabledEntities.Count - 1) * 8, chrAi);
                memoryService.WriteInt32(countLoc, _disabledEntities.Count);
                if (_disableTargetHookInstalled) return;
                var origin = Hooks.DisableTargetAi;

                var bytes = AsmLoader.GetAsmBytes(AsmScript.DisableTargetAi64);

                AsmHelper.WriteRelativeOffsets(bytes, [
                    (codeLoc.ToInt64() + 0x6, countLoc.ToInt64(), 7, 0x6 + 2),
                    (codeLoc.ToInt64() + 0xD, origin + 6, 6, 0xD + 2),
                    (codeLoc.ToInt64() + 0x15, countLoc.ToInt64(), 6, 0x15 + 2),
                    (codeLoc.ToInt64() + 0x39, origin + 6, 5, 0x39 + 1),
                    (codeLoc.ToInt64() + 0x40, origin + 9, 5, 0x40 + 1)
                ]);

                var arrayLocBytes = BitConverter.GetBytes(arrayLoc.ToInt64());
                Array.Copy(arrayLocBytes, 0, bytes, 0x1B + 2, 8);

                memoryService.WriteBytes(codeLoc, bytes);
                hookManager.InstallHook(codeLoc.ToInt64(), origin, [0x48, 0x89, 0xFA, 0x48, 0x89, 0xD9]);
                _disableTargetHookInstalled = true;
            }
            else
            {
                _disabledEntities.Remove(chrAi);
                for (int i = 0; i < _disabledEntities.Count; i++)
                {
                    memoryService.WriteInt64(arrayLoc + i * 8, _disabledEntities[i]);
                }

                memoryService.WriteInt32(countLoc, _disabledEntities.Count);
                if (_disabledEntities.Count != 0) return;
                hookManager.UninstallHook(codeLoc.ToInt64());
                ClearDisableEntities();
                _disableTargetHookInstalled = false;
            }
        }

        private void Process32BitDisable(bool isDisableTargetAiEnabled, long chrAi, IntPtr arrayLoc, IntPtr countLoc,
            IntPtr codeLoc)
        {
            if (isDisableTargetAiEnabled)
            {
                if (_disabledEntities.Count >= 20) ClearDisableEntities();
                _disabledEntities.Add(chrAi);
                memoryService.WriteInt32(arrayLoc + (_disabledEntities.Count - 1) * 4, (int)chrAi);
                memoryService.WriteInt32(countLoc, _disabledEntities.Count);
                if (_disableTargetHookInstalled) return;
                var origin = Hooks.DisableTargetAi;
                var bytes = AsmLoader.GetAsmBytes(AsmScript.DisableTargetAi32);
                AsmHelper.WriteAbsoluteAddresses32(bytes, [
                    (countLoc.ToInt64(), 0x5 + 2),
                    (countLoc.ToInt64(), 0x14 + 2),
                    (arrayLoc.ToInt64(), 0x1A + 1)
                ]);

                AsmHelper.WriteRelativeOffsets(bytes, [
                    (codeLoc.ToInt64() + 0xC, origin + 5, 6, 0xC + 2),
                    (codeLoc.ToInt64() + 0x30, origin + 5, 5, 0x30 + 1),
                    (codeLoc.ToInt64() + 0x37, origin + 0xA, 5, 0x37 + 1)
                ]);

                memoryService.WriteBytes(codeLoc, bytes);
                hookManager.InstallHook(codeLoc.ToInt64(), origin, [0x8B, 0x06, 0x8B, 0x50, 0x1C]);
                _disableTargetHookInstalled = true;
            }
            else
            {
                _disabledEntities.Remove(chrAi);
                for (int i = 0; i < _disabledEntities.Count; i++)
                {
                    memoryService.WriteInt32(arrayLoc + i * 4, (int)_disabledEntities[i]);
                }

                memoryService.WriteInt32(countLoc, _disabledEntities.Count);
                if (_disabledEntities.Count != 0) return;
                hookManager.UninstallHook(codeLoc.ToInt64());
                ClearDisableEntities();
                _disableTargetHookInstalled = false;
            }
        }

        public bool IsAiDisabled(long targetId) => _disabledEntities.Contains(GetChrAi(targetId));

        private long GetChrAi(long targetId)
        {
            if (PatchManager.IsScholar())
            {
                var operatorPtr = memoryService.Read<nint>((IntPtr)(targetId + ChrCtrlOffsets.Operator));
                var chrAiManPtr = memoryService.Read<nint>(operatorPtr + ChrCtrlOffsets.OperatorOffsets.ChrAiManipulator);
                var chrAi = memoryService.Read<nint>(chrAiManPtr + ChrCtrlOffsets.ChrAiManipulatorOffsets.ChrAi);
                return chrAi;
            }
            else
            {
                var operatorPtr = memoryService.Read<int>((IntPtr)(targetId + ChrCtrlOffsets.Operator));
                var chrAiManPtr = memoryService.Read<int>((IntPtr)(operatorPtr + ChrCtrlOffsets.OperatorOffsets.ChrAiManipulator));
                var chrAi = memoryService.Read<int>((IntPtr)(chrAiManPtr + ChrCtrlOffsets.ChrAiManipulatorOffsets.ChrAi));
                return chrAi;
            }
        }

        public void ClearDisableEntities()
        {
            memoryService.WriteBytes(CodeCaveOffsets.Base + (int)CodeCaveOffsets.DisableTargetAi.Array,
                new byte[0x100]);
            memoryService.Write(CodeCaveOffsets.Base + (int)CodeCaveOffsets.DisableTargetAi.Count, 0);
            _disabledEntities.Clear();
        }

        public bool IsLightPoiseImmune()
        {
            var targetPtr = PatchManager.IsScholar()
                ? memoryService.ReadInt64(CodeCaveOffsets.Base + CodeCaveOffsets.LockedTargetPtr)
                : memoryService.ReadInt32(CodeCaveOffsets.Base + CodeCaveOffsets.LockedTargetPtr);

            var poiseStruct = PatchManager.IsScholar()
                ? memoryService.ReadInt64((IntPtr)targetPtr + ChrCtrlOffsets.PoiseImmunityPtr)
                : memoryService.ReadInt32((IntPtr)targetPtr + ChrCtrlOffsets.PoiseImmunityPtr);

            return memoryService.Read<byte>((IntPtr)poiseStruct + ChrCtrlOffsets.PoiseStuff.LightStaggerImmuneFlag) == 1;
        }

        public int GetChrParam(int chrParamOffset)
        {
            var targetPtr = PatchManager.IsScholar()
                ? memoryService.ReadInt64(CodeCaveOffsets.Base + CodeCaveOffsets.LockedTargetPtr)
                : memoryService.ReadInt32(CodeCaveOffsets.Base + CodeCaveOffsets.LockedTargetPtr);

            var chrParamBase = PatchManager.IsScholar()
                ? memoryService.ReadInt64((IntPtr)targetPtr + ChrCtrlOffsets.ChrParam)
                : memoryService.ReadInt32((IntPtr)targetPtr + ChrCtrlOffsets.ChrParam);

            return memoryService.Read<int>((IntPtr)chrParamBase + chrParamOffset);
        }

        public float GetChrCommonParam(int chrCommonOffset)
        {
            var targetPtr = PatchManager.IsScholar()
                ? memoryService.ReadInt64(CodeCaveOffsets.Base + CodeCaveOffsets.LockedTargetPtr)
                : memoryService.ReadInt32(CodeCaveOffsets.Base + CodeCaveOffsets.LockedTargetPtr);

            var chrParamBase = PatchManager.IsScholar()
                ? memoryService.ReadInt64((IntPtr)targetPtr + ChrCtrlOffsets.ChrCommon)
                : memoryService.ReadInt32((IntPtr)targetPtr + ChrCtrlOffsets.ChrCommon);

            return memoryService.Read<float>((IntPtr)chrParamBase + chrCommonOffset);
        }

        public TargetState GetTargetState()
        {
            int memStart = ChrCtrlOffsets.Hp;
            int blockSize = (ChrCtrlOffsets.Speed + 4) - ChrCtrlOffsets.Hp;
            var ptr = GetTargetChrCtrl() + memStart;
            var block = new MemoryBlock(memoryService.ReadBytes(ptr, blockSize));

            return new TargetState
            {
                CurrentHp = block.Get<int>(ChrCtrlOffsets.Hp - memStart),
                MaxHp = block.Get<int>(ChrCtrlOffsets.MaxHp - memStart),
                CurrentHeavyPoise = block.Get<float>(ChrCtrlOffsets.HeavyPoiseCurrent - memStart),
                CurrentLightPoise = block.Get<float>(ChrCtrlOffsets.LightPoiseCurrent - memStart),
                CurrentPoison = block.Get<float>(ChrCtrlOffsets.PoisonCurrent - memStart),
                CurrentToxic = block.Get<float>(ChrCtrlOffsets.ToxicCurrent - memStart),
                CurrentBleed = block.Get<float>(ChrCtrlOffsets.BleedCurrent - memStart),
                Speed = block.Get<float>(ChrCtrlOffsets.Speed - memStart),
            };
        }

        public TargetMaxValues GetMaxValues()
        {
            int memStart = ChrCtrlOffsets.HeavyPoiseMax;
            int blockSize = (ChrCtrlOffsets.LightPoiseMax + 4) - ChrCtrlOffsets.HeavyPoiseMax;
            var ptr = GetTargetChrCtrl() + memStart;
            var block = new MemoryBlock(memoryService.ReadBytes(ptr, blockSize));

            return new TargetMaxValues
            {
                MaxHeavyPoise = block.Get<float>(ChrCtrlOffsets.HeavyPoiseMax - memStart),
                MaxLightPoise = block.Get<float>(ChrCtrlOffsets.LightPoiseMax - memStart),
                PoisonMax = block.Get<float>(ChrCtrlOffsets.PoisonMax - memStart),
                ToxicMax = block.Get<float>(ChrCtrlOffsets.ToxicMax - memStart),
                BleedMax = block.Get<float>(ChrCtrlOffsets.BleedMax - memStart),
            };
        }
    }
}