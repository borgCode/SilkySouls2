namespace SilkySouls2.Memory.Patterns
{
    public static class Patterns64
    {
        public static readonly Pattern GameManagerImp = new(
            [0x89, 0x59, 0x0C, 0x88],
            "xxxx",
            0x5,
            AddressingMode.Relative,
            3,
            7,
            anchorOffset: 1
        );

        public static readonly Pattern HkHardwareInfo = new(
            [
                0x83, 0xF8, 0x0C, 0xB9, 0x0B, 0x00, 0x00, 0x00, 0x0F, 0x4D, 0xC1, 0x89, 0x87, 0x38, 0x03, 0x00, 0x00,
                0x83, 0xBF, 0x38, 0x03, 0x00, 0x00, 0x00, 0x48, 0x8B, 0x05, 0x00, 0x00, 0x00, 0x00
            ],
            "xxxxxxxxxxxxxxxxxxxxxxxxxxx????",
            24,
            AddressingMode.Relative,
            3,
            7,
            anchorOffset: 3
        );

        public static readonly Pattern KatanaMainApp = new(
            [
                0x48, 0x8B, 0x0D, 0x00, 0x00, 0x00, 0x00, 0x48, 0x85, 0xC9, 0x74, 0x09, 0x48, 0x8B, 0x49, 0x60
            ],
            "xxx????xxxxxxxxx",
            0,
            AddressingMode.Relative,
            3,
            7,
            anchorOffset: 14
        );

        public static readonly Pattern MapId = new(
            [
                0x8B, 0x8F, 0xD0, 0x00, 0x00, 0x00, 0x89, 0x8B, 0x94, 0x00, 0x00, 0x00, 0x8B, 0x15, 0x00, 0x00, 0x00,
                0x00, 0x8B, 0xC2, 0x25, 0x00, 0x00, 0x00, 0xFF, 0x3D, 0x00, 0x00, 0x00, 0x32, 0x75, 0x37, 0x3B, 0xCA,
                0x75, 0x12
            ],
            "xxxxxxxxxxxxxx????xxxxxxxxxxxxxxxxxx",
            12,
            AddressingMode.Relative,
            2,
            6,
            anchorOffset: 31
        );

        public static readonly Pattern BuildTextFieldRetAddr = new(
            [
                0x44, 0x8B, 0x84, 0x24, 0xD4, 0x00, 0x00, 0x00, 0x48, 0x8D, 0x8C, 0x24, 0xD8, 0x00, 0x00, 0x00, 0xBA,
                0x0A,
                0x00, 0x00, 0x00
            ],
            "xxxxxxxxxxxxxxxxxxxxx",
            0,
            AddressingMode.Absolute,
            anchorOffset: 3
        );


        #region Patches

        public static readonly Pattern InfiniteStam = new(
            [0x0F, 0x83, 0x26, 0x01, 0x00, 0x00, 0x48, 0x8B],
            "xxxxxxxx",
            0,
            AddressingMode.Absolute,
            anchorOffset: 1
        );

        public static readonly Pattern InfiniteGoods = new(
            [0x66, 0x29, 0x73, 0x20],
            "xxxx",
            0,
            AddressingMode.Absolute,
            anchorOffset: 0
        );

        public static readonly Pattern InfiniteCasts = new(
            [
                0x3B, 0xF1, 0x48, 0x0F, 0x4E, 0xC2, 0x0F, 0xB6, 0x08, 0x48, 0x8B, 0xD5, 0x88, 0x4D, 0x20, 0x49, 0x8B,
                0xCE
            ],
            "xxxxxxxxxxxxxxxxxx",
            12,
            AddressingMode.Absolute,
            anchorOffset: 4
        );

        public static readonly Pattern InfiniteDurability = new(
            [0xF3, 0x0F, 0x11, 0xB4, 0xC3],
            "xxxxx",
            0,
            AddressingMode.Absolute,
            anchorOffset: 3
        );

        public static readonly Pattern HideChrModels = new(
            [0x74, 0x05, 0x80, 0xC9, 0x08],
            "xxxxx",
            0,
            AddressingMode.Absolute,
            anchorOffset: 3
        );

        public static readonly Pattern HideMap = new(
            [
                0x0F, 0x88, 0x00, 0x00, 0x00, 0x00, 0x48, 0x8B, 0x4B, 0x20, 0x48, 0x98, 0x48, 0x8D, 0x7C, 0x24, 0x50,
                0x48, 0x8B, 0x51, 0x30, 0x48, 0xC1, 0xE0, 0x04, 0x8B, 0x4A, 0x48, 0x8B, 0x72, 0x44, 0x44, 0x8B, 0x42,
                0x4C, 0x48, 0x03, 0xF8
            ],
            "xx????xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
            0,
            AddressingMode.Absolute,
            anchorOffset: 29
        );

        public static readonly Pattern DropRate = new(
            [0x41, 0xF7, 0xF2, 0x41, 0x3B, 0xD1],
            "xxxxxx",
            0,
            AddressingMode.Absolute,
            anchorOffset: 1
        );

        public static readonly Pattern DisableAi = new(
            [0x7F, 0x59, 0x48, 0x8D],
            "xxxx",
            0,
            AddressingMode.Absolute,
            anchorOffset: 0
        );

        public static readonly Pattern Silent = new(
            [
                0xF3, 0x0F, 0x10, 0x44, 0x24, 0x70, 0x44, 0x0F, 0xB6, 0xCD, 0x4D, 0x8B, 0xC6, 0x48, 0x8B, 0xD6, 0x48,
                0x8B, 0xCF, 0xF3, 0x0F, 0x11, 0x44, 0x24, 0x20, 0xE8, 0x00, 0x00, 0x00, 0x00
            ],
            "xxxxxxxxxxxxxxxxxxxxxxxxxx????",
            25,
            AddressingMode.Absolute,
            anchorOffset: 8
        );

        public static readonly Pattern Hidden = new(
            [
                0x48, 0x8B, 0x49, 0x10, 0x40, 0x32, 0xED, 0x48, 0x8B, 0xB9, 0xB8, 0x02, 0x00, 0x00, 0x49, 0x8B, 0xF1,
                0x44, 0x0F, 0xB6, 0xFA, 0x4D, 0x85, 0xC9, 0x0F, 0x84, 0x00, 0x00, 0x00, 0x00
            ],
            "xxxxxxxxxxxxxxxxxxxxxxxxxx????",
            24,
            AddressingMode.Absolute,
            anchorOffset: 20
        );

        public static readonly Pattern NegativeLevel = new(
            [
                0x0F, 0x84, 0x00, 0x00, 0x00, 0x00, 0x48, 0x8B, 0x0B, 0x48, 0x8B, 0x01, 0xFF, 0x90, 0x30, 0x01, 0x00,
                0x00
            ],
            "xx????xxxxxxxxxxxx",
            0,
            AddressingMode.Absolute,
            anchorOffset: 13
        );

        public static readonly Pattern NoSoulGain = new(
            [0xF3, 0x48, 0x0F, 0x2C, 0xD0, 0xE8],
            "xxxxxx",
            5,
            AddressingMode.Absolute,
            anchorOffset: 3
        );

        public static readonly Pattern NoHollowing = new(
            [0x80, 0xFA, 0x20, 0x49, 0x0F, 0x4E, 0xC0, 0x0F, 0xB6, 0x00, 0x88, 0x81, 0xAC, 0x01, 0x00, 0x00, 0xC3],
            "xxxxxxxxxxxxxxxxx",
            10,
            AddressingMode.Absolute,
            anchorOffset: 5
        );

        public static readonly Pattern NoSoulLoss = new(
            [
                0x48, 0x8B, 0x86, 0x90, 0x04, 0x00, 0x00, 0x38, 0x90, 0xF0, 0x00, 0x00, 0x00, 0x75, 0x06, 0x89, 0x90,
                0xEC, 0x00, 0x00, 0x00
            ],
            "xxxxxxxxxxxxxxxxxxxxx",
            15,
            AddressingMode.Absolute,
            anchorOffset: 7
        );

        public static readonly Pattern SoulMemWrite1 = new(
            [0x8B, 0x00, 0x89, 0x81, 0xF4],
            "xxxxx",
            2,
            AddressingMode.Absolute,
            anchorOffset: 3
        );

        public static readonly Pattern SoulMemWrite2 = new(
            [0x89, 0x81, 0xFC, 0x00, 0x00, 0x00, 0xB9, 0x11, 0x00, 0x00, 0x00],
            "xxxxxxxxxxx",
            0,
            AddressingMode.Absolute,
            anchorOffset: 6
        );

        public static readonly Pattern NoHitPatch = new(
            [
                0x83, 0x20, 0xFE, 0x80, 0xA1, 0xC8, 0x00, 0x00, 0x00, 0xF7, 0x80, 0xE2, 0x01, 0xC0, 0xE2, 0x03, 0x08,
                0x91,
                0xC8, 0x00, 0x00, 0x00
            ],
            "xxxxxxxxxxxxxxxxxxxxxx",
            0,
            AddressingMode.Absolute,
            anchorOffset: 4
        );

        public static readonly Pattern MenuTransition = new(
            [
                0x75, 0xEA, 0x48, 0x8B, 0x4F, 0x38, 0x33, 0xD2, 0x48, 0x8B, 0x01, 0xFF, 0x10, 0xC7, 0x47, 0x30, 0x04,
                0x00,
                0x00, 0x00
            ],
            "xxxxxxxxxxxxxxxxxxxx",
            0,
            AddressingMode.Absolute,
            anchorOffset: 0
        );

        public static readonly Pattern DisableRoll = new(
            [
                0x0F, 0xBA, 0xF0, 0x1F, 0x89, 0x44, 0x24, 0x38, 0x0F, 0x2F, 0x44, 0x24, 0x38, 0x73, 0x16, 0x0F, 0x2F,
                0x49, 0x08
            ],
            "xxxxxxxxxxxxxxxxxxx",
            44,
            AddressingMode.Absolute,
            anchorOffset: 13
        );

        #endregion

        #region Hooks

        public static readonly Pattern SetAreaVariable = new(
            [0x0F, 0x84, 0xC8, 0xB8],
            "xxxx",
            0x6,
            AddressingMode.Absolute,
            anchorOffset: 2
        );

        public static readonly Pattern CompareEventRandValueForlorn = new(
            [
                0x48, 0x8B, 0x51, 0x10, 0x48, 0x85, 0xD2, 0x74, 0x19, 0x48, 0x63, 0x41, 0x18, 0x83, 0xF8, 0x04, 0x73,
                0x10
            ],
            "xxxxxxxxxxxxxxxxxx",
            18,
            AddressingMode.Absolute,
            anchorOffset: 10
        );

        public static readonly Pattern CompareEventRandValueElana = new(
            [
                0x48, 0x8B, 0x51, 0x10, 0x48, 0x85, 0xD2, 0x74, 0x19, 0x48, 0x63, 0x41, 0x18, 0x83, 0xF8, 0x04, 0x73,
                0x10
            ],
            "xxxxxxxxxxxxxxxxxx",
            0,
            AddressingMode.Absolute,
            anchorOffset: 10
        );

        public static readonly Pattern PlayerNoDamage = new(
            [0x48, 0x0F, 0x4F, 0xC6, 0x8B, 0x00],
            "xxxxxx",
            0x6,
            AddressingMode.Absolute,
            anchorOffset: 2
        );


        public static readonly Pattern LockedTarget = new(
            [0x48, 0x89, 0xBB, 0xC0, 0x00, 0x00, 0x00, 0xEB],
            "xxxxxxxx",
            0,
            AddressingMode.Absolute,
            anchorOffset: 2
        );

        public static readonly Pattern CreditSkip = new(
            [0x4C, 0x8B, 0xDC, 0x53, 0x48, 0x81, 0xEC, 0x20, 0x02, 0x00, 0x00],
            "xxxxxxxxxxx",
            4,
            AddressingMode.Absolute,
            anchorOffset: 2
        );

        public static readonly Pattern NumOfDrops = new(
            [0x66, 0x09, 0x46, 0x0E, 0x41, 0x0F, 0xB6, 0x47, 0x01, 0xFF, 0xC5, 0x3B, 0xE8],
            "xxxxxxxxxxxxx",
            4,
            AddressingMode.Absolute,
            anchorOffset: 2
        );

        public static readonly Pattern DamageControl = new(
            [0x48, 0x8D, 0x4C, 0x24, 0x50, 0x41, 0x88, 0x87, 0xA4, 0x05, 0x00, 0x00, 0x48, 0x8B, 0x57, 0x70],
            "xxxxxxxxxxxxxxxx",
            -51,
            AddressingMode.Absolute,
            anchorOffset: 14
        );

        public static readonly Pattern TriggersAndSpace = new(
            [
                0x48, 0x21, 0x43, 0x08, 0x48, 0x8B, 0x6C, 0x24, 0x30, 0x4C, 0x8B, 0xA4, 0x24, 0x88, 0x00, 0x00, 0x00,
                0x4C, 0x8B, 0x7C, 0x24, 0x70
            ],
            "xxxxxxxxxxxxxxxxxxxxxx",
            17,
            AddressingMode.Absolute,
            anchorOffset: 1
        );

        public static readonly Pattern Ctrl = new(
            [0x74, 0x0A, 0x81, 0x8B, 0x28, 0x02, 0x00, 0x00, 0x00, 0x02],
            "xxxxxxxxxx",
            0x2,
            AddressingMode.Absolute,
            anchorOffset: 2
        );

        public static readonly Pattern NoClipUpdateCoords = new(
            [0x66, 0x0F, 0x7F, 0xB8, 0x90],
            "xxxxx",
            0,
            AddressingMode.Absolute,
            anchorOffset: 2
        );

        public static readonly Pattern KillboxFlagSet = new(
            [
                0x48, 0xB8, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x08, 0x00, 0x48, 0x09, 0x81, 0xC0, 0x04, 0x00, 0x00,
                0x84
            ],
            "xxxxxxxxxxxxxxxxxx",
            0,
            AddressingMode.Absolute,
            anchorOffset: 11
        );

        public static readonly Pattern SetCurrentAct = new(
            [0x83, 0x89, 0x50, 0x03, 0x00, 0x00, 0x01],
            "xxxxxxx",
            0,
            AddressingMode.Absolute,
            occurrenceIndex: 1,
            anchorOffset: 0
        );

        public static readonly Pattern FasterMenu = new(
            [0x48, 0x89, 0x84, 0x24, 0x50, 0x01, 0x00, 0x00, 0x83, 0x79, 0x10, 0x00, 0x48, 0x8B, 0xF9, 0x7E, 0x5D],
            "xxxxxxxxxxxxxxxxx",
            0,
            AddressingMode.Absolute,
            anchorOffset: 14
        );

        public static readonly Pattern InfinitePoise = new(
            [0x39, 0x9D, 0xEC, 0x05],
            "xxxx",
            0,
            AddressingMode.Absolute,
            anchorOffset: 1
        );

        public static readonly Pattern SetEventWrapper = new(
            [0x84, 0xC0, 0x74, 0x3D, 0x44, 0x0F, 0xB6, 0xC7],
            "xxxxxxxx",
            -34,
            AddressingMode.Absolute,
            anchorOffset: 3
        );

        public static readonly Pattern ProcessPhysics = new(
            [
                0x48, 0x8B, 0x8B, 0x78, 0x03, 0x00, 0x00, 0x48, 0x85, 0xC9, 0x74, 0x0B, 0x48, 0x8B, 0x01, 0x48, 0x8D,
                0x54, 0x24, 0x20
            ],
            "xxxxxxxxxxxxxxxxxxxx",
            30,
            AddressingMode.Absolute,
            anchorOffset: 3
        );


        public static readonly Pattern DisableTargetAi = new(
            [0x7F, 0x59, 0x48, 0x8D],
            "xxxx",
            0,
            AddressingMode.Absolute,
            anchorOffset: 0
        );

        public static readonly Pattern SetSharedFlag = new(
            [0x44, 0x88, 0x84, 0x08, 0xA1],
            "xxxxx",
            0,
            AddressingMode.Absolute,
            anchorOffset: 2
        );

        public static readonly Pattern BabyJump = new(
            [0x74, 0x12, 0xF3, 0x0F, 0x10, 0x83],
            "xxxxxx",
            -0x11,
            AddressingMode.Absolute,
            anchorOffset: 1
        );

        public static readonly Pattern EzStateCompareTimer = new(
            [0xF3, 0x0F, 0x11, 0x70, 0x18, 0x41],
            "xxxxxx",
            -0x7B,
            AddressingMode.Absolute,
            anchorOffset: 2
        );

        public static readonly Pattern NoShadedFogClose = new(
            [0x0F, 0x57, 0xC0, 0x66, 0x0F, 0x6E, 0xE0],
            "xxxxxxx",
            0,
            AddressingMode.Absolute,
            anchorOffset: 5
        );

        public static readonly Pattern ReduceGameSpeed = new(
            [0xF3, 0x0F, 0x10, 0x32, 0x33, 0xDB, 0xF6, 0x81, 0xB2, 0x24, 0x00, 0x00, 0x08, 0x48, 0x8B, 0xEA],
            "xxxxxxxxxxxxxxxx",
            0,
            AddressingMode.Absolute,
            anchorOffset: 5
        );

        public static readonly Pattern LightGutter = new(
            [0x0F, 0x14, 0xF1, 0x41, 0xBF],
            "xxxxx",
            0x68,
            AddressingMode.Absolute,
            anchorOffset: 1
        );


        public static readonly Pattern NoShadedFogFar = new(
            [0x48, 0x83, 0xEC, 0x30, 0x4C, 0x8B, 0xF2, 0xBA, 0x28, 0x00, 0x00, 0x00, 0x48, 0x8B, 0xE9],
            "xxxxxxxxxxxxxxx",
            90,
            AddressingMode.Absolute,
            anchorOffset: 7
        );

        public static readonly Pattern NoShadedFogCam = new(
            [
                0x49, 0x8D, 0xB5, 0x2C, 0x02, 0x00, 0x00, 0x89, 0x44, 0x24, 0x5C, 0x4D, 0x8D, 0xB5, 0x18, 0x03, 0x00,
                0x00, 0x49, 0x81, 0xEC, 0x58, 0x01, 0x00, 0x00
            ],
            "xxxxxxxxxxxxxxxxxxxxxxxxx",
            -18,
            AddressingMode.Absolute,
            anchorOffset: 2
        );

        public static readonly Pattern GameManUpdate = new(
            [0x33, 0xDB, 0xF6, 0x81, 0xB2],
            "xxxxx",
            -0x29,
            AddressingMode.Absolute,
            anchorOffset: 1
        );


        public static readonly Pattern NewGameDetect = new(
            [0xC7, 0x47, 0x54, 0xFF, 0xFF, 0xFF, 0xFF, 0x44, 0x89, 0x7F, 0x58, 0xC6, 0x47, 0x6F, 0x00],
            "xxxxxxxxxxxxxxx",
            0,
            AddressingMode.Absolute,
            anchorOffset: 12
        );

        public static readonly Pattern LoadingItemName = new(
            [
                0x48, 0x8B, 0x0B, 0x48, 0x8B, 0x11, 0x48, 0x8B, 0xF8, 0xFF, 0x12, 0x48, 0x85, 0xC0, 0x74, 0x1B, 0x4C,
                0x8B,
                0x00, 0x48, 0x85, 0xFF
            ],
            "xxxxxxxxxxxxxxxxxxxxxx",
            0,
            AddressingMode.Absolute,
            anchorOffset: 15
        );

        public static readonly Pattern PreAiEzState = new(
            [0x48, 0x83, 0xC7, 0x08, 0x3B, 0xB3, 0x74, 0x02, 0x00, 0x00, 0x7C, 0xE1],
            "xxxxxxxxxxxx",
            17,
            AddressingMode.Absolute,
            anchorOffset: 4
        );
        
        public static readonly Pattern NoShadedFogCamFilter = new Pattern(
            [
                0x84, 0xC0, 0x74, 0x15, 0x44, 0x8B, 0x44, 0x24, 0x78, 0x8B, 0x54, 0x24, 0x70, 0x48, 0x8B, 0x4E, 0x08, 0x0F,
                0x28, 0xDE
            ],
            "xxxxxxxxxxxxxxxxxxxx",
            4,
            AddressingMode.Absolute
            );

        #endregion

        #region Functions

        public static readonly Pattern RequestWarp = new(
            [
                0x40, 0x53, 0x48, 0x83, 0xEC, 0x60, 0x8B, 0x02, 0x48, 0x8B, 0xD9, 0x89, 0x01, 0x8B, 0x42, 0x04, 0x89,
                0x41, 0x04, 0x8B, 0x42, 0x08
            ],
            "xxxxxxxxxxxxxxxxxxxxxx",
            0,
            AddressingMode.Absolute,
            anchorOffset: 10
        );


        public static readonly Pattern SetEvent = new(
            [0x45, 0x0F, 0xB6, 0xD8, 0xB8],
            "xxxxx",
            -0xC,
            AddressingMode.Absolute,
            anchorOffset: 3
        );

        public static readonly Pattern GetEvent = new(
            [0x81, 0xFA, 0xA0, 0x31, 0x2C, 0x0D, 0x75, 0x10, 0xBA, 0x6C, 0x8A, 0x01, 0x00],
            "xxxxxxxxxxxxx",
            13,
            AddressingMode.Relative,
            1,
            5,
            anchorOffset: 1
        );

        public static readonly Pattern GiveSouls = new(
            [
                0x8B, 0x53, 0x08, 0x48, 0x8B, 0xCF, 0xE8, 0x00, 0x00, 0x00, 0x00, 0x0F, 0xB7, 0x43, 0x0C, 0x0F, 0xB7,
                0x4B, 0x06
            ],
            "xxxxxxx????xxxxxxxx",
            6,
            AddressingMode.Relative,
            1,
            5,
            anchorOffset: 17
        );

        public static readonly Pattern RestoreSpellcasts = new(
            [0x40, 0x55, 0x53, 0x41, 0x54, 0x41, 0x55, 0x48, 0x8D, 0x6C],
            "xxxxxxxxxx",
            0,
            AddressingMode.Absolute,
            anchorOffset: 2
        );

        public static readonly Pattern ParamLookup = new(
            [0x80, 0x8B, 0x88, 0x00, 0x00, 0x00, 0x01, 0x48, 0x8B, 0x4B, 0x08, 0x89, 0x7B, 0x70, 0x8B, 0x50, 0x40],
            "xxxxxxxxxxxxxxxxx",
            17,
            AddressingMode.Relative,
            1,
            5,
            anchorOffset: 12
        );

        public static readonly Pattern SetRenderTargets = new(
            [
                0x44, 0x0F, 0xB6, 0x85, 0x85, 0x05, 0x00, 0x00, 0x4C, 0x8D, 0x8D, 0xE0, 0x04, 0x00, 0x00, 0x48, 0x8B,
                0xD5, 0x49, 0x8B, 0xCF, 0x48, 0x89, 0x44, 0x24, 0x20
            ],
            "xxxxxxxxxxxxxxxxxxxxxxxxxx",
            26,
            AddressingMode.Relative,
            1,
            5,
            anchorOffset: 4
        );

        public static readonly Pattern CreateSoundEvent = new(
            [
                0xE8, 0x00, 0x00, 0x00, 0x00, 0x84, 0xC0, 0x74, 0x4C, 0x48, 0x8B, 0x83, 0x10, 0x14, 0x00, 0x00, 0x48,
                0x85, 0xC0, 0x74, 0x03, 0x48, 0x89, 0x07
            ],
            "x????xxxxxxxxxxxxxxxxxxx",
            0,
            AddressingMode.Relative,
            1,
            5,
            anchorOffset: 11
        );

        public static readonly Pattern LevelLookup = new(
            [0x48, 0x85, 0xDB, 0x74, 0x07, 0x0F, 0xB7, 0x03],
            "xxxxxxxx",
            -0x50,
            AddressingMode.Absolute,
            anchorOffset: 6
        );

        public static readonly Pattern LevelUp = new(
            [0x48, 0x85, 0xD2, 0x0F, 0x84, 0xA0, 0x01],
            "xxxxxxx",
            0,
            AddressingMode.Absolute,
            anchorOffset: 4
        );

        public static readonly Pattern CurrentItemQuantityCheck = new(
            [0x48, 0x98, 0x8B, 0x8C, 0x82, 0xCC, 0x52],
            "xxxxxxx",
            -0x4D,
            AddressingMode.Absolute,
            anchorOffset: 4
        );

        public static readonly Pattern ItemGive = new(
            [0x8D, 0x46, 0xFF, 0x83, 0xF8, 0x1F, 0x0F, 0x87, 0xF2],
            "xxxxxxxxx",
            -0x1E,
            AddressingMode.Absolute,
            anchorOffset: 4
        );

        public static readonly Pattern BuildItemDialog = new(
            [0x0F, 0x57, 0xF6, 0x0F, 0x2F, 0x36, 0x72, 0x13],
            "xxxxxxxx",
            -5,
            AddressingMode.Relative,
            1,
            5,
            anchorOffset: 4
        );

        public static readonly Pattern ShowItemDialog = new(
            [0x48, 0x8B, 0x89, 0xD8, 0x00, 0x00, 0x00, 0x48, 0x85, 0xC9, 0x0F, 0x85, 0x20],
            "xxxxxxxxxxxxx",
            0,
            AddressingMode.Absolute,
            anchorOffset: 2
        );

        public static readonly Pattern GetEyePosition = new(
            [
                0x33, 0xDB, 0x4D, 0x85, 0xF6, 0x7E, 0x64, 0x48, 0x85, 0xFF, 0x74, 0x31, 0x48, 0x83, 0xBF, 0xA0, 0x02,
                0x00, 0x00, 0x00
            ],
            "xxxxxxxxxxxxxxxxxxxx",
            -5,
            AddressingMode.Relative,
            1,
            5,
            anchorOffset: 1
        );


        public static readonly Pattern ApplySpEffect = new(
            [0x83, 0xFF, 0x06, 0x75, 0x70, 0x48, 0x8B, 0x0E, 0x48, 0x8B, 0x89, 0x28, 0x02, 0x00, 0x00],
            "xxxxxxxxxxxxxxx",
            -5,
            AddressingMode.Relative,
            1,
            5,
            anchorOffset: 10
        );

        public static readonly Pattern HavokRayCast = new(
            [0xE8, 0x00, 0x00, 0x00, 0x00, 0x80, 0x7D, 0x40, 0x00, 0x0F, 0x84, 0x80],
            "x????xxxxxxx",
            0,
            AddressingMode.Relative,
            1,
            5,
            anchorOffset: 6
        );

        public static readonly Pattern ConvertPxRigidToMapEntity = new(
            [0x42, 0xF6, 0x44, 0x21, 0x34, 0x01, 0x0F, 0x84, 0x8E, 0x02, 0x00, 0x00, 0x4A, 0x8B, 0x4C, 0x21, 0x28],
            "xxxxxxxxxxxxxxxxx",
            25,
            AddressingMode.Relative,
            1,
            5,
            anchorOffset: 0
        );

        public static readonly Pattern PackGameEntityHandle = new(
            [0x8B, 0x00, 0x39, 0x43, 0x54, 0x75, 0x0A, 0x80, 0x4B, 0x0F, 0x08],
            "xxxxxxxxxxx",
            -5,
            AddressingMode.Relative,
            1,
            5,
            anchorOffset: 4
        );

        public static readonly Pattern UnlockBonfire = new(
            [0x48, 0x8B, 0x4E, 0x58, 0x8B, 0x10, 0x44],
            "xxxxxxx",
            0xA,
            AddressingMode.Relative,
            1,
            5,
            anchorOffset: 2
        );

        public static readonly Pattern GetMapObjStateActComponent = new(
            [
                0x48, 0x85, 0xC0, 0x74, 0x19, 0x48, 0x8B, 0x48, 0x48, 0x48, 0x85, 0xC9, 0x74, 0x10, 0x48, 0x8B, 0x01,
                0xFF, 0x50, 0x10, 0x3C, 0x0A, 0x0F, 0x95, 0xC0
            ],
            "xxxxxxxxxxxxxxxxxxxxxxxxx",
            -5,
            AddressingMode.Relative,
            1,
            5,
            anchorOffset: 20
        );

        public static readonly Pattern GetMapEntityWithAreaIdAndObjId = new(
            [0x80, 0xB8, 0xE0, 0x01, 0x00, 0x00, 0x0C, 0x48],
            "xxxxxxxx",
            -0x2F,
            AddressingMode.Absolute,
            anchorOffset: 1
        );

        public static readonly Pattern AttuneSpell = new(
            [0x83, 0xFA, 0x29, 0x77, 0x1C],
            "xxxxx",
            0,
            AddressingMode.Absolute,
            anchorOffset: 1
        );

        public static readonly Pattern GetNumOfSpellSlots1 = new(
            [0x03, 0xF8, 0x89, 0xBE, 0xB0, 0x1B, 0x00, 0x00],
            "xxxxxxxx",
            -0x12,
            AddressingMode.Relative,
            1,
            5,
            anchorOffset: 3
        );

        public static readonly Pattern GetNumOfSpellSlots2 = new(
            [0x03, 0xF8, 0x89, 0xBE, 0xB0, 0x1B, 0x00, 0x00],
            "xxxxxxxx",
            -0x8,
            AddressingMode.Relative,
            1,
            5,
            anchorOffset: 3
        );

        public static readonly Pattern UpdateSpellSlots = new(
            [0x0F, 0xB6, 0x8E, 0xED, 0x59],
            "xxxxx",
            -0x18,
            AddressingMode.Absolute,
            anchorOffset: 2
        );


        public static readonly Pattern EzStateExternalEventCtor = new(
            [0x49, 0x8B, 0x06, 0x48, 0x85, 0xC0, 0x48, 0x0F, 0x44, 0xC5, 0xEB, 0x03],
            "xxxxxxxxxxxx",
            -107,
            AddressingMode.Relative,
            1,
            5,
            anchorOffset: 9
        );

        public static readonly Pattern EzStateEventExecuteCommand = new(
            [0x48, 0x8D, 0xAC, 0x24, 0xD0, 0xF0],
            "xxxxxx",
            -0x10,
            AddressingMode.Absolute,
            anchorOffset: 1
        );
        
        public static readonly Pattern OriginalMakeSound = new(
            [
                0xF3, 0x0F, 0x10, 0x44, 0x24, 0x70, 0x44, 0x0F, 0xB6, 0xCD, 0x4D, 0x8B, 0xC6, 0x48, 0x8B, 0xD6, 0x48,
                0x8B, 0xCF, 0xF3, 0x0F, 0x11, 0x44, 0x24, 0x20, 0xE8, 0x00, 0x00, 0x00, 0x00
            ],
            "xxxxxxxxxxxxxxxxxxxxxxxxxx????",
            25,
            AddressingMode.Relative,
            1,
            5,
            anchorOffset: 8
        );
        
        public static readonly Pattern OriginalSoulGain = new(
            [0xF3, 0x48, 0x0F, 0x2C, 0xD0, 0xE8],
            "xxxxxx",
            5,
            AddressingMode.Relative,
            1,
            5,
            anchorOffset: 3
        );
        
        public static readonly Pattern OpenNpcMenu = new(
            [
                0x48, 0x89, 0x5C, 0x24, 0x10, 0x48, 0x89, 0x6C, 0x24, 0x18, 0x56, 0x48, 0x83, 0xEC, 0x20, 0x48, 0x83, 0x7A,
                0x28, 0x00, 0x49, 0x8B, 0xE8, 0x48, 0x8B, 0xF2, 0x48, 0x8B, 0xD9
            ],
            "xxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
            0,
            AddressingMode.Absolute,
            anchorOffset: 17
            );


        public static readonly Pattern SetMenuOpenChrState = new(
            [
                0x4C, 0x63, 0xCA, 0x41, 0x83, 0xF9, 0x14, 0x7D, 0x74, 0x45, 0x32, 0xD2, 0x41, 0xBB, 0x01, 0x00, 0x00, 0x00
            ],
            "xxxxxxxxxxxxxxxxxx",
            0,
            AddressingMode.Absolute,
             anchorOffset: 6
            );
        
        public static readonly Pattern ApplyDurabilityDamage = new(
            [
                0x48, 0x89, 0x5C, 0x24, 0x10, 0x48, 0x89, 0x6C, 0x24, 0x18, 0x56, 0x57, 0x41, 0x56, 0x48, 0x83, 0xEC, 0x70,
                0x48, 0x8B, 0xD9, 0x48, 0x8B, 0x49, 0x08, 0x44, 0x0F, 0x29, 0x44, 0x24, 0x40, 0x48, 0x8B, 0x01, 0x41, 0x0F,
                0xB6, 0xE9, 0x44, 0x0F, 0x28, 0xC2, 0x8B, 0xF2, 0xFF, 0x90, 0x20, 0x01, 0x00, 0x00
            ],
            "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
            0,
            AddressingMode.Absolute,
            anchorOffset: 36
            );
        
        public static readonly Pattern ResolveTargetCtrlFromHandle = new(
            [
                0xE8, 0x00, 0x00, 0x00, 0x00, 0x48, 0x89, 0x43, 0x08, 0x48, 0x85, 0xC0, 0x74, 0x3C, 0x48, 0x8B, 0x10, 0x48,
                0x8B, 0xC8, 0xFF, 0x52, 0x40, 0x84, 0xC0, 0x74, 0x1B
            ],
            "x????xxxxxxxxxxxxxxxxxxxxxx",
            0,
            AddressingMode.Relative,
            1,
            5,
            anchorOffset: 12
        );

        #endregion
    }
}
