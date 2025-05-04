using System;

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
            EmptySpace = 0x110,
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
            InAirTimer = 0x4A0,
            ZDirection = 0x4C0,
            TriggersAndSpaceCheck = 0x4D0,
            CtrlCheck = 0x530,
            UpdateCoords = 0x540,
        }

        public const int Killbox = 0x690;
    }
}