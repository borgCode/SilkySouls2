﻿using System;

namespace SilkySouls2.Memory
{
    public static class Offsets
    {
        public static void Initialize(GameEdition edition)
        {
            GameManagerImp.Offsets.Initialize(edition);
            GameManagerImp.CharacterManagerOffsets.Initialize(edition);
            GameManagerImp.EventManagerOffsets.Initialize(edition);
            GameManagerImp.GameDataManagerOffsets.Initialize(edition);
            GameManagerImp.GameDataManagerOffsets.Inventory.Initialize(edition);
            GameManagerImp.GameDataManagerOffsets.Inventory.ItemInventory2BagList.Initialize(edition);
            GameManagerImp.GameDataManagerOffsets.Inventory.ItemInvetory2SpellList.Initialize(edition);
            GameManagerImp.GameDataManagerOffsets.Inventory.SpellEntry.Initialize(edition);
            GameManagerImp.SaveLoadSystem.Initialize(edition);
            GameManagerImp.ChrCtrlOffsets.Initialize(edition);
            GameManagerImp.ChrCtrlOffsets.Operator.Initialize(edition);
            GameManagerImp.ChrCtrlOffsets.ChrAiMan.Initialize(edition);
            GameManagerImp.ChrCtrlOffsets.ChrPhysicsCtrl.Initialize(edition);
            GameManagerImp.ChrCtrlOffsets.Stats.Initialize(edition);
            GameManagerImp.PxWorld.Initialize(edition);
        }
        
        public static class GameManagerImp
        {
            public static IntPtr Base;

            public static class Offsets
            {
                public static void Initialize(GameEdition edition)
                {
                    if (edition == GameEdition.Scholar)
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
                        ViewMatrixPtr = 0x650;
                        DLBackAllocator = 0xCC4;
                        Quitout = 0xDF1;
                        LoadingFlag = 0xDFC;
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
                public static void Initialize(GameEdition edition)
                {
                    if (edition == GameEdition.Scholar)
                    {
                        PlayerStatusParamPtr = 0x730;
                        PlayerStatusParam = 0xD8;
                        StartingWeapon = 0x278;
                    }
                    else
                    {
                        PlayerStatusParamPtr = 0x398;
                        PlayerStatusParam = 0x94;
                        StartingWeapon = 0x1A0;
                    }
                }
                public static int PlayerStatusParamPtr { get; private set; }
                public static int PlayerStatusParam { get; private set; }
                public static int StartingWeapon { get; private set; }
            }

            public static class EventManagerOffsets
            {
                public static void Initialize(GameEdition edition)
                {
                    if (edition == GameEdition.Scholar)
                    {
                        EventFlagManager = 0x20;
                        EventPointManager = 0x30;
                        EventBonfireManager = 0x58;
                        WarpEventEntity = 0x70;
                    }
                    else
                    {
                        EventFlagManager = 0x10;
                        EventPointManager = 0x18;
                        EventBonfireManager = 0x2C;
                        WarpEventEntity = 0x38;
                    }
                }

                public static int EventFlagManager { get; private set; }
                public static int EventPointManager { get; private set; }
                public static int EventBonfireManager { get; private set; }
                public static int WarpEventEntity { get; private set; }
            }

            public static class GameDataManagerOffsets
            {
                public static void Initialize(GameEdition edition)
                {
                    if (edition == GameEdition.Scholar)
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
                    public static void Initialize(GameEdition edition)
                    {
                        if (edition == GameEdition.Scholar)
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
                        public static void Initialize(GameEdition edition)
                        {
                            if (edition == GameEdition.Scholar)
                            {
                                ItemInvetory2SpellListPtr = 0x259C8;
                            }
                            else
                            {
                                ItemInvetory2SpellListPtr = 0x1E0E4;
                            }
                            
                        } 
                        public static int ItemInvetory2SpellListPtr { get; private set; }
                    }

                    public static class ItemInvetory2SpellList
                    {
                        public static void Initialize(GameEdition edition)
                        {
                            if (edition == GameEdition.Scholar)
                            {
                                ListStart = 0x10;
                                Count = 0x32;
                            }
                            else
                            {
                                ListStart = 0x8;
                                Count = 0x1A;
                            }
                        }
                        
                        public static int ListStart { get; private set; }
                        public static int Count { get; private set; }
                    }

                    public static class SpellEntry
                    {
                        public static void Initialize(GameEdition edition)
                        {
                            if (edition == GameEdition.Scholar)
                            {
                                NextPtr = 0x8;
                                SpellId = 0x14;
                                IsEquipped = 0x1F;
                                SlotReq = 0x21;
                                
                            }
                            else
                            {
                                NextPtr = 0x4;
                                SpellId = 0xC;
                                IsEquipped = 0x17;
                                SlotReq = 0x19;
                            }
                            
                        }
                        
                        public static int NextPtr { get; private set; }
                        public static int SpellId { get; private set; }
                        public static int IsEquipped { get; private set; }
                        public static int SlotReq { get; private set; }
                    }
                }
            }

            public static class SaveLoadSystem
            {
                public static void Initialize(GameEdition edition)
                {
                    if (edition == GameEdition.Scholar)
                    {
                        ForceSaveFlag1 = 0x68;
                        ForceSaveFlag2 = 0x1A2;
                    }
                    else
                    {
                        ForceSaveFlag1 = 0x3C;
                        ForceSaveFlag2 = 0x166;
                    }
                }
                public static int ForceSaveFlag1 {get; private set;}
                public static int ForceSaveFlag2 {get; private set;}
            }


            public static class ChrCtrlOffsets
            {
                public static void Initialize(GameEdition edition)
                {
                    if (edition == GameEdition.Scholar)
                    {
                        ChrParamPtr = 0x38;
                        ChrCommonPtr = 0x40;
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
                        ChrAsmCtrl = 0x378;
                        EquippedSpellsStart = 0x9B8;
                        ChrSpEffectCtrl = 0x3E0;
                        StatsPtr = 0x490;
                    }
                    else
                    {
                        ChrParamPtr = 0x20;
                        ChrCommonPtr = 0x24;
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
                        ChrAsmCtrl = 0x2D4;
                        EquippedSpellsStart = 0x7D8;
                        ChrSpEffectCtrl = 0x308;
                        StatsPtr = 0x378;
                    }
                }

                public static int ChrParamPtr { get; private set; }
                public static int ChrCommonPtr { get; private set; }
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
                public static int ChrAsmCtrl { get; private set; }
                public static int EquippedSpellsStart { get; private set; }
                public static int ChrSpEffectCtrl { get; private set; }
                public static int StatsPtr { get; private set; }


                public static class ChrParam
                {
                    public const int MagicResist = 0xA0;
                    public const int LightningResist = 0xA4;
                    public const int FireResist = 0xA8;
                    public const int DarkResist = 0xAC;
                    public const int PoisonToxicResist = 0xB0;
                    public const int BleedResist = 0xB4;
                }

                public static class ChrCommon
                {
                    public const int Slash = 0x270;
                    public const int Thrust = 0x274;
                    public const int Strike = 0x278;
                }

                public static class PoiseStuff
                {
                    public const int LightStaggerImmuneFlag = 0x5E8;
                }

                public static class Operator
                {
                    public static void Initialize(GameEdition edition)
                    {
                        if (edition == GameEdition.Scholar)
                        {
                            ChrAiManPtr = 0x18;
                        }
                        else
                        {
                            ChrAiManPtr = 0xC;
                        }
                    }
                    
                    public static int ChrAiManPtr  { get; private set; }
             
                }
                
                public static class ChrAiMan
                {
                    public static void Initialize(GameEdition edition)
                    {
                        if (edition == GameEdition.Scholar)
                        {
                            ChrAi = 0x20;
                        }
                        else
                        {
                            ChrAi = 0x10;
                        }
                    }
                    
                    public static int ChrAi  { get; private set; }
             
                }
                
                public static class ChrPhysicsCtrl
                {
                    public static void Initialize(GameEdition edition)
                    {
                        if (edition == GameEdition.Scholar)
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
                    public static void Initialize(GameEdition edition)
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
                public static void Initialize(GameEdition edition)
                {
                    if (edition == GameEdition.Scholar)
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
        }


        public static class HkHardwareInfo
        {
            public static IntPtr Base;
        }

        public static IntPtr MapId;
        public static IntPtr LoadLibraryW;

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
            public static long FasterMenu;
            public static long InfinitePoise;
            public static long SetEventWrapper;
            public static long ProcessPhysics;
            public static long DisableTargetAi;
            public static long SetSharedFlag;
            public static long BabyJump;
            public static long EzStateCompareTimer;
            public static long NoShadedFogClose;
            public static long ReduceGameSpeed;
            public static long LightGutter;
            public static long NoShadedFogFar;
            public static long NoShadedFogCam;
        }

        public static class Funcs
        {
            public static long WarpPrep;
            public static long BonfireWarp;
            public static long SetEvent;
            public static long GiveSouls;
            public static long RestoreSpellcasts;
            public static long ParamLookup;
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
            public static long Sleep;
            public static long SetDepthStencilSurface;
            public static long GetEvent;
        }
    }
}