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
                static Offsets()
                {
                    if (GameVersion.Current.Edition == GameEdition.Scholar)
                    {
                        CharacterManager = 0x18;
                        CameraManager = 0x20;
                        EventManager = 0x70;
                        GameDataManager = 0xA8;
                        SaveLoadSystemPtr = 0xB8;
                        PlayerCtrl = 0xD0;
                        PxWorldPtr = 0x660;
                        ViewMatrixPtr = 0x1158;
                        DLBackAllocator = 0x22E0;
                        Quitout = 0x24B1;
                        LoadingFlag = 0x24BC;
                    }
                    else
                    {
                        CharacterManager = 0x18;
                        CameraManager = 0x1C;
                        EventManager = 0x44;
                        GameDataManager = 0x60;
                        SaveLoadSystemPtr = 0x68;
                        PlayerCtrl = 0x74;
                        PxWorldPtr = 0x280;
                        // ViewMatrixPtr = 0x1154;
                        // DLBackAllocator = 0x22DC;
                        // Quitout = 0x0;
                        // LoadingFlag = 0x24B8;
                    }
                }

                public static int CharacterManager { get; private set; }
                public static int CameraManager { get; private set; }
                public static int EventManager { get; private set; }
                public static int GameDataManager { get; private set; }
                public static int SaveLoadSystemPtr { get; private set; }
                public static int PlayerCtrl { get; private set; }
                public static int PxWorldPtr { get; private set; }
                public static int ViewMatrixPtr { get; private set; }
                public static int DLBackAllocator { get; private set; }
                public static int Quitout { get; private set; }
                public static int LoadingFlag { get; private set; }
            }


            public static class CharacterManagerOffsets
            {
                public const int PlayerStatusParamPtr = 0x730;
                public const int PlayerStatusParam = 0xD8;
                public const int StartingWeapon = 0x278;
            }

            public static class EventManagerOffsets
            {
                public const int EventFlagManager = 0x20;
                public const int EventPointManager = 0x30;
                public const int EventBonfireManager = 0x58;
                public const int WarpEventEntity = 0x70;
            }

            public static class GameDataManagerOffsets
            {
                static GameDataManagerOffsets()
                {
                    if (GameVersion.Current.Edition == GameEdition.Scholar)
                    {
                        InventoryPtr = 0x10;
                        NewGamePtr = 0xC0;
                        NewGame = 0x68;
                    }
                    else
                    {
                        InventoryPtr = 0x08;
                        NewGamePtr = 0x60;
                        NewGame = 0x68;
                    }
                }

                public static int InventoryPtr { get; private set; }
                public static int NewGamePtr { get; private set; }
                public static int NewGame { get; private set; }


                public static class Inventory
                {
                    static Inventory()
                    {
                        if (GameVersion.Current.Edition == GameEdition.Scholar)
                        {
                            InventoryLists = 0x10;
                            ItemInventory2BagListPtr = 0x10;
                            ItemInventory2BagListForSpells = 0xD0;
                        }
                        else
                        {
                            InventoryLists = 0x8;
                            ItemInventory2BagListPtr = 0x8;
                            // ItemInventory2BagListForSpells = 0xD0;
                        }
                    }

                    public static int InventoryLists { get; private set; }
                    public static int ItemInventory2BagListPtr { get; private set; }
                    public static int ItemInventory2BagListForSpells { get; private set; }

                    public static class ItemInventory2BagList
                    {
                        public const int ItemInvetory2SpellListPtr = 0x259C8;
                    }

                    public static class ItemInvetory2SpellList
                    {
                        public const int ListStart = 0x10;
                        public const int Count = 0x32;
                    }

                    public static class SpellEntry
                    {
                        public const int NextPtr = 0x8;
                        public const int SpellId = 0x14;
                        public const int IsEquipped = 0x1F;
                        public const int SlotReq = 0x21;
                    }
                }
            }

            public static class SaveLoadSystem
            {
                public const int ForceSaveFlag1 = 0x68;
                public const int ForceSaveFlag2 = 0x1A2;
            }


            public static class ChrCtrlOffsets
            {
                static ChrCtrlOffsets()
                {
                    if (GameVersion.Current.Edition == GameEdition.Scholar)
                    {
                        ChrParamPtr = 0x38;
                        Coords = 0x90;
                        PoiseImmunityPtr = 0xB8;
                        OperatorPtr = 0xE8;
                        ChrPhysicsCtrlPtr = 0x100;
                        Hp = 0x168;
                        MinHp = 0x16C;
                        MaxHp = 0x170;
                        FullHpWithHollowing = 0x174;
                        Stamina = 0x1AC;
                        HeavyPoiseCurrent = 0x1B8;
                        HeavyPoiseMax = 0x1C0;
                        PoisonCurrent = 0x1C4;
                        PoisonMax = 0x1CC;
                        BleedCurrent = 0x1D0;
                        BleedMax = 0x1D8;
                        ToxicCurrent = 0x200;
                        ToxicMax = 0x208;
                        LightPoiseCurrent = 0x218;
                        LightPoiseMax = 0x220;
                        Speed = 0x2A8;
                        EquippedSpellsPtr = 0x378;
                        EquippedSpellsStart = 0x9B8;
                        ChrSpEffectCtrl = 0x3E0;
                        StatsPtr = 0x490;
                    }
                    else
                    {
                        ChrParamPtr = 0x20;
                        Coords = 0x80;
                        PoiseImmunityPtr = 0x94;
                        OperatorPtr = 0xAC;
                        ChrPhysicsCtrlPtr = 0xB8;
                        Hp = 0xFC;
                        MinHp = 0x100;
                        MaxHp = 0x104;
                        FullHpWithHollowing = 0x108;
                        Stamina = 0x140;
                        HeavyPoiseCurrent = 0x14C;
                        HeavyPoiseMax = 0x154;
                        PoisonCurrent = 0x158;
                        PoisonMax = 0x160;
                        BleedCurrent = 0x164;
                        BleedMax = 0x16C;
                        ToxicCurrent = 0x194;
                        ToxicMax = 0x19C;
                        LightPoiseCurrent = 0x1AC;
                        LightPoiseMax = 0x1B4;
                        Speed = 0x208;
                        // EquippedSpellsPtr = 0x374;
                        // EquippedSpellsStart = 0x9B4;
                        ChrSpEffectCtrl = 0x308;
                        StatsPtr = 0x378;
                    }
                }

                public static int ChrParamPtr { get; private set; }
                public static int Coords { get; private set; }
                public static int PoiseImmunityPtr { get; private set; }
                public static int OperatorPtr { get; private set; }
                public static int ChrPhysicsCtrlPtr { get; private set; }
                public static int Hp { get; private set; }
                public static int MinHp { get; private set; }
                public static int MaxHp { get; private set; }
                public static int FullHpWithHollowing { get; private set; }
                public static int Stamina { get; private set; }
                public static int HeavyPoiseCurrent { get; private set; }
                public static int HeavyPoiseMax { get; private set; }
                public static int PoisonCurrent { get; private set; }
                public static int PoisonMax { get; private set; }
                public static int BleedCurrent { get; private set; }
                public static int BleedMax { get; private set; }
                public static int ToxicCurrent { get; private set; }
                public static int ToxicMax { get; private set; }
                public static int LightPoiseCurrent { get; private set; }
                public static int LightPoiseMax { get; private set; }
                public static int Speed { get; private set; }
                public static int EquippedSpellsPtr { get; private set; }
                public static int EquippedSpellsStart { get; private set; }
                public static int ChrSpEffectCtrl { get; private set; }
                public static int StatsPtr { get; private set; }


                public static class ChrParam
                {
                    public const int PoisonToxicResist = 0xB0;
                    public const int BleedResist = 0xB4;
                }

                public static class PoiseStuff
                {
                    public const int LightStaggerImmuneFlag = 0x5E8;
                }

                public static class Operator
                {
                    public const int ChrPadMan = 0x18;
                    public const int ChrAiManPtr = 0x18;
                    public const int MovementEntity = 0x98;
                }

                public static class ChrAiMan
                {
                    public const int ChrAi = 0x20;
                }


                public static class ChrPhysicsCtrl
                {
                    static ChrPhysicsCtrl()
                    {
                        if (GameVersion.Current.Edition == GameEdition.Scholar)
                        {
                            Gravity = 0x134;
                            Xyz = 0x1C0;
                        }
                        else
                        {
                            Gravity = 0x11C;
                            Xyz = 0x70;
                        }
                    }

                    public static int Gravity { get; private set; }
                    public static int Xyz { get; private set; }
                }


                public static class Stats
                {
                    static Stats()
                    {
                        var version = GameVersion.Current;

                        if (version.Edition == GameEdition.Scholar)
                        {
                            Vigor = 0x08;
                            Endurance = 0x0A;
                            Vitality = 0x0C;
                            Attunement = 0x0E;
                            Strength = 0x10;
                            Dexterity = 0x12;
                            Intelligence = 0x14;
                            Faith = 0x16;
                            Adp = 0x18;
                            SoulLevel = 0xD0;
                            CurrentSouls = 0xEC;
                            SoulMemory = 0xF4;
                        }
                        else
                        {
                            Vigor = 0x04;
                            Endurance = 0x06;
                            Vitality = 0x08;
                            Attunement = 0x0A;
                            Strength = 0x0C;
                            Dexterity = 0x0E;
                            Intelligence = 0x10;
                            Faith = 0x12;
                            Adp = 0x14;
                            SoulLevel = 0xCC;
                            CurrentSouls = 0xE8;
                            SoulMemory = 0xF0;
                        }
                    }

                    public static int Vigor { get; private set; }
                    public static int Endurance { get; private set; }
                    public static int Vitality { get; private set; }
                    public static int Attunement { get; private set; }
                    public static int Strength { get; private set; }
                    public static int Dexterity { get; private set; }
                    public static int Intelligence { get; private set; }
                    public static int Faith { get; private set; }
                    public static int Adp { get; private set; }
                    public static int SoulLevel { get; private set; }
                    public static int CurrentSouls { get; private set; }
                    public static int SoulMemory { get; private set; }
                }
            }

            public static class PxWorld
            {
                static PxWorld()
                {
                    if (GameVersion.Current.Edition == GameEdition.Scholar)
                    {
                        HkpWorld = 0x18;
                        HkpChrRigidBodyArray = 0x1F8;
                        HkpChrRigidBody = 0x18;
                        HkpRigidBodyPtr = 0x8;
                        PlayerCoords = 0x1A0;
                    }
                    else
                    {
                        HkpWorld = 0xC;
                        HkpChrRigidBodyArray = 0x168;
                        HkpChrRigidBody = 0xC;
                        HkpRigidBodyPtr = 0x4;
                        PlayerCoords = 0x120;
                    }
                }

                public static int HkpWorld { get; private set; }
                public static int HkpChrRigidBodyArray { get; private set; }
                public static int HkpChrRigidBody { get; private set; }
                public static int HkpRigidBodyPtr { get; private set; }
                public static int PlayerCoords { get; private set; }
            }

            public static class DLAllocator
            {
                public const int FeOperatorFrontend = 0x10;

                public static class FeOperatorFrontendOffsets
                {
                    public const int FeSceneAdoptionItem = 0x90;
                    public const int AdoptionCleanupFlag = 0x24;
                }
            }
        }


        public static class HkHardwareInfo
        {
            public static IntPtr Base;
        }

        public static IntPtr MapId;

        public static class Patches
        {
            public static IntPtr InfiniteStam;
            public static IntPtr InfiniteGoods;
            public static IntPtr HideChrModels;
            public static IntPtr HideMap;
            public static IntPtr InfiniteCasts;
            public static IntPtr InfiniteDurability;
            public static IntPtr DropRate;
            public static IntPtr DisableAi;
            public static IntPtr Silent;
            public static IntPtr Hidden;
            public static IntPtr NegativeLevel;
            public static IntPtr Ng7;
            public static IntPtr NoSoulGain;
            public static IntPtr NoHollowing;
            public static IntPtr NoSoulLoss;
            public static IntPtr SoulMemWrite1;
            public static IntPtr SoulMemWrite2;
        }


        public static class Hooks
        {
            public static long SetAreaVariable; // Sets which of the forlorns should spawn
            public static long CompareEventRandValue; // Checks if forlorn should spawn
            public static long HpWrite;
            public static long WarpCoordWrite;
            public static long LockedTarget;
            public static long CreditSkip;
            public static long NumOfDrops;
            public static long DamageControl;
            public static long TriggersAndSpace;
            public static long Ctrl;
            public static long NoClipUpdateCoords;
            public static long KillboxFlagSet;
            public static long SetCurrectAct;
            public static long SetCurrectAct2;
            public static long FastQuitout;
            public static long InfinitePoise;
            public static long EzStateSetEvent;
            public static long ProcessPhysics;
            public static long DisableTargetAi;
            public static long SetSharedFlag;
            public static long BabyJump;
            public static long EzStateCompareTimer;
            public static long FogRender;
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
            public static long LevelLookup;
            public static long LevelUp;
            public static long CurrentItemQuantityCheck;
            public static long ItemGive;
            public static long BuildItemDialog;
            public static long ShowItemDialog;
            public static long GetEyePosition;
            public static long SetSpEffect;
            public static long HavokRayCast;
            public static long ConvertPxRigidToMapEntity;
            public static long ConvertMapEntityToGameId;
            public static long UnlockBonfire;
            public static long GetMapObjStateActComponent;
            public static long GetMapEntityWithAreaIdAndObjId;
            public static long GetNavimeshLoc;
            public static long DisableNaviMesh;
            public static long GetWhiteDoorComponent;
            public static long AttuneSpell;
            public static long GetNumOfSpellslots1;
            public static long GetNumOfSpellslots2;
            public static long UpdateSpellSlots;
        }

        private static int GetVanillaOffset(string playerctrl, string versionPatchVersion)
        {
            return 0x0;
        }

        private static int GetScholarOffset(string playerctrl, string versionPatchVersion)
        {
            return 0xD0;
        }
    }
}