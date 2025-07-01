using System;
using System.IO;

namespace SilkySouls2.Utilities
{
    public class SettingsManager
    {
        private static SettingsManager _default;
        public static SettingsManager Default => _default ?? (_default = Load());

        public string HotkeyActionIds { get; set; } = "";
        public bool BabyJump { get; set; } 
        public bool EnableHotkeys { get; set; }
        public bool FastQuitout { get; set; }
        public bool AlwaysOnTop { get; set; }
        public double WindowLeft { get; set; }
        public double WindowTop { get; set; }
        public double ResistancesWindowScaleX { get; set; } = 1.0;
        public double ResistancesWindowScaleY { get; set; } = 1.0;
        public double ResistancesWindowWidth { get; set; }
        public double ResistancesWindowLeft { get; set; }
        public double ResistancesWindowTop { get; set; }
        public double AttunementWindowLeft { get; set; }
        public double AttunementWindowTop { get; set; }
        public double DefenseWindowLeft { get; set; }
        public double DefenseWindowTop { get; set; }
        public double HealthWindowLeft { get; set; }
        public double HealthWindowTop { get; set; }
        public bool EnableUpdateChecks { get; set; } = true;

        private static string SettingsPath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "SilkySouls2",
            "settings.txt");

        public void Save()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath));
                
                var lines = new[]
                {
                    $"HotkeyActionIds={HotkeyActionIds}",
                    $"BabyJump={BabyJump}",
                    $"EnableHotkeys={EnableHotkeys}",
                    $"FastQuitout={FastQuitout}",
                    $"AlwaysOnTop={AlwaysOnTop}",
                    $"WindowLeft={WindowLeft}",
                    $"WindowTop={WindowTop}",
                    $"ResistancesWindowScaleX={ResistancesWindowScaleX}",
                    $"ResistancesWindowScaleY={ResistancesWindowScaleY}",
                    $"ResistancesWindowWidth={ResistancesWindowWidth}",
                    $"ResistancesWindowLeft={ResistancesWindowLeft}",
                    $"ResistancesWindowTop={ResistancesWindowTop}",
                    $"AttunementWindowLeft={AttunementWindowLeft}",
                    $"AttunementWindowTop={AttunementWindowTop}",
                    $"DefenseWindowLeft={DefenseWindowLeft}",
                    $"DefenseWindowTop={DefenseWindowTop}",
                    $"HealthWindowLeft={HealthWindowLeft}",
                    $"HealthWindowTop={HealthWindowTop}",
                    $"EnableUpdateChecks={EnableUpdateChecks}",
                };

                File.WriteAllLines(SettingsPath, lines);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving settings: {ex.Message}");
            }
        }

        private static SettingsManager Load()
        {
            var settings = new SettingsManager();

            if (File.Exists(SettingsPath))
            {
                try
                {
                    foreach (var line in File.ReadAllLines(SettingsPath))
                    {
                        var parts = line.Split(new[] { '=' }, 2);
                        if (parts.Length == 2)
                        {
                            var key = parts[0];
                            var value = parts[1];

                            switch (key)
                            {
                                case "HotkeyActionIds": settings.HotkeyActionIds = value; break;
                                case "EnableHotkeys":
                                    bool.TryParse(value, out bool eh);
                                    settings.EnableHotkeys = eh;
                                    break;
                                case "BabyJump":
                                    bool.TryParse(value, out bool bj);
                                    settings.BabyJump = bj;
                                    break;
                                case "FastQuitout":
                                    bool.TryParse(value, out bool fq);
                                    settings.FastQuitout = fq;
                                    break; 
                                case "AlwaysOnTop":
                                    bool.TryParse(value, out bool aot);
                                    settings.AlwaysOnTop = aot;
                                    break;
                                case "WindowLeft":
                                    double.TryParse(value, out double wl);
                                    settings.WindowLeft = wl;
                                    break;
                                case "WindowTop":
                                    double.TryParse(value, out double wt);
                                    settings.WindowTop = wt;
                                    break;
                                case "ResistancesWindowScaleX":
                                    double.TryParse(value, out double rwx);
                                    settings.ResistancesWindowScaleX = rwx;
                                    break;
                                case "ResistancesWindowScaleY":
                                    double.TryParse(value, out double rwy);
                                    settings.ResistancesWindowScaleY = rwy;
                                    break;
                                case "ResistancesWindowLeft":
                                    double.TryParse(value, out double rwl);
                                    settings.ResistancesWindowLeft = rwl;
                                    break;
                                case "ResistancesWindowTop":
                                    double.TryParse(value, out double rwt);
                                    settings.ResistancesWindowTop = rwt;
                                    break;
                                case "ResistancesWindowWidth":
                                    double.TryParse(value, out double rww);
                                    settings.ResistancesWindowWidth = rww;
                                    break;
                                case "HealthWindowLeft":
                                    double.TryParse(value, out double hwl);
                                    settings.HealthWindowLeft = hwl;
                                    break;
                                case "HealthWindowTop":
                                    double.TryParse(value, out double hwt);
                                    settings.HealthWindowTop = hwt;
                                    break;
                                case "AttunementWindowLeft":
                                    double.TryParse(value, out double awl);
                                    settings.AttunementWindowLeft = awl;
                                    break;
                                case "AttunementWindowTop":
                                    double.TryParse(value, out double awt);
                                    settings.AttunementWindowTop = awt;
                                    break;
                                case "DefenseWindowLeft":
                                    double.TryParse(value, out double dwl);
                                    settings.DefenseWindowLeft = dwl;
                                    break;
                                case "DefenseWindowTop":
                                    double.TryParse(value, out double dwt);
                                    settings.DefenseWindowTop = dwt;
                                    break;
                                case "EnableUpdateChecks":
                                    bool.TryParse(value, out bool euc);
                                    settings.EnableUpdateChecks = euc;
                                    break;
                            }
                        }
                    }
                }
                catch
                {
                    // Return default settings on error
                }
            }

            return settings;
        }
    }
}