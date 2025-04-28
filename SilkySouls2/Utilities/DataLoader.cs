using System.Collections.Generic;
using System.Globalization;
using System.IO;
using SilkySouls2.Models;

namespace SilkySouls2.Utilities
{
     public static class DataLoader
    {
      
        public static Dictionary<string, List<WarpLocation>> GetLocations()
        {
            Dictionary<string, List<WarpLocation>> warpDict = new Dictionary<string, List<WarpLocation>>();
            string csvData = Properties.Resources.WarpLocations;

            if (string.IsNullOrWhiteSpace(csvData))
                return warpDict;

            using (StringReader reader = new StringReader(csvData))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue;

                    string[] parts = line.Split(',');
                    if (parts.Length < 3) continue;
                    
                    int bonfireId = int.Parse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture);
                    string mainAreaName = parts[1].Trim();
                    string bonfireName = parts[2].Trim();
                    
                    WarpLocation location = new WarpLocation
                    {
                        BonfireId = bonfireId,
                        MainArea = mainAreaName,
                        LocationName = bonfireName
                    };
                    
                    if (parts.Length > 3 && !string.IsNullOrWhiteSpace(parts[3]))
                    {
                        string[] coordParts = parts[3].Split('|');
                        
                        location.Coordinates = new float[coordParts.Length];
                        
                        for (int i = 0; i < coordParts.Length; i++)
                        {
                            if (!string.IsNullOrWhiteSpace(coordParts[i]))
                            {
                                location.Coordinates[i] = float.Parse(coordParts[i], CultureInfo.InvariantCulture);
                            }
                        }
                    }
                    
                    if (!warpDict.ContainsKey(mainAreaName))
                    {
                        warpDict[mainAreaName] = new List<WarpLocation>();
                    }
                    
                    warpDict[mainAreaName].Add(location);
                }
            }

            return warpDict;
        }
        
    }
}