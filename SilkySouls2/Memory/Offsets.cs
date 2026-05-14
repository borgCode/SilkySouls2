using System;
using static SilkySouls2.Memory.Patch;

namespace SilkySouls2.Memory
{
    public static class Offsets
    {
        public static class GameManagerImp
        {
            public static nint Base;

            public static int EventManager => PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 or Vanilla1_0_12 => 0x44,
                Scholar1_0_2 or Scholar1_0_3 => 0x70,
                _ => 0x0
            };

            public static int GameDataManager => PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 or Vanilla1_0_12 => 0x60,
                Scholar1_0_2 or Scholar1_0_3 => 0xA8,
                _ => 0x0
            };

            public static int SaveLoadSystem => PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 or Vanilla1_0_12 => 0x68,
                Scholar1_0_2 or Scholar1_0_3 => 0xB8,
                _ => 0x0
            };

            public static int PlayerCtrl => PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 or Vanilla1_0_12 => 0x74,
                Scholar1_0_2 or Scholar1_0_3 => 0xD0,
                _ => 0x0
            };

            public static int PxWorld => PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 or Vanilla1_0_12 => 0x280,
                Scholar1_0_2 or Scholar1_0_3 => 0x660,
                _ => 0x0
            };

            public static int DLBackAllocator => PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 or Vanilla1_0_12 => 0xCC4,
                Scholar1_0_2 or Scholar1_0_3 => 0x22E0,
                _ => 0x0
            };

            public static int[] NpcMenuFixOrderJobSequence => PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 or Vanilla1_0_12 => [DLBackAllocator, 0x88, 0x8],
                Scholar1_0_2 or Scholar1_0_3 => [DLBackAllocator, 0x110, 0x10],
                _ => []
            };

            // DLBackAllocator -> FeOperatorNpcMenu* -> FeSceneNpcMenu*
            //   -> FixOrderJobSequence* -> third job -> FeItemSelectMenu*
            //   -> open/active byte
            public static int[] FeItemSelectMenuOpen => PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 or Vanilla1_0_12 => [DLBackAllocator, 0x88, 0x8, 0x1C, 0x18, 0x18, 0x12],
                Scholar1_0_2 or Scholar1_0_3 => [DLBackAllocator, 0x110, 0x10, 0x38, 0x30, 0x30, 0x1E],
                _ => []
            };

            public static int Quitout => PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 or Vanilla1_0_12 => 0xDF1,
                Scholar1_0_2 or Scholar1_0_3 => 0x24B1,
                _ => 0x0
            };

            public static int LoadingFlag => PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 or Vanilla1_0_12 => 0xDFC,
                Scholar1_0_2 or Scholar1_0_3 => 0x24BC,
                _ => 0x0
            };

            public static int PendingCutsceneId => PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 or Vanilla1_0_12 => 0xE18,
                Scholar1_0_2 or Scholar1_0_3 => 0x24D8,
                _ => 0x0
            };

            public static class EventManagerOffsets
            {
                public static int EventFlagManager => PatchManager.Current.PatchVersion switch
                {
                    Vanilla1_0_11 or Vanilla1_0_12 => 0x10,
                    Scholar1_0_2 or Scholar1_0_3 => 0x20,
                    _ => 0x0
                };

                public static int EventPointManager => PatchManager.Current.PatchVersion switch
                {
                    Vanilla1_0_11 or Vanilla1_0_12 => 0x18,
                    Scholar1_0_2 or Scholar1_0_3 => 0x30,
                    _ => 0x0
                };

                public static int EventWindowManager => PatchManager.Current.PatchVersion switch
                {
                    Vanilla1_0_11 or Vanilla1_0_12 => 0x28,
                    Scholar1_0_2 or Scholar1_0_3 => 0x50,
                    _ => 0x0
                };

                public static int EventBonfireManager => PatchManager.Current.PatchVersion switch
                {
                    Vanilla1_0_11 or Vanilla1_0_12 => 0x2C,
                    Scholar1_0_2 or Scholar1_0_3 => 0x58,
                    _ => 0x0
                };

                public static int WarpEventEntity => PatchManager.Current.PatchVersion switch
                {
                    Vanilla1_0_11 or Vanilla1_0_12 => 0x38,
                    Scholar1_0_2 or Scholar1_0_3 => 0x70,
                    _ => 0x0
                };
            }

            public static class GameDataManagerOffsets
            {
                public static int InventoryPtr => PatchManager.Current.PatchVersion switch
                {
                    Vanilla1_0_11 or Vanilla1_0_12 => 0x08,
                    Scholar1_0_2 or Scholar1_0_3 => 0x10,
                    _ => 0x0
                };

                public static int NewGamePtr => PatchManager.Current.PatchVersion switch
                {
                    Vanilla1_0_11 or Vanilla1_0_12 => 0x60,
                    Scholar1_0_2 or Scholar1_0_3 => 0xC0,
                    _ => 0x0
                };

                public static int NewGame => PatchManager.Current.PatchVersion switch
                {
                    Vanilla1_0_11 or Vanilla1_0_12 => 0x68,
                    Scholar1_0_2 or Scholar1_0_3 => 0x68,
                    _ => 0x0
                };

                public static class Inventory
                {
                    public static int InventoryLists => PatchManager.Current.PatchVersion switch
                    {
                        Vanilla1_0_11 or Vanilla1_0_12 => 0x8,
                        Scholar1_0_2 or Scholar1_0_3 => 0x10,
                        _ => 0x0
                    };

                    public static int ItemInventory2BagListPtr => PatchManager.Current.PatchVersion switch
                    {
                        Vanilla1_0_11 or Vanilla1_0_12 => 0x8,
                        Scholar1_0_2 or Scholar1_0_3 => 0x10,
                        _ => 0x0
                    };

                    public static int ItemInventory2BagListForSpells => PatchManager.Current.PatchVersion switch
                    {
                        Vanilla1_0_11 or Vanilla1_0_12 => 0x0,
                        Scholar1_0_2 or Scholar1_0_3 => 0xD0,
                        _ => 0x0
                    };

                    public static class ItemInventory2BagList
                    {
                        public static int ItemInvetory2SpellListPtr => PatchManager.Current.PatchVersion switch
                        {
                            Vanilla1_0_11 or Vanilla1_0_12 => 0x1E0E4,
                            Scholar1_0_2 or Scholar1_0_3 => 0x259C8,
                            _ => 0x0
                        };
                    }

                    public static class ItemInvetory2SpellList
                    {
                        public static int ListStart => PatchManager.Current.PatchVersion switch
                        {
                            Vanilla1_0_11 or Vanilla1_0_12 => 0x8,
                            Scholar1_0_2 or Scholar1_0_3 => 0x10,
                            _ => 0x0
                        };

                        public static int Count => PatchManager.Current.PatchVersion switch
                        {
                            Vanilla1_0_11 or Vanilla1_0_12 => 0x1A,
                            Scholar1_0_2 or Scholar1_0_3 => 0x32,
                            _ => 0x0
                        };
                    }

                    public static class SpellEntry
                    {
                        public static int NextPtr => PatchManager.Current.PatchVersion switch
                        {
                            Vanilla1_0_11 or Vanilla1_0_12 => 0x4,
                            Scholar1_0_2 or Scholar1_0_3 => 0x8,
                            _ => 0x0
                        };

                        public static int SpellId => PatchManager.Current.PatchVersion switch
                        {
                            Vanilla1_0_11 or Vanilla1_0_12 => 0xC,
                            Scholar1_0_2 or Scholar1_0_3 => 0x14,
                            _ => 0x0
                        };

                        public static int IsEquipped => PatchManager.Current.PatchVersion switch
                        {
                            Vanilla1_0_11 or Vanilla1_0_12 => 0x17,
                            Scholar1_0_2 or Scholar1_0_3 => 0x1F,
                            _ => 0x0
                        };

                        public static int SlotReq => PatchManager.Current.PatchVersion switch
                        {
                            Vanilla1_0_11 or Vanilla1_0_12 => 0x19,
                            Scholar1_0_2 or Scholar1_0_3 => 0x21,
                            _ => 0x0
                        };
                    }
                }
            }

            public static class SaveLoadSystemOffsets
            {
                public static int ForceSaveFlag1 => PatchManager.Current.PatchVersion switch
                {
                    Vanilla1_0_11 or Vanilla1_0_12 => 0x3C,
                    Scholar1_0_2 or Scholar1_0_3 => 0x68,
                    _ => 0x0
                };

                public static int ForceSaveFlag2 => PatchManager.Current.PatchVersion switch
                {
                    Vanilla1_0_11 or Vanilla1_0_12 => 0x166,
                    Scholar1_0_2 or Scholar1_0_3 => 0x1A2,
                    _ => 0x0
                };
            }

            public static class PxWorldOffsets
            {
                public static int[] PlayerCoordsChain => PatchManager.Current.PatchVersion switch
                {
                    Vanilla1_0_11 or Vanilla1_0_12 => [0xC, 0x168, 0xC, 0x4, 0x120],
                    Scholar1_0_2 or Scholar1_0_3 => [0x18, 0x1F8, 0x18, 0x8, 0x1A0],
                    _ => []
                };
            }
        }
        
        public static class DLBackAllocator
        {
            public static int UnkFlag => PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 or Vanilla1_0_12 => 0x1A3,
                Scholar1_0_2 or Scholar1_0_3 => 0x30F,
                _ => 0x0
            };
            
            public static int RefCount => PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 or Vanilla1_0_12 => 0x1B0,
                Scholar1_0_2 or Scholar1_0_3 => 0x31C,
                _ => 0x0
            };
        }

        public static class ChrCtrl
        {
            public static int ChrParam => PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 or Vanilla1_0_12 => 0x20,
                Scholar1_0_2 or Scholar1_0_3 => 0x38,
                _ => 0x0
            };

            public static int ChrCommon => PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 or Vanilla1_0_12 => 0x24,
                Scholar1_0_2 or Scholar1_0_3 => 0x40,
                _ => 0x0
            };

            public static int Coords => PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 or Vanilla1_0_12 => 0x80,
                Scholar1_0_2 or Scholar1_0_3 => 0x90,
                _ => 0x0
            };

            public static int PoiseImmunityPtr => PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 or Vanilla1_0_12 => 0x94,
                Scholar1_0_2 or Scholar1_0_3 => 0xB8,
                _ => 0x0
            };

            public static int ChrFlags => PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 or Vanilla1_0_12 => 0xA4,
                Scholar1_0_2 or Scholar1_0_3 => 0xD8,
                _ => 0x0
            };

            public static readonly BitFlag NoHit = new(0x0, 1 << 0);

            public static int ChrActionCtrl => PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 or Vanilla1_0_12 => 0xA8,
                Scholar1_0_2 or Scholar1_0_3 => 0xE0,
                _ => 0x0
            };

            public static int Operator => PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 or Vanilla1_0_12 => 0xAC,
                Scholar1_0_2 or Scholar1_0_3 => 0xE8,
                _ => 0x0
            };

            public static int ChrPhysicsCtrlPtr => PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 or Vanilla1_0_12 => 0xB8,
                Scholar1_0_2 or Scholar1_0_3 => 0x100,
                _ => 0x0
            };

            public static int Hp => PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 or Vanilla1_0_12 => 0xFC,
                Scholar1_0_2 or Scholar1_0_3 => 0x168,
                _ => 0x0
            };

            public static int MinHp => PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 or Vanilla1_0_12 => 0x100,
                Scholar1_0_2 or Scholar1_0_3 => 0x16C,
                _ => 0x0
            };

            public static int MaxHp => PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 or Vanilla1_0_12 => 0x104,
                Scholar1_0_2 or Scholar1_0_3 => 0x170,
                _ => 0x0
            };

            public static int FullHpWithHollowing => PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 or Vanilla1_0_12 => 0x108,
                Scholar1_0_2 or Scholar1_0_3 => 0x174,
                _ => 0x0
            };

            public static int Stamina => PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 or Vanilla1_0_12 => 0x140,
                Scholar1_0_2 or Scholar1_0_3 => 0x1AC,
                _ => 0x0
            };

            public static int HeavyPoiseCurrent => PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 or Vanilla1_0_12 => 0x14C,
                Scholar1_0_2 or Scholar1_0_3 => 0x1B8,
                _ => 0x0
            };

            public static int HeavyPoiseMax => PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 or Vanilla1_0_12 => 0x154,
                Scholar1_0_2 or Scholar1_0_3 => 0x1C0,
                _ => 0x0
            };

            public static int PoisonCurrent => PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 or Vanilla1_0_12 => 0x158,
                Scholar1_0_2 or Scholar1_0_3 => 0x1C4,
                _ => 0x0
            };

            public static int PoisonMax => PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 or Vanilla1_0_12 => 0x160,
                Scholar1_0_2 or Scholar1_0_3 => 0x1CC,
                _ => 0x0
            };

            public static int BleedCurrent => PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 or Vanilla1_0_12 => 0x164,
                Scholar1_0_2 or Scholar1_0_3 => 0x1D0,
                _ => 0x0
            };

            public static int BleedMax => PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 or Vanilla1_0_12 => 0x16C,
                Scholar1_0_2 or Scholar1_0_3 => 0x1D8,
                _ => 0x0
            };

            public static int ToxicCurrent => PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 or Vanilla1_0_12 => 0x194,
                Scholar1_0_2 or Scholar1_0_3 => 0x200,
                _ => 0x0
            };

            public static int ToxicMax => PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 or Vanilla1_0_12 => 0x19C,
                Scholar1_0_2 or Scholar1_0_3 => 0x208,
                _ => 0x0
            };

            public static int LightPoiseCurrent => PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 or Vanilla1_0_12 => 0x1AC,
                Scholar1_0_2 or Scholar1_0_3 => 0x218,
                _ => 0x0
            };

            public static int LightPoiseMax => PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 or Vanilla1_0_12 => 0x1B4,
                Scholar1_0_2 or Scholar1_0_3 => 0x220,
                _ => 0x0
            };

            public static int Speed => PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 or Vanilla1_0_12 => 0x208,
                Scholar1_0_2 or Scholar1_0_3 => 0x2A8,
                _ => 0x0
            };

            public static int ChrAsmCtrl => PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 or Vanilla1_0_12 => 0x2D4,
                Scholar1_0_2 or Scholar1_0_3 => 0x378,
                _ => 0x0
            };

            public static int EquippedSpellsStart => PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 or Vanilla1_0_12 => 0x7D8,
                Scholar1_0_2 or Scholar1_0_3 => 0x9B8,
                _ => 0x0
            };

            public static int ChrSpEffectCtrl => PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 or Vanilla1_0_12 => 0x308,
                Scholar1_0_2 or Scholar1_0_3 => 0x3E0,
                _ => 0x0
            };

            public static int StatsPtr => PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 or Vanilla1_0_12 => 0x378,
                Scholar1_0_2 or Scholar1_0_3 => 0x490,
                _ => 0x0
            };

            public static class ChrParamOffsets
            {
                public const int MagicResist = 0xA0;
                public const int LightningResist = 0xA4;
                public const int FireResist = 0xA8;
                public const int DarkResist = 0xAC;
                public const int PoisonToxicResist = 0xB0;
                public const int BleedResist = 0xB4;
            }

            public static class ChrCommonOffsets
            {
                public const int Slash = 0x270;
                public const int Thrust = 0x274;
                public const int Strike = 0x278;
            }

            public static class PoiseStuff
            {
                public const int LightStaggerImmuneFlag = 0x5E8;
            }

            public static class OperatorOffsets
            {
                public static int ChrAiManipulator => PatchManager.Current.PatchVersion switch
                {
                    Vanilla1_0_11 or Vanilla1_0_12 => 0xC,
                    Scholar1_0_2 or Scholar1_0_3 => 0x18,
                    _ => 0x0
                };
            }

            public static class ChrAiManipulatorOffsets
            {
                public static int ChrAi => PatchManager.Current.PatchVersion switch
                {
                    Vanilla1_0_11 or Vanilla1_0_12 => 0x10,
                    Scholar1_0_2 or Scholar1_0_3 => 0x20,
                    _ => 0x0
                };
            }

            public static class ChrPhysicsCtrl
            {
                public static int Gravity => PatchManager.Current.PatchVersion switch
                {
                    Vanilla1_0_11 or Vanilla1_0_12 => 0x11C,
                    Scholar1_0_2 or Scholar1_0_3 => 0x134,
                    _ => 0x0
                };

                public static int Xyz => PatchManager.Current.PatchVersion switch
                {
                    Vanilla1_0_11 or Vanilla1_0_12 => 0x70,
                    Scholar1_0_2 or Scholar1_0_3 => 0x1C0,
                    _ => 0x0
                };
            }

            public static class Stats
            {
                public static int Vigor => PatchManager.Current.PatchVersion switch
                {
                    Vanilla1_0_11 or Vanilla1_0_12 => 0x04,
                    Scholar1_0_2 or Scholar1_0_3 => 0x08,
                    _ => 0x0
                };

                public static int Endurance => PatchManager.Current.PatchVersion switch
                {
                    Vanilla1_0_11 or Vanilla1_0_12 => 0x06,
                    Scholar1_0_2 or Scholar1_0_3 => 0x0A,
                    _ => 0x0
                };

                public static int Vitality => PatchManager.Current.PatchVersion switch
                {
                    Vanilla1_0_11 or Vanilla1_0_12 => 0x08,
                    Scholar1_0_2 or Scholar1_0_3 => 0x0C,
                    _ => 0x0
                };

                public static int Attunement => PatchManager.Current.PatchVersion switch
                {
                    Vanilla1_0_11 or Vanilla1_0_12 => 0x0A,
                    Scholar1_0_2 or Scholar1_0_3 => 0x0E,
                    _ => 0x0
                };

                public static int Strength => PatchManager.Current.PatchVersion switch
                {
                    Vanilla1_0_11 or Vanilla1_0_12 => 0x0C,
                    Scholar1_0_2 or Scholar1_0_3 => 0x10,
                    _ => 0x0
                };

                public static int Dexterity => PatchManager.Current.PatchVersion switch
                {
                    Vanilla1_0_11 or Vanilla1_0_12 => 0x0E,
                    Scholar1_0_2 or Scholar1_0_3 => 0x12,
                    _ => 0x0
                };

                public static int Intelligence => PatchManager.Current.PatchVersion switch
                {
                    Vanilla1_0_11 or Vanilla1_0_12 => 0x10,
                    Scholar1_0_2 or Scholar1_0_3 => 0x14,
                    _ => 0x0
                };

                public static int Faith => PatchManager.Current.PatchVersion switch
                {
                    Vanilla1_0_11 or Vanilla1_0_12 => 0x12,
                    Scholar1_0_2 or Scholar1_0_3 => 0x16,
                    _ => 0x0
                };

                public static int Adp => PatchManager.Current.PatchVersion switch
                {
                    Vanilla1_0_11 or Vanilla1_0_12 => 0x14,
                    Scholar1_0_2 or Scholar1_0_3 => 0x18,
                    _ => 0x0
                };

                public static int SoulLevel => PatchManager.Current.PatchVersion switch
                {
                    Vanilla1_0_11 or Vanilla1_0_12 => 0xCC,
                    Scholar1_0_2 or Scholar1_0_3 => 0xD0,
                    _ => 0x0
                };

                public static int CurrentSouls => PatchManager.Current.PatchVersion switch
                {
                    Vanilla1_0_11 or Vanilla1_0_12 => 0xE8,
                    Scholar1_0_2 or Scholar1_0_3 => 0xEC,
                    _ => 0x0
                };

                public static int SoulMemory => PatchManager.Current.PatchVersion switch
                {
                    Vanilla1_0_11 or Vanilla1_0_12 => 0xF0,
                    Scholar1_0_2 or Scholar1_0_3 => 0xF4,
                    _ => 0x0
                };
            }
        }

        public static class ChrActionCtrl
        {
            public static int ChrEquipBrokenActionCtrl => PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 or Vanilla1_0_12 => 0x11C,
                Scholar1_0_2 or Scholar1_0_3 => 0x1B8,
                _ => 0x0
            };
        }

        public static class HkHardwareInfo
        {
            public static nint Base;
        }

        public static nint MapId;
        public static nint LoadLibraryW;
        public static nint BuildTextFieldRetAddr;

        public static class KatanaMainApp
        {
            public static nint Base;

            private static readonly int[] VanillaDoubleClickPtrChain = { 0x3C, 0x0, 0x4, 0x39 };
            private static readonly int[] ScholarDoubleClickPtrChain = { 0x60, 0x0, 0x8, 0x6D };
            private static readonly int[] EmptyPtrChain = [];

            public static int[] DoubleClickPtrChain => PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 or Vanilla1_0_12 => VanillaDoubleClickPtrChain,
                Scholar1_0_2 or Scholar1_0_3 => ScholarDoubleClickPtrChain,
                _ => EmptyPtrChain
            };
        }

        public static class Patches
        {
            public static nint InfiniteStam;
            public static nint InfiniteGoods;
            public static nint HideChrModels;
            public static nint HideMap;
            public static nint InfiniteCasts;
            public static nint InfiniteDurability;
            public static nint DropRate;
            public static nint DisableAi;
            public static nint Silent;
            public static nint Hidden;
            public static nint NegativeLevel;
            public static nint Ng7;
            public static nint NoSoulGain;
            public static nint NoHollowing;
            public static nint NoSoulLoss;
            public static nint SoulMemWrite1;
            public static nint SoulMemWrite2;
            public static nint NoHitPatch;
            public static nint MenuTransition;
        }

        public static class Hooks
        {
            public static nint SetAreaVariable;
            public static nint CompareEventRandValueForlorn;
            public static nint CompareEventRandValueElana;
            public static nint PlayerNoDamage;
            public static nint WarpCoordWrite;
            public static nint LockedTarget;
            public static nint CreditSkip;
            public static nint NumOfDrops;
            public static nint DamageControl;
            public static nint TriggersAndSpace;
            public static nint Ctrl;
            public static nint NoClipUpdateCoords;
            public static nint KillboxFlagSet;
            public static nint SetCurrentAct;
            public static nint FasterMenu;
            public static nint InfinitePoise;
            public static nint SetEventWrapper;
            public static nint ProcessPhysics;
            public static nint DisableTargetAi;
            public static nint SetSharedFlag;
            public static nint BabyJump;
            public static nint EzStateCompareTimer;
            public static nint NoShadedFogClose;
            public static nint ReduceGameSpeed;
            public static nint LightGutter;
            public static nint NoShadedFogFar;
            public static nint NoShadedFogCam;
            public static nint GameManUpdate;
            public static nint NewGameDetect;
            public static nint LoadingItemName;
            public static nint PreAiEzState;
        }

        public static class Functions
        {
            public static nint WarpPrep;
            public static nint BonfireWarp;
            public static nint SetEvent;
            public static nint GiveSouls;
            public static nint RestoreSpellcasts;
            public static nint ParamLookup;
            public static nint SetRenderTargets;
            public static nint CreateSoundEvent;
            public static nint LevelLookup;
            public static nint LevelUp;
            public static nint CurrentItemQuantityCheck;
            public static nint ItemGive;
            public static nint BuildItemDialog;
            public static nint ShowItemDialog;
            public static nint GetEyePosition;
            public static nint SetSpEffect;
            public static nint HavokRayCast;
            public static nint ConvertPxRigidToMapEntity;
            public static nint ConvertMapEntityToGameId;
            public static nint UnlockBonfire;
            public static nint GetMapObjStateActComponent;
            public static nint GetMapEntityWithAreaIdAndObjId;
            public static nint GetNavimeshLoc;
            public static nint DisableNavimesh;
            public static nint GetWhiteDoorComponent;
            public static nint AttuneSpell;
            public static nint GetNumOfSpellSlots1;
            public static nint GetNumOfSpellSlots2;
            public static nint UpdateSpellSlots;
            public static nint Sleep;
            public static nint SetDepthStencilSurface;
            public static nint GetEvent;
            public static nint EzStateExternalEventCtor;
            public static nint EzStateEventExecuteCommand;
            public static nint OpenNpcMenu;
            public static nint SetMenuOpenChrState;
            public static nint OriginalSoulGain;
            public static nint OriginalMakeSound;
            public static nint ApplyDurabilityDamage;
            public static nint ResolveTargetCtrlFromHandle;
        }

        public static bool InitializeStatics(nint baseAddr)
        {
            GameManagerImp.Base = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x11493F4,
                Vanilla1_0_12 => 0x1150414,
                Scholar1_0_2 => 0x160B8D0,
                Scholar1_0_3 => 0x16148F0,
                _ => 0
            };

            KatanaMainApp.Base = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x11936C4,
                Vanilla1_0_12 => 0x119A6E4,
                Scholar1_0_2 => 0x166C1D8,
                Scholar1_0_3 => 0x16751F8,
                _ => 0
            };

            MapId = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x10CD2D0,
                Vanilla1_0_12 => 0x10D42D8,
                Scholar1_0_2 => 0x15641B4,
                Scholar1_0_3 => 0x156D1C4,
                _ => 0
            };

            LoadLibraryW = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x13726C0,
                Vanilla1_0_12 => 0x14546C0,
                _ => 0
            };

            BuildTextFieldRetAddr = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x1567FC,
                Vanilla1_0_12 => 0x1569EC,
                Scholar1_0_2 => 0xA94AE,
                Scholar1_0_3 => 0xA956E,
                _ => 0
            };


            Patches.HideMap = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x3F5153,
                Vanilla1_0_12 => 0x3FC123,
                Scholar1_0_2 => 0x3D57B6,
                Scholar1_0_3 => 0x3DC5D6,
                _ => 0
            };

            Patches.HideChrModels = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x33855F,
                Vanilla1_0_12 => 0x33E6EF,
                Scholar1_0_2 => 0x30F301,
                Scholar1_0_3 => 0x315691,
                _ => 0
            };

            Patches.InfiniteStam = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x359B4E,
                Vanilla1_0_12 => 0x35FCEE,
                Scholar1_0_2 => 0x32D2A9,
                Scholar1_0_3 => 0x333639,
                _ => 0
            };

            Patches.InfiniteGoods = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x231102,
                Vanilla1_0_12 => 0x233F22,
                Scholar1_0_2 => 0x1ABB5D,
                Scholar1_0_3 => 0x1AF2CD,
                _ => 0
            };

            Patches.InfiniteDurability = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x36FFDA,
                Vanilla1_0_12 => 0x37651A,
                Scholar1_0_2 => 0x34516D,
                Scholar1_0_3 => 0x34B67D,
                _ => 0
            };

            Patches.InfiniteCasts = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x22BFF9,
                Vanilla1_0_12 => 0x22EE19,
                Scholar1_0_2 => 0x1AB790,
                Scholar1_0_3 => 0x1AEF00,
                _ => 0
            };

            Patches.NoSoulGain = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x273C94,
                Vanilla1_0_12 => 0x276F74,
                Scholar1_0_2 => 0x1FE9AA,
                Scholar1_0_3 => 0x20249A,
                _ => 0
            };

            Patches.NoHollowing = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x3ACE17,
                Vanilla1_0_12 => 0x3B37B7,
                Scholar1_0_2 => 0x385199,
                Scholar1_0_3 => 0x38BAF9,
                _ => 0
            };

            Patches.NoSoulLoss = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x2C12C1,
                Vanilla1_0_12 => 0x2C4D91,
                Scholar1_0_2 => 0x266BF3,
                Scholar1_0_3 => 0x26AFD3,
                _ => 0
            };

            Patches.Hidden = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x44245D,
                Vanilla1_0_12 => 0x44969D,
                Scholar1_0_2 => 0x434DA9,
                Scholar1_0_3 => 0x43BF29,
                _ => 0
            };

            Patches.SoulMemWrite1 = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x3AD22C,
                Vanilla1_0_12 => 0x3B3BCC,
                Scholar1_0_2 => 0x3842B8,
                Scholar1_0_3 => 0x38AC18,
                _ => 0
            };

            Patches.SoulMemWrite2 = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x3AD25D,
                Vanilla1_0_12 => 0x3B3BFD,
                Scholar1_0_2 => 0x38430E,
                Scholar1_0_3 => 0x38AC6E,
                _ => 0
            };

            Patches.DisableAi = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x42FAA0,
                Vanilla1_0_12 => 0x436D30,
                Scholar1_0_2 => 0x41C1EB,
                Scholar1_0_3 => 0x42336B,
                _ => 0
            };

            Patches.Silent = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x1A1731,
                Vanilla1_0_12 => 0x1A1970,
                Scholar1_0_2 => 0x10E232,
                Scholar1_0_3 => 0x10E306,
                _ => 0
            };

            Patches.Ng7 = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x194DA0,
                Vanilla1_0_12 => 0x194FB0,
                Scholar1_0_2 => 0xFE97D,
                Scholar1_0_3 => 0xFEA3D,
                _ => 0
            };

            Patches.DropRate = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x257DC3,
                Vanilla1_0_12 => 0x25ABF3,
                Scholar1_0_2 => 0x1E0033,
                Scholar1_0_3 => 0x1E37B3,
                _ => 0
            };

            Patches.NoHitPatch = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x464826,
                Vanilla1_0_12 => 0x46BB06,
                Scholar1_0_2 => 0x46A29A,
                Scholar1_0_3 => 0x4714EA,
                _ => 0
            };

            Patches.MenuTransition = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x187C9E,
                Vanilla1_0_12 => 0x187E9E,
                Scholar1_0_2 => 0xEF554,
                Scholar1_0_3 => 0xEF614,
                _ => 0
            };

            Functions.GetEvent = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x478750,
                Vanilla1_0_12 => 0x47F9F0,
                Scholar1_0_2 => 0x46CFE0,
                Scholar1_0_3 => 0x474230,
                _ => 0
            };

            Functions.SetSpEffect = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x1DB3E0,
                Vanilla1_0_12 => 0x1DC980,
                Scholar1_0_2 => 0x1497B0,
                Scholar1_0_3 => 0x14BEC0,
                _ => 0
            };

            Functions.GiveSouls = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x3AD170,
                Vanilla1_0_12 => 0x3B3B10,
                Scholar1_0_2 => 0x3841E0,
                Scholar1_0_3 => 0x38AB40,
                _ => 0
            };

            Functions.LevelUp = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x3AD600,
                Vanilla1_0_12 => 0x3B3FA0,
                Scholar1_0_2 => 0x384840,
                Scholar1_0_3 => 0x38B1A0,
                _ => 0
            };

            Patches.NegativeLevel = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x3AD631,
                Vanilla1_0_12 => 0x3B3FD1,
                Scholar1_0_2 => 0x384878,
                Scholar1_0_3 => 0x38B1D8,
                _ => 0
            };

            Functions.LevelLookup = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x3AE0E0,
                Vanilla1_0_12 => 0x3B4A80,
                Scholar1_0_2 => 0x3867E0,
                Scholar1_0_3 => 0x38D140,
                _ => 0
            };

            Functions.RestoreSpellcasts = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x230B30,
                Vanilla1_0_12 => 0x233950,
                Scholar1_0_2 => 0x1AF710,
                Scholar1_0_3 => 0x1B2E80,
                _ => 0
            };

            Functions.CreateSoundEvent = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x1A1630,
                Vanilla1_0_12 => 0x1A1860,
                Scholar1_0_2 => 0x10E0D0,
                Scholar1_0_3 => 0x10E190,
                _ => 0
            };

            Functions.BonfireWarp = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x20AE10,
                Vanilla1_0_12 => 0x20D5E0,
                Scholar1_0_2 => 0x181650,
                Scholar1_0_3 => 0x184830,
                _ => 0
            };

            Functions.UnlockBonfire = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x204BD0,
                Vanilla1_0_12 => 0x207370,
                Scholar1_0_2 => 0x17B170,
                Scholar1_0_3 => 0x17E320,
                _ => 0
            };

            Functions.SetEvent = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x478840,
                Vanilla1_0_12 => 0x47FAE0,
                Scholar1_0_2 => 0x46DEC0,
                Scholar1_0_3 => 0x4750B0,
                _ => 0
            };

            Functions.GetMapEntityWithAreaIdAndObjId = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x3E0670,
                Vanilla1_0_12 => 0x3E7610,
                Scholar1_0_2 => 0x3BAD70,
                Scholar1_0_3 => 0x3C1B90,
                _ => 0
            };

            Functions.GetMapObjStateActComponent = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x2433F0,
                Vanilla1_0_12 => 0x246220,
                Scholar1_0_2 => 0x1C7010,
                Scholar1_0_3 => 0x1CA790,
                _ => 0
            };

            Functions.GetNavimeshLoc = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x218950,
                Vanilla1_0_12 => 0x21B700,
                Scholar1_0_2 => 0x472800,
                Scholar1_0_3 => 0x4799F0,
                _ => 0
            };

            Functions.DisableNavimesh = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x260030,
                Vanilla1_0_12 => 0x262E70,
                Scholar1_0_2 => 0x1EA9C0,
                Scholar1_0_3 => 0x1EE140,
                _ => 0
            };

            Functions.GetWhiteDoorComponent = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x458060,
                Vanilla1_0_12 => 0x45F340,
                Scholar1_0_2 => 0x449480,
                Scholar1_0_3 => 0x4506C0,
                _ => 0
            };

            Functions.HavokRayCast = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0xAA55B0,
                Vanilla1_0_12 => 0xAACDF0,
                Scholar1_0_2 => 0xBB84B0,
                Scholar1_0_3 => 0xBBFC10,
                _ => 0
            };

            Functions.ConvertPxRigidToMapEntity = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x41C660,
                Vanilla1_0_12 => 0x423630,
                Scholar1_0_2 => 0x406810,
                Scholar1_0_3 => 0x40D630,
                _ => 0
            };

            Functions.ConvertMapEntityToGameId = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x2039E0,
                Vanilla1_0_12 => 0x206180,
                Scholar1_0_2 => 0x178110,
                Scholar1_0_3 => 0x17B2C0,
                _ => 0
            };

            Functions.ItemGive = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x227F00,
                Vanilla1_0_12 => 0x22AD20,
                Scholar1_0_2 => 0x1A3D00,
                Scholar1_0_3 => 0x1A7470,
                _ => 0
            };

            Functions.BuildItemDialog = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x11F360,
                Vanilla1_0_12 => 0x11F430,
                Scholar1_0_2 => 0x5D8C0,
                Scholar1_0_3 => 0x5D950,
                _ => 0
            };

            Functions.ShowItemDialog = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x4F3730,
                Vanilla1_0_12 => 0x4FA9B0,
                Scholar1_0_2 => 0x4F9E70,
                Scholar1_0_3 => 0x501080,
                _ => 0
            };


            Functions.CurrentItemQuantityCheck = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x22EEF0,
                Vanilla1_0_12 => 0x231D10,
                Scholar1_0_2 => 0x1B1A40,
                Scholar1_0_3 => 0x1B51B0,
                _ => 0
            };


            Functions.Sleep = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x13727BC,
                Vanilla1_0_12 => 0x14547BC,
                _ => 0
            };

            Functions.UpdateSpellSlots = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x230080,
                Vanilla1_0_12 => 0x232EA0,
                Scholar1_0_2 => 0x1B0980,
                Scholar1_0_3 => 0x1B40F0,
                _ => 0
            };

            Functions.GetNumOfSpellSlots1 = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x229380,
                Vanilla1_0_12 => 0x22C1A0,
                Scholar1_0_2 => 0x1A84C0,
                Scholar1_0_3 => 0x1ABC30,
                _ => 0
            };

            Functions.GetNumOfSpellSlots2 = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x229390,
                Vanilla1_0_12 => 0x22C1B0,
                Scholar1_0_2 => 0x1A89A0,
                Scholar1_0_3 => 0x1AC110,
                _ => 0
            };

            Functions.AttuneSpell = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x223F30,
                Vanilla1_0_12 => 0x226D50,
                Scholar1_0_2 => 0x1A4410,
                Scholar1_0_3 => 0x1A7B80,
                _ => 0
            };

            Functions.OpenNpcMenu = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x219EF0,
                Vanilla1_0_12 => 0x21CCA0,
                Scholar1_0_2 => 0x195920,
                Scholar1_0_3 => 0x199020,
                _ => 0
            };

            Functions.SetMenuOpenChrState = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x4F3AB0,
                Vanilla1_0_12 => 0x4FAD30,
                Scholar1_0_2 => 0x4FA1D0,
                Scholar1_0_3 => 0x5013E0,
                _ => 0
            };

            Functions.ParamLookup = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x216460,
                Vanilla1_0_12 => 0x219220,
                Scholar1_0_2 => 0x3E8C0,
                Scholar1_0_3 => 0x3E8F0,
                _ => 0
            };

            Functions.GetEyePosition = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x436250,
                Vanilla1_0_12 => 0x43D490,
                Scholar1_0_2 => 0x422500,
                Scholar1_0_3 => 0x429680,
                _ => 0
            };

            Functions.SetDepthStencilSurface = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x9F41D0,
                Vanilla1_0_12 => 0x9FB740,
                _ => 0
            };
            
            Functions.ApplyDurabilityDamage = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x268D80,
                Vanilla1_0_12 => 0x26BE10,
                Scholar1_0_2 => 0x1F4E80,
                Scholar1_0_3 => 0x1F8710,
                _ => 0
            };



            Hooks.LightGutter = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 or Vanilla1_0_12 => 0xE9259,
                Scholar1_0_2 => 0x14D73,
                Scholar1_0_3 => 0x14DA3,
                _ => 0
            };

            Hooks.NoShadedFogClose = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 or Vanilla1_0_12 => 0xE63FF,
                Scholar1_0_2 => 0x1140B,
                Scholar1_0_3 => 0x1143B,
                _ => 0
            };

            Hooks.NoShadedFogFar = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 or Vanilla1_0_12 => 0xE3C75,
                Scholar1_0_2 => 0xEA90,
                Scholar1_0_3 => 0xEAC0,
                _ => 0
            };

            Hooks.NoShadedFogCam = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x3E6B0F,
                Vanilla1_0_12 => 0x3EDABF,
                Scholar1_0_2 => 0x3C1D30,
                Scholar1_0_3 => 0x3C8B50,
                _ => 0
            };

            Hooks.LockedTarget = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x49E271,
                Vanilla1_0_12 => 0x4A54F1,
                Scholar1_0_2 => 0x495FB2,
                Scholar1_0_3 => 0x49D192,
                _ => 0
            };

            Hooks.SetCurrentAct = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x430E9D,
                Vanilla1_0_12 => 0x43812D,
                Scholar1_0_2 => 0x41F320,
                Scholar1_0_3 => 0x4264A0,
                _ => 0
            };

            Hooks.InfinitePoise = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x1C8F33,
                Vanilla1_0_12 => 0x1CA4A3,
                Scholar1_0_2 => 0x133F6A,
                Scholar1_0_3 => 0x1365DA,
                _ => 0
            };

            Hooks.DamageControl = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x1C5506,
                Vanilla1_0_12 => 0x1C6A76,
                Scholar1_0_2 => 0x133BD2,
                Scholar1_0_3 => 0x136242,
                _ => 0
            };

            Hooks.PlayerNoDamage = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x1F33D1,
                Vanilla1_0_12 => 0x1F5AD1,
                Scholar1_0_2 => 0x16727A,
                Scholar1_0_3 => 0x16A39A,
                _ => 0
            };

            Hooks.ReduceGameSpeed = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x23D348,
                Vanilla1_0_12 => 0x240178,
                Scholar1_0_2 => 0x1BC0C5,
                Scholar1_0_3 => 0x1BEC2DC,
                _ => 0
            };

            Hooks.WarpCoordWrite = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x7F9FB0,
                Vanilla1_0_12 => 0x8015B0,
                Scholar1_0_2 => 0x711939,
                Scholar1_0_3 => 0x718E99,
                _ => 0
            };

            Hooks.SetSharedFlag = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x43120B,
                Vanilla1_0_12 => 0x43849B,
                Scholar1_0_2 => 0x41F452,
                Scholar1_0_3 => 0x4265D2,
                _ => 0
            };

            Hooks.TriggersAndSpace = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0xA09E8F,
                Vanilla1_0_12 => 0xA119CF,
                Scholar1_0_2 => 0xB01E05,
                Scholar1_0_3 => 0xB09B45,
                _ => 0
            };

            Hooks.Ctrl = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0xA09BD2,
                Vanilla1_0_12 => 0xA11712,
                Scholar1_0_2 => 0xB01B5E,
                Scholar1_0_3 => 0xB0989E,
                _ => 0
            };

            Hooks.NoClipUpdateCoords = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x3911ED,
                Vanilla1_0_12 => 0x397B7D,
                Scholar1_0_2 => 0x367622,
                Scholar1_0_3 => 0x36DF42,
                _ => 0
            };

            Hooks.KillboxFlagSet = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x38E8E0,
                Vanilla1_0_12 => 0x395270,
                Scholar1_0_2 => 0x369BCF,
                Scholar1_0_3 => 0x3704EF,
                _ => 0
            };

            Hooks.ProcessPhysics = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x338A9C,
                Vanilla1_0_12 => 0x33EC2C,
                Scholar1_0_2 => 0x30EBA5,
                Scholar1_0_3 => 0x314F35,
                _ => 0
            };

            Hooks.CreditSkip = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x11BD53,
                Vanilla1_0_12 => 0x11BE23,
                Scholar1_0_2 => 0x599D4,
                Scholar1_0_3 => 0x59A64,
                _ => 0
            };

            Hooks.NumOfDrops = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x257E3E,
                Vanilla1_0_12 => 0x25AC6E,
                Scholar1_0_2 => 0x1E00AE,
                Scholar1_0_3 => 0x1E382E,
                _ => 0
            };

            Hooks.SetEventWrapper = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x478C5B,
                Vanilla1_0_12 => 0x47FEFB,
                Scholar1_0_2 => 0x46D889,
                Scholar1_0_3 => 0x474A79,
                _ => 0
            };

            Hooks.EzStateCompareTimer = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x473FA4,
                Vanilla1_0_12 => 0x47B294,
                Scholar1_0_2 => 0x46513C,
                Scholar1_0_3 => 0x46C38C,
                _ => 0
            };

            Hooks.FasterMenu = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x19979E,
                Vanilla1_0_12 => 0x1999BE,
                Scholar1_0_2 => 0x1053B3,
                Scholar1_0_3 => 0x105473,
                _ => 0
            };

            Hooks.BabyJump = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x3A09C9,
                Vanilla1_0_12 => 0x3A7369,
                Scholar1_0_2 => 0x37B4C7,
                Scholar1_0_3 => 0x381E27,
                _ => 0
            };

            Hooks.DisableTargetAi = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x42FAC5,
                Vanilla1_0_12 => 0x436D55,
                Scholar1_0_2 => 0x41C1EB,
                Scholar1_0_3 => 0x42336B,
                _ => 0
            };

            Hooks.CompareEventRandValueElana = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x4631A0,
                Vanilla1_0_12 => 0x46A480,
                Scholar1_0_2 => 0x468380,
                Scholar1_0_3 => 0x46F5D0,
                _ => 0
            };

            HkHardwareInfo.Base = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Scholar1_0_2 => 0x16114C0,
                Scholar1_0_3 => 0x161A4E0,
                _ => 0
            };

            Hooks.SetAreaVariable = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Scholar1_0_2 => 0x45AF77,
                Scholar1_0_3 => 0x4621C7,
                _ => 0
            };

            Hooks.CompareEventRandValueForlorn = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Scholar1_0_2 => 0x468392,
                Scholar1_0_3 => 0x46F5E2,
                _ => 0
            };

            Hooks.GameManUpdate = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Scholar1_0_2 => 0x1BC0A0,
                Scholar1_0_3 => 0x1BEC2B7,
                _ => 0
            };

            Hooks.NewGameDetect = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x4833E8,
                Vanilla1_0_12 => 0x48A688,
                Scholar1_0_2 => 0x47B1DB,
                Scholar1_0_3 => 0x4823CB,
                _ => 0
            };

            Hooks.LoadingItemName = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 or Vanilla1_0_12 => 0xF9B3F,
                Scholar1_0_2 => 0x29825,
                Scholar1_0_3 => 0x29855,
                _ => 0
            };
            
            Hooks.PreAiEzState = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x4326F8,
                Vanilla1_0_12 => 0x439938,
                Scholar1_0_2 => 0x41F87B,
                Scholar1_0_3 => 0x4269FB,
                _ => 0
            };


            Functions.WarpPrep = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x20A670,
                Vanilla1_0_12 => 0x20CE40,
                Scholar1_0_2 => 0x1811D0,
                Scholar1_0_3 => 0x1843B0,
                _ => 0
            };


            Functions.SetRenderTargets = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Scholar1_0_2 => 0xF1C6E0,
                Scholar1_0_3 => 0xF23E40,
                _ => 0
            };

            Functions.EzStateExternalEventCtor = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Scholar1_0_2 => 0x98DC60,
                Scholar1_0_3 => 0x9951B0,
                _ => 0
            };

            Functions.EzStateEventExecuteCommand = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Scholar1_0_2 => 0x45ACD0,
                Scholar1_0_3 => 0x461F20,
                _ => 0
            };

            Functions.OriginalSoulGain = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x273290,
                Vanilla1_0_12 => 0x276570,
                Scholar1_0_2 => 0x1FEB20,
                Scholar1_0_3 => 0x202610,
                _ => 0
            };

            Functions.OriginalMakeSound = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x1A1630,
                Vanilla1_0_12 => 0x1A1860,
                Scholar1_0_2 => 0x10E0D0,
                Scholar1_0_3 => 0x10E190,
                _ => 0
            };
            
            Functions.ResolveTargetCtrlFromHandle = baseAddr + PatchManager.Current.PatchVersion switch
            {
                Vanilla1_0_11 => 0x2A9EE0,
                Vanilla1_0_12 => 0x2AD2A0,
                Scholar1_0_2 => 0x244980,
                Scholar1_0_3 => 0x2485B0,
                _ => 0
            };



#if DEBUG

            _baseAddr = baseAddr;
            Console.WriteLine("\n========== OFFSETS DEBUG ==========\n");

            Console.WriteLine("--- Base Pointers ---");
            PrintOffset("GameManagerImp.Base", GameManagerImp.Base);
            PrintOffset("HkHardwareInfo.Base", HkHardwareInfo.Base);
            PrintOffset("KatanaMainApp.Base", KatanaMainApp.Base);
            PrintOffset("MapId", MapId);
            PrintOffset("LoadLibraryW", LoadLibraryW);
            PrintOffset("BuildTextFieldRetAddr", BuildTextFieldRetAddr);

            Console.WriteLine("\n--- Patches ---");
            PrintOffset("InfiniteStam", Patches.InfiniteStam);
            PrintOffset("InfiniteGoods", Patches.InfiniteGoods);
            PrintOffset("InfiniteCasts", Patches.InfiniteCasts);
            PrintOffset("InfiniteDurability", Patches.InfiniteDurability);
            PrintOffset("HideChrModels", Patches.HideChrModels);
            PrintOffset("HideMap", Patches.HideMap);
            PrintOffset("DropRate", Patches.DropRate);
            PrintOffset("DisableAi", Patches.DisableAi);
            PrintOffset("Silent", Patches.Silent);
            PrintOffset("Hidden", Patches.Hidden);
            PrintOffset("NegativeLevel", Patches.NegativeLevel);
            PrintOffset("Ng7", Patches.Ng7);
            PrintOffset("NoSoulGain", Patches.NoSoulGain);
            PrintOffset("NoHollowing", Patches.NoHollowing);
            PrintOffset("NoSoulLoss", Patches.NoSoulLoss);
            PrintOffset("SoulMemWrite1", Patches.SoulMemWrite1);
            PrintOffset("SoulMemWrite2", Patches.SoulMemWrite2);
            PrintOffset("NoHitPatch", Patches.NoHitPatch);
            PrintOffset("MenuTransition", Patches.MenuTransition);

            Console.WriteLine("\n--- Hooks ---");
            PrintOffset("SetAreaVariable", Hooks.SetAreaVariable);
            PrintOffset("CompareEventRandValueForlorn", Hooks.CompareEventRandValueForlorn);
            PrintOffset("CompareEventRandValueElana", Hooks.CompareEventRandValueElana);
            PrintOffset("PlayerNoDamage", Hooks.PlayerNoDamage);
            PrintOffset("WarpCoordWrite", Hooks.WarpCoordWrite);
            PrintOffset("LockedTarget", Hooks.LockedTarget);
            PrintOffset("CreditSkip", Hooks.CreditSkip);
            PrintOffset("NumOfDrops", Hooks.NumOfDrops);
            PrintOffset("DamageControl", Hooks.DamageControl);
            PrintOffset("TriggersAndSpace", Hooks.TriggersAndSpace);
            PrintOffset("Ctrl", Hooks.Ctrl);
            PrintOffset("NoClipUpdateCoords", Hooks.NoClipUpdateCoords);
            PrintOffset("KillboxFlagSet", Hooks.KillboxFlagSet);
            PrintOffset("SetCurrentAct", Hooks.SetCurrentAct);
            PrintOffset("FasterMenu", Hooks.FasterMenu);
            PrintOffset("InfinitePoise", Hooks.InfinitePoise);
            PrintOffset("SetEventWrapper", Hooks.SetEventWrapper);
            PrintOffset("ProcessPhysics", Hooks.ProcessPhysics);
            PrintOffset("DisableTargetAi", Hooks.DisableTargetAi);
            PrintOffset("SetSharedFlag", Hooks.SetSharedFlag);
            PrintOffset("BabyJump", Hooks.BabyJump);
            PrintOffset("EzStateCompareTimer", Hooks.EzStateCompareTimer);
            PrintOffset("NoShadedFogClose", Hooks.NoShadedFogClose);
            PrintOffset("ReduceGameSpeed", Hooks.ReduceGameSpeed);
            PrintOffset("LightGutter", Hooks.LightGutter);
            PrintOffset("NoShadedFogFar", Hooks.NoShadedFogFar);
            PrintOffset("NoShadedFogCam", Hooks.NoShadedFogCam);
            PrintOffset("GameManUpdate", Hooks.GameManUpdate);
            PrintOffset("NewGameDetect", Hooks.NewGameDetect);
            PrintOffset("LoadingItemName", Hooks.LoadingItemName);
            PrintOffset("PreAiEzState", Hooks.PreAiEzState);

            Console.WriteLine("\n--- Functions ---");
            PrintOffset("WarpPrep", Functions.WarpPrep);
            PrintOffset("BonfireWarp", Functions.BonfireWarp);
            PrintOffset("SetEvent", Functions.SetEvent);
            PrintOffset("GetEvent", Functions.GetEvent);
            PrintOffset("GiveSouls", Functions.GiveSouls);
            PrintOffset("RestoreSpellcasts", Functions.RestoreSpellcasts);
            PrintOffset("ParamLookup", Functions.ParamLookup);
            PrintOffset("SetRenderTargets", Functions.SetRenderTargets);
            PrintOffset("CreateSoundEvent", Functions.CreateSoundEvent);
            PrintOffset("LevelLookup", Functions.LevelLookup);
            PrintOffset("LevelUp", Functions.LevelUp);
            PrintOffset("CurrentItemQuantityCheck", Functions.CurrentItemQuantityCheck);
            PrintOffset("ItemGive", Functions.ItemGive);
            PrintOffset("BuildItemDialog", Functions.BuildItemDialog);
            PrintOffset("ShowItemDialog", Functions.ShowItemDialog);
            PrintOffset("GetEyePosition", Functions.GetEyePosition);
            PrintOffset("SetSpEffect", Functions.SetSpEffect);
            PrintOffset("HavokRayCast", Functions.HavokRayCast);
            PrintOffset("ConvertPxRigidToMapEntity", Functions.ConvertPxRigidToMapEntity);
            PrintOffset("ConvertMapEntityToGameId", Functions.ConvertMapEntityToGameId);
            PrintOffset("UnlockBonfire", Functions.UnlockBonfire);
            PrintOffset("GetMapObjStateActComponent", Functions.GetMapObjStateActComponent);
            PrintOffset("GetMapEntityWithAreaIdAndObjId", Functions.GetMapEntityWithAreaIdAndObjId);
            PrintOffset("GetNavimeshLoc", Functions.GetNavimeshLoc);
            PrintOffset("DisableNavimesh", Functions.DisableNavimesh);
            PrintOffset("GetWhiteDoorComponent", Functions.GetWhiteDoorComponent);
            PrintOffset("AttuneSpell", Functions.AttuneSpell);
            PrintOffset("GetNumOfSpellSlots1", Functions.GetNumOfSpellSlots1);
            PrintOffset("GetNumOfSpellSlots2", Functions.GetNumOfSpellSlots2);
            PrintOffset("UpdateSpellSlots", Functions.UpdateSpellSlots);
            PrintOffset("Sleep", Functions.Sleep);
            PrintOffset("SetDepthStencilSurface", Functions.SetDepthStencilSurface);
            PrintOffset("EzStateExternalEventCtor", Functions.EzStateExternalEventCtor);
            PrintOffset("EzStateEventExecuteCommand", Functions.EzStateEventExecuteCommand);
            PrintOffset("OriginalMakeSound", Functions.OriginalMakeSound);
            PrintOffset("OriginalSoulGain", Functions.OriginalSoulGain);
            PrintOffset("OpenNpcMenu", Functions.OpenNpcMenu);
            PrintOffset("ApplyDurabilityDamage", Functions.ApplyDurabilityDamage);
            PrintOffset("ResolveTargetCtrlFromHandle", Functions.ResolveTargetCtrlFromHandle);

            Console.WriteLine("\n====================================\n");
#endif
            return true;
        }

#if DEBUG

        private static nint _baseAddr;
        private static void PrintOffset(string name, long value)
        {
            var rel = value - _baseAddr;
            Console.WriteLine(rel <= 0
                ? $"  {name,-40} *** NOT SET ***"
                : $"  {name,-40} 0x{value:X}  (0x{rel:X})");
        }
#endif
    }
}