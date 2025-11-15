using System.Collections.Generic;
using SilkySouls2.Memory;

namespace SilkySouls2.Models
{
    public class ForlornSpawn
    {
        public string LocationName { get; }
        public GameIds.EzStateEventCalls.EzStateEventCommand OverridePositionCommand { get; }
        public GameIds.EzStateEventCalls.EzStateEventCommand GenerateNpcCommand { get; }

        public ForlornSpawn(string locationName, GameIds.EzStateEventCalls.EzStateEventCommand positionCommand,
            GameIds.EzStateEventCalls.EzStateEventCommand generateNpcCommand)
        {
            LocationName = locationName;
            OverridePositionCommand = positionCommand;
            GenerateNpcCommand = generateNpcCommand;
        }
    }


    public class ForlornArea
    {
        public string AreaName { get; }
        public int AreaId { get; }
        public int AreaIndex { get; }
        public int FunctionId { get; }
        public List<ForlornSpawn> Spawns { get; }

        public ForlornArea(string areaName, int areaId, int areaIndex, int functionId, params ForlornSpawn[] spawns)
        {
            AreaName = areaName;
            AreaId = areaId;
            AreaIndex = areaIndex;
            FunctionId = functionId;
            Spawns = new List<ForlornSpawn>(spawns);
        }

        public static readonly ForlornArea LostBastille = new ForlornArea(
            "The Lost Bastille",
            0xA100000,
            4,
            4017000,
            new ForlornSpawn("Exile Holdings Cell",
                new GameIds.EzStateEventCalls.EzStateEventCommand(131704, 932, 80000001, 80000099),
                new GameIds.EzStateEventCalls.EzStateEventCommand(131741, 932)),
            new ForlornSpawn("McDuff",
                new GameIds.EzStateEventCalls.EzStateEventCommand(131704, 932, 80000101, 80000199),
                new GameIds.EzStateEventCalls.EzStateEventCommand(131741, 932)),
            new ForlornSpawn("Sinner Forlorn",
                new GameIds.EzStateEventCalls.EzStateEventCommand(131704, 932, 80000201, 80000299),
                new GameIds.EzStateEventCalls.EzStateEventCommand(131741, 932))
        );

        public static readonly ForlornArea EarthenPeak = new ForlornArea(
            "Harvest Valley / Earthen Peak",
            0xA110000,
            5,
            4002000,
            new ForlornSpawn("Poison Pools",
                new GameIds.EzStateEventCalls.EzStateEventCommand(131704, 935, 80000001, 80000099),
                new GameIds.EzStateEventCalls.EzStateEventCommand(131741, 935)),
            new ForlornSpawn("Poison Pools #2",
                new GameIds.EzStateEventCalls.EzStateEventCommand(131704, 935, 80000101, 80000199),
                new GameIds.EzStateEventCalls.EzStateEventCommand(131741, 935)),
            new ForlornSpawn("Pre-Covetous",
                new GameIds.EzStateEventCalls.EzStateEventCommand(131704, 935, 80000201, 80000299),
                new GameIds.EzStateEventCalls.EzStateEventCommand(131741, 935)),
            new ForlornSpawn("Pre-Covetous #2",
                new GameIds.EzStateEventCalls.EzStateEventCommand(131704, 935, 80000301, 80000399),
                new GameIds.EzStateEventCalls.EzStateEventCommand(131741, 935)),
            new ForlornSpawn("Earthen Peak",
                new GameIds.EzStateEventCalls.EzStateEventCommand(131704, 935, 80000401, 80000499),
                new GameIds.EzStateEventCalls.EzStateEventCommand(131741, 935)),
            new ForlornSpawn("Pre-Mytha",
                new GameIds.EzStateEventCalls.EzStateEventCommand(131704, 935, 80000501, 80000599),
                new GameIds.EzStateEventCalls.EzStateEventCommand(131741, 935))
        );

        public static readonly ForlornArea IronKeep = new ForlornArea(
            "Iron Keep",
            0xA130000,
            7,
            4021000,
            new ForlornSpawn("Iron Keep",
                new GameIds.EzStateEventCalls.EzStateEventCommand(131704, 941, 80000001, 80000099),
                new GameIds.EzStateEventCalls.EzStateEventCommand(131741, 941)),
            new ForlornSpawn("Post-Smelter bridge",
                new GameIds.EzStateEventCalls.EzStateEventCommand(131704, 941, 80000101, 80000199),
                new GameIds.EzStateEventCalls.EzStateEventCommand(131741, 941))
        );

        public static readonly ForlornArea ShadedWoods = new ForlornArea(
            "Shaded Woods",
            0xA200000,
            13,
            4011000,
            new ForlornSpawn("Fog forest",
                new GameIds.EzStateEventCalls.EzStateEventCommand(131704, 956, 80000001, 80000099),
                new GameIds.EzStateEventCalls.EzStateEventCommand(131741, 956)),
            new ForlornSpawn("Shaded Ruins",
                new GameIds.EzStateEventCalls.EzStateEventCommand(131704, 956, 80000101, 80000199),
                new GameIds.EzStateEventCalls.EzStateEventCommand(131741, 956))
        );

        public static readonly ForlornArea Tseldora = new ForlornArea(
            "Tseldora",
            0xA0E0000,
            3,
            4012000,
            new ForlornSpawn("Ornifex",
                new GameIds.EzStateEventCalls.EzStateEventCommand(131704, 926, 80000001, 80000099),
                new GameIds.EzStateEventCalls.EzStateEventCommand(131741, 926)),
            new ForlornSpawn("Titanite Chunks",
                new GameIds.EzStateEventCalls.EzStateEventCommand(131704, 926, 80000101, 80000199),
                new GameIds.EzStateEventCalls.EzStateEventCommand(131741, 926)),
            new ForlornSpawn("Spider Room",
                new GameIds.EzStateEventCalls.EzStateEventCommand(131704, 926, 80000201, 80000299),
                new GameIds.EzStateEventCalls.EzStateEventCommand(131741, 926)),
            new ForlornSpawn("Freja",
                new GameIds.EzStateEventCalls.EzStateEventCommand(131704, 926, 80000301, 80000399),
                new GameIds.EzStateEventCalls.EzStateEventCommand(131741, 926))
        );

        public static readonly ForlornArea BlackGulch = new ForlornArea(
            "Black Gulch",
            0xA190000,
            10,
            4006000,
            new ForlornSpawn("Black Gulch",
                new GameIds.EzStateEventCalls.EzStateEventCommand(131704, 947, 80000001, 80000099),
                new GameIds.EzStateEventCalls.EzStateEventCommand(131741, 947))
        );

        public static readonly ForlornArea Drangleic = new ForlornArea(
            "Drangleic Castle",
            0x14150000,
            19,
            4012000,
            new ForlornSpawn("Drangleic Gate",
                new GameIds.EzStateEventCalls.EzStateEventCommand(131704, 968, 80000001, 80000099),
                new GameIds.EzStateEventCalls.EzStateEventCommand(131741, 968)),
            new ForlornSpawn("Pre-Dragonriders",
                new GameIds.EzStateEventCalls.EzStateEventCommand(131704, 968, 80000101, 80000199),
                new GameIds.EzStateEventCalls.EzStateEventCommand(131741, 968)),
            new ForlornSpawn("Left-side Entrance Hall",
                new GameIds.EzStateEventCalls.EzStateEventCommand(131704, 968, 80000201, 80000299),
                new GameIds.EzStateEventCalls.EzStateEventCommand(131741, 968))
        );

        public static readonly ForlornArea ShrineOfAmana = new ForlornArea(
            "Shrine of Amana",
            0x140B0000,
            17,
            4001000,
            new ForlornSpawn("Tower of Prayer",
                new GameIds.EzStateEventCalls.EzStateEventCommand(131704, 965, 80000001, 80000099),
                new GameIds.EzStateEventCalls.EzStateEventCommand(131741, 965)),
            new ForlornSpawn("Building by Hippo",
                new GameIds.EzStateEventCalls.EzStateEventCommand(131704, 965, 80000101, 80000199),
                new GameIds.EzStateEventCalls.EzStateEventCommand(131741, 965)),
            new ForlornSpawn("Crumbled Ruins",
                new GameIds.EzStateEventCalls.EzStateEventCommand(131704, 965, 80000201, 80000299),
                new GameIds.EzStateEventCalls.EzStateEventCommand(131741, 965))
        );

        public static readonly ForlornArea UndeadCrypt = new ForlornArea(
            "Undead Crypt",
            0x14180000,
            20,
            4020000,
            new ForlornSpawn("Forlorn #1",
                new GameIds.EzStateEventCalls.EzStateEventCommand(131704, 971, 80000001, 80000099),
                new GameIds.EzStateEventCalls.EzStateEventCommand(131741, 971)),
            new ForlornSpawn("Forlorn #2",
                new GameIds.EzStateEventCalls.EzStateEventCommand(131704, 971, 80000101, 80000199),
                new GameIds.EzStateEventCalls.EzStateEventCommand(131741, 971))
        );

        public static readonly ForlornArea ForestOfTheGiants = new ForlornArea(
            "Forest of the Fallen Giants",
            0xA0A0000,
            2,
            4001000,
            new ForlornSpawn("Pre-Cardinal Tower",
                new GameIds.EzStateEventCalls.EzStateEventCommand(131704, 923, 80000001, 80000099),
                new GameIds.EzStateEventCalls.EzStateEventCommand(131741, 923)),
            new ForlornSpawn("King's Door",
                new GameIds.EzStateEventCalls.EzStateEventCommand(131704, 923, 80000101, 80000199),
                new GameIds.EzStateEventCalls.EzStateEventCommand(131741, 923)),
            new ForlornSpawn("Pre-pursuer",
                new GameIds.EzStateEventCalls.EzStateEventCommand(131704, 923, 80000201, 80000299),
                new GameIds.EzStateEventCalls.EzStateEventCommand(131741, 923)),
            new ForlornSpawn("Vammar Tree",
                new GameIds.EzStateEventCalls.EzStateEventCommand(131704, 923, 80000301, 80000399),
                new GameIds.EzStateEventCalls.EzStateEventCommand(131741, 923))
        );

        public static readonly ForlornArea Shulva = new ForlornArea(
            "Shulva, Sanctum City",
            0x32230000,
            34,
            4000000,
            new ForlornSpawn("Bridge",
                new GameIds.EzStateEventCalls.EzStateEventCommand(131704, 974, 80000001, 80000099),
                new GameIds.EzStateEventCalls.EzStateEventCommand(131741, 974)),
            new ForlornSpawn("Spinning Door",
                new GameIds.EzStateEventCalls.EzStateEventCommand(131704, 974, 80000101, 80000199),
                new GameIds.EzStateEventCalls.EzStateEventCommand(131741, 974)),
            new ForlornSpawn("Ghost Room",
                new GameIds.EzStateEventCalls.EzStateEventCommand(131704, 974, 80000201, 80000299),
                new GameIds.EzStateEventCalls.EzStateEventCommand(131741, 974)),
            new ForlornSpawn("Lair of the Imperfect",
                new GameIds.EzStateEventCalls.EzStateEventCommand(131704, 974, 80000301, 80000399),
                new GameIds.EzStateEventCalls.EzStateEventCommand(131741, 974)),
            new ForlornSpawn("Pre-Elana",
                new GameIds.EzStateEventCalls.EzStateEventCommand(131704, 974, 80000401, 80000499),
                new GameIds.EzStateEventCalls.EzStateEventCommand(131741, 974))
        );

        public static readonly ForlornArea Brume = new ForlornArea(
            "Brume Tower",
            0x32240000,
            35,
            4000000,
            new ForlornSpawn("Dex Ring",
                new GameIds.EzStateEventCalls.EzStateEventCommand(131704, 977, 80000001, 80000099),
                new GameIds.EzStateEventCalls.EzStateEventCommand(131741, 977)),
            new ForlornSpawn("Foyer",
                new GameIds.EzStateEventCalls.EzStateEventCommand(131704, 977, 80000101, 80000199),
                new GameIds.EzStateEventCalls.EzStateEventCommand(131741, 977)),
            new ForlornSpawn("Fume Knight Bridge",
                new GameIds.EzStateEventCalls.EzStateEventCommand(131704, 977, 80000201, 80000299),
                new GameIds.EzStateEventCalls.EzStateEventCommand(131741, 977))
        );

        public static readonly ForlornArea EleumLoyce = new ForlornArea(
            "Eleum Loyce",
            0x32250000,
            36,
            4000000,
            new ForlornSpawn("Pre-Fountain",
                new GameIds.EzStateEventCalls.EzStateEventCommand(131704, 980, 80000001, 80000099),
                new GameIds.EzStateEventCalls.EzStateEventCommand(131741, 980)),
            new ForlornSpawn("Ballistas",
                new GameIds.EzStateEventCalls.EzStateEventCommand(131704, 980, 80000101, 80000199),
                new GameIds.EzStateEventCalls.EzStateEventCommand(131741, 980)),
            new ForlornSpawn("Post-Covetous",
                new GameIds.EzStateEventCalls.EzStateEventCommand(131704, 980, 80000201, 80000299),
                new GameIds.EzStateEventCalls.EzStateEventCommand(131741, 980))
        );

        public static readonly ForlornArea[] All =
        {
            LostBastille, EarthenPeak, IronKeep, ShadedWoods, Tseldora,
            BlackGulch, Drangleic, ShrineOfAmana, UndeadCrypt, ForestOfTheGiants, 
            Shulva, Brume, EleumLoyce
        };
    }
}