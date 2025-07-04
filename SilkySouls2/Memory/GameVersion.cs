﻿using System;
using System.Collections.Generic;

namespace SilkySouls2.Memory
{
    public enum GameEdition
    {
        Vanilla,
        Scholar
    }

    public class GameVersionInfo
    {
        public GameEdition Edition { get; set; }
        public string PatchVersion { get; set; }
        public long FileSize { get; set; }
    }

    public static class GameVersion
    {
        public static GameVersionInfo Current { get; private set; }

        private static readonly Dictionary<long, GameVersionInfo> VersionMap = new Dictionary<long, GameVersionInfo>
        {
            { 32340760, new GameVersionInfo { Edition = GameEdition.Vanilla, PatchVersion = "1.11" } }, 
            { 29588960, new GameVersionInfo { Edition = GameEdition.Vanilla, PatchVersion = "1.12" } },
            { 28200992, new GameVersionInfo { Edition = GameEdition.Scholar, PatchVersion = "1.03" } }
        };

        public static bool TryDetectVersion(long fileSize)
        {
            Console.WriteLine($"Detected file size: {fileSize}");
            if (VersionMap.TryGetValue(fileSize, out var version))
            {
                Current = version;
                Current.FileSize = fileSize;
                return true;
            }
            return false;
        }
    }
}