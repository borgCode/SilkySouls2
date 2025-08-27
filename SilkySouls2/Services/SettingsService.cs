using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
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
            _memoryIo.WriteByte((IntPtr)_memoryIo.ReadInt64(GameManagerImp.Base) + GameManagerImp.Offsets.Quitout, 6);
        
        public void ToggleFastQuitout(bool isEnabled)
        {
            var code = CodeCaveOffsets.Base + CodeCaveOffsets.FastQuitout;

            if (isEnabled)
            {
                var origin = Hooks.FasterMenu;

                if (GameVersion.Current.Edition == GameEdition.Scholar)
                {
                    var codeBytes = AsmLoader.GetAsmBytes("FasterMenu64");
                    var jmpBytes = AsmHelper.GetJmpOriginOffsetBytes(origin, 8, code + 0x1A);
                    Array.Copy(jmpBytes, 0, codeBytes, 0x15 + 1, 4);
                
                    _memoryIo.WriteBytes(code, codeBytes);
                    _hookManager.InstallHook(code.ToInt64(), origin, new byte[]
                        { 0x48, 0x8B, 0x9C, 0x24, 0x78, 0x01, 0x00, 0x00 });
                }
                else
                {
                    var codeBytes = AsmLoader.GetAsmBytes("FasterMenu32");
                    var jmpBytes = AsmHelper.GetJmpOriginOffsetBytes(origin, 5, code + 0x14);
                    Array.Copy(jmpBytes, 0, codeBytes, 0xF + 1, 4);
                    _memoryIo.WriteBytes(code, codeBytes);
                    _hookManager.InstallHook(code.ToInt64(), origin, new byte[]
                        { 0x33, 0xC5, 0x89, 0x45, 0xFC });
                }
            }
            else
            {
                _hookManager.UninstallHook(code.ToInt64());
            }
        }

        public void ToggleBabyJumpFix(bool isEnabled)
        {
            var code = CodeCaveOffsets.Base + CodeCaveOffsets.BabyJump;
            
            if (isEnabled)
            {
                var origin = Hooks.BabyJump;

                if (GameVersion.Current.Edition == GameEdition.Scholar)
                {
                    var codeBytes = AsmLoader.GetAsmBytes("BabyJump64");
                    var bytes = BitConverter.GetBytes(GameManagerImp.Base.ToInt64());
                    Array.Copy(bytes, 0, codeBytes, 0x1 + 2, 8);
                    bytes = AsmHelper.GetJmpOriginOffsetBytes(origin, 5, code + 0x33);
                    Array.Copy(bytes, 0, codeBytes, 0x2E + 1, 4);
                
                    _memoryIo.WriteBytes(code, codeBytes);
                    _hookManager.InstallHook(code.ToInt64(), origin, new byte[]
                        { 0x0F, 0x29, 0x44, 0x24, 0x20 });
                }
                else
                {
                    var codeBytes = AsmLoader.GetAsmBytes("BabyJump32");
                    var bytes = BitConverter.GetBytes(GameManagerImp.Base.ToInt32());
                    Array.Copy(bytes, 0, codeBytes, 0x4 + 1, 4);
                    bytes = AsmHelper.GetJmpOriginOffsetBytes(origin, 7, code + 0x24);
                    Array.Copy(bytes, 0, codeBytes, 0x1F + 1, 4);
                    _memoryIo.WriteBytes(code, codeBytes);
                    _hookManager.InstallHook(code.ToInt64(), origin, new byte[]
                        { 0x0F, 0x51, 0xC0, 0x0F, 0x29, 0x45, 0xB0 });
                }
                
            }
            else
            {
                _hookManager.UninstallHook(code.ToInt64());
            }
        }


        public void ToggleDoubleClick(bool isDisableDoubleClickEnabled)
        {
            var ptr = _memoryIo.FollowPointers(KatanaMainApp.Base, KatanaMainApp.DoubleClickPtrChain, false);
            _memoryIo.WriteByte(ptr, isDisableDoubleClickEnabled ? 1 : 0);
        }
    }
}