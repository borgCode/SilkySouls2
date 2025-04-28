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
            PlayerNoDamageFlag = 0x70,
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
            Coords = 0x1F0,
            CoordWrite = 0x230,
        }
        
        public const int LockedTargetPtr = 0x300;
        public const int SaveLockedTarget = 0x310;
    }
}