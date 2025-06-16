namespace SilkySouls2.Memory.Patterns
{
    public class Pattern
    {
        public byte[] Bytes { get; }
        public string Mask { get; }
        public int InstructionOffset { get; }
        public AddressingMode AddressingMode { get; }
        public int OffsetLocation { get; }
        public int InstructionLength { get; }

        public Pattern(byte[] bytes, string mask, int instructionOffset, AddressingMode addressingMode, int offsetLocation = 0, int instructionLength = 0)
        {
            Bytes = bytes;
            Mask = mask;
            InstructionOffset = instructionOffset;
            AddressingMode = addressingMode;
            OffsetLocation = offsetLocation;
            InstructionLength = instructionLength;
        }
    }

    public enum AddressingMode
    {
        Absolute,
        Relative
    }
}