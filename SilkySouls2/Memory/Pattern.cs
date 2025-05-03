using System.Diagnostics.Tracing;
using static SilkySouls2.Memory.RipType;

namespace SilkySouls2.Memory
{
    public class Pattern
    {
        public byte[] Bytes { get; }
        public string Mask { get; }
        public int InstructionOffset { get; }
        public RipType RipType { get; }

        public Pattern(byte[] bytes, string mask, int instructionOffset, RipType ripType)
        {
            Bytes = bytes;
            Mask = mask;
            InstructionOffset = instructionOffset;
            RipType = ripType;
        }
    }

    public static class Patterns
    {
        public static readonly Pattern GameManagerImp = new Pattern(
            new byte[] { 0x89, 0x59, 0x0C, 0x88 },
            "xxxx",
            0x5,
            Mov64
        );

        public static readonly Pattern HkpPtrEntity = new Pattern(
            new byte[] { 0x48, 0x8B, 0x05, 0x00, 0x00, 0x00, 0x00, 0x83, 0x78, 0x10 },
            "xxx????xxx",
            0,
            Mov64
        );


        // Hooks

        public static readonly Pattern HpWrite = new Pattern(
            new byte[] { 0x48, 0x0F, 0x4F, 0xC6, 0x8B, 0x00 },
            "xxxxxx",
            0x6,
            None
        );

        public static readonly Pattern DamageControl = new Pattern(
            new byte[] { 0x0F, 0x29, 0x74, 0x24, 0x20, 0x48, 0x8B, 0xB0 },
            "xxxxxxxx",
            0,
            None
        );

        public static readonly Pattern InAirTimer = new Pattern(
            new byte[] { 0xF3, 0x0F, 0x11, 0x4F, 0x10, 0x49 },
            "xxxxxx",
            0,
            None
        );

        public static readonly Pattern TriggersAndSpace = new Pattern(
            new byte[] { 0x4C, 0x8B, 0x7C, 0x24, 0x70, 0x48, 0x8B, 0x43 },
            "xxxxxxxx",
            0,
            None
        );

        public static readonly Pattern Ctrl = new Pattern(
            new byte[] { 0x74, 0x0A, 0x81, 0x8B, 0x28, 0x02, 0x00, 0x00, 0x00, 0x02 },
            "xxxxxxxxxx",
            0x2,
            None
        );

        public static readonly Pattern NoClipUpdateCoords = new Pattern(
            new byte[] { 0x0F, 0x29, 0x47, 0x70, 0x8B },
            "xxxxx",
            0,
            None
        );

        
        public static readonly Pattern WarpCoordWrite = new Pattern(
            new byte[] { 0x0F, 0x5C, 0xC2, 0x0F, 0x29, 0x47, 0x50 },
            "xxxxxxx",
            0,
            None);

        public static readonly Pattern SetAreaVariable = new Pattern(
            new byte[] { 0x0F, 0x84, 0xC8, 0xB8 },
            "xxxx",
            0x6,
            None
        );

        public static readonly Pattern CompareEventRandValue = new Pattern(
            new byte[] { 0x44, 0x8B, 0x41, 0x1C, 0x8B, 0x54 },
            "xxxxxx",
            0,
            None
        );

        public static readonly Pattern LockedTarget = new Pattern(
            new byte[] { 0x48, 0x89, 0xBB, 0xC0, 0x00, 0x00, 0x00, 0xEB },
            "xxxxxxxx",
            0,
            None
        );


        public static readonly Pattern CreditSkip = new Pattern(
            new byte[] { 0x48, 0x81, 0xEC, 0x20, 0x02, 0x00, 0x00, 0x8B, 0x41 },
            "xxxxxxxxx",
            0,
            None
        );

        // public static readonly Pattern DropRate = new Pattern(
        //     new byte[] { 0xF3, 0x41, 0x0F, 0x10, 0x04, 0x07 },
        //     "xxxxxx",
        //     0,
        //     None
        // );

        public static readonly Pattern NumOfDrops = new Pattern(
            new byte[] { 0x41, 0x0F, 0xB6, 0x47, 0x01, 0xFF },
            "xxxxxx",
            0,
            None
        );

        



        //Funcs

        public static readonly Pattern WarpPrep = new Pattern(
            new byte[] { 0x48, 0x8D, 0x4C, 0x24, 0x78, 0x0F, 0xB7 },
            "xxxxxxx",
            -0x15,
            None
        );

        public static readonly Pattern BonfireWarp = new Pattern(
            new byte[] { 0x40, 0x53, 0x48, 0x83, 0xEC, 0x60, 0x8B },
            "xxxxxxx",
            0,
            None
        );

        public static readonly Pattern SetEvent = new Pattern(
            new byte[] { 0x45, 0x0F, 0xB6, 0xD8, 0xB8 },
            "xxxxx",
            -0xC,
            None
        );


        // Patches

        public static readonly Pattern InfiniteStam = new Pattern(
            new byte[] { 0x0F, 0x83, 0x26, 0x01, 0x00, 0x00, 0x48, 0x8B },
            "xxxxxxxx",
            0,
            None
        );

        public static readonly Pattern ForceSave = new Pattern(
            new byte[] { 0x74, 0x5A, 0x8B, 0x4B },
            "xxxx",
            0,
            None
        );

        public static readonly Pattern InfiniteGoods = new Pattern(
            new byte[] { 0x66, 0x29, 0x73, 0x20 },
            "xxxx",
            0,
            None
        );

        public static readonly Pattern HideChrModels = new Pattern(
            new byte[] { 0x74, 0x05, 0x80, 0xC9, 0x08 },
            "xxxxx",
            0,
            None
        );

        public static readonly Pattern InfiniteCasts = new Pattern(
            new byte[] { 0x88, 0x4D, 0x20, 0x49 },
            "xxxx",
            0,
            None
        );

        public static readonly Pattern InfiniteDurability = new Pattern(
            new byte[] { 0xF3, 0x0F, 0x11, 0xB4, 0xC3 },
            "xxxxx",
            0,
            None
        );

        public static readonly Pattern DropRate = new Pattern(
            new byte[] { 0x41, 0xF7, 0xF2, 0x41, 0x3B, 0xD1 },
            "xxxxxx",
            0,
            None
        );

    }
}