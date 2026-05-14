using System;
using System.Collections.Generic;
using System.IO;
using SilkySouls2.Interfaces;
using SilkySouls2.Memory.Patterns;

namespace SilkySouls2.Memory
{
    public class AoBScanner(IMemoryService memoryService)
    {
        public void FallBackScan()
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
                Offsets.MapId = FindAddressByPattern(Patterns32.MapId);
                Offsets.LoadLibraryW = FindAddressByPattern(Patterns32.LoadLibraryW);
                Offsets.KatanaMainApp.Base = FindAddressByPattern(Patterns32.KatanaMainApp);
                
                
                
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
                TryPatternWithFallback("DisableAi", Patterns32.DisableAi,
                    addr => Offsets.Patches.DisableAi = addr, saved);
                TryPatternWithFallback("Silent", Patterns32.Silent,
                    addr => Offsets.Patches.Silent = addr, saved);
                TryPatternWithFallback("Ng7", Patterns32.Ng7Patch,
                    addr => Offsets.Patches.Ng7 = addr, saved);
                TryPatternWithFallback("DropRate", Patterns32.DropRate,
                    addr => Offsets.Patches.DropRate = addr, saved);
                TryPatternWithFallback("HideMap", Patterns32.HideMap,
                    addr => Offsets.Patches.HideMap = addr, saved);
                TryPatternWithFallback("HideChrModels", Patterns32.HideChrModels,
                    addr => Offsets.Patches.HideChrModels = addr, saved);
                
                TryPatternWithFallback("LockedTarget", Patterns32.LockedTarget,
                    addr => Offsets.Hooks.LockedTarget = addr, saved);
                TryPatternWithFallback("InfinitePoise", Patterns32.InfinitePoise,
                    addr => Offsets.Hooks.InfinitePoise = addr, saved);
                TryPatternWithFallback("DamageControl", Patterns32.DamageControl,
                    addr => Offsets.Hooks.DamageControl = addr, saved);
                // TryPatternWithFallback("PlayerNoDamage", Patterns32.PlayerNoDamage,
                //     addr => Offsets.Hooks.PlayerNoDamage = addr, saved);
                TryPatternWithFallback("WarpCoordWrite", Patterns32.WarpCoordWrite,
                    addr => Offsets.Hooks.WarpCoordWrite = addr, saved);
                TryPatternWithFallback("SetSharedFlag", Patterns32.SetSharedFlag,
                    addr => Offsets.Hooks.SetSharedFlag = addr, saved);
                TryPatternWithFallback("TriggersAndSpace", Patterns32.TriggersAndSpace,
                    addr => Offsets.Hooks.TriggersAndSpace = addr, saved);
                TryPatternWithFallback("Ctrl", Patterns32.Ctrl,
                    addr => Offsets.Hooks.Ctrl = addr, saved);
                TryPatternWithFallback("NoClipUpdateCoords", Patterns32.NoClipUpdateCoords,
                    addr => Offsets.Hooks.NoClipUpdateCoords = addr, saved);
                TryPatternWithFallback("ProcessPhysics", Patterns32.ProcessPhysics,
                    addr => Offsets.Hooks.ProcessPhysics = addr, saved);
                TryPatternWithFallback("KillboxFlagSet", Patterns32.KillboxFlagSet,
                    addr => Offsets.Hooks.KillboxFlagSet = addr, saved);
                TryPatternWithFallback("CreditSkip", Patterns32.CreditSkip,
                    addr => Offsets.Hooks.CreditSkip = addr, saved);
                TryPatternWithFallback("NumOfDrops", Patterns32.NumOfDrops,
                    addr => Offsets.Hooks.NumOfDrops = addr, saved);
                TryPatternWithFallback("SetEventWrapper", Patterns32.SetEventWrapper,
                    addr => Offsets.Hooks.SetEventWrapper = addr, saved);
                TryPatternWithFallback("EzStateCompareTimer", Patterns32.EzStateCompareTimer,
                    addr => Offsets.Hooks.EzStateCompareTimer = addr, saved);
                TryPatternWithFallback("FasterMenu", Patterns32.FasterMenu,
                    addr => Offsets.Hooks.FasterMenu = addr, saved);
                TryPatternWithFallback("BabyJump", Patterns32.BabyJump,
                    addr => Offsets.Hooks.BabyJump = addr, saved);
                TryPatternWithFallback("DisableTargetAi", Patterns32.DisableTargetAi,
                    addr => Offsets.Hooks.DisableTargetAi = addr, saved);
                TryPatternWithFallback("ReduceGameSpeed", Patterns32.ReduceGameSpeed,
                    addr => Offsets.Hooks.ReduceGameSpeed = addr, saved);
                TryPatternWithFallback("LightGutter", Patterns32.LightGutter,
                    addr => Offsets.Hooks.LightGutter = addr, saved);
                TryPatternWithFallback("NoShadedFogClose", Patterns32.NoShadedFogClose,
                    addr => Offsets.Hooks.NoShadedFogClose = addr, saved);
                TryPatternWithFallback("NoShadedFogFar", Patterns32.NoShadedFogFar,
                    addr => Offsets.Hooks.NoShadedFogFar = addr, saved);
                TryPatternWithFallback("NoShadedFogCam", Patterns32.NoShadedFogCam,
                    addr => Offsets.Hooks.NoShadedFogCam = addr, saved);
                TryPatternWithFallback("CompareEventRandValueElana", Patterns32.CompareEventRandValueElana,
                    addr => Offsets.Hooks.CompareEventRandValueElana = addr, saved);
                
                var setCurrectActLocs = FindAddressesByPattern(Patterns32.SetCurrentAct, 2);
                if (setCurrectActLocs.Count < 2 || setCurrectActLocs[0] == IntPtr.Zero)
                {
                    if (saved.TryGetValue("SetCurrectAct", out var value))
                    {
                        Offsets.Hooks.SetCurrentAct = (nint)value;
                    }
                }
                else
                {
                    byte[] bytes0 = memoryService.ReadBytes(setCurrectActLocs[0] - 10, 3);

                    bool isReturnInstruction = bytes0[0] == 0xC2 && bytes0[1] == 0x04 && bytes0[2] == 0x00;

                    IntPtr validAddress = !isReturnInstruction ? setCurrectActLocs[0] : setCurrectActLocs[1];

                    Offsets.Hooks.SetCurrentAct = validAddress;
                    saved["SetCurrectAct"] = validAddress.ToInt32();
                }
                
                
                Offsets.Functions.SetSpEffect = FindAddressByPattern(Patterns32.SetSpEffect).ToInt32();
                Offsets.Functions.GiveSouls = FindAddressByPattern(Patterns32.GiveSouls).ToInt32();
                Offsets.Functions.LevelUp = FindAddressByPattern(Patterns32.LevelUp).ToInt32();
                Offsets.Functions.LevelLookup = FindAddressByPattern(Patterns32.LevelLookup).ToInt32();
                Offsets.Functions.RestoreSpellcasts = FindAddressByPattern(Patterns32.RestoreSpellcasts).ToInt32();
                Offsets.Functions.CreateSoundEvent = FindAddressByPattern(Patterns32.CreateSoundEvent).ToInt32();
                Offsets.Functions.UnlockBonfire = FindAddressByPattern(Patterns32.UnlockBonfire).ToInt32();
                Offsets.Functions.SetEvent = FindAddressByPattern(Patterns32.SetEvent).ToInt32();
                Offsets.Functions.GetMapEntityWithAreaIdAndObjId = FindAddressByPattern(Patterns32.GetMapEntityWithAreaIdAndObjId).ToInt32();
                Offsets.Functions.GetMapObjStateActComponent = FindAddressByPattern(Patterns32.GetStateActComp).ToInt32();
                Offsets.Functions.GetWhiteDoorComponent = FindAddressByPattern(Patterns32.GetWhiteDoorComponent).ToInt32();
                Offsets.Functions.HavokRayCast = FindAddressByPattern(Patterns32.HavokRayCast).ToInt32();
                Offsets.Functions.ItemGive = FindAddressByPattern(Patterns32.ItemGive).ToInt32();
                Offsets.Functions.BuildItemDialog = FindAddressByPattern(Patterns32.BuildItemDialog).ToInt32();
                Offsets.Functions.ShowItemDialog = FindAddressByPattern(Patterns32.ShowItemDialog).ToInt32();
                Offsets.Functions.CurrentItemQuantityCheck = FindAddressByPattern(Patterns32.CurrentItemQuantityCheck).ToInt32();
                Offsets.Functions.Sleep = FindAddressByPattern(Patterns32.Sleep).ToInt32();
                Offsets.Functions.UpdateSpellSlots = FindAddressByPattern(Patterns32.UpdateSpellSlots).ToInt32();
                Offsets.Functions.AttuneSpell = FindAddressByPattern(Patterns32.AttuneSpell).ToInt32();
                Offsets.Functions.ParamLookup = FindAddressByPattern(Patterns32.ParamLookup).ToInt32();
                Offsets.Functions.GetEyePosition = FindAddressByPattern(Patterns32.GetEyePosition).ToInt32();
                Offsets.Functions.GetEvent = FindAddressByPattern(Patterns32.GetEvent).ToInt32();
                
                FindMultipleCallsInFunction(Patterns32.GetNumOfSpellSlots, new Dictionary<Action<long>, int>
                {
                    { addr => Offsets.Functions.GetNumOfSpellSlots1 = (nint)addr, -0xE },
                    { addr => Offsets.Functions.GetNumOfSpellSlots2 = (nint)addr, -0x5 },
                });

                FindMultipleCallsInFunction(Patterns32.ConvertPxRigidToMapEntity, new Dictionary<Action<long>, int>
                {
                    { addr => Offsets.Functions.ConvertPxRigidToMapEntity = (nint)addr, 0 },
                    { addr => Offsets.Functions.ConvertMapEntityToGameId = (nint)addr, 0x17 },
                });

                FindMultipleCallsInFunction(Patterns32.BonfireWarp, new Dictionary<Action<long>, int>
                {
                    { addr => Offsets.Functions.WarpPrep = (nint)addr, 0x7 },
                    { addr => Offsets.Functions.BonfireWarp = (nint)addr, 0x1F },
                });

                FindMultipleCallsInFunction(Patterns32.DisableNavimesh, new Dictionary<Action<long>, int>
                {
                    { addr => Offsets.Functions.GetNavimeshLoc = (nint)addr, -0xE },
                    { addr => Offsets.Functions.DisableNavimesh = (nint)addr, 0xB },
                });
                
                Offsets.Patches.NegativeLevel = (IntPtr)Offsets.Functions.LevelUp + 0x31;
                Offsets.Patches.SoulMemWrite1 = FindAddressByPattern(Patterns32.SoulMemWrite);
                Offsets.Patches.SoulMemWrite2 = Offsets.Patches.SoulMemWrite1 + 0x4A;
                
                
                TryPatternWithFallback("SetDepthStencilSurface",
                    Patterns32.SetDepthStencilSurface,
                    addr => Offsets.Functions.SetDepthStencilSurface = addr, saved);
                
                using (var writer = new StreamWriter(savePath))
                {
                    foreach (var pair in saved)
                        writer.WriteLine($"{pair.Key}={pair.Value:X}");
                }
            
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
                        uint absoluteAddr = memoryService.Read<uint>(IntPtr.Add(instructionAddress, pattern.OffsetLocation));
                        addresses[i] = (IntPtr)absoluteAddr;
                        break;
                    }
                    default:
                    {
                        int offset = memoryService.Read<int>(IntPtr.Add(instructionAddress, pattern.OffsetLocation));
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

            IntPtr currentAddress = memoryService.BaseAddress;
            IntPtr endAddress = IntPtr.Add(currentAddress, 0x3200000);

            List<IntPtr> addresses = new List<IntPtr>();

            while (currentAddress.ToInt64() < endAddress.ToInt64())
            {
                int bytesRemaining = (int)(endAddress.ToInt64() - currentAddress.ToInt64());
                int bytesToRead = Math.Min(bytesRemaining, buffer.Length);

                if (bytesToRead < pattern.Length)
                    break;

                buffer = memoryService.ReadBytes(currentAddress, bytesToRead);

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
        
                int callOffset = memoryService.Read<int>(IntPtr.Add(callInstructionAddr, 1));
                var callTarget = IntPtr.Add(callInstructionAddr, callOffset + 5);
        
                mapping.Key(callTarget.ToInt64());
            }
        }
    }
}