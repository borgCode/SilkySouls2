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

        public const int NoDamagePlayer = 0x70;
        public const int OneShot = 0xA0;
        public const int DealNoDamage = 0xD0;

        public enum BonfireWarp
        {
            EmptySpace = 0x110,
            BonfireId = 0x190,
            WarpCode = 0x1A0,
            Coords = 0x1E0,
            CoordWrite = 0x200 
        }
    }
}