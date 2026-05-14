using System;
using SilkySouls2.enums;
using SilkySouls2.Interfaces;
using SilkySouls2.Memory;
using SilkySouls2.Utilities;
using static SilkySouls2.Memory.Offsets;

namespace SilkySouls2.Services
{
    public class EnemyService(IMemoryService memoryService, HookManager hookManager) : IEnemyService
    {
        public void ToggleForlornSpawn(bool isGuaranteedSpawnEnabled, int funcId = 0, int currentSelected = 0)
        {
            var esdFuncIdLoc = CustomCodeOffsets.Base + (int)CustomCodeOffsets.Forlorn.EsdFuncId;
            var selectedIndexLoc = CustomCodeOffsets.Base + (int)CustomCodeOffsets.Forlorn.SelectedForlorn;
            var cmpRandCode = CustomCodeOffsets.Base + (int)CustomCodeOffsets.Forlorn.CompRandValueCode;
            var setVarCode = CustomCodeOffsets.Base + (int)CustomCodeOffsets.Forlorn.SetAreaVarCode;
            memoryService.Write(esdFuncIdLoc, funcId);
            memoryService.Write(selectedIndexLoc, currentSelected);
            if (isGuaranteedSpawnEnabled)
            {
                var cmpRandHook = Hooks.CompareEventRandValueForlorn;
                var setAreaVarHook = Hooks.SetAreaVariable;

                var cmpBytes = AsmLoader.GetAsmBytes(AsmScript.CompareEventRandValue);
                AsmHelper.WriteRelativeOffsets(cmpBytes, [
                    (cmpRandCode + 0x5, esdFuncIdLoc, 6, 0x5 + 2),
                    (cmpRandCode + 0x16, cmpRandHook + 0x8, 5, 0x16 + 1),
                    (cmpRandCode + 0x20, cmpRandHook + 0x8, 5, 0x20 + 1),
                ]);

                var setVarBytes = AsmLoader.GetAsmBytes(AsmScript.SetAreaVariable);
                AsmHelper.WriteRelativeOffsets(setVarBytes, [
                    (setVarCode + 0x8, esdFuncIdLoc, 6, 0x8 + 2),
                    (setVarCode + 0x14, selectedIndexLoc, 6, 0x14 + 2)
                ]);

                var bytes = AsmHelper.GetJmpOriginOffsetBytes(setAreaVarHook, 7, setVarCode + 0x20);
                Array.Copy(bytes, 0, setVarBytes, 0x1B + 1, 4);
                memoryService.WriteBytes(cmpRandCode, cmpBytes);
                memoryService.WriteBytes(setVarCode, setVarBytes);
                hookManager.InstallHook(cmpRandCode, cmpRandHook, [0x44, 0x8B, 0x41, 0x1C, 0x8B, 0x54, 0x82, 0x40]);
                hookManager.InstallHook(setVarCode, setAreaVarHook, [0x49, 0x8B, 0x8F, 0xB0, 0x00, 0x00, 0x00]);
            }
            else
            {
                hookManager.UninstallHook(cmpRandCode);
                hookManager.UninstallHook(setVarCode);
            }
        }

        public void UpdateForlornIndex(int selectedForlornIndex)
        {
            var selectedIndexLoc = CustomCodeOffsets.Base + (int)CustomCodeOffsets.Forlorn.SelectedForlorn;
            memoryService.Write(selectedIndexLoc, selectedForlornIndex);
        }

        public void ToggleDisableAi(bool isAllDisableAiEnabled) =>
            memoryService.Write(Patches.DisableAi, isAllDisableAiEnabled ? (byte)0xEB : (byte)0x7F);

        public void ToggleElanaSummons(bool isElanaSummonsEnabled, int rngVal = 0)
        {
            var code = CustomCodeOffsets.Base + CustomCodeOffsets.ElanaSummons;
            if (isElanaSummonsEnabled)
            {
                if (PatchManager.Current.Edition == GameEdition.Scholar) ScholarElanaSummon(code, rngVal);
                else VanillaElanaSummon(code, rngVal);
            }
            else
            {
                hookManager.UninstallHook(code);
            }
        }

        private void ScholarElanaSummon(IntPtr code, int rngVal)
        {
            var hookLoc = Hooks.CompareEventRandValueElana;
            var bytes = AsmLoader.GetAsmBytes(AsmScript.ElanaSummon64);
            var jmpBytes = AsmHelper.GetJmpOriginOffsetBytes(hookLoc, 7, code + 0x21);
            Array.Copy(jmpBytes, 0, bytes, 0x1C + 1, 4);
            memoryService.WriteBytes(code, bytes);
            memoryService.Write(code + 0x15, (byte)rngVal);
            hookManager.InstallHook(code, hookLoc,
                [0x48, 0x8B, 0x51, 0x10, 0x48, 0x85, 0xD2]);
        }

        private void VanillaElanaSummon(IntPtr code, int rngVal)
        {
            var hookLoc = Hooks.CompareEventRandValueElana;
            var bytes = AsmLoader.GetAsmBytes(AsmScript.ElanaSummon32);
            var jmpBytes = AsmHelper.GetJmpOriginOffsetBytes(hookLoc, 5, code + 0x1E);
            Array.Copy(jmpBytes, 0, bytes, 0x19 + 1, 4);
            memoryService.WriteBytes(code, bytes);
            memoryService.Write(code + 0x13, (byte)rngVal);
            hookManager.InstallHook(code, hookLoc,
                [0x8B, 0x51, 0x08, 0x85, 0xD2]);
        }
    }
}