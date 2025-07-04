﻿namespace SilkySouls2.Memory.Patterns
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

        public static readonly Pattern MapId = new Pattern(
            new byte[] { 0x8B, 0x15, 0x00, 0x00, 0x00, 0x00, 0x8B, 0xF2 },
            "xx????xx",
            0,
            AddressingMode.Direct32,
            2
        );

        public static readonly Pattern LoadLibraryW = new Pattern(
            new byte[] { 0xFF, 0x15, 0x00, 0x00, 0x00, 0x00, 0x8B, 0xF8, 0x85, 0xFF, 0x0F },
            "xx????xxxxx",
            0,
            AddressingMode.Direct32,
            2,
            6
        );

        public static readonly Pattern ParamLookup = new Pattern(
            new byte[] { 0x80, 0x7B, 0x36, 0x00 },
            "xxxx",
            0xC,
            AddressingMode.Relative,
            1,
            5
        );

        public static readonly Pattern GetEyePosition = new Pattern(
            new byte[] { 0xFF, 0xD0, 0x83, 0x7D, 0xC0 },
            "xxxxx",
            -0x4F,
            AddressingMode.Absolute
        );

        public static readonly Pattern SetDepthStencilSurface = new Pattern(
            new byte[] { 0xE8, 0x00, 0x00, 0x00, 0x00, 0x8B, 0x56, 0x44, 0x8B, 0x4C },
            "x????xxxxx",
            0,
            AddressingMode.Relative,
            1,
            5
        );


        //Patches


        public static readonly Pattern HideMap = new Pattern(
            new byte[] { 0x0F, 0x88, 0x94, 0x00 },
            "xxxx",
            0,
            AddressingMode.Absolute
        );

        public static readonly Pattern HideChrModels = new Pattern(
            new byte[] { 0x74, 0x05, 0x0A, 0x4D },
            "xxxx",
            0,
            AddressingMode.Absolute
        );

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

        public static readonly Pattern DropRate = new Pattern(
            new byte[] { 0xF7, 0xF6, 0x89, 0x79 },
            "xxxx",
            0,
            AddressingMode.Absolute
        );


        // Funcs

        public static readonly Pattern GetEvent = new Pattern(
            new byte[] { 0x74, 0x10, 0x8B, 0x52 },
            "xxxx",
            -0x3B,
            AddressingMode.Absolute
        );


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

        public static readonly Pattern BonfireWarp = new Pattern(
            new byte[] { 0x6A, 0x02, 0x51, 0x8D, 0x55, 0xB0 },
            "xxxxxx",
            0,
            AddressingMode.Absolute
        );

        public static readonly Pattern UnlockBonfire = new Pattern(
            new byte[] { 0x74, 0x60, 0x3D, 0xFF },
            "xxxx",
            -0xF,
            AddressingMode.Absolute
        );

        public static readonly Pattern SetEvent = new Pattern(
            new byte[] { 0x39, 0x4A, 0x08, 0x74, 0x12 },
            "xxxxx",
            -0x40,
            AddressingMode.Absolute
        );

        public static readonly Pattern GetMapEntityWithAreaIdAndObjId = new Pattern(
            new byte[] { 0x39, 0x42, 0x1C, 0x7E },
            "xxxx",
            0x1B,
            AddressingMode.Relative,
            1,
            5
        );

        public static readonly Pattern GetStateActComp = new Pattern(
            new byte[] { 0x75, 0x09, 0x8B, 0x01, 0x8B, 0x40, 0x20 },
            "xxxxxxx",
            -0xB,
            AddressingMode.Absolute
        );


        public static readonly Pattern DisableNavimesh = new Pattern(
            new byte[] { 0x85, 0xC0, 0x0F, 0x84, 0xB5, 0x60 },
            "xxxxxx",
            0,
            AddressingMode.Absolute
        );

        public static readonly Pattern GetWhiteDoorComponent = new Pattern(
            new byte[] { 0x85, 0xF6, 0x0F, 0x95, 0xC1, 0x88 },
            "xxxxxx",
            -0xD,
            AddressingMode.Relative,
            1,
            5
        );

        public static readonly Pattern HavokRayCast = new Pattern(
            new byte[] { 0x83, 0xCF, 0xFF, 0x33, 0xD2 },
            "xxxxx",
            -0x33,
            AddressingMode.Absolute
        );

        public static readonly Pattern ConvertPxRigidToMapEntity = new Pattern(
            new byte[] { 0xE8, 0x00, 0x00, 0x00, 0x00, 0x83, 0xC4, 0x08, 0x85, 0xC0, 0x0F, 0x84, 0x91 },
            "x????xxxxxxxx",
            0,
            AddressingMode.Absolute
        );

        public static readonly Pattern ItemGive = new Pattern(
            new byte[] { 0x83, 0xF8, 0x1F, 0x0F, 0x87, 0xA0 },
            "xxxxxx",
            -0x1B,
            AddressingMode.Absolute
        );

        public static readonly Pattern BuildItemDialog = new Pattern(
            new byte[] { 0x83, 0xFB, 0x08, 0xC7 },
            "xxxx",
            -0x72,
            AddressingMode.Absolute
        );

        public static readonly Pattern ShowItemDialog = new Pattern(
            new byte[] { 0x83, 0x78, 0x08, 0x00, 0x74, 0x36 },
            "xxxxxx",
            0x24,
            AddressingMode.Relative,
            1,
            5
        );


        public static readonly Pattern CurrentItemQuantityCheck = new Pattern(
            new byte[] { 0x80, 0xE1, 0x03, 0x57, 0x8B },
            "xxxxx",
            0x17,
            AddressingMode.Relative,
            1,
            5
        );

        public static readonly Pattern Sleep = new Pattern(
            new byte[] { 0x8B, 0x1D, 0x00, 0x00, 0x00, 0x00, 0x83, 0xF8 },
            "xx????xx",
            0,
            AddressingMode.Direct32,
            2,
            6
        );

        public static readonly Pattern UpdateSpellSlots = new Pattern(
            new byte[] { 0x80, 0xF9, 0x0E, 0x77 },
            "xxxx",
            -0x38,
            AddressingMode.Absolute
        );

        public static readonly Pattern GetNumOfSpellSlots = new Pattern(
            new byte[] { 0x03, 0xD8, 0x8B, 0xCE },
            "xxxx",
            0,
            AddressingMode.Absolute
        );


        public static readonly Pattern AttuneSpell = new Pattern(
            new byte[] { 0x83, 0xF8, 0x29, 0x77, 0x29 },
            "xxxxx",
            -0x7,
            AddressingMode.Absolute
        );


        // Hooks

        public static readonly Pattern LightGutter = new Pattern(
            new byte[] { 0xF3, 0x0F, 0x7E, 0x86, 0x58, 0xFF, 0xFF, 0xFF, 0x66, 0x0F, 0xD6, 0x87, 0xE8 },
            "xxxxxxxxxxxxx",
            0,
            AddressingMode.Absolute
        );

        public static readonly Pattern NoShadedFogClose = new Pattern(
            new byte[] { 0x0F, 0xB6, 0x46, 0x07, 0x89 },
            "xxxxx",
            0,
            AddressingMode.Absolute
        );

        public static readonly Pattern NoShadedFogFar = new Pattern(
            new byte[] { 0x52, 0x8B, 0xCF, 0x89, 0x46, 0x0C, 0xF3 },
            "xxxxxxx",
            0x6,
            AddressingMode.Absolute
        );

        public static readonly Pattern NoShadedFogCam = new Pattern(
            new byte[] { 0x89, 0x4D, 0xE4, 0x33, 0xC9, 0x81 },
            "xxxxxx",
            0,
            AddressingMode.Absolute
        );


        public static readonly Pattern LockedTarget = new Pattern(
            new byte[] { 0x89, 0xB7, 0xB8, 0x00, 0x00, 0x00, 0xEB },
            "xxxxxxx",
            0,
            AddressingMode.Absolute
        );

        public static readonly Pattern SetCurrentAct = new Pattern(
            new byte[] { 0x83, 0x89, 0x50, 0x02, 0x00, 0x00, 0x01 },
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

        public static readonly Pattern ReduceGameSpeed = new Pattern(
            new byte[] { 0x8B, 0x7B, 0x08, 0xF3, 0x0F, 0x10, 0x07, 0x8B, 0xF1, 0xF6 },
            "xxxxxxxxxx",
            0,
            AddressingMode.Absolute
        );


        public static readonly Pattern WarpCoordWrite = new Pattern(
            new byte[] { 0x0F, 0x5C, 0xC1, 0x0F, 0x29, 0x46, 0x40, 0x80 },
            "xxxxxxxx",
            0,
            AddressingMode.Absolute
        );

        public static readonly Pattern SetSharedFlag = new Pattern(
            new byte[] { 0x88, 0x94, 0x08, 0xA1 },
            "xxxx",
            0,
            AddressingMode.Absolute
        );

        public static readonly Pattern TriggersAndSpace = new Pattern(
            new byte[] { 0x8B, 0x56, 0x08, 0x89, 0x86 },
            "xxxxx",
            0,
            AddressingMode.Absolute
        );

        public static readonly Pattern Ctrl = new Pattern(
            new byte[] { 0x81, 0x8E, 0x28, 0x02, 0x00, 0x00, 0x00, 0x02 },
            "xxxxxxxx",
            0,
            AddressingMode.Absolute
        );

        public static readonly Pattern NoClipUpdateCoords = new Pattern(
            new byte[] { 0xF3, 0x0F, 0x7E, 0x45, 0xD0, 0x66, 0x0F, 0xD6, 0x40, 0x70 },
            "xxxxxxxxxx",
            0,
            AddressingMode.Absolute
        );

        public static readonly Pattern KillboxFlagSet = new Pattern(
            new byte[] { 0x81, 0x88, 0xC4, 0x04, 0x00, 0x00, 0x00, 0x00, 0x08, 0x00, 0x83 },
            "xxxxxxxxxxx",
            -0x25,
            AddressingMode.Absolute
        );


        public static readonly Pattern ProcessPhysics = new Pattern(
            new byte[] { 0x8B, 0x8E, 0xB8, 0x00, 0x00, 0x00, 0x8D, 0x45 },
            "xxxxxxxx",
            0,
            AddressingMode.Absolute
        );

        public static readonly Pattern CreditSkip = new Pattern(
            new byte[] { 0x81, 0xEC, 0xFC, 0x01, 0x00, 0x00, 0x53, 0x8B, 0xD9, 0x8B, 0x43, 0x14 },
            "xxxxxxxxxxxx",
            0,
            AddressingMode.Absolute
        );

        public static readonly Pattern NumOfDrops = new Pattern(
            new byte[] { 0x0F, 0xB6, 0x51, 0x01, 0x40 },
            "xxxxx",
            0,
            AddressingMode.Absolute
        );

        public static readonly Pattern SetEventWrapper = new Pattern(
            new byte[] { 0x8B, 0x7D, 0x08, 0x85, 0xC9, 0x74, 0x16 },
            "xxxxxxx",
            -0x7,
            AddressingMode.Absolute
        );

        public static readonly Pattern EzStateCompareTimer = new Pattern(
            new byte[] { 0x83, 0xC4, 0x0C, 0x85, 0xC0, 0x0F, 0x84, 0x27, 0xD0 },
            "xxxxxxxxx",
            -0x59,
            AddressingMode.Absolute
        );

        public static readonly Pattern FasterMenu = new Pattern(
            new byte[] { 0x33, 0xC5, 0x89, 0x45, 0xFC, 0x56, 0x8B, 0xF1, 0x83, 0x7E, 0x0C, 0x00, 0x7E },
            "xxxxxxxxxxxxx",
            0,
            AddressingMode.Absolute
        );

        public static readonly Pattern BabyJump = new Pattern(
            new byte[] { 0x74, 0x30, 0xF3, 0x0F, 0x10, 0x80, 0x84 },
            "xxxxxxx",
            -0x1C,
            AddressingMode.Absolute
        );

        public static readonly Pattern DisableTargetAi = new Pattern(
            new byte[] { 0x74, 0x11, 0x8B, 0x06, 0x8B, 0x50, 0x1C },
            "xxxxxxx",
            2,
            AddressingMode.Absolute
        );
    }
}