using System.Collections.Generic;
using System.Globalization;
using System.IO;
using SilkySouls2.Models;

namespace SilkySouls2.Utilities
{
     public static class DataLoader
    {
      
        public static Dictionary<string, List<Bonfire>> GetBonfires()
        {
            Dictionary<string, List<Bonfire>> warpDict = new Dictionary<string, List<Bonfire>>();
            string csvData = Properties.Resources.Bonfires;

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
                    
                    Bonfire location = new Bonfire
                    {
                        BonfireId = bonfireId,
                        MainArea = mainAreaName,
                        BonfireName = bonfireName
                    };
                    
                    if (!warpDict.ContainsKey(mainAreaName))
                    {
                        warpDict[mainAreaName] = new List<Bonfire>();
                    }
                    
                    warpDict[mainAreaName].Add(location);
                }
            }

            return warpDict;
        }
    }
}