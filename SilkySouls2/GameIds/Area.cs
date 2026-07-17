// 

namespace SilkySouls2.GameIds
{
    public class Area
    {
        public int MapId;
        public int MapAreaIndex;

        public static readonly Area Bastille = new() { MapId = 0xA100000, MapAreaIndex = 2 };
        public static readonly Area Wharf = new() { MapId = 0xA120000, MapAreaIndex = 4 };
        public static readonly Area ShadedWoods = new() { MapId = 0xA200000, MapAreaIndex = 13 };
    }
}