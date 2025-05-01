namespace SilkySouls2.Memory
{
    public static class GameIds
    {

        public static class AreaId
        {
            public const int HarvestPeak = 1017000;
            public const int ShadedWoods = 1032000;
        }

        public static class EventFlags
        {
            public const long KingsRingAcquired = 100804;
            public const long GiantLordDefeated = 100972;
            public static readonly long[] UnlockAldia = { 100747, 100978 };
            public const long Dlc3Ice = 537000011;
            public const long VisibleAava = 537000012;
            public static readonly long[] Dlc3Knights = { 537000020, 537000021 };
            public static readonly long[] IvoryBlackKnights = { 537020035, 537020036, 537020037 };
            public static readonly long[] DarklurkerDungeonsLit = { 403000001, 403000002, 403000003 };
            public const long ClearedDrangelicDungeon = 403020011;
        }
    }
}