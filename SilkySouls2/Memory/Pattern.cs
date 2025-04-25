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


        // Hooks

        public static readonly Pattern OverrideGeneratorStartPositionRandom = new Pattern(
            new byte[] { 0x8B, 0x41, 0x18, 0x8B, 0x40, 0x50 },
            "xxxxxx",
            0,
            None
        );

        public static readonly Pattern SetAreaVariable = new Pattern(
            new byte[] { 0x40, 0x88, 0x74, 0x24, 0x28, 0x89, 0x6C },
            "xxxxxxx",
            0,
            None
        );

        public static readonly Pattern CompareEventRandValue = new Pattern(
            new byte[] { 0x44, 0x8B, 0x41, 0x1C, 0x8B, 0x54 },
            "xxxxxx",
            0,
            None
            );
    }
}