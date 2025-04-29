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
    }
}