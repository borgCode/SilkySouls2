// 

namespace SilkySouls2.GameIds;

public readonly struct ShopRange(int start, int end)
{
    public int Start { get; } = start;
    public int End { get; } = end;
}

public readonly struct TradeRange(int start, int end)
{
    public int Start { get; } = start;
    public int End { get; } = end;
}

public static class ShopLineup
{
    public static readonly ShopRange Melentia = new (75400000, 75409999);
    public static readonly ShopRange Gilligan = new (70400000, 70409999);
    public static readonly ShopRange Wellager = new (72110000, 72119999);
    public static readonly ShopRange Grandahl = new (72500000, 72509999);
    public static readonly ShopRange Gavlan = new (72600000, 72609999);
    public static readonly ShopRange RatKing = new (75600000, 75609999);
    public static readonly ShopRange Maughlin = new (76100000, 76109999);
    public static readonly ShopRange Chloanne = new (76200000, 76209999);
    public static readonly ShopRange Rosabeth = new (76300000, 76309999);
    public static readonly ShopRange Lenigrast = new (76400000, 76409999);
    public static readonly ShopRange McDuff = new (76430000, 76439999);
    public static readonly ShopRange Carhillion = new (76600000, 76609999);
    public static readonly ShopRange Straid = new (76800000, 76800999);
    public static readonly ShopRange Licia = new (76900000, 76909999);
    public static readonly ShopRange Felkin = new (77000000, 77009999);
    public static readonly ShopRange Navlaan = new (77100000, 77109999);
    public static readonly ShopRange Magerold = new (77200000, 77209999);
    public static readonly ShopRange Ornifex = new (77600000, 77600999);
    public static readonly ShopRange Shalquoir = new (77700000, 77709999);
    public static readonly ShopRange TitchyGren = new (78300000, 78309999);
    public static readonly ShopRange Cromwell = new (78400000, 78409999);
    public static readonly ShopRange Targray = new (78500000, 78509999);
    public static readonly ShopRange Vengarl = new (30700000, 30709999);
    public static readonly ShopRange Agdayne = new (50600000, 50609999);
}

public static class TradeLineup
{
    public static readonly TradeRange Straid = new (76801000, 76801999);
    public static readonly TradeRange Ornifex = new (77601000, 77601999);
}