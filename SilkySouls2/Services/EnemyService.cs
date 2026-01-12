using System;
using SilkySouls2.enums;
using SilkySouls2.Interfaces;
using SilkySouls2.Memory;
using SilkySouls2.Utilities;
using static SilkySouls2.Memory.Offsets;

namespace SilkySouls2.Services
{
    public class EnemyService(IMemoryService memoryService, HookManager hookManager)
    {
        public void ToggleForlornSpawn(bool isGuaranteedSpawnEnabled, int funcId = 0, int currentSelected = 0)
        {
            var esdFuncIdLoc = CodeCaveOffsets.Base + (int)CodeCaveOffsets.Forlorn.EsdFuncId;
            var selectedIndexLoc = CodeCaveOffsets.Base + (int)CodeCaveOffsets.Forlorn.SelectedForlorn;
            var cmpRandCode = CodeCaveOffsets.Base + (int)CodeCaveOffsets.Forlorn.CompRandValueCode;
            var setVarCode = CodeCaveOffsets.Base + (int)CodeCaveOffsets.Forlorn.SetAreaVarCode;
            memoryService.Write(esdFuncIdLoc, funcId);
            memoryService.Write(selectedIndexLoc, currentSelected);
            if (isGuaranteedSpawnEnabled)
            {
                var cmpRandHook = Hooks.CompareEventRandValueForlorn;
                var setAreaVarHook = Hooks.SetAreaVariable;

                var cmpBytes = AsmLoader.GetAsmBytes(AsmScript.CompareEventRandValue);
                AsmHelper.WriteRelativeOffsets(cmpBytes, new[]
                {
                    (cmpRandCode.ToInt64() + 0x5, esdFuncIdLoc.ToInt64(), 6, 0x5 + 2),
                    (cmpRandCode.ToInt64() + 0x16, cmpRandHook + 0x8, 5, 0x16 + 1),
                    (cmpRandCode.ToInt64() + 0x20, cmpRandHook + 0x8, 5, 0x20 + 1),
                });

                var setVarBytes = AsmLoader.GetAsmBytes(AsmScript.SetAreaVariable);
                AsmHelper.WriteRelativeOffsets(setVarBytes, new[]
                {
                    (setVarCode.ToInt64() + 0x8, esdFuncIdLoc.ToInt64(), 6, 0x8 + 2),
                    (setVarCode.ToInt64() + 0x14, selectedIndexLoc.ToInt64(), 6, 0x14 + 2)
                });

                var bytes = AsmHelper.GetJmpOriginOffsetBytes(setAreaVarHook, 7, setVarCode + 0x20);
                Array.Copy(bytes, 0, setVarBytes, 0x1B + 1, 4);
                memoryService.WriteBytes(cmpRandCode, cmpBytes);
                memoryService.WriteBytes(setVarCode, setVarBytes);
                hookManager.InstallHook(cmpRandCode.ToInt64(), cmpRandHook, new byte[]
                    { 0x44, 0x8B, 0x41, 0x1C, 0x8B, 0x54, 0x82, 0x40 });
                hookManager.InstallHook(setVarCode.ToInt64(), setAreaVarHook, new byte[]
                    { 0x49, 0x8B, 0x8F, 0xB0, 0x00, 0x00, 0x00 });
            }
            else
            {
                hookManager.UninstallHook(cmpRandCode.ToInt64());
                hookManager.UninstallHook(setVarCode.ToInt64());
            }
        }

        public void UpdateForlornIndex(int selectedForlornIndex)
        {
            var selectedIndexLoc = CodeCaveOffsets.Base + (int)CodeCaveOffsets.Forlorn.SelectedForlorn;
            memoryService.Write(selectedIndexLoc, selectedForlornIndex);
        }

        public void ToggleDisableAi(bool isAllDisableAiEnabled) =>
            memoryService.Write(Patches.DisableAi, isAllDisableAiEnabled ? (byte)0xEB : (byte)0x7F);

        public void ToggleElanaSummons(bool isElanaSummonsEnabled, int rngVal = 0)
        {
            var code = CodeCaveOffsets.Base + CodeCaveOffsets.ElanaSummons;
            if (isElanaSummonsEnabled)
            {
                if (PatchManager.Current.Edition == GameEdition.Scholar) ScholarElanaSummon(code, rngVal);
                else VanillaElanaSummon(code, rngVal);
            }
            else
            {
                hookManager.UninstallHook(code.ToInt64());
            }
        }

        private void ScholarElanaSummon(IntPtr code, int rngVal)
        {
            var hookLoc = Hooks.CompareEventRandValueElana;
            var bytes = AsmLoader.GetAsmBytes(AsmScript.ElanaSummon64);
            var jmpBytes = AsmHelper.GetJmpOriginOffsetBytes(hookLoc, 7, code + 0x21);
            Array.Copy(jmpBytes, 0, bytes, 0x1C + 1, 4);
            memoryService.WriteBytes(code, bytes);
            memoryService.WriteByte(code + 0x15, rngVal);
            hookManager.InstallHook(code.ToInt64(), hookLoc,
                new byte[] { 0x48, 0x8B, 0x51, 0x10, 0x48, 0x85, 0xD2 });
        }

        private void VanillaElanaSummon(IntPtr code, int rngVal)
        {
            var hookLoc = Hooks.CompareEventRandValueElana;
            var bytes = AsmLoader.GetAsmBytes(AsmScript.ElanaSummon32);
            var jmpBytes = AsmHelper.GetJmpOriginOffsetBytes(hookLoc, 5, code + 0x1E);
            Array.Copy(jmpBytes, 0, bytes, 0x19 + 1, 4);
            memoryService.WriteBytes(code, bytes);
            memoryService.WriteByte(code + 0x13, rngVal);
            hookManager.InstallHook(code.ToInt64(), hookLoc,
                new byte[] { 0x8B, 0x51, 0x08, 0x85, 0xD2 });
        }
    }
}