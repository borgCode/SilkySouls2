using SilkySouls2.enums;
using SilkySouls2.GameIds;
using SilkySouls2.Interfaces;
using SilkySouls2.Memory;
using SilkySouls2.Utilities;
using static SilkySouls2.Memory.Offsets;

namespace SilkySouls2.Services
{
    public class SpEffectService(IMemoryService memoryService) : ISpEffectService
    {
        public void ApplySpEffect(nint chrCtrl, SpEffect spEffect)
        {
            var spEffectParams = CustomCodeOffsets.Base + CustomCodeOffsets.SpEffectParams;
            var code = CustomCodeOffsets.Base + CustomCodeOffsets.SpEffectCode;

            var chrSpEffectCtrl = memoryService.ReadPointer(chrCtrl + ChrCtrl.ChrSpEffectCtrl);

            memoryService.WriteBytes(spEffectParams, spEffect.ToBytes());

            if (PatchManager.IsScholar()) WriteScholarApplySpEffect(code, chrSpEffectCtrl, spEffectParams);
            else WriteVanillaApplySpEffect(code, chrSpEffectCtrl, spEffectParams);

            memoryService.RunThread(code);
        }

        private void WriteScholarApplySpEffect(nint code, nint chrSpEffectCtrl, nint spEffectParams)
        {
            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.SetSpEffect64);
            AsmHelper.WriteAbsoluteAddress64(codeBytes, chrSpEffectCtrl, 0x7 + 2);
            AsmHelper.WriteRelativeOffsets(codeBytes, [
                (code, spEffectParams, 7, 0x0 + 3),
                (code + 0x15, Functions.SetSpEffect, 5, 0x15 + 1)
            ]);

            memoryService.WriteBytes(code, codeBytes);
        }

        private void WriteVanillaApplySpEffect(nint code, nint chrSpEffectCtrl, nint spEffectParams)
        {
            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.SetSpEffect32);
            AsmHelper.WriteAbsoluteAddress32(codeBytes, spEffectParams, 0x3 + 2);
            AsmHelper.WriteAbsoluteAddress32(codeBytes, chrSpEffectCtrl, 0xA + 1);
            AsmHelper.WriteRelativeOffsets(codeBytes, [
                (code + 0xF, Functions.SetSpEffect, 5, 0xF + 1)
            ]);

            memoryService.WriteBytes(code, codeBytes);
        }
    }
}
