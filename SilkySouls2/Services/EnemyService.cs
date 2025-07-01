using System;
using System.Collections.Generic;
using SilkySouls2.Memory;
using SilkySouls2.Utilities;
using static SilkySouls2.Memory.Offsets;
using static SilkySouls2.Memory.Offsets.GameManagerImp;

namespace SilkySouls2.Services
{
    public class EnemyService
    {
        private readonly MemoryIo _memoryIo;
        private readonly HookManager _hookManager;
        private readonly List<long> _disabledEntities = new List<long>();
        private bool _disableTargetHookInstalled;

        public EnemyService(MemoryIo memoryIo, HookManager hookManager, DamageControlService damageControlService)
        {
            _memoryIo = memoryIo;
            _hookManager = hookManager;
        }

        public void ToggleForlornSpawn(bool isGuaranteedSpawnEnabled, int funcId = 0, int currentSelected = 0)
        {
            var esdFuncIdLoc = CodeCaveOffsets.Base + (int)CodeCaveOffsets.Forlorn.EsdFuncId;
            var selectedIndexLoc = CodeCaveOffsets.Base + (int)CodeCaveOffsets.Forlorn.SelectedForlorn;
            var cmpRandCode = CodeCaveOffsets.Base + (int)CodeCaveOffsets.Forlorn.CompRandValueCode;
            var setVarCode = CodeCaveOffsets.Base + (int)CodeCaveOffsets.Forlorn.SetAreaVarCode;
            _memoryIo.WriteInt32(esdFuncIdLoc, funcId);
            _memoryIo.WriteInt32(selectedIndexLoc, currentSelected);
            if (isGuaranteedSpawnEnabled)
            {
                var cmpRandHook = Hooks.CompareEventRandValue;
                var setAreaVarHook = Hooks.SetAreaVariable;

                var cmpBytes = AsmLoader.GetAsmBytes("CompareEventRandValue");
                var bytes = AsmHelper.GetRelOffsetBytes(cmpRandCode + 0x5, esdFuncIdLoc, 6);
                Array.Copy(bytes, 0, cmpBytes, 0x5 + 2, 4);
                AsmHelper.WriteJumpOffsets(cmpBytes, new[]
                {
                    (cmpRandHook, 8, cmpRandCode + 0x1B, 0x16 + 1),
                    (cmpRandHook, 8, cmpRandCode + 0x25, 0x20 + 1)
                });

                var setVarBytes = AsmLoader.GetAsmBytes("SetAreaVariable");
                AsmHelper.WriteRelativeOffsets(setVarBytes, new[]
                {
                    (setVarCode.ToInt64() + 0x8, esdFuncIdLoc.ToInt64(), 6, 0x8 + 2),
                    (setVarCode.ToInt64() + 0x14, selectedIndexLoc.ToInt64(), 6, 0x14 + 2)
                });

                bytes = AsmHelper.GetJmpOriginOffsetBytes(setAreaVarHook, 7, setVarCode + 0x20);
                Array.Copy(bytes, 0, setVarBytes, 0x1B + 1, 4);
                _memoryIo.WriteBytes(cmpRandCode, cmpBytes);
                _memoryIo.WriteBytes(setVarCode, setVarBytes);
                _hookManager.InstallHook(cmpRandCode.ToInt64(), cmpRandHook, new byte[]
                    { 0x44, 0x8B, 0x41, 0x1C, 0x8B, 0x54, 0x82, 0x40 });
                _hookManager.InstallHook(setVarCode.ToInt64(), setAreaVarHook, new byte[]
                    { 0x49, 0x8B, 0x8F, 0xB0, 0x00, 0x00, 0x00 });
            }
            else
            {
                _hookManager.UninstallHook(cmpRandCode.ToInt64());
                _hookManager.UninstallHook(setVarCode.ToInt64());
            }
        }

        public void UpdateForlornIndex(int selectedForlornIndex)
        {
            var selectedIndexLoc = CodeCaveOffsets.Base + (int)CodeCaveOffsets.Forlorn.SelectedForlorn;
            _memoryIo.WriteInt32(selectedIndexLoc, selectedForlornIndex);
        }

        public void ToggleTargetHook(bool isEnabled)
        {
            var saveTargetPtrCode = CodeCaveOffsets.Base + CodeCaveOffsets.SaveLockedTarget;
            if (isEnabled)
            {
                var saveTargetHook = Hooks.LockedTarget;
                var savedTargetPtr = CodeCaveOffsets.Base + CodeCaveOffsets.LockedTargetPtr;
                if (GameVersion.Current.Edition == GameEdition.Scholar)
                {
                    byte[] saveTargetPtrBytes = AsmLoader.GetAsmBytes("SaveTargetPtr64");
                    byte[] bytes =
                        AsmHelper.GetRelOffsetBytes(saveTargetPtrCode.ToInt64(), savedTargetPtr.ToInt64(), 7);
                    Array.Copy(bytes, 0, saveTargetPtrBytes, 0x3, 4);
                    bytes = AsmHelper.GetRelOffsetBytes(saveTargetPtrCode.ToInt64() + 0xE, saveTargetHook + 0x7, 5);
                    Array.Copy(bytes, 0, saveTargetPtrBytes, 0xE + 1, 4);

                    _memoryIo.WriteBytes(saveTargetPtrCode, saveTargetPtrBytes);
                    _hookManager.InstallHook(saveTargetPtrCode.ToInt64(), saveTargetHook,
                        new byte[] { 0x48, 0x89, 0xBB, 0xC0, 0x00, 0x00, 0x00 });
                }
                else
                {
                    byte[] saveTargetPtrBytes = AsmLoader.GetAsmBytes("SaveTargetPtr32");
                    var bytes = BitConverter.GetBytes(savedTargetPtr.ToInt32());
                    Array.Copy(bytes, 0, saveTargetPtrBytes, 0x6 + 2, 4);
                    bytes = AsmHelper.GetRelOffsetBytes(saveTargetPtrCode.ToInt64() + 0xC, saveTargetHook + 0x6, 5);
                    Array.Copy(bytes, 0, saveTargetPtrBytes, 0xC + 1, 4);
                    _memoryIo.WriteBytes(saveTargetPtrCode, saveTargetPtrBytes);
                    _hookManager.InstallHook(saveTargetPtrCode.ToInt64(), saveTargetHook,
                        new byte[] { 0x89, 0xB7, 0xB8, 0x00, 0x00, 0x00 });
                }
            }
            else
            {
                _hookManager.UninstallHook(saveTargetPtrCode.ToInt64());
            }
        }

        public int GetTargetHp()
        {
            if (GameVersion.Current.Edition == GameEdition.Scholar)
            {
                return _memoryIo.ReadInt32(
                    (IntPtr)_memoryIo.ReadInt64(CodeCaveOffsets.Base + CodeCaveOffsets.LockedTargetPtr) +
                    ChrCtrlOffsets.Hp);
            }

            return _memoryIo.ReadInt32(
                (IntPtr)_memoryIo.ReadInt32(CodeCaveOffsets.Base + CodeCaveOffsets.LockedTargetPtr) +
                ChrCtrlOffsets.Hp);
        }

        public int GetTargetMaxHp()
        {
            if (GameVersion.Current.Edition == GameEdition.Scholar)
            {
                return _memoryIo.ReadInt32(
                    (IntPtr)_memoryIo.ReadInt64(CodeCaveOffsets.Base + CodeCaveOffsets.LockedTargetPtr) +
                    ChrCtrlOffsets.MaxHp);
            }

            return _memoryIo.ReadInt32(
                (IntPtr)_memoryIo.ReadInt32(CodeCaveOffsets.Base + CodeCaveOffsets.LockedTargetPtr) +
                ChrCtrlOffsets.MaxHp);
        }

        public void SetTargetHp(int health)
        {
            if (GameVersion.Current.Edition == GameEdition.Scholar)
            {
                _memoryIo.WriteInt32(
                    (IntPtr)_memoryIo.ReadInt64(CodeCaveOffsets.Base + CodeCaveOffsets.LockedTargetPtr) +
                    ChrCtrlOffsets.Hp, health);
            }
            else
            {
                _memoryIo.WriteInt32(
                    (IntPtr)_memoryIo.ReadInt32(CodeCaveOffsets.Base + CodeCaveOffsets.LockedTargetPtr) +
                    ChrCtrlOffsets.Hp, health);
            }
        }

        public long GetTargetId()
        {
            return GameVersion.Current.Edition == GameEdition.Scholar
                ? _memoryIo.ReadInt64(CodeCaveOffsets.Base + CodeCaveOffsets.LockedTargetPtr)
                : _memoryIo.ReadInt32(CodeCaveOffsets.Base + CodeCaveOffsets.LockedTargetPtr);
        }

        public float[] GetTargetPos()
        {
            var targetPtr = (IntPtr)(GameVersion.Current.Edition == GameEdition.Scholar
                ? _memoryIo.ReadInt64(CodeCaveOffsets.Base + CodeCaveOffsets.LockedTargetPtr)
                : _memoryIo.ReadInt32(CodeCaveOffsets.Base + CodeCaveOffsets.LockedTargetPtr));

            float[] position = new float[3];
            position[0] = _memoryIo.ReadFloat(targetPtr + ChrCtrlOffsets.Coords);
            position[1] = _memoryIo.ReadFloat(targetPtr + ChrCtrlOffsets.Coords + 0x4);
            position[2] = _memoryIo.ReadFloat(targetPtr + ChrCtrlOffsets.Coords + 0x8);

            return position;
        }

        public void ToggleDisableAi(bool isAllDisableAiEnabled) =>
            _memoryIo.WriteByte(Patches.DisableAi, isAllDisableAiEnabled ? 0xEB : 0x7F);

        public int GetLastAct() => _memoryIo.ReadInt32(CodeCaveOffsets.Base + (int)CodeCaveOffsets.RepeatAct.AttackId);

        public (bool PoisonToxic, bool Bleed) GetImmunities()
        {
            var chrParamPtr = GameVersion.Current.Edition == GameEdition.Scholar
                ? (IntPtr)_memoryIo.ReadInt64(
                    (IntPtr)_memoryIo.ReadInt64(CodeCaveOffsets.Base + CodeCaveOffsets.LockedTargetPtr) +
                    ChrCtrlOffsets.ChrParamPtr)
                : (IntPtr)_memoryIo.ReadInt32(
                    (IntPtr)_memoryIo.ReadInt32(CodeCaveOffsets.Base + CodeCaveOffsets.LockedTargetPtr) +
                    ChrCtrlOffsets.ChrParamPtr);

            return (
                _memoryIo.ReadInt32(chrParamPtr + ChrCtrlOffsets.ChrParam.PoisonToxicResist) == 100,
                _memoryIo.ReadInt32(chrParamPtr + ChrCtrlOffsets.ChrParam.BleedResist) == 100
            );
        }

        public float GetTargetResistance(int offset)
        {
            return GameVersion.Current.Edition == GameEdition.Scholar
                ? _memoryIo.ReadFloat(
                    (IntPtr)_memoryIo.ReadInt64(CodeCaveOffsets.Base + CodeCaveOffsets.LockedTargetPtr) +
                    offset)
                : _memoryIo.ReadFloat(
                    (IntPtr)_memoryIo.ReadInt32(CodeCaveOffsets.Base + CodeCaveOffsets.LockedTargetPtr) +
                    offset);
        }


        public void ToggleCurrentActHook(bool isEnabled)
        {
            var code = CodeCaveOffsets.Base + (int)CodeCaveOffsets.RepeatAct.Code;
            var code2 = CodeCaveOffsets.Base + (int)CodeCaveOffsets.RepeatAct.Code2;

            if (isEnabled)
            {
                var repeatFlagLoc = CodeCaveOffsets.Base + (int)CodeCaveOffsets.RepeatAct.RepeatFlag;
                var attackId = CodeCaveOffsets.Base + (int)CodeCaveOffsets.RepeatAct.AttackId;
                var lockedTargetPtr = CodeCaveOffsets.Base + CodeCaveOffsets.LockedTargetPtr;
                var currectActOrigin = Hooks.SetCurrectAct;
                var currectActOrigin2 = Hooks.SetCurrectAct2;

                if (GameVersion.Current.Edition == GameEdition.Scholar)
                {
                    var codeBytes = AsmLoader.GetAsmBytes("RepeatAct64");
                    AsmHelper.WriteRelativeOffsets(codeBytes, new[]
                    {
                        (code.ToInt64() + 0x8, lockedTargetPtr.ToInt64(), 7, 0x8 + 3),
                        (code.ToInt64() + 0x28, repeatFlagLoc.ToInt64(), 7, 0x28 + 2),
                        (code.ToInt64() + 0x31, attackId.ToInt64(), 6, 0x31 + 2),
                        (code.ToInt64() + 0x3F, attackId.ToInt64(), 6, 0x3F + 2),
                    });

                    _memoryIo.WriteBytes(code, codeBytes);
                    AsmHelper.WriteRelativeOffsets(codeBytes, new[]
                    {
                        (code2.ToInt64() + 0x8, lockedTargetPtr.ToInt64(), 7, 0x8 + 3),
                        (code2.ToInt64() + 0x28, repeatFlagLoc.ToInt64(), 7, 0x28 + 2),
                        (code2.ToInt64() + 0x31, attackId.ToInt64(), 6, 0x31 + 2),
                        (code2.ToInt64() + 0x3F, attackId.ToInt64(), 6, 0x3F + 2),
                    });

                    _memoryIo.WriteBytes(code2, codeBytes);

                    _hookManager.InstallHook(code.ToInt64(), currectActOrigin, new byte[]
                        { 0x83, 0x89, 0x50, 0x03, 0x00, 0x00, 0x01 });
                    _hookManager.InstallHook(code2.ToInt64(), currectActOrigin2, new byte[]
                        { 0x83, 0x89, 0x50, 0x03, 0x00, 0x00, 0x01 });
                }

                else
                {
                    var codeBytes = AsmLoader.GetAsmBytes("RepeatAct32");
                    AsmHelper.WriteAbsoluteAddresses32(codeBytes, new[]
                    {
                        (lockedTargetPtr.ToInt64(), 0x1 + 2),
                        (repeatFlagLoc.ToInt64(), 0x1B + 2),
                        (attackId.ToInt64(), 0x24 + 1),
                        (attackId.ToInt64(), 0x35 + 1),
                    });
                    AsmHelper.WriteJumpOffsets(codeBytes, new[]
                    {
                        (currectActOrigin, 6, code + 0x30, 0x30 + 1),
                        (currectActOrigin, 6, code + 0x41, 0x41 + 1),
                    });
                    _memoryIo.WriteBytes(code, codeBytes);

                    AsmHelper.WriteAbsoluteAddresses32(codeBytes, new[]
                    {
                        (lockedTargetPtr.ToInt64(), 0x1 + 2),
                        (repeatFlagLoc.ToInt64(), 0x1B + 2),
                        (attackId.ToInt64(), 0x24 + 1),
                        (attackId.ToInt64(), 0x35 + 1),
                    });
                    AsmHelper.WriteJumpOffsets(codeBytes, new[]
                    {
                        (currectActOrigin2, 6, code2 + 0x30, 0x30 + 1),
                        (currectActOrigin2, 6, code2 + 0x41, 0x41 + 1),
                    });
                    _memoryIo.WriteBytes(code2, codeBytes);

                    _hookManager.InstallHook(code.ToInt64(), currectActOrigin, new byte[]
                        { 0x89, 0x81, 0x5C, 0x02, 0x00, 0x00 });
                    _hookManager.InstallHook(code2.ToInt64(), currectActOrigin2, new byte[]
                        { 0x89, 0x81, 0x5C, 0x02, 0x00, 0x00 });
                }
            }
            else
            {
                _hookManager.UninstallHook(code.ToInt64());
                _hookManager.UninstallHook(code2.ToInt64());
            }
        }

        public void ToggleRepeatAct(bool isRepeatActEnabled) =>
            _memoryIo.WriteByte(CodeCaveOffsets.Base + (int)CodeCaveOffsets.RepeatAct.RepeatFlag,
                isRepeatActEnabled ? 1 : 0);

        public void SetTargetSpeed(float value)
        {
            if (GameVersion.Current.Edition == GameEdition.Scholar)
            {
                _memoryIo.WriteFloat(
                    (IntPtr)_memoryIo.ReadInt64(CodeCaveOffsets.Base + CodeCaveOffsets.LockedTargetPtr) +
                    ChrCtrlOffsets.Speed, value);
            }
            else
            {
                _memoryIo.WriteFloat(
                    (IntPtr)_memoryIo.ReadInt32(CodeCaveOffsets.Base + CodeCaveOffsets.LockedTargetPtr) +
                    ChrCtrlOffsets.Speed, value);
            }
        }

        public float GetTargetSpeed()
        {
            if (GameVersion.Current.Edition == GameEdition.Scholar)
            {
                return _memoryIo.ReadFloat(
                    (IntPtr)_memoryIo.ReadInt64(CodeCaveOffsets.Base + CodeCaveOffsets.LockedTargetPtr) +
                    ChrCtrlOffsets.Speed);
            }

            return _memoryIo.ReadFloat(
                (IntPtr)_memoryIo.ReadInt32(CodeCaveOffsets.Base + CodeCaveOffsets.LockedTargetPtr) +
                ChrCtrlOffsets.Speed);
        }

        public void ClearLockedTarget()
        {
            if (GameVersion.Current.Edition == GameEdition.Scholar)
            {
                _memoryIo.WriteBytes(CodeCaveOffsets.Base + CodeCaveOffsets.LockedTargetPtr, new byte[]
                    { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
            }
            else
            {
                _memoryIo.WriteBytes(CodeCaveOffsets.Base + CodeCaveOffsets.LockedTargetPtr, new byte[]
                    { 0x00, 0x00, 0x00, 0x00 });
            }
        }

        public void ToggleTargetAi(bool isDisableTargetAiEnabled)
        {
            var chrAi = GetChrAi(GetTargetId());
            var arrayLoc = CodeCaveOffsets.Base + (int)CodeCaveOffsets.DisableTargetAi.Array;
            var countLoc = CodeCaveOffsets.Base + (int)CodeCaveOffsets.DisableTargetAi.Count;
            var codeLoc = CodeCaveOffsets.Base + (int)CodeCaveOffsets.DisableTargetAi.Code;
            if (GameVersion.Current.Edition == GameEdition.Scholar)
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
                _memoryIo.WriteInt64(arrayLoc + (_disabledEntities.Count - 1) * 8, chrAi);
                _memoryIo.WriteInt32(countLoc, _disabledEntities.Count);
                if (_disableTargetHookInstalled) return;
                var origin = Hooks.DisableTargetAi;
                    
                var bytes = AsmLoader.GetAsmBytes("DisableTargetAi64");

                AsmHelper.WriteRelativeOffsets(bytes, new[]
                {
                    (codeLoc.ToInt64() + 0x6, countLoc.ToInt64(), 7, 0x6 + 2),
                    (codeLoc.ToInt64() + 0xD, origin + 6, 6, 0xD + 2),
                    (codeLoc.ToInt64() + 0x15, countLoc.ToInt64(), 6, 0x15 + 2),
                    (codeLoc.ToInt64() + 0x39, origin + 6, 5, 0x39 + 1),
                    (codeLoc.ToInt64() + 0x40, origin + 9, 5, 0x40 + 1),
                });

                var arrayLocBytes = BitConverter.GetBytes(arrayLoc.ToInt64());
                Array.Copy(arrayLocBytes, 0, bytes, 0x1B + 2, 8);

                _memoryIo.WriteBytes(codeLoc, bytes);
                _hookManager.InstallHook(codeLoc.ToInt64(), origin, new byte[]
                    { 0x48, 0x89, 0xFA, 0x48, 0x89, 0xD9 });
                _disableTargetHookInstalled = true;
            }
            else
            {
                _disabledEntities.Remove(chrAi);
                for (int i = 0; i < _disabledEntities.Count; i++)
                {
                    _memoryIo.WriteInt64(arrayLoc + i * 8, _disabledEntities[i]);
                }

                _memoryIo.WriteInt32(countLoc, _disabledEntities.Count);
                if (_disabledEntities.Count != 0) return;
                _hookManager.UninstallHook(codeLoc.ToInt64());
                ClearDisableEntities();
                _disableTargetHookInstalled = false;
            }
        }

        private void Process32BitDisable(bool isDisableTargetAiEnabled, long chrAi, IntPtr arrayLoc, IntPtr countLoc, IntPtr codeLoc)
        {
            if (isDisableTargetAiEnabled)
            {
                if (_disabledEntities.Count >= 20) ClearDisableEntities();
                _disabledEntities.Add(chrAi);
                _memoryIo.WriteInt32(arrayLoc + (_disabledEntities.Count - 1) * 4, (int)chrAi);
                _memoryIo.WriteInt32(countLoc, _disabledEntities.Count);
                if (_disableTargetHookInstalled) return;
                var origin = Hooks.DisableTargetAi;
                var bytes = AsmLoader.GetAsmBytes("DisableTargetAi32");
                AsmHelper.WriteAbsoluteAddresses32(bytes, new []
                {
                    (countLoc.ToInt64(), 0x5 + 2),
                    (countLoc.ToInt64(), 0x14 + 2),
                    (arrayLoc.ToInt64(), 0x1A + 1),
                });
                    
                AsmHelper.WriteRelativeOffsets(bytes, new []
                {
                    (codeLoc.ToInt64() + 0xC, origin + 5, 6, 0xC + 2),
                    (codeLoc.ToInt64() + 0x30, origin + 5, 5, 0x30 + 1),
                    (codeLoc.ToInt64() + 0x37, origin + 0xA, 5, 0x37 + 1),
                        
                });
                    
                _memoryIo.WriteBytes(codeLoc, bytes);
                _hookManager.InstallHook(codeLoc.ToInt64(), origin, new byte[]
                    { 0x8B, 0x06, 0x8B, 0x50, 0x1C });
                _disableTargetHookInstalled = true;

            }
            else
            {
                _disabledEntities.Remove(chrAi);
                for (int i = 0; i < _disabledEntities.Count; i++)
                {
                    _memoryIo.WriteInt32(arrayLoc + i * 4, (int)_disabledEntities[i]);
                }

                _memoryIo.WriteInt32(countLoc, _disabledEntities.Count);
                if (_disabledEntities.Count != 0) return;
                _hookManager.UninstallHook(codeLoc.ToInt64());
                ClearDisableEntities();
                _disableTargetHookInstalled = false;
            }
        }

        public bool IsAiDisabled(long targetId) => _disabledEntities.Contains(GetChrAi(targetId));

        private long GetChrAi(long targetId)
        {
            if (GameVersion.Current.Edition == GameEdition.Scholar)
            {
                var operatorPtr = _memoryIo.ReadInt64((IntPtr)(targetId + ChrCtrlOffsets.OperatorPtr));
                var chrAiManPtr = _memoryIo.ReadInt64((IntPtr)(operatorPtr + ChrCtrlOffsets.Operator.ChrAiManPtr));
                var chrAi = _memoryIo.ReadInt64((IntPtr)(chrAiManPtr + ChrCtrlOffsets.ChrAiMan.ChrAi));
                return chrAi;
            }
            else
            {
                var operatorPtr = _memoryIo.ReadInt32((IntPtr)(targetId + ChrCtrlOffsets.OperatorPtr));
                var chrAiManPtr = _memoryIo.ReadInt32((IntPtr)(operatorPtr + ChrCtrlOffsets.Operator.ChrAiManPtr));
                var chrAi = _memoryIo.ReadInt32((IntPtr)(chrAiManPtr + ChrCtrlOffsets.ChrAiMan.ChrAi));
                return chrAi;
            }
        }


        public void ClearDisableEntities()
        {
            _memoryIo.WriteBytes(CodeCaveOffsets.Base + (int)CodeCaveOffsets.DisableTargetAi.Array, new byte[0x100]);
            _memoryIo.WriteInt32(CodeCaveOffsets.Base + (int)CodeCaveOffsets.DisableTargetAi.Count, 0);
            _disabledEntities.Clear();
        }

        public bool IsLightPoiseImmune()
        {
            var targetPtr = GameVersion.Current.Edition == GameEdition.Scholar
                ? _memoryIo.ReadInt64(CodeCaveOffsets.Base + CodeCaveOffsets.LockedTargetPtr)
                : _memoryIo.ReadInt32(CodeCaveOffsets.Base + CodeCaveOffsets.LockedTargetPtr);

            var poiseStruct = GameVersion.Current.Edition == GameEdition.Scholar
                ? _memoryIo.ReadInt64((IntPtr)targetPtr + ChrCtrlOffsets.PoiseImmunityPtr)
                : _memoryIo.ReadInt32((IntPtr)targetPtr + ChrCtrlOffsets.PoiseImmunityPtr);

            return _memoryIo.ReadUInt8((IntPtr)poiseStruct + ChrCtrlOffsets.PoiseStuff.LightStaggerImmuneFlag) == 1;
        }

        public int GetChrParam(int chrParamOffset)
        {
            var chrParamBase =
                _memoryIo.ReadInt64(
                    (IntPtr)_memoryIo.ReadInt64(CodeCaveOffsets.Base + CodeCaveOffsets.LockedTargetPtr) +
                    ChrCtrlOffsets.ChrParamPtr);
            return _memoryIo.ReadInt32((IntPtr)chrParamBase + chrParamOffset);
        }

        public float GetChrCommonParam(int chrCommonOffset)
        {
            var chrParamBase =
                _memoryIo.ReadInt64(
                    (IntPtr)_memoryIo.ReadInt64(CodeCaveOffsets.Base + CodeCaveOffsets.LockedTargetPtr) +
                    ChrCtrlOffsets.ChrCommonPtr);
            return _memoryIo.ReadFloat((IntPtr)chrParamBase + chrCommonOffset);
        }
    }
}