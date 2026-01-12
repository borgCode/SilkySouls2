using System;
using SilkySouls2.enums;
using SilkySouls2.Interfaces;
using SilkySouls2.Memory;
using SilkySouls2.Utilities;
using static SilkySouls2.Memory.Offsets;

namespace SilkySouls2.Services
{
    public class SettingsService(IMemoryService memoryService, HookManager hookManager) : ISettingsService
    {
        public void Quitout() =>
            memoryService.WriteByte((IntPtr)memoryService.ReadInt64(GameManagerImp.Base) + GameManagerImp.Quitout, 6);

        public void ToggleFastQuitout(bool isEnabled)
        {
            var code = CodeCaveOffsets.Base + CodeCaveOffsets.FastQuitout;

            if (!isEnabled)
            {
                hookManager.UninstallHook(code.ToInt64());
                return;
            }

            var origin = Hooks.FasterMenu;

            if (PatchManager.IsScholar())
            {
                InstallScholarFasterMenu(code, origin);
            }
            else
            {
                InstallVanillaFasterMenu(code, origin);
            }
        }

        private void InstallScholarFasterMenu(IntPtr code, long origin)
        {
            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.FasterMenu64);
            var jmpBytes = AsmHelper.GetJmpOriginOffsetBytes(origin, 8, code + 0x1A);
            Array.Copy(jmpBytes, 0, codeBytes, 0x15 + 1, 4);

            memoryService.WriteBytes(code, codeBytes);
            hookManager.InstallHook(code.ToInt64(), origin, [0x48, 0x8B, 0x9C, 0x24, 0x78, 0x01, 0x00, 0x00]);
        }

        private void InstallVanillaFasterMenu(IntPtr code, long origin)
        {
            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.FasterMenu32);
            var jmpBytes = AsmHelper.GetJmpOriginOffsetBytes(origin, 5, code + 0x14);
            Array.Copy(jmpBytes, 0, codeBytes, 0xF + 1, 4);
            memoryService.WriteBytes(code, codeBytes);
            hookManager.InstallHook(code.ToInt64(), origin, [0x33, 0xC5, 0x89, 0x45, 0xFC]);
        }

        public void ToggleBabyJumpFix(bool isEnabled)
        {
            var code = CodeCaveOffsets.Base + CodeCaveOffsets.BabyJump;

            if (!isEnabled)
            {
                hookManager.UninstallHook(code.ToInt64());
                return;
            }
            
            var origin = Hooks.BabyJump;

            if (PatchManager.IsScholar())
            {
                InstallScholarBabyJumpFix(code, origin);
            }
            else
            {
                InstallVanillaBabyJumpFix(code, origin);
            }
        }
        
        private void InstallScholarBabyJumpFix(IntPtr code, long origin)
        {
            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.BabyJump64);
            var bytes = BitConverter.GetBytes(GameManagerImp.Base.ToInt64());
            Array.Copy(bytes, 0, codeBytes, 0x1 + 2, 8);
            bytes = AsmHelper.GetJmpOriginOffsetBytes(origin, 5, code + 0x33);
            Array.Copy(bytes, 0, codeBytes, 0x2E + 1, 4);

            memoryService.WriteBytes(code, codeBytes);
            hookManager.InstallHook(code.ToInt64(), origin, [0x0F, 0x29, 0x44, 0x24, 0x20]);
        }
        
        private void InstallVanillaBabyJumpFix(IntPtr code, long origin)
        {
            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.BabyJump32);
            var bytes = BitConverter.GetBytes(GameManagerImp.Base.ToInt32());
            Array.Copy(bytes, 0, codeBytes, 0x4 + 1, 4);
            bytes = AsmHelper.GetJmpOriginOffsetBytes(origin, 7, code + 0x24);
            Array.Copy(bytes, 0, codeBytes, 0x1F + 1, 4);
            memoryService.WriteBytes(code, codeBytes);
            hookManager.InstallHook(code.ToInt64(), origin, [0x0F, 0x51, 0xC0, 0x0F, 0x29, 0x45, 0xB0]);
        }

        public void ToggleDoubleClick(bool isDisableDoubleClickEnabled)
        {
            var ptr = memoryService.FollowPointers(KatanaMainApp.Base, KatanaMainApp.DoubleClickPtrChain, false);
            memoryService.Write(ptr, isDisableDoubleClickEnabled);
        }
    }
}