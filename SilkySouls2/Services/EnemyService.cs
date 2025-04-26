using System;
using SilkySouls2.Memory;
using SilkySouls2.Utilities;
using static SilkySouls2.Memory.Offsets;

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
    }
}