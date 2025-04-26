using System;

namespace SilkySouls2.Memory
{
    public static class Offsets
    {

        public static class GameManagerImp
        {
            public static IntPtr Base;

            public static class Offsets
            {
                public const int CharacterManager = 0x18;
                public const int CameraManager = 0x20;
                public const int AiManager = 0x28;
                public const int EnemyGeneratorManager = 0x40;
                public const int TargetManager = 0x48;
                public const int EventManager = 0x70;
                public const int StateActManager = 0xA0;
                public const int GameDataManager = 0xA8;
                public const int PlayerCtrl = 0xD0;
            }
            
            public static class CameraManagerOffsets
            {
                public const int MapId = 0x45C;
            }

            public static class AiManagerOffsets
            {
                public const int DisableAi = 0x18;
            }

            public static class EventManagerOffsets
            {
                public const int EventFlagManager = 0x20;
                public const int WarpEventEntity = 0x70;
            }

            public static class GameDataManagerOffsets
            {
                public const int NewGamePtr = 0xC0;
                public const int NewGame = 0x68; 
                
            }

            public static class PlayerCtrlOffsets
            {

                public const int PlayerActionCtrlPtr = 0xE0;
                public const int Hp = 0x168;
                public const int MinHp = 0x16C;
                public const int MaxHp = 0x170;
                public const int Stamina = 0x1AC;
                public const int MaxStamina = 0x1B4;
                public const int StatsPtr = 0x490;

                public static class PlayerActionCtrl
                {
                    public const int PlayerDamageActionCtrl = 0x200;
                }
                
                
                public static class Stats
                {
                    public const int Vig = 0x08;
                    public const int End = 0x0A;
                    public const int Vit = 0x0C;
                    public const int Atn = 0x0E;
                    public const int Str = 0x10;
                    public const int Dex = 0x12;
                    public const int Int = 0x14;
                    public const int Fth = 0x16;
                    public const int Adp = 0x18;
                }
            }
        }

        

        public static class Patches
        {
        }
        
        
        public static class Hooks
        {
            public static long SetAreaVariable; // Sets which of the forlorns should spawn
            public static long CompareEventRandValue; // Checks if forlorn should spawn
            public static long HpWrite;
            public static long OneShot; // Also Deal no damage 
            public static long WarpCoordWrite;

        }

        public static class Funcs
        {
            public static long WarpPrep;
            public static long BonfireWarp;
        }
    }
}