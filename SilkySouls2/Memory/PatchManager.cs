// 

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using SilkySouls2.Interfaces;
using SilkySouls2.Services;

namespace SilkySouls2.Memory
{
    public enum GameEdition
    {
        Vanilla,
        Scholar
    }

    public enum Patch
    {
        Vanilla1_0_11,
        Vanilla1_0_12,
        Scholar1_0_2,
        Scholar1_0_3
    }

    public class GameVersionInfo
    {
        public GameEdition Edition { get; set; }
        public Patch PatchVersion { get; set; }
    }

    public static class PatchManager
    {
        public static GameVersionInfo Current { get; private set; }

        private static readonly Dictionary<long, GameVersionInfo> VersionMap = new Dictionary<long, GameVersionInfo>
        {
            { 32340760, new GameVersionInfo { Edition = GameEdition.Vanilla, PatchVersion = Patch.Vanilla1_0_11 } },
            { 29588960, new GameVersionInfo { Edition = GameEdition.Vanilla, PatchVersion = Patch.Vanilla1_0_12 } },
            { 31605096, new GameVersionInfo { Edition = GameEdition.Scholar, PatchVersion = Patch.Scholar1_0_2 } },
            { 28200992, new GameVersionInfo { Edition = GameEdition.Scholar, PatchVersion = Patch.Scholar1_0_3 } }
        };
        
        public static bool IsScholar() => Current.Edition == GameEdition.Scholar;

        public static bool IsInitialized { get; set; }

        public static bool TryDetectVersion(IMemoryService memoryService)
        {
            try
            {
                var module = memoryService.TargetProcess.MainModule;
                if (module == null) return false;
        
                var fileInfo = new FileInfo(module.FileName);
                var fileSize = fileInfo.Length;
        
                Console.WriteLine($"FileVersion: {module.FileVersionInfo.FileVersion}");
                Console.WriteLine($"FileSize: {fileSize}");

                if (!VersionMap.TryGetValue(fileSize, out var version)) return false;
        
                Current = version;
        
                IsInitialized = Offsets.InitializeStatics(memoryService.BaseAddress);
                return IsInitialized;
            }
            catch (Win32Exception)
            {
                return false;
            }
            catch (InvalidOperationException)
            {
                // Process has exited
                return false;
            }
        }
    }
    
  
}