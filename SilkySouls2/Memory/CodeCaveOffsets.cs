﻿using System;

namespace SilkySouls2.Memory
{
    public static class CodeCaveOffsets
    {
        public static IntPtr Base;

        public enum Forlorn
        {
            EsdFuncId = 0x0,
            SelectedForlorn = 0x4,
            CompRandValueCode = 0x10,
            SetAreaVarCode = 0x40
        }

        public enum DamageControl
        {
            OneShotFlag = 0x71,
            DealNoDamageFlag = 0x72,
            FreezeTargetHpFlag = 0x73,
            DamageControlCode = 0x80
        }

        public enum BonfireWarp
        {
            Output = 0x110,
            BonfireId = 0x190,
            WarpCode = 0x1A0,
            Coords = 0x200,
            CoordWrite = 0x240,
        }

        public const int LockedTargetPtr = 0x300;
        public const int SaveLockedTarget = 0x310;

        public enum EventWarp
        {
            Params = 0x320,
            Code = 0x360
        }

        public enum CreditSkip
        {
            ModifyOnceFlag = 0x390,
            Code = 0x3A0
        }

        public enum SavedPos
        {
            Pos1 = 0x3D0,
            Pos2 = 0x410
        }

        public enum Drop100
        {
            DropCount = 0x460,
        }

        public const int PlayerNoDamage = 0x470;

        public enum NoClip
        {
            ZDirection = 0x4A0,
            TriggersAndSpaceCheck = 0x4B0,
            CtrlCheck = 0x510,
            UpdateCoords = 0x1750,
            RayInput = 0xF00,
            RayOutput = 0x2200,
            MapId = 0xF80,
            FrameCounter = 0xF84,
            RayCastCode = 0xF90
        }

        public const int Killbox = 0x670;

        public enum RepeatAct
        {
            RepeatFlag = 0x6C0,
            AttackId = 0x6C4,
            Code = 0x6D0,
            Code2 = 0x720
        }

        public const int FastQuitout = 0x770;
        public const int InfinitePoise = 0x790;

        public enum LevelUp
        {
            NegativeFlag = 0x7BF,
            Buffer = 0x7C0,
            Code = 0x8C0
        }

        public enum ItemSpawn
        {
            ShouldExitFlag = 0x9BD,
            ShouldProcessFlag = 0x9BE,
            AdjustQuantityFlag = 0x9BF,
            MaxQuantity = 0x9C0,
            ItemCount = 0x9C4,
            CurrentQuantity = 0x9C8,
            StackCount = 0x9CC,
            ItemStruct = 0x9D0,
            StackSpace = 0x9E0,
            Code = 0xB00,
        }

        public const int Snowstorm = 0xD00;
        public const int SpEffectParams = 0xD20;
        public const int SpEffectCode = 0xD30;

        public const int IvorySkip = 0x1400;

        public enum DisableTargetAi
        {
            Array = 0x1500,
            Count = 0x1600,
            Code = 0x1610,
        }

        public const int IvoryKnights = 0x1700;

        public const int BabyJump = 0x2000;

        public const int DisableMemoryTimer = 0x2050;
        public const int NumOfSpellSlots = 0x2150;

        public const int SlowdownFactor = 0x21A0;
        public const int SlowdownCode = 0x21B0;


        public const int LightGutter = 0x1A00;
        public const int NoFogClose = 0x1A40;
        public const int NoFogFar = 0x1A80;
        public const int NoFogCam = 0x1AB0;
        
        public const int GetEventResult = 0x1B00;
    }
}