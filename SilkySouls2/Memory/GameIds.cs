namespace SilkySouls2.Memory
{
    public static class GameIds
    {
        
   

        public static class EventFlags
        {
            public const long KingsRingAcquired = 100804;
            public const long GiantLordDefeated = 100972;
            public static readonly long[] UnlockAldia = { 100747, 100978 };
            public static readonly long[] Dlc3Ice = { 537000001, 537000011 };
            public const long VisibleAava = 537000012;
            public const long FrigidSnowstorm = 537010014;
            public static readonly long[] Dlc3Knights = { 537000020, 537000021 };
            public static readonly long[] DarklurkerDungeonsLit = { 403000001, 403000002, 403000003 };
            public static readonly long[] Scepter = { 536000024, 536000010 };
        }

        public static class NpcMenu
        {
            public const long Chloanne = 76200000;
        }
        
        
        public static class SpEffects
        {
            public struct SpEffectData
            {
                public int EffectId;
                public int Quantity;
                public float FloatValue;
                public byte EffectType;
                public byte Param1;
                public byte Param2;
                public byte Param3;

                public static readonly SpEffectData RestoreHumanity = new SpEffectData
                {
                    EffectId = 60151000,
                    Quantity = 1,
                    FloatValue = -1.0f,
                    EffectType = 0x19,
                    Param1 = 0x02,
                    Param2 = 0x00,
                    Param3 = 0x00
                };

                public static readonly SpEffectData BonfireRest = new SpEffectData
                {
                    EffectId = 110000010,
                    Quantity = 1,
                    FloatValue = -1.0f,
                    EffectType = 0x19,
                    Param1 = 0x02,
                    Param2 = 0x00,
                    Param3 = 0x00
                };
            }
        }
    }
}