using System;
using System.Threading.Tasks;
using Microsoft.SqlServer.Server;
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
                var cmpRandHook = Hooks.CompareEventRandValueForlorn;
                var setAreaVarHook = Hooks.SetAreaVariable;

                var cmpBytes = AsmLoader.GetAsmBytes("CompareEventRandValue");
                AsmHelper.WriteRelativeOffsets(cmpBytes, new []
                {
                    (cmpRandCode.ToInt64() + 0x5, esdFuncIdLoc.ToInt64(), 6, 0x5 + 2),
                    (cmpRandCode.ToInt64() + 0x16, cmpRandHook + 0x8, 5, 0x16 + 1),
                    (cmpRandCode.ToInt64() + 0x20, cmpRandHook + 0x8, 5, 0x20 + 1),
                });
                
                var setVarBytes = AsmLoader.GetAsmBytes("SetAreaVariable");
                AsmHelper.WriteRelativeOffsets(setVarBytes, new[]
                {
                    (setVarCode.ToInt64() + 0x8, esdFuncIdLoc.ToInt64(), 6, 0x8 + 2),
                    (setVarCode.ToInt64() + 0x14, selectedIndexLoc.ToInt64(), 6, 0x14 + 2)
                });

                var bytes = AsmHelper.GetJmpOriginOffsetBytes(setAreaVarHook, 7, setVarCode + 0x20);
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
        
        public void ToggleDisableAi(bool isAllDisableAiEnabled) =>
            _memoryIo.WriteByte(Patches.DisableAi, isAllDisableAiEnabled ? 0xEB : 0x7F);

        public void ToggleElanaSummons(bool isElanaSummonsEnabled, int rngVal = 0)
        {
            var code = CodeCaveOffsets.Base + CodeCaveOffsets.ElanaSummons;
            if (isElanaSummonsEnabled)
            {
                var hookLoc = Hooks.CompareEventRandValueElana;
                var bytes = AsmLoader.GetAsmBytes("ElanaSummon64");
                var jmpBytes = AsmHelper.GetJmpOriginOffsetBytes(hookLoc, 7, code + 0x21);
                Array.Copy(jmpBytes, 0, bytes, 0x1C + 1,  4);
                _memoryIo.WriteBytes(code, bytes);
                _memoryIo.WriteByte(code + 0x15, rngVal);
                _hookManager.InstallHook(code.ToInt64(), hookLoc,
                new byte[] {0x48, 0x8B, 0x51, 0x10, 0x48, 0x85, 0xD2});
            }
            else
            {
                _hookManager.UninstallHook(code.ToInt64());
            }
        }
      
    }
}