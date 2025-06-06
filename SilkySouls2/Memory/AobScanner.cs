using System;
using System.Collections.Generic;
using System.IO;
using static SilkySouls2.Memory.RipType;

namespace SilkySouls2.Memory
{
    public class AoBScanner
    {
        private readonly MemoryIo _memoryIo;

        public AoBScanner(MemoryIo memoryIo)
        {
            _memoryIo = memoryIo;
        }

        public void Scan()
        {
            string appData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "SilkySouls2");
            Directory.CreateDirectory(appData);
            string savePath = Path.Combine(appData, "backup_addresses.txt");

            Dictionary<string, long> saved = new Dictionary<string, long>();
            if (File.Exists(savePath))
            {
                foreach (string line in File.ReadAllLines(savePath))
                {
                    string[] parts = line.Split('=');
                    saved[parts[0]] = Convert.ToInt64(parts[1], 16);
                }
            }

            //
            Offsets.GameManagerImp.Base = FindAddressByPattern(Patterns.GameManagerImp);
            Offsets.HkHardwareInfo.Base = FindAddressByPattern(Patterns.HkpPtrEntity);
            // Offsets.GameMan.Base = FindAddressByPattern(Patterns.GameMan);
            // Offsets.LuaEventMan.Base = FindAddressByPattern(Patterns.LuaEventMan);
            // Offsets.SoloParamRepo.Base = FindAddressByPattern(Patterns.SoloParamRepo);
            // Offsets.AiTargetingFlags.Base = FindAddressByPattern(Patterns.AiTargetingFlags);
            // Offsets.EventFlagMan.Base = FindAddressByPattern(Patterns.EventFlagMan);
            // Offsets.MenuMan.Base = FindAddressByPattern(Patterns.MenuMan);
            // Offsets.DebugFlags.Base = FindAddressByPattern(Patterns.DebugFlags);
            // Offsets.DebugEvent.Base = FindAddressByPattern(Patterns.DebugEvent);
            // Offsets.MapItemMan.Base = FindAddressByPattern(Patterns.MapItemMan);
            // Offsets.GameDataMan.Base = FindAddressByPattern(Patterns.GameDataMan);
            // Offsets.DamageMan.Base = FindAddressByPattern(Patterns.DamageMan);
            // Offsets.FieldArea.Base = FindAddressByPattern(Patterns.FieldArea);
            // Offsets.GroupMask.Base = FindAddressByPattern(Patterns.GroupMask);
            // Offsets.UserInputManager.Base = FindAddressByPattern(Patterns.UserInputManager);
            // Offsets.HitIns.Base = FindAddressByPattern(Patterns.HitIns);
            // Offsets.SprjFlipper.Base = FindAddressByPattern(Patterns.SprjFlipper);
            // Offsets.WorldObjMan.Base = FindAddressByPattern(Patterns.WorldObjManImpl);
            //
            //
            TryPatternWithFallback("InfiniteStam", Patterns.InfiniteStam, addr => Offsets.Patches.InfiniteStam = addr, saved);
            TryPatternWithFallback("InfiniteGoods", Patterns.InfiniteGoods, addr => Offsets.Patches.InfiniteGoods = addr, saved);
            TryPatternWithFallback("HideChrModels", Patterns.HideChrModels, addr => Offsets.Patches.HideChrModels = addr, saved);
            TryPatternWithFallback("HideMap", Patterns.HideMap, addr => Offsets.Patches.HideMap = addr, saved);
            TryPatternWithFallback("InfiniteCasts", Patterns.InfiniteCasts,
                addr => Offsets.Patches.InfiniteCasts = addr, saved);
            TryPatternWithFallback("InfiniteDurability", Patterns.InfiniteDurability,
                addr => Offsets.Patches.InfiniteDurability = addr, saved);
            TryPatternWithFallback("DropRate", Patterns.DropRate,
                addr => Offsets.Patches.DropRate = addr, saved);
            TryPatternWithFallback("Silent", Patterns.Silent,
                addr => Offsets.Patches.Silent = addr, saved);
            TryPatternWithFallback("Hidden", Patterns.Hidden,
                addr => Offsets.Patches.Hidden = addr, saved);
            TryPatternWithFallback("DisableAi", Patterns.DisableAi, addr => Offsets.Patches.DisableAi = addr, saved);
            // TryPatternWithFallback("TargetingView", Patterns.DbgDrawFlag, addr => Offsets.Patches.DbgDrawFlag = addr,
            //     saved);
            // TryPatternWithFallback("FreeCam", Patterns.FreeCamPatch, addr => Offsets.Patches.FreeCam = addr, saved);
            //
            // TryPatternWithFallback("OverrideGeneratorStartPositionRandom",
            //     Patterns.OverrideGeneratorStartPositionRandom,
            //     addr => Offsets.Hooks.OverrideGeneratorStartPositionRandom = addr.ToInt64(), saved);
            
            TryPatternWithFallback("SetAreaVariable",
                Patterns.SetAreaVariable,
                addr => Offsets.Hooks.SetAreaVariable = addr.ToInt64(), saved);
            
            TryPatternWithFallback("CompareEventRandValue",
                Patterns.CompareEventRandValue,
                addr => Offsets.Hooks.CompareEventRandValue = addr.ToInt64(), saved);
            TryPatternWithFallback("EzStateSetEvent",
                Patterns.EzStateSetEvent,
                addr => Offsets.Hooks.EzStateSetEvent = addr.ToInt64(), saved);
            TryPatternWithFallback("HpWrite", Patterns.HpWrite,
                addr => Offsets.Hooks.HpWrite = addr.ToInt64(), saved);
            Offsets.Hooks.OneShot = Offsets.Hooks.HpWrite - 0xB;
            
            TryPatternWithFallback("WarpCoordWrite", Patterns.WarpCoordWrite,
                addr => Offsets.Hooks.WarpCoordWrite = addr.ToInt64(), saved);
            TryPatternWithFallback("LockedTarget", Patterns.LockedTarget,
                addr => Offsets.Hooks.LockedTarget = addr.ToInt64(), saved);
            TryPatternWithFallback("CreditSkip", Patterns.CreditSkip,
                addr => Offsets.Hooks.CreditSkip = addr.ToInt64(), saved);
            TryPatternWithFallback("NumOfDrops", Patterns.NumOfDrops,
                addr => Offsets.Hooks.NumOfDrops = addr.ToInt64(), saved);
            TryPatternWithFallback("KillboxFlagSet", Patterns.KillboxFlagSet,
                addr => Offsets.Hooks.KillboxFlagSet = addr.ToInt64(), saved);
            
            TryPatternWithFallback("DamageControl", Patterns.DamageControl,
                addr => Offsets.Hooks.DamageControl = addr.ToInt64(), saved);  
            TryPatternWithFallback("InAirTimer", Patterns.InAirTimer,
                addr => Offsets.Hooks.InAirTimer = addr.ToInt64(), saved);
            TryPatternWithFallback("TriggersAndSpace", Patterns.TriggersAndSpace,
                addr => Offsets.Hooks.TriggersAndSpace = addr.ToInt64(), saved);
            TryPatternWithFallback("Ctrl", Patterns.Ctrl,
                addr => Offsets.Hooks.Ctrl = addr.ToInt64(), saved);
            TryPatternWithFallback("NoClipUpdateCoords", Patterns.NoClipUpdateCoords,
                addr => Offsets.Hooks.NoClipUpdateCoords = addr.ToInt64(), saved);
            TryPatternWithFallback("FastQuitout", Patterns.FastQuitout,
                addr => Offsets.Hooks.FastQuitout = addr.ToInt64(), saved);
            TryPatternWithFallback("InfinitePoise", Patterns.InfinitePoise,
                addr => Offsets.Hooks.InfinitePoise = addr.ToInt64(), saved);
            
            var setCurrectActLocs = FindAddressesByPattern(Patterns.SetCurrectAct, 2);
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
            //
            Offsets.Funcs.WarpPrep = FindAddressByPattern(Patterns.WarpPrep).ToInt64();
            Offsets.Funcs.BonfireWarp = FindAddressByPattern(Patterns.BonfireWarp).ToInt64();
            Offsets.Funcs.RestoreSpellcasts = FindAddressByPattern(Patterns.RestoreSpellcasts).ToInt64();
            Offsets.Funcs.ParamLookUp = FindAddressByPattern(Patterns.ParamLookUp).ToInt64();
            Offsets.Funcs.SetEvent = FindAddressByPattern(Patterns.SetEvent).ToInt64();
            Offsets.Funcs.GiveSouls = FindAddressByPattern(Patterns.GiveSouls).ToInt64();
            Offsets.Funcs.LevelLookUp = FindAddressByPattern(Patterns.LevelLookUp).ToInt64();
            Offsets.Funcs.LevelUp = FindAddressByPattern(Patterns.LevelUp).ToInt64();
            Offsets.Patches.NegativeLevel = (IntPtr)Offsets.Funcs.LevelUp + 0x38;
            
            Offsets.Funcs.CurrentItemQuantityCheck = FindAddressByPattern(Patterns.CurrentItemQuantityCheck).ToInt64();
            Offsets.Funcs.ItemGive = FindAddressByPattern(Patterns.ItemGive).ToInt64();
            Offsets.Funcs.BuildItemDialog = FindAddressByPattern(Patterns.BuildItemDialog).ToInt64();
            Offsets.Funcs.ShowItemDialog = FindAddressByPattern(Patterns.ShowItemDialog).ToInt64();
            
            TryPatternWithFallback("SetRenderTargets",
                Patterns.SetRenderTargetsWrapper,
                addr => Offsets.Funcs.SetRenderTargets = addr.ToInt64(), saved);
            TryPatternWithFallback("CreateSoundEvent",
                Patterns.CreateSoundEvent,
                addr => Offsets.Funcs.CreateSoundEvent = addr.ToInt64(), saved);
            TryPatternWithFallback("ChrCtrlUpdate",
                Patterns.ChrCtrlUpdate,
                addr => Offsets.Funcs.ChrCtrlUpdate = addr.ToInt64(), saved);
    


#if DEBUG
            Console.WriteLine($"GameManagerImp.Base: 0x{Offsets.GameManagerImp.Base.ToInt64():X}");
            Console.WriteLine($"HkpPtrEntity.Base: 0x{Offsets.HkHardwareInfo.Base.ToInt64():X}");
            // Console.WriteLine($"LuaEventMan.Base: 0x{Offsets.LuaEventMan.Base.ToInt64():X}");
            // Console.WriteLine($"EventFlagMan.Base: 0x{Offsets.EventFlagMan.Base.ToInt64():X}");
            // Console.WriteLine($"SoloParamRepo.Base: 0x{Offsets.SoloParamRepo.Base.ToInt64():X}");
            // Console.WriteLine($"AiTargetingFlags.Base: 0x{Offsets.AiTargetingFlags.Base.ToInt64():X}");
            // Console.WriteLine($"MenuMan.Base: 0x{Offsets.MenuMan.Base.ToInt64():X}");
            // Console.WriteLine($"DebugFlags.Base: 0x{Offsets.DebugFlags.Base.ToInt64():X}");
            // Console.WriteLine($"DebugEvent.Base: 0x{Offsets.DebugEvent.Base.ToInt64():X}");
            // Console.WriteLine($"MapItemMan.Base: 0x{Offsets.MapItemMan.Base.ToInt64():X}");
            // Console.WriteLine($"GameDataMan.Base: 0x{Offsets.GameDataMan.Base.ToInt64():X}");
            // Console.WriteLine($"DamageMan.Base: 0x{Offsets.DamageMan.Base.ToInt64():X}");
            // Console.WriteLine($"FieldArea.Base: 0x{Offsets.FieldArea.Base.ToInt64():X}");
            // Console.WriteLine($"GroupMask.Base: 0x{Offsets.GroupMask.Base.ToInt64():X}");
            // Console.WriteLine($"UserInputManager.Base: 0x{Offsets.UserInputManager.Base.ToInt64():X}");
            // Console.WriteLine($"Mesh.Base: 0x{Offsets.HitIns.Base.ToInt64():X}");
            // Console.WriteLine($"SprjFlipper.Base: 0x{Offsets.SprjFlipper.Base.ToInt64():X}");
            // Console.WriteLine($"WorldObjMan.Base: 0x{Offsets.WorldObjMan.Base.ToInt64():X}");
            //
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
            
            // Console.WriteLine($"Patches.TargetingView: 0x{Offsets.Patches.DbgDrawFlag.ToInt64():X}");
            // Console.WriteLine($"Patches.FreeCam: 0x{Offsets.Patches.FreeCam.ToInt64():X}");
            //
           
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
            //
            Console.WriteLine($"Funcs.WarpPrep: 0x{Offsets.Funcs.WarpPrep:X}");
            Console.WriteLine($"Funcs.BonfireWarp: 0x{Offsets.Funcs.BonfireWarp:X}");
            Console.WriteLine($"Funcs.GiveSouls: 0x{Offsets.Funcs.GiveSouls:X}");
            Console.WriteLine($"Funcs.SetEvent: 0x{Offsets.Funcs.SetEvent:X}");
            Console.WriteLine($"Funcs.RestoreSpellcasts: 0x{Offsets.Funcs.RestoreSpellcasts:X}");
            Console.WriteLine($"Funcs.ParamLookUp: 0x{Offsets.Funcs.ParamLookUp:X}");
            Console.WriteLine($"Funcs.SetRenderTargets: 0x{Offsets.Funcs.SetRenderTargets:X}");
            Console.WriteLine($"Funcs.CreateSoundEvent: 0x{Offsets.Funcs.CreateSoundEvent:X}");
            Console.WriteLine($"Funcs.ChrCtrlUpdate: 0x{Offsets.Funcs.ChrCtrlUpdate:X}");
            Console.WriteLine($"Funcs.LevelLookUp: 0x{Offsets.Funcs.LevelLookUp:X}");
            Console.WriteLine($"Funcs.LevelUp: 0x{Offsets.Funcs.LevelUp:X}");
            Console.WriteLine($"Funcs.CurrentItemQuantityCheck: 0x{Offsets.Funcs.CurrentItemQuantityCheck:X}");
            Console.WriteLine($"Funcs.ItemGive: 0x{Offsets.Funcs.ItemGive:X}");
            Console.WriteLine($"Funcs.BuildItemDialog: 0x{Offsets.Funcs.BuildItemDialog:X}");
            Console.WriteLine($"Funcs.ShowItemDialog: 0x{Offsets.Funcs.ShowItemDialog:X}");
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

                switch (pattern.RipType)
                {
                    case None:
                        addresses[i] = instructionAddress;
                        break;
                    case Mov64:
                        int stdOffset = _memoryIo.ReadInt32(IntPtr.Add(instructionAddress, 3));
                        addresses[i] = IntPtr.Add(instructionAddress, stdOffset + 7);
                        break;
                    case Mov32:
                        int mov32Offset = _memoryIo.ReadInt32(IntPtr.Add(instructionAddress, 2));
                        addresses[i] = IntPtr.Add(instructionAddress, mov32Offset + 6);
                        break;
                    case Cmp:
                        int cmpOffset = _memoryIo.ReadInt32(IntPtr.Add(instructionAddress, 2));
                        addresses[i] = IntPtr.Add(instructionAddress, cmpOffset + 7);
                        break;
                    case QwordCmp:
                        int qwordCmpOffset = _memoryIo.ReadInt32(IntPtr.Add(instructionAddress, 3));
                        addresses[i] = IntPtr.Add(instructionAddress, qwordCmpOffset + 8);
                        break;
                    case Call:
                        int callOffset = _memoryIo.ReadInt32(IntPtr.Add(instructionAddress, 1));
                        addresses[i] = IntPtr.Add(instructionAddress, callOffset + 5);
                        break;
                    case MovzxByte:
                        int movzxOffset = _memoryIo.ReadInt32(IntPtr.Add(instructionAddress, 4));
                        addresses[i] = IntPtr.Add(instructionAddress, movzxOffset + 8);
                        break;
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
    }
}