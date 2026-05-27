namespace SilkySouls2.Memory.Patterns
{
    public static class Patterns32
    {
        public static readonly Pattern GameManagerImp = new(
            [0xA1, 0x00, 0x00, 0x00, 0x00, 0x56, 0x8B, 0xB0, 0xCC, 0x0C, 0x00, 0x00],
            "x????xxxxxxx",
            0,
            AddressingMode.Direct32,
            1,
            anchorOffset: 7
        );

        public static readonly Pattern KatanaMainApp = new(
            [0x8B, 0x0D, 0x00, 0x00, 0x00, 0x00, 0x8B, 0x49, 0x4C, 0x89, 0x55],
            "xx????xxxxx",
            0,
            AddressingMode.Direct32,
            2,
            anchorOffset: 7
        );


        public static readonly Pattern MapId = new(
            [
                0x8B, 0x15, 0x00, 0x00, 0x00, 0x00, 0x8B, 0xF2, 0x81, 0xE6, 0x00, 0x00, 0x00, 0xFF, 0x81, 0xFE, 0x00,
                0x00, 0x00, 0x32, 0x75, 0x46
            ],
            "xx????xxxxxxxxxxxxxxxx",
            0,
            AddressingMode.Direct32,
            2,
            anchorOffset: 19
        );

        public static readonly Pattern LoadLibraryW = new(
            [0x8D, 0x85, 0xF4, 0xFD, 0xFF, 0xFF, 0x50, 0x66, 0xA5, 0xFF, 0x15, 0x00, 0x00, 0x00, 0x00],
            "xxxxxxxxxxx????",
            9,
            AddressingMode.Direct32,
            2,
            6,
            anchorOffset: 7
        );

        public static readonly Pattern BuildTextFieldRetAddr = new(
            [
                0x8B, 0x95, 0xFC, 0xFE, 0xFF, 0xFF, 0x52, 0x8D, 0x85, 0x04, 0xFF, 0xFF, 0xFF, 0x6A, 0x0A, 0x50
            ],
            "xxxxxxxxxxxxxxxx",
            0,
            AddressingMode.Absolute,
            anchorOffset: 14
        );

        #region Patches

        public static readonly Pattern InfiniteStam = new(
            [
                0x25, 0xFF, 0xFF, 0xFF, 0x7F, 0x89, 0x45, 0x08, 0x0F, 0x2F, 0x45, 0x08, 0x0F, 0x83, 0x00, 0x00, 0x00,
                0x00, 0x8B, 0x06
            ],
            "xxxxxxxxxxxxxx????xx",
            12,
            AddressingMode.Absolute,
            anchorOffset: 9
        );

        public static readonly Pattern InfiniteGoods = new(
            [
                0x8B, 0x51, 0x20, 0x8A, 0x42, 0x18, 0x3C, 0x08, 0x74, 0x0C, 0x3C, 0x02, 0x74, 0x08, 0x3C, 0x09, 0xEB,
                0x02
            ],
            "xxxxxxxxxxxxxxxxxx",
            43,
            AddressingMode.Absolute,
            anchorOffset: 14
        );

        public static readonly Pattern InfiniteCasts = new(
            [0x8B, 0xCF, 0x88, 0x43],
            "xxxx",
            2,
            AddressingMode.Absolute,
            anchorOffset: 1
        );

        public static readonly Pattern InfiniteDurability = new(
            [0x66, 0x0F, 0x2E, 0xC1, 0x9F, 0xF6, 0xC4, 0x44, 0x7A, 0x0A, 0x8B, 0x47, 0x44, 0x50],
            "xxxxxxxxxxxxxx",
            32,
            AddressingMode.Absolute,
            anchorOffset: 8
        );

        public static readonly Pattern HideChrModels = new(
            [0x74, 0x05, 0x0A, 0x4D],
            "xxxx",
            0,
            AddressingMode.Absolute,
            anchorOffset: 2
        );

        public static readonly Pattern HideMap = new(
            [0x0F, 0x88, 0x94, 0x00],
            "xxxx",
            0,
            AddressingMode.Absolute,
            anchorOffset: 1
        );

        public static readonly Pattern DropRate = new(
            [0xF7, 0xF6, 0x89, 0x79],
            "xxxx",
            0,
            AddressingMode.Absolute,
            anchorOffset: 0
        );

        public static readonly Pattern DisableAi = new(
            [0x83, 0x7B, 0x18, 0x00, 0x7F],
            "xxxxx",
            4,
            AddressingMode.Absolute,
            anchorOffset: 1
        );

        public static readonly Pattern Silent = new(
            [0xE8, 0x00, 0x00, 0x00, 0x00, 0x84, 0xC0, 0x74, 0x3F, 0x8B, 0x86],
            "x????xxxxxx",
            -0xA,
            AddressingMode.Absolute,
            anchorOffset: 8
        );

        public static readonly Pattern Hidden = new(
            [0x0F, 0x84, 0x3D, 0x02, 0x00, 0x00, 0x85],
            "xxxxxxx",
            0,
            AddressingMode.Absolute,
            anchorOffset: 2
        );

        public static readonly Pattern NegativeLevel = new(
            [0x84, 0xC0, 0x0F, 0x84, 0xAA, 0x00, 0x00, 0x00, 0x8B, 0x0B],
            "xxxxxxxxxx",
            -0x2,
            AddressingMode.Absolute,
            anchorOffset: 3
        );

        public static readonly Pattern NoSoulGain = new(
            [0xD9, 0x6D, 0x16, 0xE8],
            "xxxx",
            3,
            AddressingMode.Absolute,
            anchorOffset: 1
        );

        public static readonly Pattern NoHollowing = new(
            [0x88, 0x45, 0x0B, 0x79],
            "xxxx",
            0x22,
            AddressingMode.Absolute,
            anchorOffset: 2
        );

        public static readonly Pattern NoSoulLoss = new(
            [0x75, 0x0A, 0xC7, 0x80, 0xE8],
            "xxxxx",
            2,
            AddressingMode.Absolute,
            anchorOffset: 2
        );

        public static readonly Pattern SoulMemWrite1 = new(
            [0x89, 0x81, 0xF0, 0x00, 0x00, 0x00, 0x8B, 0x81],
            "xxxxxxxx",
            0,
            AddressingMode.Absolute,
            anchorOffset: 1
        );

        public static readonly Pattern SoulMemWrite2 = new(
            [0x89, 0x81, 0xF0, 0x00, 0x00, 0x00, 0x8B, 0x81],
            "xxxxxxxx",
            0x31,
            AddressingMode.Absolute,
            anchorOffset: 1
        );

        public static readonly Pattern NoHitPatch = new(
            [0x83, 0x22, 0xFE, 0x02, 0xC0, 0x02, 0xC0, 0x02, 0xC0, 0x32, 0x81, 0x84, 0x00, 0x00, 0x00],
            "xxxxxxxxxxxxxxx",
            0,
            AddressingMode.Absolute,
            anchorOffset: 0
        );

        public static readonly Pattern MenuTransition = new(
            [
                0x84, 0xC0, 0x0F, 0x85, 0x5D, 0x01, 0x00, 0x00, 0x8B, 0x4E, 0x1C, 0x8B, 0x11, 0x8B, 0x02, 0x6A, 0x00,
                0xFF,
                0xD0
            ],
            "xxxxxxxxxxxxxxxxxxx",
            2,
            AddressingMode.Absolute,
            anchorOffset: 3
        );

        public static readonly Pattern DisableRoll = new(
            [
                0x0F, 0x2F, 0x45, 0x08, 0x73, 0x17, 0x0F, 0x57, 0xC0, 0x0F, 0x2F, 0x41, 0x04, 0x72, 0x14, 0x8B, 0x0D,
                0x00,
                0x00, 0x00, 0x00, 0x8B, 0x49, 0x18, 0xE8, 0x00, 0x00, 0x00, 0x00, 0x32, 0xC0, 0x5D, 0xC2, 0x04, 0x00,
                0xB0,
                0x01, 0x5D, 0xC2, 0x04, 0x00
            ],
            "xxxxxxxxxxxxxxxxx????xxxx????xxxxxxxxxxxx",
            35,
            AddressingMode.Absolute,
            anchorOffset: 10
        );

        #endregion


        #region Hooks

        public static readonly Pattern CompareEventRandValueElana = new(
            [0x8B, 0x51, 0x08, 0x85, 0xD2, 0x74, 0x20, 0x8B, 0x41, 0x0C, 0x83, 0xF8, 0x04, 0x73, 0x18],
            "xxxxxxxxxxxxxxx",
            0,
            AddressingMode.Absolute,
            anchorOffset: 13
        );

        public static readonly Pattern PlayerNoDamage = new(
            [0x89, 0x8E, 0xFC, 0x00, 0x00, 0x00, 0x8B, 0x02],
            "xxxxxxxx",
            0,
            AddressingMode.Absolute,
            anchorOffset: 1
        );

        public static readonly Pattern LockedTarget = new(
            [0x89, 0xB7, 0xB8, 0x00, 0x00, 0x00, 0xEB],
            "xxxxxxx",
            0,
            AddressingMode.Absolute,
            anchorOffset: 1
        );

        public static readonly Pattern CreditSkip = new(
            [0x81, 0xEC, 0xFC, 0x01, 0x00, 0x00, 0x53, 0x8B, 0xD9, 0x8B, 0x43, 0x14],
            "xxxxxxxxxxxx",
            0,
            AddressingMode.Absolute,
            anchorOffset: 1
        );

        public static readonly Pattern NumOfDrops = new(
            [0x0F, 0xB6, 0x51, 0x01, 0x40],
            "xxxxx",
            0,
            AddressingMode.Absolute,
            anchorOffset: 3
        );

        public static readonly Pattern DamageControl = new(
            [
                0x8B, 0x82, 0x9C, 0x00, 0x00, 0x00, 0x51, 0x8B, 0xCF, 0xFF, 0xD0, 0x8B, 0x4D, 0x10, 0x88, 0x87, 0x74,
                0x02, 0x00, 0x00
            ],
            "xxxxxxxxxxxxxxxxxxxx",
            -49,
            AddressingMode.Absolute,
            anchorOffset: 1
        );

        public static readonly Pattern TriggersAndSpace = new(
            [
                0x8B, 0x56, 0x08, 0x89, 0x86, 0x04, 0x01, 0x00, 0x00, 0x6A, 0x20, 0x8D, 0x46, 0x28, 0x50, 0x8D, 0x8E,
                0x20, 0x01, 0x00, 0x00
            ],
            "xxxxxxxxxxxxxxxxxxxxx",
            0,
            AddressingMode.Absolute,
            anchorOffset: 16
        );

        public static readonly Pattern Ctrl = new(
            [0x81, 0x8E, 0x28, 0x02, 0x00, 0x00, 0x00, 0x02],
            "xxxxxxxx",
            0,
            AddressingMode.Absolute,
            anchorOffset: 0
        );

        public static readonly Pattern NoClipUpdateCoords = new(
            [0xF3, 0x0F, 0x7E, 0x45, 0xD0, 0x66, 0x0F, 0xD6, 0x40, 0x70],
            "xxxxxxxxxx",
            0,
            AddressingMode.Absolute,
            anchorOffset: 8
        );

        public static readonly Pattern KillboxFlagSet = new(
            [0x81, 0x88, 0xC4, 0x04, 0x00, 0x00, 0x00, 0x00, 0x08, 0x00, 0x83],
            "xxxxxxxxxxx",
            -0x25,
            AddressingMode.Absolute,
            anchorOffset: 0
        );

        public static readonly Pattern SetCurrentAct = new(
            [0x83, 0x89, 0x50, 0x02, 0x00, 0x00, 0x01],
            "xxxxxxx",
            7,
            AddressingMode.Absolute,
            anchorOffset: 0,
            occurrenceIndex: 1
        );

        public static readonly Pattern FasterMenu = new(
            [0x33, 0xC5, 0x89, 0x45, 0xFC, 0x56, 0x8B, 0xF1, 0x83, 0x7E, 0x0C, 0x00, 0x7E],
            "xxxxxxxxxxxxx",
            0,
            AddressingMode.Absolute,
            anchorOffset: 7
        );

        public static readonly Pattern InfinitePoise = new(
            [0x83, 0xBB, 0xEC, 0x05, 0x00, 0x00, 0x00, 0x0F, 0x95, 0x45, 0xFF],
            "xxxxxxxxxxx",
            0,
            AddressingMode.Absolute,
            anchorOffset: 8
        );

        public static readonly Pattern SetEventWrapper = new(
            [0x83, 0xC4, 0x04, 0x84, 0xC0, 0x74, 0x34, 0x53, 0x8B, 0x5D, 0x0C],
            "xxxxxxxxxxx",
            -29,
            AddressingMode.Absolute,
            anchorOffset: 6
        );

        public static readonly Pattern ProcessPhysics = new(
            [0x8B, 0x8E, 0xB8, 0x00, 0x00, 0x00, 0x8D, 0x45],
            "xxxxxxxx",
            0,
            AddressingMode.Absolute,
            anchorOffset: 1
        );

        public static readonly Pattern DisableTargetAi = new(
            [0x8B, 0x76, 0x08, 0x85, 0xF6, 0x75, 0xEF, 0x8B, 0x73, 0x1C],
            "xxxxxxxxxx",
            -10,
            AddressingMode.Absolute,
            anchorOffset: 8
        );

        public static readonly Pattern SetSharedFlag = new(
            [0x88, 0x94, 0x08, 0xA1],
            "xxxx",
            0,
            AddressingMode.Absolute,
            anchorOffset: 1
        );


        public static readonly Pattern BabyJump = new(
            [0x74, 0x30, 0xF3, 0x0F, 0x10, 0x80, 0x84],
            "xxxxxxx",
            -0x1C,
            AddressingMode.Absolute,
            anchorOffset: 5
        );

        public static readonly Pattern EzStateCompareTimer = new(
            [0x83, 0xC4, 0x0C, 0x85, 0xC0, 0x0F, 0x84, 0x27, 0xD0],
            "xxxxxxxxx",
            -0x59,
            AddressingMode.Absolute,
            anchorOffset: 7
        );


        public static readonly Pattern NoShadedFogClose = new(
            [0x0F, 0xB6, 0x46, 0x07, 0x89],
            "xxxxx",
            0,
            AddressingMode.Absolute,
            anchorOffset: 2
        );

        public static readonly Pattern ReduceGameSpeed = new(
            [0x8B, 0x7B, 0x08, 0xF3, 0x0F, 0x10, 0x07, 0x8B, 0xF1, 0xF6],
            "xxxxxxxxxx",
            0,
            AddressingMode.Absolute,
            anchorOffset: 8
        );


        public static readonly Pattern LightGutter = new(
            [0xF3, 0x0F, 0x7E, 0x86, 0x58, 0xFF, 0xFF, 0xFF, 0x66, 0x0F, 0xD6, 0x87, 0xE8],
            "xxxxxxxxxxxxx",
            0,
            AddressingMode.Absolute,
            anchorOffset: 11
        );


        public static readonly Pattern NoShadedFogFar = new(
            [
                0xF3, 0x0F, 0x11, 0x46, 0x10, 0xE8, 0x00, 0x00, 0x00, 0x00, 0x8D, 0x45, 0xF8, 0x50, 0x53, 0x8B, 0xCF,
                0x89, 0x75, 0xF8
            ],
            "xxxxxx????xxxxxxxxxx",
            0,
            AddressingMode.Absolute,
            anchorOffset: 16
        );

        public static readonly Pattern NoShadedFogCam = new(
            [0x89, 0x4D, 0xE4, 0x33, 0xC9, 0x81],
            "xxxxxx",
            0,
            AddressingMode.Absolute,
            anchorOffset: 4
        );

        public static readonly Pattern NewGameDetect = new(
            [0xC7, 0x46, 0x2C, 0xFF, 0xFF, 0xFF, 0xFF, 0x89, 0x7E, 0x30, 0xC6, 0x46, 0x46, 0x00],
            "xxxxxxxxxxxxxx",
            0,
            AddressingMode.Absolute,
            anchorOffset: 9
        );

        public static readonly Pattern LoadingItemName = new(
            [
                0x8B, 0x0E, 0x8B, 0xF8, 0x8B, 0x01, 0x8B, 0x10, 0xFF, 0xD2, 0x85, 0xC0, 0x74, 0x16, 0x85, 0xFF, 0x75,
                0x05
            ],
            "xxxxxxxxxxxxxxxxxx",
            0,
            AddressingMode.Absolute,
            anchorOffset: 13
        );


        public static readonly Pattern PreAiEzState = new(
            [
                0x8B, 0x4E, 0x3C, 0x8B, 0x11, 0xF3, 0x0F, 0x10, 0x45, 0xFC, 0x8B, 0x42, 0x08, 0x51, 0xF3, 0x0F, 0x11,
                0x04,
                0x24, 0xFF, 0xD0, 0x8B, 0x4E, 0x40, 0x8B, 0x11
            ],
            "xxxxxxxxxxxxxxxxxxxxxxxxxx",
            0,
            AddressingMode.Absolute,
            anchorOffset: 4
        );

        #endregion


        #region Functions

        public static readonly Pattern RequestWarp = new(
            [0x8B, 0x4A, 0x38, 0x83, 0xC4, 0x0C, 0x8D, 0x45, 0xB0, 0x50, 0xE8, 0x00, 0x00, 0x00, 0x00],
            "xxxxxxxxxxx????",
            10,
            AddressingMode.Relative,
            1,
            5,
            anchorOffset: 1
        );

        public static readonly Pattern SetEvent = new(
            [0x39, 0x4A, 0x08, 0x74, 0x12],
            "xxxxx",
            -0x40,
            AddressingMode.Absolute,
            anchorOffset: 0
        );

        public static readonly Pattern GetEvent = new(
            [0x68, 0xD6, 0x77, 0x8E, 0x06, 0x8B, 0xCB, 0xE8, 0x00, 0x00, 0x00, 0x00],
            "xxxxxxxx????",
            7,
            AddressingMode.Relative,
            1,
            5,
            anchorOffset: 2
        );

        public static readonly Pattern GiveSouls = new(
            [0xE8, 0x00, 0x00, 0x00, 0x00, 0xC6, 0x86, 0x04, 0x07],
            "x????xxxx",
            0,
            AddressingMode.Relative,
            1,
            5,
            anchorOffset: 6
        );

        public static readonly Pattern RestoreSpellcasts = new(
            [0x55, 0x8B, 0xEC, 0x83, 0xEC, 0x3C, 0xF3, 0x0F, 0x10, 0x45, 0x08],
            "xxxxxxxxxxx",
            0,
            AddressingMode.Absolute,
            anchorOffset: 5
        );

        public static readonly Pattern ParamLookup = new(
            [0x80, 0x7B, 0x36, 0x00],
            "xxxx",
            0xC,
            AddressingMode.Relative,
            1,
            5,
            anchorOffset: 1
        );

        public static readonly Pattern CreateSoundEvent = new(
            [0xE8, 0x00, 0x00, 0x00, 0x00, 0x84, 0xC0, 0x74, 0x3F, 0x8B, 0x86],
            "x????xxxxxx",
            0,
            AddressingMode.Relative,
            1,
            5,
            anchorOffset: 8
        );

        public static readonly Pattern LevelLookup = new(
            [0x75, 0x0B, 0x50, 0xE8, 0x00, 0x00, 0x00, 0x00, 0x83, 0xC4, 0x04, 0xEB, 0x02],
            "xxxx????xxxxx",
            0x3,
            AddressingMode.Relative,
            1,
            5,
            anchorOffset: 1
        );

        public static readonly Pattern LevelUp = new(
            [0x84, 0xC0, 0x0F, 0x84, 0xAA, 0x00, 0x00, 0x00, 0x8B, 0x0B],
            "xxxxxxxxxx",
            -0x2F,
            AddressingMode.Absolute,
            anchorOffset: 3
        );

        public static readonly Pattern CurrentItemQuantityCheck = new(
            [0x80, 0xE1, 0x03, 0x57, 0x8B],
            "xxxxx",
            0x17,
            AddressingMode.Relative,
            1,
            5,
            anchorOffset: 2
        );

        public static readonly Pattern ItemGive = new(
            [0x83, 0xF8, 0x1F, 0x0F, 0x87, 0xA0],
            "xxxxxx",
            -0x1B,
            AddressingMode.Absolute,
            anchorOffset: 1
        );

        public static readonly Pattern BuildItemDialog = new(
            [0x83, 0xFB, 0x08, 0xC7],
            "xxxx",
            -0x72,
            AddressingMode.Absolute,
            anchorOffset: 1
        );

        public static readonly Pattern ShowItemDialog = new(
            [
                0xF3, 0x0F, 0x11, 0x85, 0x58, 0xFE, 0xFF, 0xFF, 0x8D, 0x95, 0x58, 0xFE, 0xFF, 0xFF, 0x52, 0x8B, 0xCE,
                0xE8, 0x00, 0x00, 0x00, 0x00
            ],
            "xxxxxxxxxxxxxxxxxx????",
            17,
            AddressingMode.Relative,
            1,
            5,
            anchorOffset: 9
        );

        public static readonly Pattern GetEyePosition = new(
            [0xFF, 0xD0, 0x83, 0x7D, 0xC0],
            "xxxxx",
            -0x4F,
            AddressingMode.Absolute,
            anchorOffset: 3
        );


        public static readonly Pattern ApplySpEffect = new(
            [
                0xC7, 0x45, 0xE0, 0xDE, 0x6E, 0x71, 0x05, 0xC7, 0x45, 0xE4, 0x01, 0x00, 0x00, 0x00, 0xF3, 0x0F, 0x11,
                0x45, 0xE8, 0x88, 0x45, 0xEC, 0x66, 0xC7, 0x45, 0xED, 0x02, 0x00, 0xE8, 0x00, 0x00, 0x00, 0x00
            ],
            "xxxxxxxxxxxxxxxxxxxxxxxxxxxxx????",
            28,
            AddressingMode.Relative,
            1,
            5,
            anchorOffset: 4
        );

        public static readonly Pattern HavokRayCast = new(
            [0x83, 0xCF, 0xFF, 0x33, 0xD2],
            "xxxxx",
            -0x33,
            AddressingMode.Absolute,
            anchorOffset: 0
        );

        public static readonly Pattern ConvertPxRigidToMapEntity = new(
            [0xE8, 0x00, 0x00, 0x00, 0x00, 0x83, 0xC4, 0x08, 0x85, 0xC0, 0x0F, 0x84, 0x91],
            "x????xxxxxxxx",
            0,
            AddressingMode.Relative,
            1,
            5,
            anchorOffset: 11
        );

        public static readonly Pattern PackGameEntityHandle = new(
            [0xE8, 0x00, 0x00, 0x00, 0x00, 0x83, 0xC4, 0x08, 0x85, 0xC0, 0x0F, 0x84, 0x91],
            "x????xxxxxxxx",
            0x17,
            AddressingMode.Relative,
            1,
            5,
            anchorOffset: 11
        );

        public static readonly Pattern UnlockBonfire = new(
            [0x74, 0x60, 0x3D, 0xFF],
            "xxxx",
            -0xF,
            AddressingMode.Absolute,
            anchorOffset: 0
        );

        public static readonly Pattern GetMapObjStateActComponent = new(
            [
                0x56, 0x8D, 0x8E, 0x84, 0x00, 0x00, 0x00, 0xE8, 0x00, 0x00, 0x00, 0x00, 0x8B, 0x48, 0x24, 0x8B, 0x01,
                0x8B, 0x50, 0x10, 0x6A, 0x46
            ],
            "xxxxxxxx????xxxxxxxxxx",
            7,
            AddressingMode.Relative,
            1,
            5,
            anchorOffset: 20
        );

        public static readonly Pattern GetMapEntityWithAreaIdAndObjId = new(
            [0x39, 0x42, 0x1C, 0x7E],
            "xxxx",
            0x1B,
            AddressingMode.Relative,
            1,
            5,
            anchorOffset: 2
        );

        public static readonly Pattern AttuneSpell = new(
            [0x83, 0xF8, 0x29, 0x77, 0x29],
            "xxxxx",
            -0x7,
            AddressingMode.Absolute,
            anchorOffset: 1
        );

        public static readonly Pattern GetNumOfSpellSlots1 = new(
            [0x03, 0xD8, 0x8B, 0xCE],
            "xxxx",
            -0xE,
            AddressingMode.Relative,
            1,
            5,
            anchorOffset: 0
        );

        public static readonly Pattern GetNumOfSpellSlots2 = new(
            [0x03, 0xD8, 0x8B, 0xCE],
            "xxxx",
            -0x5,
            AddressingMode.Relative,
            1,
            5,
            anchorOffset: 0
        );

        public static readonly Pattern UpdateSpellSlots = new(
            [0x80, 0xF9, 0x0E, 0x77],
            "xxxx",
            -0x38,
            AddressingMode.Absolute,
            anchorOffset: 1
        );

        public static readonly Pattern Sleep = new(
            [0x8B, 0x1D, 0x00, 0x00, 0x00, 0x00, 0x83, 0xF8],
            "xx????xx",
            0,
            AddressingMode.Direct32,
            2,
            6,
            anchorOffset: 0
        );

        public static readonly Pattern SetDepthStencilSurface = new(
            [0xE8, 0x00, 0x00, 0x00, 0x00, 0x8B, 0x56, 0x44, 0x8B, 0x4C],
            "x????xxxxx",
            0,
            AddressingMode.Relative,
            1,
            5,
            anchorOffset: 6
        );

        public static readonly Pattern EzStateExternalEventCtor = new(
            [
                0xC7, 0x40, 0x08, 0x02, 0x00, 0x00, 0x00, 0x89, 0x48, 0x04, 0xC7, 0x80, 0xFC, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x5D, 0xC2, 0x04, 0x00
            ],
            "xxxxxxxxxxxxxxxxxxxxxxxx",
            -14,
            AddressingMode.Absolute,
            anchorOffset: 10
        );

        public static readonly Pattern EzStateEventExecuteCommand = new(
            [
                0x53, 0x8B, 0xDC, 0x83, 0xEC, 0x08, 0x83, 0xE4, 0xF0, 0x83, 0xC4, 0x04, 0x55, 0x8B, 0x6B, 0x04, 0x89,
                0x6C, 0x24, 0x04, 0x8B, 0xEC, 0x81, 0xEC, 0x48, 0x0D, 0x00, 0x00
            ],
            "xxxxxxxxxxxxxxxxxxxxxxxxxxxx",
            0,
            AddressingMode.Absolute,
            anchorOffset: 23
        );


        public static readonly Pattern OriginalMakeSound = new(
            [0xE8, 0x00, 0x00, 0x00, 0x00, 0x84, 0xC0, 0x74, 0x3F, 0x8B, 0x86],
            "x????xxxxxx",
            0,
            AddressingMode.Relative,
            1,
            5,
            anchorOffset: 8
        );

        public static readonly Pattern OriginalSoulGain = new(
            [0xD9, 0x6D, 0x16, 0xE8],
            "xxxx",
            3,
            AddressingMode.Relative,
            1,
            5,
            anchorOffset: 1
        );

        public static readonly Pattern OpenNpcMenu = new(
            [
                0x8D, 0x85, 0x40, 0xFE, 0xFF, 0xFF, 0x50, 0x8B, 0x42, 0x44, 0x8D, 0x8D, 0x0C, 0xFF, 0xFF, 0xFF, 0x51,
                0x8B,
                0x48, 0x28, 0xE8, 0x00, 0x00, 0x00, 0x00
            ],
            "xxxxxxxxxxxxxxxxxxxxx????",
            20,
            AddressingMode.Relative,
            1,
            5,
            anchorOffset: 8
        );


        public static readonly Pattern SetMenuOpenChrState = new(
            [
                0x55, 0x8B, 0xEC, 0x8B, 0x45, 0x08, 0x83, 0xF8, 0x14, 0x7D, 0x50, 0x53, 0x8B, 0x5D, 0x0C, 0x32, 0xD2,
                0x56,
                0x8B, 0xB4, 0x81, 0xC4, 0x01, 0x00, 0x00
            ],
            "xxxxxxxxxxxxxxxxxxxxxxxxx",
            0,
            AddressingMode.Absolute,
            anchorOffset: 9
        );

        public static readonly Pattern ApplyDurabilityDamage = new(
            [
                0x55, 0x8B, 0xEC, 0x83, 0xEC, 0x0C, 0x53, 0x56, 0x57, 0x8B, 0xF9, 0x8B, 0x4F, 0x04, 0x8B, 0x01, 0x8B,
                0x90,
                0x90, 0x00, 0x00, 0x00, 0xFF, 0xD2, 0x85, 0xC0, 0x74, 0x0D, 0x8B, 0x10, 0x8B, 0xC8, 0x8B, 0x42, 0x38,
                0xFF,
                0xD0, 0x8B, 0xF0, 0xEB, 0x02, 0x33, 0xF6, 0x8B, 0x4F, 0x04, 0x32, 0xDB, 0x89, 0x4D, 0xF4, 0x89, 0x75,
                0xF8,
                0x85, 0xF6, 0x0F, 0x84, 0xBB, 0x00, 0x00, 0x00
            ],
            "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
            0,
            AddressingMode.Absolute,
            anchorOffset: 57
        );

        public static readonly Pattern ResolveTargetCtrlFromHandle = new(
            [
                0x8D, 0x8F, 0xC4, 0x00, 0x00, 0x00, 0xC6, 0x87, 0xE8, 0x00, 0x00, 0x00, 0x00, 0xE8, 0x00, 0x00, 0x00,
                0x00
            ],
            "xxxxxxxxxxxxxx????",
            13,
            AddressingMode.Relative,
            1,
            5,
            anchorOffset: 1
        );

        #endregion
    }
}