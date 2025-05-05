﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using Microsoft.Win32;

namespace SilkySouls2.Utilities
{
    public static class GameLauncher
    {
        public static bool IsDlc2Available { get; private set; } = true;
        
        public static void LaunchDarkSouls2()
        {
            try
            {
                string exePath = GetDarkSouls2ExePath();
                if (exePath == null)
                {
                    return;
                }

                var process = new Process { StartInfo = new ProcessStartInfo(exePath) };
                process.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to launch Dark Souls II: {ex.Message}",
                    "Launch Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private static string GetDarkSouls2ExePath()
        {
            try
            {
                string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "SilkySouls2");
                string configFile = Path.Combine(appDataPath, "config.txt");

                if (File.Exists(configFile))
                {
                    string savedPath = File.ReadAllText(configFile);
                    if (File.Exists(savedPath))
                        return savedPath;
                }

                string steamPath =
                    Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Valve\Steam", "InstallPath",
                        null) as string;
                if (string.IsNullOrEmpty(steamPath))
                    throw new FileNotFoundException("Steam installation path not found in registry.");

                string configPath = Path.Combine(steamPath, @"steamapps\libraryfolders.vdf");
                if (!File.Exists(configPath))
                    throw new FileNotFoundException($"Steam library configuration not found at {configPath}");

                var paths = new List<string> { steamPath };
                var regex = new Regex(@"""path""\s*""(.+?)""");

                foreach (var line in File.ReadLines(configPath))
                {
                    var match = regex.Match(line);
                    if (match.Success) paths.Add(match.Groups[1].Value.Replace(@"\\", @"\"));
                }

                foreach (var path in paths)
                {
                    string fullPath = Path.Combine(path, @"steamapps\common\Dark Souls II Scholar of the First Sin\Game\DarkSoulsII.exe");
                    if (File.Exists(fullPath)) return fullPath;
                }

                var result = MessageBox.Show(
                    "Dark Souls II executable could not be found automatically.\n\n" +
                    "Please select DarkSoulsII.exe manually.\n\n" +
                    "Note: Certain features will not work unless the correct executable is selected.",
                    "Executable Not Found",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.OK)
                {
                    var openFileDialog = new OpenFileDialog
                    {
                        Title = "Select Dark Souls II Executable",
                        Filter = "Executable Files (*.exe)|*.exe",
                        InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                    };

                    if (openFileDialog.ShowDialog() != true) return null;
                    string selectedPath = openFileDialog.FileName;
                    if (Path.GetFileName(selectedPath).Equals("DarkSoulsII.exe", StringComparison.OrdinalIgnoreCase))
                    {
                        Directory.CreateDirectory(appDataPath);
                        File.WriteAllText(configFile, selectedPath);
                        return selectedPath;
                    }

                    MessageBox.Show("Please select DarkSoulsII.exe.", "Invalid File", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return null;
                }
                return null;
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error finding Dark Souls II: {ex.Message}", "Game Not Found", MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);
                return null;
            }
        }
    }
}