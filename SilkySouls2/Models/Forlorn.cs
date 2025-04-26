namespace SilkySouls2.Models
{
    public class Forlorn
    {
        public string AreaName { get; }
        public int EsdFuncId { get; }
        public string[] SpawnNames { get; }

        public Forlorn( string areaName, int esdFuncId, string[] spawnNames)
        {
            AreaName = areaName;
            EsdFuncId = esdFuncId;
            SpawnNames = spawnNames;
        }

        public static readonly Forlorn LostBastille = new Forlorn( "The Lost Bastille", 4017000,
            new[] { "Exile Holdings Cell", "McDuff", "Sinner Forlorn" });

        public static readonly Forlorn EarthenPeak = new Forlorn( "Harvest Valley / Earthen Peak", 4002000,
            new[]
            {
                "Poison Pools", "Poison Pools #2", "Pre-Covetous",
                "Pre-Covetous #2", "Earthen Peak", "Pre-Mytha"
            });

        public static readonly Forlorn IronKeep = new Forlorn( "Iron Keep", 4021000,
            new[] { "Iron Keep", "Post-Smelter bridge" });

        public static readonly Forlorn ShadedWoods = new Forlorn( "Shaded Woods", 4011000,
            new[] { "Fog forest", "Shaded Ruins" });

        public static readonly Forlorn Tseldora = new Forlorn( "Tseldora", 4012000,
            new[] { "Ornifex", "Titanite Chunks", "Spider Room", "Freja" });

        public static readonly Forlorn BlackGulch = new Forlorn("Black Gulch", 4006000,
            new[] { "Black Gulch" });

        public static readonly Forlorn Drangleic = new Forlorn( "Drangleic Castle", 4012000,
            new[] { "Drangleic Gate", "Left-side Entrance Hall", "Pre-Dragonriders" });

        public static readonly Forlorn UndeadCrypt = new Forlorn( "Undead Crypt", 4020000,
            new[] { "Forlorn #1", "Forlorn #2" });

        public static readonly Forlorn ForestOftheGiants = new Forlorn( "Forest of the Fallen Giants", 4001000,
            new[] { "Pre-Cardinal Tower", "King's Door", "Pre-pursuer", "Vammar Tree", });

        public static readonly Forlorn Shulva = new Forlorn("Shulva, Sanctum City", 4000000,
            new[] { "Bridge", "Spinning Door", "Ghost Room", "Lair of the Imperfect", "Pre-Elana" });

        public static readonly Forlorn Brume = new Forlorn("Brume Tower", 4000000,
            new[] { "Dex Ring", "Foyer", "Fume Knight Bridge", });

        public static readonly Forlorn EleumLoyce = new Forlorn( "Eleum Loyce", 4000000,
            new[] { "Pre-Fountain", "Ballistas", "Post-Covetous" });

        
        public static readonly Forlorn[] All =
        {
            LostBastille, EarthenPeak, IronKeep, ShadedWoods, Tseldora,
            BlackGulch, Drangleic, UndeadCrypt, ForestOftheGiants, Shulva,
            Brume, EleumLoyce
        };
    }
}