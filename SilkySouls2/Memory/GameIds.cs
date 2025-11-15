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
            public const long MadWarriorSpawn = 119020014;
        }

        public static class Area
        {
            public const long Bastille = 0xA100000;
        }

        public static class Obj
        {
            public struct SetObjState
            {
                public int ObjId;
                public int State;
            }

            public static readonly SetObjState SinnerLighting1 = new SetObjState { ObjId = 10161002, State = 71 };
            public static readonly SetObjState SinnerLighting2 = new SetObjState { ObjId = 10161000, State = 70 };
            public static readonly SetObjState SinnerLighting3 = new SetObjState { ObjId = 10161003, State = 71 };
            public static readonly SetObjState SinnerLighting4 = new SetObjState { ObjId = 10161001, State = 70 };

            public static readonly SetObjState GargoylesDoor = new SetObjState { ObjId = 10161051, State = 20 };
        }

        public static class Navimesh
        {
            public struct DisableNavimesh
            {
                public int EventId;
                public int State;
            }

            public static readonly DisableNavimesh GargoylesDoor = new DisableNavimesh
                { EventId = 400000, State = 0x100 };
        }

        public static class WhiteDoor
        {
            public struct DisableWhiteDoor
            {
                public int ObjId;
                public int State;
            }

            public static readonly DisableWhiteDoor GargoylesDoor = new DisableWhiteDoor
                { ObjId = 10160620, State = 0 };
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

        public static class EzStateEventCalls
        {
            public struct EzStateEventCommand
            {
                public int CommandId { get; }
                public int[] Params { get; }
        
                public EzStateEventCommand(int commandId, params int[] parameters)
                {
                    CommandId = commandId;
                    Params = parameters;
                }
            }
        }
    }
}