using System;
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

        public EnemyService(MemoryIo memoryIo, HookManager hookManager)
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
                byte[] saveTargetPtrBytes = AsmLoader.GetAsmBytes("SaveTargetPtr");
                byte[] bytes = AsmHelper.GetRelOffsetBytes(saveTargetPtrCode.ToInt64(), savedTargetPtr.ToInt64(), 7);
                Array.Copy(bytes, 0, saveTargetPtrBytes, 0x3, 4);
                bytes = AsmHelper.GetRelOffsetBytes(saveTargetPtrCode.ToInt64() + 0xE, saveTargetHook + 0x7, 5);
                Array.Copy(bytes, 0, saveTargetPtrBytes, 0xE + 1, 4);

                _memoryIo.WriteBytes(saveTargetPtrCode, saveTargetPtrBytes);
                _hookManager.InstallHook(saveTargetPtrCode.ToInt64(), saveTargetHook,
                    new byte[] { 0x48, 0x89, 0xBB, 0xC0, 0x00, 0x00, 0x00 });
            }
            else
            {
                _hookManager.UninstallHook(saveTargetPtrCode.ToInt64());
            }
        }

        public int GetTargetHp() =>
            _memoryIo.ReadInt32((IntPtr)_memoryIo.ReadInt64(CodeCaveOffsets.Base + CodeCaveOffsets.LockedTargetPtr) +
                                PlayerCtrlOffsets.Hp);

        public int GetTargetMaxHp() =>
            _memoryIo.ReadInt32((IntPtr)_memoryIo.ReadInt64(CodeCaveOffsets.Base + CodeCaveOffsets.LockedTargetPtr) +
                                PlayerCtrlOffsets.MaxHp);

        public void SetTargetHp(int health) =>
            _memoryIo.WriteInt32((IntPtr)_memoryIo.ReadInt64(CodeCaveOffsets.Base + CodeCaveOffsets.LockedTargetPtr) +
                                 PlayerCtrlOffsets.Hp, health);

        public long GetTargetId() => _memoryIo.ReadInt64(CodeCaveOffsets.Base + CodeCaveOffsets.LockedTargetPtr);

        public float[] GetTargetPos()
        {
            var targetPtr = (IntPtr)_memoryIo.ReadInt64(CodeCaveOffsets.Base + CodeCaveOffsets.LockedTargetPtr);

            float[] position = new float[3];
            position[0] = _memoryIo.ReadFloat(targetPtr + PlayerCtrlOffsets.Coords);
            position[1] = _memoryIo.ReadFloat(targetPtr + PlayerCtrlOffsets.Coords + 0x4);
            position[2] = _memoryIo.ReadFloat(targetPtr + PlayerCtrlOffsets.Coords + 0x8);

            return position;
        }

        public void ToggleFreezeTargetHp(bool isFreezeTargetHpEnabled)
        {
            var dmgControlCode = CodeCaveOffsets.Base + (int)CodeCaveOffsets.DamageControl.DamageControlCode;
            var freezeTargetHpFlag = CodeCaveOffsets.Base + (int)CodeCaveOffsets.DamageControl.FreezeTargetHpFlag;

            if (isFreezeTargetHpEnabled)
            {
                if (!_hookManager.IsHookInstalled(dmgControlCode.ToInt64()))
                {
                    _hookManager.InstallHook(dmgControlCode.ToInt64(), Hooks.HpWrite,
                        new byte[] { 0x89, 0x83, 0x68, 0x01, 0x00, 0x00 });
                }

                _memoryIo.WriteByte(freezeTargetHpFlag, 1);
            }
            else
            {
                _memoryIo.WriteByte(freezeTargetHpFlag, 0);

                bool allFlagsOff =
                    _memoryIo.ReadUInt8(CodeCaveOffsets.Base + (int)CodeCaveOffsets.DamageControl.PlayerNoDamageFlag) ==
                    0 &&
                    _memoryIo.ReadUInt8(CodeCaveOffsets.Base + (int)CodeCaveOffsets.DamageControl.OneShotFlag) == 0 &&
                    _memoryIo.ReadUInt8(CodeCaveOffsets.Base + (int)CodeCaveOffsets.DamageControl.DealNoDamageFlag) ==
                    0;

                if (allFlagsOff && _hookManager.IsHookInstalled(dmgControlCode.ToInt64()))
                {
                    _hookManager.UninstallHook(dmgControlCode.ToInt64());
                }
            }
        }

        public void ToggleDisableAi(bool isAllDisableAiEnabled)
        {
            var disableAiPtr = _memoryIo.FollowPointers(Base, new[]
            {
                GameManagerImp.Offsets.AiManager,
                AiManagerOffsets.DisableAi
            }, false);
            _memoryIo.WriteByte(disableAiPtr, isAllDisableAiEnabled ? 1 : 0);
        }

        public int GetLastAct()
        {
            var lastActPtr = _memoryIo.FollowPointers(CodeCaveOffsets.Base + CodeCaveOffsets.LockedTargetPtr, new[]
            {
                PlayerCtrlOffsets.PlayerActionCtrlPtr,
                PlayerCtrlOffsets.ChrActionCtrl.BossAttackCtrlPtr,
                PlayerCtrlOffsets.BossAttackCtrl.LastAttackPtr,
                PlayerCtrlOffsets.BossAttackCtrl.LastAttack
            }, false);
            return _memoryIo.ReadInt32(lastActPtr);
        }

        public (bool PoisonToxic, bool Bleed) GetImmunities()
        {
            var chrParamPtr =
                (IntPtr)_memoryIo.ReadInt64(
                    (IntPtr)_memoryIo.ReadInt64(CodeCaveOffsets.Base + CodeCaveOffsets.LockedTargetPtr) +
                    PlayerCtrlOffsets.ChrParamPtr);

            return (
                _memoryIo.ReadInt32(chrParamPtr + PlayerCtrlOffsets.ChrParam.PoisonToxicResist) == 100,
                _memoryIo.ReadInt32(chrParamPtr + PlayerCtrlOffsets.ChrParam.BleedResist) == 100
            );
        }

        public float GetTargetResistance(int offset) =>
                _memoryIo.ReadFloat(
                    (IntPtr)_memoryIo.ReadInt64(CodeCaveOffsets.Base + CodeCaveOffsets.LockedTargetPtr) +
                    offset);


        public void FreezeTarget(bool isFreezeTargetEnabled)
        {
         
        }
    }
}