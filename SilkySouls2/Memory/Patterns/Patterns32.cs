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
        
        
        
        
        //Patches

        public static readonly Pattern InfiniteStam = new Pattern(
            new byte[] { 0x0F, 0x83, 0x79, 0x01 },
            "xxxx",
            0,
            AddressingMode.Absolute
        );

        public static readonly Pattern InfiniteGoods = new Pattern(
            new byte[] { 0x80, 0xFA, 0x02, 0x75, 0x17 },
            "xxxxx",
            -0xDB,
            AddressingMode.Absolute
        );

        public static readonly Pattern InfiniteDurability = new Pattern(
            new byte[] { 0x53, 0x8B, 0xCE, 0xF3, 0x0F, 0x11, 0x47 },
            "xxxxxxx",
            3,
            AddressingMode.Absolute
        );

        public static readonly Pattern InfiniteCasts = new Pattern(
            new byte[] { 0x8B, 0xCF, 0x88, 0x43 },
            "xxxx",
            2,
            AddressingMode.Absolute
        );
        
        
        
        
        // Funcs

        public static readonly Pattern SetSpEffect = new Pattern(
            new byte[] { 0x50, 0x8D, 0x45, 0xE8, 0x50, 0xE8 },
            "xxxxxx",
            0x10,
            AddressingMode.Relative,
            1,
            5
        );
    }
    
}