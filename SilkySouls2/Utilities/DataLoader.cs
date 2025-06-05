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

                    List<int> majulaFlags = new List<int>();
                    string majulaFlagStr = parts[3].Trim();

                    if (majulaFlagStr.Contains("|"))
                    {
                        string[] flagParts = majulaFlagStr.Split('|');
                        foreach (string flagPart in flagParts)
                        {
                            if (int.TryParse(flagPart, NumberStyles.Integer, CultureInfo.InvariantCulture,
                                    out int flagValue))
                            {
                                majulaFlags.Add(flagValue);
                            }
                        }
                    }
                    else if (int.TryParse(majulaFlagStr, NumberStyles.Integer, CultureInfo.InvariantCulture,
                                 out int singleFlag))
                    {
                        majulaFlags.Add(singleFlag);
                    }

                    NpcInfo npc = new NpcInfo(
                        npcName,
                        deathFlag,
                        hostileFlag,
                        majulaFlags.ToArray()
                    );

                    npcList.Add(npc);
                }
            }

            return npcList;
        }

        public static List<Item> GetItemList(string listName)
        {
            List<Item> items = new List<Item>();

            string csvData = Properties.Resources.ResourceManager.GetString(listName);

            if (string.IsNullOrEmpty(csvData)) return new List<Item>();

            using (StringReader reader = new StringReader(csvData))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    string[] parts = line.Split(',');

                    if (parts.Length >= 5)
                    {
                        items.Add(new Item
                        {
                            Id = int.Parse(parts[0]),
                            Name = parts[1],
                            StackSize = int.Parse(parts[2]),
                            MaxUpgrade = int.Parse(parts[3]),
                            InfuseId = int.Parse(parts[4]),
                            CategoryName = listName
                        });
                    }
                }
            }

            return items;
        }

        public static Dictionary<int, int[]> GetInfusions()
        {
            Dictionary<int, int[]> infusionsMap = new Dictionary<int, int[]>();

            string csvData = Properties.Resources.ResourceManager.GetString("Infusions");

            if (string.IsNullOrEmpty(csvData)) return new Dictionary<int, int[]>();

            using (StringReader reader = new StringReader(csvData))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    string[] parts = line.Split(',');

                    if (parts.Length >= 11)
                    {
                        int[] infusions = new int[10]; 
                        for (int i = 1; i < 11; i++)
                        {
                            infusions[i - 1] = int.Parse(parts[i]);
                        }
                        
                        infusionsMap.Add(int.Parse(parts[0]), infusions);
                        
                    }
                }
            }

            return infusionsMap;
        }
    }
}