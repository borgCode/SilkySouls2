namespace SilkySouls2.Memory.Patterns
{
    public static class Patterns32
    {
        public static readonly Pattern GameManagerImp = new Pattern(
            new byte[] { 0xA1, 0x00, 0x00, 0x00, 0x00, 0x56, 0x8B, 0xB0, 0xCC, 0x0C, 0x00, 0x00 },
            "x????xxxxxxx",
            0,
            AddressingMode.Direct32,
            1
        );
    }
}