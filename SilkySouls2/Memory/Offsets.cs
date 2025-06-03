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
                public const int ViewMatrixPtr = 0x1158;
                public const int ProjectionMatrixPtr = 0x16F0;
                public const int ProjectionMatrix = 0x170;
                public const int MenuKick = 0x24B1;
                public const int LoadingFlag = 0x24BC;
            }
            
            public static class CameraManagerOffsets
            {
                //TODO 0x18 FREE CAMERA 
                public const int IngameCameraOperatorPtr = 0x28;
                public const int CamLeaveChr= 0x450; // TODO Maybe free cam?
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
                public const int InventoryPtr = 0x10;
                public const int NewGamePtr = 0xC0;
                public const int NewGame = 0x68;


                public static class Inventory
                {
                    public const int ItemInventory2BagList = 0xD0;
                }
            }

            //For locked target
            public static class CharacterCtrlOffsets
            {
                public const int Operator = 0xE8;
                public const int ChrAiManip = 0x18;
                public const int ChrAi = 0x20;
            }

            public static class PlayerCtrlOffsets
            {

                public const int ChrParamPtr = 0x38;
                public const int Coords = 0x90;
                public const int CollisionPtr = 0xB8;
                public const int ChrFlagsPtr = 0xD8;
                public const int PlayerActionCtrlPtr = 0xE0;
                public const int PlayerOperatorPtr = 0xE8;
                public const int ChrMotionCtrlPtr = 0xF8;
                public const int ChrPhysicsCtrlPtr = 0x100;
                public const int Hp = 0x168;
                public const int MinHp = 0x16C;
                public const int MaxHp = 0x170;
                public const int Stamina = 0x1AC;
                public const int MaxStamina = 0x1B4;
                public const int PoiseCurrent = 0x1B8;
                public const int PoiseMax = 0x1C0;
                public const int PoisonCurrent = 0x1C4;
                public const int PoisonMax = 0x1CC;
                public const int BleedCurrent = 0x1D0;
                public const int BleedMax = 0x1D8;
                public const int ToxicCurrent = 0x200;
                public const int ToxicMax = 0x208;
                public const int ChrCullingGroupCtrlPtr = 0x240;
                public const int Speed = 0x2A8;
                
                public const int StatsPtr = 0x490;
                
                //TODO 4c0 flags

                public static class ChrParam
                {
                    public const int PoisonToxicResist = 0xB0;
                    public const int BleedResist = 0xB4;
                }

                public static class ChrActionCtrl
                {
                    public const int BossAttackCtrlPtr = 0x170;
                }

                
                public static class BossAttackCtrl
                {
                    public const int LastAttackPtr = 0x10;
                    public const int LastAttack = 0xEC;
                }

                public static class Collision
                {
                    public const int CollisionFlag = 0x62C;
                }

                public static class ChrFlags
                {
                    public const int Invincible = 0;
                    public const int HideModel = 6;
                }

                public static class PlayerOperator
                {
                    public const int ChrPadMan = 0x18;
                    public const int MovementEntity = 0x98;
                }

                public static class ChrMotionCtrl
                {
                    public const int MorphemeMotionCtrl = 0x28;
                    public const int MorphemeChrCtrl = 0x30;
                }

                public static class ChrPhysicsCtrl
                {
                    public const int Gravity = 0x134;
                    public const int Xyz = 0x1C0; //Read only
                }

                public static class ChrCullingGroupCtrl
                {
                    public const int InAirTimerEntity = 0x90;
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
                    public const int SoulLevel = 0xD0;
                    public const int SoulMemory = 0xF4;
                }
            }
        }

        public static class HkHardwareInfo
        {
            public static IntPtr Base;
            public const int HkpWorld = 0x58;
            public const int HkpChrRigidBodyPtr = 0x1F8;
            public const int HkpChrRigidBody = 0x18;
            public const int HkpRigidBodyPtr = 0x8;
            

            public static class HkpRigidBody
            {
                public const int PlayerIdentifier = 0x150;
                public const int PlayerCoords = 0x1A0;
            }
        }
        

        public static class Patches
        {
            public static IntPtr InfiniteStam;
            public static IntPtr ForceSave;
            public static IntPtr InfiniteGoods;
            public static IntPtr HideChrModels;
            public static IntPtr HideMap;
            public static IntPtr InfiniteCasts;
            public static IntPtr InfiniteDurability;
            public static IntPtr DropRate;
            public static IntPtr DisableAi;
            public static IntPtr Silent;
        }
        
        
        public static class Hooks
        {
            public static long SetAreaVariable; // Sets which of the forlorns should spawn
            public static long CompareEventRandValue; // Checks if forlorn should spawn
            public static long HpWrite;
            public static long OneShot; // Also Deal no damage 
            public static long WarpCoordWrite;
            public static long LockedTarget;
            public static long CreditSkip;
            public static long NumOfDrops;
            public static long DamageControl;
            public static long InAirTimer;
            public static long TriggersAndSpace;
            public static long Ctrl;
            public static long NoClipUpdateCoords;
            public static long KillboxFlagSet;
            public static long SetCurrectAct;
            public static long SetCurrectAct2;
            public static long FastQuitout;
        }

        public static class Funcs
        {
            public static long WarpPrep;
            public static long BonfireWarp;
            public static long SetEvent;
            public static long GiveSouls;
            public static long RestoreSpellcasts;
            public static long ParamLookUp;
            public static long SetRenderTargets;
            public static long CreateSoundEvent;
            public static long ChrCtrlUpdate;
        }
    }
}