// 

using System;
using SilkySouls2.Memory;
using SilkySouls2.Services;

namespace SilkySouls2.Utilities
{
    public static class VersionManager
    {
        public static bool Initialize(MemoryService memoryService)
        {
            if (memoryService.TargetProcess == null) return false;
            var module = memoryService.TargetProcess.MainModule;
            var fileVersion = module?.FileVersionInfo.FileVersion;
            var moduleBase = memoryService.BaseAddress;
        
            Console.WriteLine($@"Patch: {fileVersion}");

            
            // Offsets.Initialize(fileVersion, moduleBase);
            return false;
        }
    }
}