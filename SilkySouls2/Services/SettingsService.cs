using System;
using H.Hooks;
using SilkySouls2.Memory;
using SilkySouls2.Utilities;
using static SilkySouls2.Memory.Offsets;

namespace SilkySouls2.Services
{
    public class SettingsService
    {
        private readonly MemoryIo _memoryIo;
        private readonly HookManager _hookManager;
        
        public SettingsService(MemoryIo memoryIo, HookManager hookManager)
        {
            _memoryIo = memoryIo;
            _hookManager = hookManager;
        }
        
        public void Quitout() =>
            _memoryIo.WriteByte((IntPtr)_memoryIo.ReadInt64(GameManagerImp.Base) + GameManagerImp.Offsets.MenuKick, 6);
        
        public void ToggleFastQuitout(bool isEnabled)
        {
            var code = CodeCaveOffsets.Base + CodeCaveOffsets.FastQuitout;

            if (isEnabled)
            {
                var origin = Hooks.FastQuitout;
                var codeBytes = AsmLoader.GetAsmBytes("FastQuitout");
                var jmpBytes = AsmHelper.GetJmpOriginOffsetBytes(origin, 8, code + 0x1A);
                Array.Copy(jmpBytes, 0, codeBytes, 0x15 + 1, 4);
                
                _memoryIo.WriteBytes(code, codeBytes);
                _hookManager.InstallHook(code.ToInt64(), origin, new byte[]
                    { 0x48, 0x8B, 0x9C, 0x24, 0x78, 0x01, 0x00, 0x00 });
            }
            else
            {
                _hookManager.UninstallHook(code.ToInt64());
            }
        }
    }
}