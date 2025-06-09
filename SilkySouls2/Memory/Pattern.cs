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

        public static readonly Pattern ProcessPhysics = new Pattern(
            new byte[] { 0x48, 0x8B, 0x8B, 0x00, 0x01, 0x00, 0x00, 0x48, 0x8D, 0x54, 0x24, 0x20 },
            "xxxxxxxxxxxx",
            7,
            None
        );


        public static readonly Pattern InfinitePoise = new Pattern(
            new byte[] { 0x39, 0x9D, 0xEC, 0x05 },
            "xxxx",
            0,
            None
        );

        public static readonly Pattern SetSharedFlag = new Pattern(
            new byte[] { 0x44, 0x88, 0x84, 0x08, 0xA1 },
            "xxxxx",
            0,
            None
        );
        
        public static readonly Pattern EzStateSetEvent = new Pattern(
            new byte[] { 0x41, 0x0F, 0xB6, 0xF8, 0x48, 0x8B, 0x88 },
            "xxxxxxx",
            0,
            None
        );

        public static readonly Pattern ConditionGroupSetFlag = new Pattern(
            new byte[] { 0x80, 0x7B, 0x1D, 0x00, 0x89, 0x73, 0x18, 0x74 },
            "xxxxxxxx",
            0,
            None
        );
        
        public static readonly Pattern FastQuitout = new Pattern(
            new byte[] { 0x48, 0x89, 0x84, 0x24, 0x50, 0x01, 0x00, 0x00, 0x83, 0x79, 0x10 },
            "xxxxxxxxxxx",
            0,
            None
        );

        public static readonly Pattern SetCurrectAct = new Pattern(
            new byte[] { 0x83, 0x89, 0x50, 0x03, 0x00, 0x00, 0x01 },
            "xxxxxxx",
            0,
            None
        );

        public static readonly Pattern DamageControl = new Pattern(
            new byte[] { 0x0F, 0x29, 0x74, 0x24, 0x20, 0x48, 0x8B, 0xB0 },
            "xxxxxxxx",
            0,
            None
        );

        // public static readonly Pattern InAirTimer = new Pattern(
        //     new byte[] { 0xF3, 0x0F, 0x11, 0x4F, 0x10, 0x49 },
        //     "xxxxxx",
        //     0,
        //     None
        // );

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
            new byte[] { 0x66, 0x0F, 0x7F, 0xB8, 0x90 },
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

        public static readonly Pattern GiveSouls = new Pattern(
            new byte[] { 0x74, 0x23, 0x48, 0x8B, 0x80 },
            "xxxxx",
            -0xA,
            None
        );


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

        public static readonly Pattern SetSpEffect = new Pattern(
            new byte[] { 0x48, 0x8B, 0xCD, 0xF3, 0x0F, 0x11, 0x44, 0x24, 0x28, 0xE8 },
            "xxxxxxxxxx",
            0x9,
            Call
        );

        public static readonly Pattern UnlockBonfire = new Pattern(
            new byte[] { 0x48, 0x8B, 0x4E, 0x58, 0x8B, 0x10, 0x44 },
            "xxxxxxx",
            0xA,
            Call
        );

        public static readonly Pattern GetMapObjStateActComponent = new Pattern(
            new byte[] { 0x75, 0x08, 0x48, 0x8B, 0x01, 0x48, 0x8B, 0x40, 0x40 },
            "xxxxxxxxx",
            -0x9,
            None
        );

        public static readonly Pattern GetMapEntityWithAreaIdAndObjId = new Pattern(
            new byte[] { 0x80, 0xB8, 0xE0, 0x01, 0x00, 0x00, 0x0C, 0x48 },
            "xxxxxxxx",
            -0x2F,
            None
        );
            
        public static readonly Pattern HavokRayCast = new Pattern(
            new byte[] { 0xE8, 0x00, 0x00, 0x00, 0x00, 0x80, 0x7D, 0x40, 0x00, 0x0F, 0x84, 0x80 },
            "x????xxxxxxx",
            0,
            Call
        );

        public static readonly Pattern ConvertPxRigidToMapEntity = new Pattern(
            new byte[] { 0x48, 0x85, 0xC9, 0x74, 0x2F, 0x48, 0x8B, 0x79 },
            "xxxxxxxx",
            -0xA,
            None
        );


        public static readonly Pattern ConvertMapEntityToGameId = new Pattern(
            new byte[] { 0x48, 0x8B, 0xC1, 0x48, 0x8B, 0x5C, 0x24, 0x48 },
            "xxxxxxxx",
            -0x19,
            None
        );

        public static readonly Pattern LevelLookUp = new Pattern(
            new byte[] { 0x48, 0x85, 0xDB, 0x74, 0x07, 0x0F, 0xB7, 0x03 },
            "xxxxxxxx",
            -0x50,
            None
        );

        public static readonly Pattern LevelUp = new Pattern(
            new byte[] { 0x48, 0x85, 0xD2, 0x0F, 0x84, 0xA0, 0x01 },
            "xxxxxxx",
            0,
            None
        );


        public static readonly Pattern CurrentItemQuantityCheck = new Pattern(
            new byte[] { 0x48, 0x98, 0x8B, 0x8C, 0x82, 0xCC, 0x52 },
            "xxxxxxx",
            -0x4D,
            None
        );

        public static readonly Pattern ItemGive = new Pattern(
            new byte[] { 0x8D, 0x46, 0xFF, 0x83, 0xF8, 0x1F, 0x0F, 0x87, 0xF2 },
            "xxxxxxxxx",
            -0x1E,
            None
        );

        public static readonly Pattern BuildItemDialog = new Pattern(
            new byte[] { 0x44, 0x89, 0x11, 0x4C },
            "xxxx",
            -0x0F,
            None
        );

        public static readonly Pattern ShowItemDialog = new Pattern(
            new byte[] { 0x48, 0x8B, 0x89, 0xD8, 0x00, 0x00, 0x00, 0x48, 0x85, 0xC9, 0x0F, 0x85, 0x20 },
            "xxxxxxxxxxxxx",
            0,
            None
        );


        public static readonly Pattern CreateSoundEvent = new Pattern(
            new byte[] { 0xE8, 0x00, 0x00, 0x00, 0x00, 0x84, 0xC0, 0x74, 0x4C, 0x48, 0x8B, 0x83 },
            "x????xxxxxxx",
            0,
            Call);


        public static readonly Pattern SetRenderTargetsWrapper = new Pattern(
            new byte[] { 0x40, 0x53, 0x48, 0x83, 0xEC, 0x60, 0x41, 0x8B },
            "xxxxxxxx",
            0,
            None
        );


        public static readonly Pattern ParamLookUp = new Pattern(
            new byte[] { 0x48, 0x89, 0x5C, 0x24, 0x08, 0x4C, 0x8B, 0x59, 0x10 },
            "xxxxxxxxx",
            0,
            None
        );


        public static readonly Pattern SetEvent = new Pattern(
            new byte[] { 0x45, 0x0F, 0xB6, 0xD8, 0xB8 },
            "xxxxx",
            -0xC,
            None
        );

        public static readonly Pattern RestoreSpellcasts = new Pattern(
            new byte[] { 0x40, 0x55, 0x53, 0x41, 0x54, 0x41, 0x55, 0x48, 0x8D, 0x6C },
            "xxxxxxxxxx",
            0,
            None
        );


        // Patches

        public static readonly Pattern InfiniteStam = new Pattern(
            new byte[] { 0x0F, 0x83, 0x26, 0x01, 0x00, 0x00, 0x48, 0x8B },
            "xxxxxxxx",
            0,
            None
        );

        public static readonly Pattern Ng7Patch = new Pattern(
            new byte[] { 0x48, 0x8B, 0xCF, 0x44, 0x89, 0x77 },
            "xxxxxx",
            0xC,
            None
        );

        public static readonly Pattern NoSoulGain = new Pattern(
            new byte[] { 0xF3, 0x48, 0x0F, 0x2C, 0xD0, 0xE8 },
            "xxxxxx",
            5,
            None
        );

        public static readonly Pattern NoHollowing = new Pattern(
            new byte[] { 0x88, 0x54, 0x24, 0x18 },
            "xxxx",
            0x29,
            None
        );

        public static readonly Pattern NoSoulLoss = new Pattern(
            new byte[] { 0x89, 0x90, 0xEC, 0x00 },
            "xxxx",
            0,
            None
        );


        public static readonly Pattern GetEyePosition = new Pattern(
            new byte[] { 0x48, 0x83, 0xC4, 0x30, 0x5F, 0xC3, 0x48, 0x8D, 0x54, 0x24, 0x20, 0xE8 },
            "xxxxxxxxxxxx",
            0xB,
            Call
        );

        public static readonly Pattern Silent = new Pattern(
            new byte[] { 0xE8, 0x00, 0x00, 0x00, 0x00, 0x84, 0xC0, 0x74, 0x4C, 0x48, 0x8B, 0x83 },
            "x????xxxxxxx",
            0,
            None);

        public static readonly Pattern Hidden = new Pattern(
            new byte[] { 0x0F, 0x84, 0x02, 0x02, 0x00, 0x00, 0x48, 0x85 },
            "xxxxxxxx",
            0,
            None
        );

        public static readonly Pattern DisableAi = new Pattern(
            new byte[] { 0x7F, 0x59, 0x48, 0x8D },
            "xxxx",
            0,
            None);


        public static readonly Pattern KillboxFlagSet = new Pattern(
            new byte[]
            {
                0x48, 0xB8, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x08, 0x00, 0x48, 0x09, 0x81, 0xC0, 0x04, 0x00, 0x00,
                0x84
            },
            "xxxxxxxxxxxxxxxxxx",
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

        public static readonly Pattern HideMap = new Pattern(
            new byte[] { 0x48, 0x8B, 0x4B, 0x20, 0x48, 0x98 },
            "xxxxxx",
            -0x6,
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