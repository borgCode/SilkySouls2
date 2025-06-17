using System;
using System.Collections.Generic;
using System.IO;
using SilkySouls2.Memory.Patterns;

namespace SilkySouls2.Memory
{
    public class AoBScanner
    {
        private readonly MemoryIo _memoryIo;

        public AoBScanner(MemoryIo memoryIo)
        {
            _memoryIo = memoryIo;
        }

        public void Scan(bool is64Bit)
        {

            if (is64Bit)
            {

                string appData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "SilkySouls2");
                Directory.CreateDirectory(appData);
                string savePath = Path.Combine(appData, "backup_addresses_scholar.txt");

                Dictionary<string, long> saved = new Dictionary<string, long>();
                if (File.Exists(savePath))
                {
                    foreach (string line in File.ReadAllLines(savePath))
                    {
                        string[] parts = line.Split('=');
                        saved[parts[0]] = Convert.ToInt64(parts[1], 16);
                    }
                }


                Offsets.GameManagerImp.Base = FindAddressByPattern(Patterns64.GameManagerImp);
                Offsets.HkHardwareInfo.Base = FindAddressByPattern(Patterns64.HkpPtrEntity);
                Offsets.MapId = FindAddressByPattern(Patterns64.MapId);


                TryPatternWithFallback("InfiniteStam", Patterns64.InfiniteStam,
                    addr => Offsets.Patches.InfiniteStam = addr, saved);
                TryPatternWithFallback("InfiniteGoods", Patterns64.InfiniteGoods,
                    addr => Offsets.Patches.InfiniteGoods = addr, saved);
                TryPatternWithFallback("HideChrModels", Patterns64.HideChrModels,
                    addr => Offsets.Patches.HideChrModels = addr, saved);
                TryPatternWithFallback("HideMap", Patterns64.HideMap, addr => Offsets.Patches.HideMap = addr,
                    saved);
                TryPatternWithFallback("InfiniteCasts", Patterns64.InfiniteCasts,
                    addr => Offsets.Patches.InfiniteCasts = addr, saved);
                TryPatternWithFallback("InfiniteDurability", Patterns64.InfiniteDurability,
                    addr => Offsets.Patches.InfiniteDurability = addr, saved);
                TryPatternWithFallback("DropRate", Patterns64.DropRate,
                    addr => Offsets.Patches.DropRate = addr, saved);
                TryPatternWithFallback("Silent", Patterns64.Silent,
                    addr => Offsets.Patches.Silent = addr, saved);
                TryPatternWithFallback("Hidden", Patterns64.Hidden,
                    addr => Offsets.Patches.Hidden = addr, saved);
                TryPatternWithFallback("Ng7", Patterns64.Ng7Patch,
                    addr => Offsets.Patches.Ng7 = addr, saved);
                TryPatternWithFallback("DisableAi", Patterns64.DisableAi,
                    addr => Offsets.Patches.DisableAi = addr, saved);
                TryPatternWithFallback("NoSoulGain", Patterns64.NoSoulGain,
                    addr => Offsets.Patches.NoSoulGain = addr,
                    saved);
                TryPatternWithFallback("NoHollowing", Patterns64.NoHollowing,
                    addr => Offsets.Patches.NoHollowing = addr,
                    saved);
                TryPatternWithFallback("NoSoulLoss", Patterns64.NoSoulLoss,
                    addr => Offsets.Patches.NoSoulLoss = addr,
                    saved);
                // TryPatternWithFallback("FreeCam", FreeCamPatch, addr => Offsets.Patches.FreeCam = addr, saved);
                //
                // TryPatternWithFallback("OverrideGeneratorStartPositionRandom",
                //     OverrideGeneratorStartPositionRandom,
                //     addr => Offsets.Hooks.OverrideGeneratorStartPositionRandom = addr.ToInt64(), saved);

                TryPatternWithFallback("SetAreaVariable",
                    Patterns64.SetAreaVariable,
                    addr => Offsets.Hooks.SetAreaVariable = addr.ToInt64(), saved);

                TryPatternWithFallback("CompareEventRandValue",
                    Patterns64.CompareEventRandValue,
                    addr => Offsets.Hooks.CompareEventRandValue = addr.ToInt64(), saved);
                TryPatternWithFallback("EzStateSetEvent",
                    Patterns64.EzStateSetEvent,
                    addr => Offsets.Hooks.EzStateSetEvent = addr.ToInt64(), saved);
                TryPatternWithFallback("HpWrite", Patterns64.HpWrite,
                    addr => Offsets.Hooks.HpWrite = addr.ToInt64(), saved);
                Offsets.Hooks.OneShot = Offsets.Hooks.HpWrite - 0xB;

                TryPatternWithFallback("WarpCoordWrite", Patterns64.WarpCoordWrite,
                    addr => Offsets.Hooks.WarpCoordWrite = addr.ToInt64(), saved);
                TryPatternWithFallback("LockedTarget", Patterns64.LockedTarget,
                    addr => Offsets.Hooks.LockedTarget = addr.ToInt64(), saved);
                TryPatternWithFallback("CreditSkip", Patterns64.CreditSkip,
                    addr => Offsets.Hooks.CreditSkip = addr.ToInt64(), saved);
                TryPatternWithFallback("NumOfDrops", Patterns64.NumOfDrops,
                    addr => Offsets.Hooks.NumOfDrops = addr.ToInt64(), saved);
                TryPatternWithFallback("KillboxFlagSet", Patterns64.KillboxFlagSet,
                    addr => Offsets.Hooks.KillboxFlagSet = addr.ToInt64(), saved);

                TryPatternWithFallback("DamageControl", Patterns64.DamageControl,
                    addr => Offsets.Hooks.DamageControl = addr.ToInt64(), saved);
                TryPatternWithFallback("TriggersAndSpace", Patterns64.TriggersAndSpace,
                    addr => Offsets.Hooks.TriggersAndSpace = addr.ToInt64(), saved);
                TryPatternWithFallback("Ctrl", Patterns64.Ctrl,
                    addr => Offsets.Hooks.Ctrl = addr.ToInt64(), saved);
                TryPatternWithFallback("NoClipUpdateCoords", Patterns64.NoClipUpdateCoords,
                    addr => Offsets.Hooks.NoClipUpdateCoords = addr.ToInt64(), saved);
                TryPatternWithFallback("FastQuitout", Patterns64.FastQuitout,
                    addr => Offsets.Hooks.FastQuitout = addr.ToInt64(), saved);
                TryPatternWithFallback("InfinitePoise", Patterns64.InfinitePoise,
                    addr => Offsets.Hooks.InfinitePoise = addr.ToInt64(), saved);
                TryPatternWithFallback("MapIdWrite", Patterns64.ProcessPhysics,
                    addr => Offsets.Hooks.ProcessPhysics = addr.ToInt64(), saved);
                TryPatternWithFallback("EzStateCompareTimer", Patterns64.EzStateCompareTimer,
                    addr => Offsets.Hooks.EzStateCompareTimer = addr.ToInt64(), saved);
                TryPatternWithFallback("SetSharedFlag", Patterns64.SetSharedFlag,
                    addr => Offsets.Hooks.SetSharedFlag = addr.ToInt64(), saved);
                TryPatternWithFallback("BabyJump", Patterns64.BabyJump,
                    addr => Offsets.Hooks.BabyJump = addr.ToInt64(), saved);
                TryPatternWithFallback("FogRender", Patterns64.FogRender,
                    addr => Offsets.Hooks.FogRender = addr.ToInt64(), saved);

                Offsets.Hooks.DisableTargetAi = Offsets.Patches.DisableAi.ToInt64() + 0x2C;

                var setCurrectActLocs = FindAddressesByPattern(Patterns64.SetCurrectAct, 2);
                if (setCurrectActLocs.Count < 2 || setCurrectActLocs[0] == IntPtr.Zero)
                {
                    if (saved.TryGetValue("SetCurrectAct", out var value))
                    {
                        Offsets.Hooks.SetCurrectAct = value;
                        Offsets.Hooks.SetCurrectAct2 = saved["SetCurrectAct2"];
                    }
                }
                else
                {
                    Offsets.Hooks.SetCurrectAct = setCurrectActLocs[0].ToInt64();
                    Offsets.Hooks.SetCurrectAct2 = setCurrectActLocs[1].ToInt64();
                    saved["SetCurrectAct"] = setCurrectActLocs[0].ToInt64();
                    saved["SetCurrectAct2"] = setCurrectActLocs[1].ToInt64();
                }

                using (var writer = new StreamWriter(savePath))
                {
                    foreach (var pair in saved)
                        writer.WriteLine($"{pair.Key}={pair.Value:X}");
                }

                Offsets.Funcs.WarpPrep = FindAddressByPattern(Patterns64.WarpPrep).ToInt64();
                Offsets.Funcs.BonfireWarp = FindAddressByPattern(Patterns64.BonfireWarp).ToInt64();
                Offsets.Funcs.RestoreSpellcasts = FindAddressByPattern(Patterns64.RestoreSpellcasts).ToInt64();
                Offsets.Funcs.ParamLookUp = FindAddressByPattern(Patterns64.ParamLookUp).ToInt64();
                Offsets.Funcs.SetEvent = FindAddressByPattern(Patterns64.SetEvent).ToInt64();
                Offsets.Funcs.GiveSouls = FindAddressByPattern(Patterns64.GiveSouls).ToInt64();
                Offsets.Funcs.LevelLookup = FindAddressByPattern(Patterns64.LevelLookUp).ToInt64();
                Offsets.Funcs.LevelUp = FindAddressByPattern(Patterns64.LevelUp).ToInt64();
                Offsets.Patches.NegativeLevel = (IntPtr)Offsets.Funcs.LevelUp + 0x38;

                Offsets.Funcs.CurrentItemQuantityCheck =
                    FindAddressByPattern(Patterns64.CurrentItemQuantityCheck).ToInt64();
                Offsets.Funcs.ItemGive = FindAddressByPattern(Patterns64.ItemGive).ToInt64();
                Offsets.Funcs.BuildItemDialog = FindAddressByPattern(Patterns64.BuildItemDialog).ToInt64();
                Offsets.Funcs.ShowItemDialog = FindAddressByPattern(Patterns64.ShowItemDialog).ToInt64();
                Offsets.Funcs.GetEyePosition = FindAddressByPattern(Patterns64.GetEyePosition).ToInt64();
                Offsets.Funcs.SetSpEffect = FindAddressByPattern(Patterns64.SetSpEffect).ToInt64();
                Offsets.Funcs.HavokRayCast = FindAddressByPattern(Patterns64.HavokRayCast).ToInt64();
                Offsets.Funcs.ConvertPxRigidToMapEntity =
                    FindAddressByPattern(Patterns64.ConvertPxRigidToMapEntity).ToInt64();
                Offsets.Funcs.ConvertMapEntityToGameId =
                    FindAddressByPattern(Patterns64.ConvertMapEntityToGameId).ToInt64();
                Offsets.Funcs.UnlockBonfire = FindAddressByPattern(Patterns64.UnlockBonfire).ToInt64();
                Offsets.Funcs.GetMapObjStateActComponent =
                    FindAddressByPattern(Patterns64.GetMapObjStateActComponent).ToInt64();
                Offsets.Funcs.GetMapEntityWithAreaIdAndObjId =
                    FindAddressByPattern(Patterns64.GetMapEntityWithAreaIdAndObjId).ToInt64();
                Offsets.Funcs.GetWhiteDoorComponent =
                    FindAddressByPattern(Patterns64.GetWhiteDoorComponent).ToInt64();
                Offsets.Funcs.AttuneSpell = FindAddressByPattern(Patterns64.AttuneSpell).ToInt64();
                Offsets.Funcs.UpdateSpellSlots = FindAddressByPattern(Patterns64.UpdateSpellSlots).ToInt64();

                FindMultipleCallsInFunction(Patterns64.DisableNaviMesh, new Dictionary<Action<long>, int>
                {
                    { addr => Offsets.Funcs.GetNavimeshLoc = addr, -0xD },
                    { addr => Offsets.Funcs.DisableNaviMesh = addr, 0xE },
                });

                FindMultipleCallsInFunction(Patterns64.GetNumOfSpellSlots, new Dictionary<Action<long>, int>
                {
                    { addr => Offsets.Funcs.GetNumOfSpellslots1 = addr, -0x12 },
                    { addr => Offsets.Funcs.GetNumOfSpellslots2 = addr, -0x8 },
                });

                TryPatternWithFallback("SetRenderTargets",
                    Patterns64.SetRenderTargetsWrapper,
                    addr => Offsets.Funcs.SetRenderTargets = addr.ToInt64(), saved);
                TryPatternWithFallback("CreateSoundEvent",
                    Patterns64.CreateSoundEvent,
                    addr => Offsets.Funcs.CreateSoundEvent = addr.ToInt64(), saved);
            }
            else
            {
                string appData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "SilkySouls2");
                Directory.CreateDirectory(appData);
                string savePath = Path.Combine(appData, "backup_addresses_vanilla.txt");

                Dictionary<string, long> saved = new Dictionary<string, long>();
                if (File.Exists(savePath))
                {
                    foreach (string line in File.ReadAllLines(savePath))
                    {
                        string[] parts = line.Split('=');
                        saved[parts[0]] = Convert.ToInt64(parts[1], 16);
                    }
                }
                
                
                Offsets.GameManagerImp.Base = FindAddressByPattern(Patterns32.GameManagerImp);
                
                
                
                TryPatternWithFallback("InfiniteStam", Patterns32.InfiniteStam,
                    addr => Offsets.Patches.InfiniteStam = addr, saved);
                TryPatternWithFallback("InfiniteGoods", Patterns32.InfiniteGoods,
                    addr => Offsets.Patches.InfiniteGoods = addr, saved);
                TryPatternWithFallback("InfiniteDurability", Patterns32.InfiniteDurability,
                    addr => Offsets.Patches.InfiniteDurability = addr, saved);
                TryPatternWithFallback("InfiniteCasts", Patterns32.InfiniteCasts,
                    addr => Offsets.Patches.InfiniteCasts = addr, saved);
                TryPatternWithFallback("NoSoulGain", Patterns32.NoSoulGain,
                    addr => Offsets.Patches.NoSoulGain = addr, saved);
                TryPatternWithFallback("NoHollowing", Patterns32.NoHollowing,
                    addr => Offsets.Patches.NoHollowing = addr, saved);
                TryPatternWithFallback("NoSoulLoss", Patterns32.NoSoulLoss,
                    addr => Offsets.Patches.NoSoulLoss = addr, saved);
                TryPatternWithFallback("Hidden", Patterns32.Hidden,
                    addr => Offsets.Patches.Hidden = addr, saved);
                
                
                
                Offsets.Funcs.SetSpEffect = FindAddressByPattern(Patterns32.SetSpEffect).ToInt32();
                Offsets.Funcs.GiveSouls = FindAddressByPattern(Patterns32.GiveSouls).ToInt32();
                Offsets.Funcs.LevelUp = FindAddressByPattern(Patterns32.LevelUp).ToInt32();
                Offsets.Funcs.LevelLookup = FindAddressByPattern(Patterns32.LevelLookup).ToInt32();
                Offsets.Patches.NegativeLevel = (IntPtr)Offsets.Funcs.LevelUp + 0x31;
                
                using (var writer = new StreamWriter(savePath))
                {
                    foreach (var pair in saved)
                        writer.WriteLine($"{pair.Key}={pair.Value:X}");
                }
            }


#if DEBUG
            Console.WriteLine($"GameManagerImp.Base: 0x{Offsets.GameManagerImp.Base.ToInt64():X}");
            Console.WriteLine($"HkpPtrEntity.Base: 0x{Offsets.HkHardwareInfo.Base.ToInt64():X}");
            Console.WriteLine($"MapId: 0x{Offsets.MapId.ToInt64():X}");
            
            Console.WriteLine($"Patches.InfiniteStam: 0x{Offsets.Patches.InfiniteStam.ToInt64():X}");
            Console.WriteLine($"Patches.InfiniteGoods: 0x{Offsets.Patches.InfiniteGoods.ToInt64():X}");
            Console.WriteLine($"Patches.HideChrModels: 0x{Offsets.Patches.HideChrModels.ToInt64():X}");
            Console.WriteLine($"Patches.HideMap: 0x{Offsets.Patches.HideMap.ToInt64():X}");
            Console.WriteLine($"Patches.InfiniteCasts: 0x{Offsets.Patches.InfiniteCasts.ToInt64():X}");
            Console.WriteLine($"Patches.InfiniteDurability: 0x{Offsets.Patches.InfiniteDurability.ToInt64():X}");
            Console.WriteLine($"Patches.DropRate: 0x{Offsets.Patches.DropRate.ToInt64():X}");
            Console.WriteLine($"Patches.DisableAi: 0x{Offsets.Patches.DisableAi.ToInt64():X}");
            Console.WriteLine($"Patches.Silent: 0x{Offsets.Patches.Silent.ToInt64():X}");
            Console.WriteLine($"Patches.Hidden: 0x{Offsets.Patches.Hidden.ToInt64():X}");
            Console.WriteLine($"Patches.NegativeLevel: 0x{Offsets.Patches.NegativeLevel.ToInt64():X}");
            Console.WriteLine($"Patches.Ng7: 0x{Offsets.Patches.Ng7.ToInt64():X}");
            Console.WriteLine($"Patches.NoSoulGain: 0x{Offsets.Patches.NoSoulGain.ToInt64():X}");
            Console.WriteLine($"Patches.NoHollowing: 0x{Offsets.Patches.NoHollowing.ToInt64():X}");
            Console.WriteLine($"Patches.NoSoulLoss: 0x{Offsets.Patches.NoSoulLoss.ToInt64():X}");
            
           
            Console.WriteLine($"Hooks.SetAreaVariable: 0x{Offsets.Hooks.SetAreaVariable:X}");
            Console.WriteLine($"Hooks.CompareEventRandValue: 0x{Offsets.Hooks.CompareEventRandValue:X}");
            Console.WriteLine($"Hooks.HpWrite: 0x{Offsets.Hooks.HpWrite:X}");
            Console.WriteLine($"Hooks.EzStateSetEvent: 0x{Offsets.Hooks.EzStateSetEvent:X}");
            Console.WriteLine($"Hooks.OneShot: 0x{Offsets.Hooks.OneShot:X}");
            Console.WriteLine($"Hooks.WarpCoordWrite: 0x{Offsets.Hooks.WarpCoordWrite:X}");
            Console.WriteLine($"Hooks.LockedTarget: 0x{Offsets.Hooks.LockedTarget:X}");
            Console.WriteLine($"Hooks.CreditSkip: 0x{Offsets.Hooks.CreditSkip:X}");
            Console.WriteLine($"Hooks.NumOfDrops: 0x{Offsets.Hooks.NumOfDrops:X}");
            Console.WriteLine($"Hooks.DamageControl: 0x{Offsets.Hooks.DamageControl:X}");
            Console.WriteLine($"Hooks.InAirTimer: 0x{Offsets.Hooks.InAirTimer:X}");
            Console.WriteLine($"Hooks.TriggersAndSpace: 0x{Offsets.Hooks.TriggersAndSpace:X}");
            Console.WriteLine($"Hooks.Ctrl: 0x{Offsets.Hooks.Ctrl:X}");
            Console.WriteLine($"Hooks.NoClipUpdateCoords: 0x{Offsets.Hooks.NoClipUpdateCoords:X}");
            Console.WriteLine($"Hooks.KillboxFlagSet: 0x{Offsets.Hooks.KillboxFlagSet:X}");
            Console.WriteLine($"Hooks.SetCurrectAct: 0x{Offsets.Hooks.SetCurrectAct:X}");
            Console.WriteLine($"Hooks.SetCurrectAct2: 0x{Offsets.Hooks.SetCurrectAct2:X}");
            Console.WriteLine($"Hooks.FastQuitout: 0x{Offsets.Hooks.FastQuitout:X}");
            Console.WriteLine($"Hooks.ProcessPhysics: 0x{Offsets.Hooks.ProcessPhysics:X}");
            Console.WriteLine($"Hooks.DisableTargetAi: 0x{Offsets.Hooks.DisableTargetAi:X}");
            Console.WriteLine($"Hooks.InfinitePoise: 0x{Offsets.Hooks.InfinitePoise:X}");
            Console.WriteLine($"Hooks.SetSharedFlag: 0x{Offsets.Hooks.SetSharedFlag:X}");
            Console.WriteLine($"Hooks.BabyJump: 0x{Offsets.Hooks.BabyJump:X}");
            Console.WriteLine($"Hooks.EzStateCompareTimer: 0x{Offsets.Hooks.EzStateCompareTimer:X}");
            Console.WriteLine($"Hooks.FogRender: 0x{Offsets.Hooks.FogRender:X}");
            
            Console.WriteLine($"Funcs.WarpPrep: 0x{Offsets.Funcs.WarpPrep:X}");
            Console.WriteLine($"Funcs.BonfireWarp: 0x{Offsets.Funcs.BonfireWarp:X}");
            Console.WriteLine($"Funcs.GiveSouls: 0x{Offsets.Funcs.GiveSouls:X}");
            Console.WriteLine($"Funcs.SetEvent: 0x{Offsets.Funcs.SetEvent:X}");
            Console.WriteLine($"Funcs.RestoreSpellcasts: 0x{Offsets.Funcs.RestoreSpellcasts:X}");
            Console.WriteLine($"Funcs.ParamLookUp: 0x{Offsets.Funcs.ParamLookUp:X}");
            Console.WriteLine($"Funcs.SetRenderTargets: 0x{Offsets.Funcs.SetRenderTargets:X}");
            Console.WriteLine($"Funcs.CreateSoundEvent: 0x{Offsets.Funcs.CreateSoundEvent:X}");
            Console.WriteLine($"Funcs.LevelLookUp: 0x{Offsets.Funcs.LevelLookup:X}");
            Console.WriteLine($"Funcs.LevelUp: 0x{Offsets.Funcs.LevelUp:X}");
            Console.WriteLine($"Funcs.CurrentItemQuantityCheck: 0x{Offsets.Funcs.CurrentItemQuantityCheck:X}");
            Console.WriteLine($"Funcs.ItemGive: 0x{Offsets.Funcs.ItemGive:X}");
            Console.WriteLine($"Funcs.BuildItemDialog: 0x{Offsets.Funcs.BuildItemDialog:X}");
            Console.WriteLine($"Funcs.ShowItemDialog: 0x{Offsets.Funcs.ShowItemDialog:X}");
            Console.WriteLine($"Funcs.GetEyePosition: 0x{Offsets.Funcs.GetEyePosition:X}");
            Console.WriteLine($"Funcs.SetSpEffect: 0x{Offsets.Funcs.SetSpEffect:X}");
            Console.WriteLine($"Funcs.HavokRayCast: 0x{Offsets.Funcs.HavokRayCast:X}");
            Console.WriteLine($"Funcs.ConvertPxRigidToMapEntity: 0x{Offsets.Funcs.ConvertPxRigidToMapEntity:X}");
            Console.WriteLine($"Funcs.ConvertMapEntityToGameId: 0x{Offsets.Funcs.ConvertMapEntityToGameId:X}");
            Console.WriteLine($"Funcs.UnlockBonfire: 0x{Offsets.Funcs.UnlockBonfire:X}");
            Console.WriteLine($"Funcs.GetMapObjStateActComponent: 0x{Offsets.Funcs.GetMapObjStateActComponent:X}");
            Console.WriteLine($"Funcs.GetMapEntityWithAreaIdAndObjId: 0x{Offsets.Funcs.GetMapEntityWithAreaIdAndObjId:X}");
            Console.WriteLine($"Funcs.GetNavimeshLoc: 0x{Offsets.Funcs.GetNavimeshLoc:X}");
            Console.WriteLine($"Funcs.DisableNaviMesh: 0x{Offsets.Funcs.DisableNaviMesh:X}");
            Console.WriteLine($"Funcs.GetWhiteDoorComponent: 0x{Offsets.Funcs.GetWhiteDoorComponent:X}");
            Console.WriteLine($"Funcs.AttuneSpell: 0x{Offsets.Funcs.AttuneSpell:X}");
            Console.WriteLine($"Funcs.GetNumOfSpellslots1: 0x{Offsets.Funcs.GetNumOfSpellslots1:X}");
            Console.WriteLine($"Funcs.GetNumOfSpellslots2: 0x{Offsets.Funcs.GetNumOfSpellslots2:X}");
            Console.WriteLine($"Funcs.UpdateSpellSlots: 0x{Offsets.Funcs.UpdateSpellSlots:X}");
#endif
        }

        private void TryPatternWithFallback(string name, Pattern pattern, Action<IntPtr> setter,
            Dictionary<string, long> saved)
        {
            var addr = FindAddressByPattern(pattern);

            if (addr == IntPtr.Zero && saved.TryGetValue(name, out var value))
                addr = new IntPtr(value);
            else if (addr != IntPtr.Zero)
                saved[name] = addr.ToInt64();

            setter(addr);
        }


        public IntPtr FindAddressByPattern(Pattern pattern)
        {
            var results = FindAddressesByPattern(pattern, 1);
            return results.Count > 0 ? results[0] : IntPtr.Zero;
        }

        public List<IntPtr> FindAddressesByPattern(Pattern pattern, int size)
        {
            List<IntPtr> addresses = PatternScanMultiple(pattern.Bytes, pattern.Mask, size);

            for (int i = 0; i < addresses.Count; i++)
            {
                IntPtr instructionAddress = IntPtr.Add(addresses[i], pattern.InstructionOffset);

                switch (pattern.AddressingMode)
                {
                    case AddressingMode.Absolute:
                        addresses[i] = instructionAddress;
                        break;
                    case AddressingMode.Direct32:
                    {
                        uint absoluteAddr = _memoryIo.ReadUInt32(IntPtr.Add(instructionAddress, pattern.OffsetLocation));
                        addresses[i] = (IntPtr)absoluteAddr;
                        break;
                    }
                    default:
                    {
                        int offset = _memoryIo.ReadInt32(IntPtr.Add(instructionAddress, pattern.OffsetLocation));
                        addresses[i] = IntPtr.Add(instructionAddress, offset + pattern.InstructionLength);
                        break;
                    }
                }
            }

            return addresses;
        }

        private List<IntPtr> PatternScanMultiple(byte[] pattern, string mask, int size)
        {
            const int chunkSize = 4096 * 16;
            byte[] buffer = new byte[chunkSize];

            IntPtr currentAddress = _memoryIo.BaseAddress;
            IntPtr endAddress = IntPtr.Add(currentAddress, 0x3200000);

            List<IntPtr> addresses = new List<IntPtr>();

            while (currentAddress.ToInt64() < endAddress.ToInt64())
            {
                int bytesRemaining = (int)(endAddress.ToInt64() - currentAddress.ToInt64());
                int bytesToRead = Math.Min(bytesRemaining, buffer.Length);

                if (bytesToRead < pattern.Length)
                    break;

                buffer = _memoryIo.ReadBytes(currentAddress, bytesToRead);

                for (int i = 0; i <= bytesToRead - pattern.Length; i++)
                {
                    bool found = true;

                    for (int j = 0; j < pattern.Length; j++)
                    {
                        if (j < mask.Length && mask[j] == '?')
                            continue;

                        if (buffer[i + j] != pattern[j])
                        {
                            found = false;
                            break;
                        }
                    }

                    if (found)
                        addresses.Add(IntPtr.Add(currentAddress, i));
                    if (addresses.Count == size) break;
                }

                currentAddress = IntPtr.Add(currentAddress, bytesToRead - pattern.Length + 1);
            }

            return addresses;
        }
        
        private void FindMultipleCallsInFunction(Pattern basePattern, Dictionary<Action<long>, int> callMappings)
        {
            var baseInstructionAddr = FindAddressByPattern(basePattern);
    
            foreach (var mapping in callMappings)
            {
                var callInstructionAddr = IntPtr.Add(baseInstructionAddr, mapping.Value);
        
                int callOffset = _memoryIo.ReadInt32(IntPtr.Add(callInstructionAddr, 1));
                var callTarget = IntPtr.Add(callInstructionAddr, callOffset + 5);
        
                mapping.Key(callTarget.ToInt64());
            }
        }
    }
}