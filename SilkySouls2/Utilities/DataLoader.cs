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
                    
                    if (parts[3].Contains("|"))
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
                    else
                    {
                        location.EventObjId = int.Parse(parts[3], NumberStyles.Integer, CultureInfo.InvariantCulture);
                    }


                    if (parts.Length > 4 && !string.IsNullOrWhiteSpace(parts[4]))
                    {
                        location.EventObjId = int.Parse(parts[4], NumberStyles.Integer, CultureInfo.InvariantCulture);
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
        
        
        public static List<NpcInfo> GetNpcs()
        {
            List<NpcInfo> npcList = new List<NpcInfo>();
            string csvData = Properties.Resources.NPC;
            
            if (string.IsNullOrWhiteSpace(csvData))
                return npcList;
                
            using (StringReader reader = new StringReader(csvData))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue;
                    
                    string[] parts = line.Split(',');
                    if (parts.Length < 4) continue;
                    
                    string npcName = parts[0].Trim();
                    int deathFlag = int.Parse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture);
                    int hostileFlag = int.Parse(parts[2], NumberStyles.Integer, CultureInfo.InvariantCulture);
                    int majulaFlag = int.Parse(parts[3], NumberStyles.Integer, CultureInfo.InvariantCulture);
                    
                    NpcInfo npc = new NpcInfo(
                        npcName,
                        deathFlag,
                        hostileFlag,
                        majulaFlag
                    );
                    
                    npcList.Add(npc);
                }
            }
            
            return npcList;
        }
    }
}