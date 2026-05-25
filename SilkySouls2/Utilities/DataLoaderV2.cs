using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using SilkySouls2.Models.V2;
using SilkySouls2.Properties;

namespace SilkySouls2.Utilities
{
    public static class DataLoaderV2
    {
        private static readonly JsonSerializerOptions ReadOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
            Converters = { new JsonStringEnumConverter() }
        };

        private static readonly JsonSerializerOptions WriteOptions = new()
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = { new JsonStringEnumConverter() }
        };

        public static List<WarpEntry> GetShippingEntries()
        {
            var json = Resources.ResourceManager.GetString("WarpLocationsV2");
            if (string.IsNullOrWhiteSpace(json)) return new List<WarpEntry>();
            return JsonSerializer.Deserialize<List<WarpEntry>>(json, ReadOptions) ?? new List<WarpEntry>();
        }

        public static List<WarpEntry> LoadCustomWarps()
        {
            var path = CustomWarpsPath;
            if (!File.Exists(path)) return new List<WarpEntry>();
            try
            {
                var json = File.ReadAllText(path);
                return JsonSerializer.Deserialize<List<WarpEntry>>(json, ReadOptions) ?? new List<WarpEntry>();
            }
            catch
            {
                return new List<WarpEntry>();
            }
        }

        public static void SaveCustomWarps(IEnumerable<WarpEntry> entries)
        {
            var dir = Path.GetDirectoryName(CustomWarpsPath);
            if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);

            var json = JsonSerializer.Serialize(entries.ToList(), WriteOptions);
            File.WriteAllText(CustomWarpsPath, json);
        }

        public static Dictionary<string, List<WarpEntry>> GroupByArea(IEnumerable<WarpEntry> entries) =>
            entries.GroupBy(e => e.Area).ToDictionary(g => g.Key, g => g.ToList());

        private static string CustomWarpsPath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "SilkySouls2",
            "CustomWarpsV2.json");
    }
}
