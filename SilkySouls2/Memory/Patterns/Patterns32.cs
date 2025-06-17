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

        public static readonly Pattern NoSoulGain = new Pattern(
            new byte[] { 0xD9, 0x6D, 0x16, 0xE8 },
            "xxxx",
            3,
            AddressingMode.Absolute
        );

        public static readonly Pattern NoHollowing = new Pattern(
            new byte[] { 0x88, 0x45, 0x0B, 0x79 },
            "xxxx",
            0x22,
            AddressingMode.Absolute
        );

        public static readonly Pattern NoSoulLoss = new Pattern(
            new byte[] { 0x75, 0x0A, 0xC7, 0x80, 0xE8 },
            "xxxxx",
            2,
            AddressingMode.Absolute
        );

        public static readonly Pattern Hidden = new Pattern(
            new byte[] { 0x0F, 0x84, 0x3D, 0x02, 0x00, 0x00, 0x85 },
            "xxxxxxx",
            0,
            AddressingMode.Absolute
        );

        public static readonly Pattern SoulMemWrite = new Pattern(
            new byte[] { 0x89, 0x81, 0xF0, 0x00, 0x00, 0x00, 0x8B, 0x81 },
            "xxxxxxxx",
            0,
            AddressingMode.Absolute
        );

        public static readonly Pattern DisableAi = new Pattern(
            new byte[] { 0x83, 0x7B, 0x18, 0x00, 0x7F },
            "xxxxx",
            4,
            AddressingMode.Absolute
        );
        
        public static readonly Pattern Silent = new Pattern(
            new byte[] { 0xE8, 0x00, 0x00, 0x00, 0x00, 0x84, 0xC0, 0x74, 0x3F, 0x8B, 0x86 },
            "x????xxxxxx",
            -0xB,
            AddressingMode.Absolute
        );


        public static readonly Pattern Ng7Patch = new Pattern(
            new byte[] { 0xC7, 0x47, 0x68, 0x01 },
            "xxxx",
            0,
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

        public static readonly Pattern GiveSouls = new Pattern(
            new byte[] { 0xE8, 0x00, 0x00, 0x00, 0x00, 0xC6, 0x86, 0x04, 0x07 },
            "x????xxxx",
            0,
            AddressingMode.Relative,
            1,
            5
        );

        public static readonly Pattern LevelUp = new Pattern(
            new byte[] { 0x84, 0xC0, 0x0F, 0x84, 0xAA, 0x00, 0x00, 0x00, 0x8B, 0x0B },
            "xxxxxxxxxx",
            -0x2F,
            AddressingMode.Absolute
        );

        public static readonly Pattern LevelLookup = new Pattern(
            new byte[] { 0x75, 0x0B, 0x50, 0xE8, 0x00, 0x00, 0x00, 0x00, 0x83, 0xC4, 0x04, 0xEB, 0x02 },
            "xxxx????xxxxx",
            0x3,
            AddressingMode.Relative,
            1,
            5
        );

        public static readonly Pattern RestoreSpellcasts = new Pattern(
            new byte[] { 0x55, 0x8B, 0xEC, 0x83, 0xEC, 0x3C, 0xF3, 0x0F, 0x10, 0x45, 0x08 },
            "xxxxxxxxxxx",
            0,
            AddressingMode.Absolute
        );

        public static readonly Pattern CreateSoundEvent = new Pattern(
            new byte[] { 0xE8, 0x00, 0x00, 0x00, 0x00, 0x84, 0xC0, 0x74, 0x3F, 0x8B, 0x86 },
            "x????xxxxxx",
            0,
            AddressingMode.Relative,
            1,
            5
        );

        
        
        // Hooks

        public static readonly Pattern LockedTarget = new Pattern(
            new byte[] { 0x89, 0xB7, 0xB8, 0x00, 0x00, 0x00, 0xEB },
            "xxxxxxx",
            0,
            AddressingMode.Absolute
        );
        
        public static readonly Pattern SetCurrentAct = new Pattern(
            new byte[]{ 0x83, 0x89, 0x50, 0x02, 0x00, 0x00, 0x01 },
            "xxxxxxx",
            7,
            AddressingMode.Absolute
        );

        public static readonly Pattern InfinitePoise = new Pattern(
            new byte[] { 0x83, 0xBB, 0xEC, 0x05, 0x00, 0x00, 0x00, 0x0F, 0x95, 0x45, 0xFF },
            "xxxxxxxxxxx",
            0,
            AddressingMode.Absolute
        );

        public static readonly Pattern DamageControl = new Pattern(
            new byte[] { 0x8B, 0x45, 0x10, 0x8B, 0x17 },
            "xxxxx",
            -0x17,
            AddressingMode.Absolute
        );

        public static readonly Pattern HpWrite = new Pattern(
            new byte[] { 0x89, 0x8E, 0xFC, 0x00, 0x00, 0x00, 0x8B, 0x02 },
            "xxxxxxxx",
            0,
            AddressingMode.Absolute
        );
    }
    
}