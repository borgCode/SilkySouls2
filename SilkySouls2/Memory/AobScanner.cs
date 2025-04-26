﻿using System;
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
            // TryPatternWithFallback("NoLogo", Patterns.NoLogo, addr => Offsets.Patches.NoLogo = addr, saved);
            // TryPatternWithFallback("RepeatAct", Patterns.RepeatAct, addr => Offsets.Patches.RepeatAct = addr, saved);
            // TryPatternWithFallback("GameSpeed", Patterns.GameSpeed, addr => Offsets.Patches.GameSpeed = addr, saved);
            // TryPatternWithFallback("InfiniteDurability", Patterns.InfiniteDurability,
            //     addr => Offsets.Patches.InfiniteDurability = addr, saved);
            // TryPatternWithFallback("PlayerSoundView", Patterns.PlayerSoundView,
            //     addr => Offsets.Patches.PlayerSoundView = addr, saved);
            // TryPatternWithFallback("DebugFont", Patterns.DebugFont, addr => Offsets.Patches.DebugFont = addr, saved);
            // TryPatternWithFallback("NoRoll", Patterns.NoRoll, addr => Offsets.Patches.NoRoll = addr, saved);
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
            TryPatternWithFallback("HpWrite", Patterns.HpWrite,
                addr => Offsets.Hooks.HpWrite = addr.ToInt64(), saved);
            Offsets.Hooks.OneShot = Offsets.Hooks.HpWrite - 0xB;
            
            TryPatternWithFallback("WarpCoordWrite", Patterns.WarpCoordWrite,
                addr => Offsets.Hooks.WarpCoordWrite = addr.ToInt64(), saved);
            // TryPatternWithFallback("NoClipKeyboard", Patterns.NoClipKeyboard,
            //     addr => Offsets.Hooks.NoClipKeyboard = addr.ToInt64(), saved);
            // TryPatternWithFallback("NoClipUpdateCoords", Patterns.NoClipUpdateCoords,
            //     addr => Offsets.Hooks.NoClipUpdateCoords = addr.ToInt64(), saved);
            // TryPatternWithFallback("CameraUpLimit", Patterns.CameraUpLimit,
            //     addr => Offsets.Hooks.CameraUpLimit = addr.ToInt64(), saved);
            // TryPatternWithFallback("ItemLotBase", Patterns.ItemLotBase,
            //     addr => Offsets.Hooks.ItemLotBase = addr.ToInt64(), saved);
            //
            // var triggers = FindAddressesByPattern(Patterns.NoClipTriggers, 2);
            // if (triggers[0] == IntPtr.Zero && saved.TryGetValue("NoClipTriggers", out var value))
            // {
            //     Offsets.Hooks.NoClipTriggers = value;
            //     Offsets.Hooks.NoClipTriggers2 = saved["NoClipTriggers2"];
            // }
            // else if (triggers[0] != IntPtr.Zero)
            // {
            //     Offsets.Hooks.NoClipTriggers = triggers[0].ToInt64();
            //     Offsets.Hooks.NoClipTriggers2 = triggers[1].ToInt64();
            //     saved["NoClipTriggers"] = triggers[0].ToInt64();
            //     saved["NoClipTriggers2"] = triggers[1].ToInt64();
            // }
            //
            using (var writer = new StreamWriter(savePath))
            {
                foreach (var pair in saved)
                    writer.WriteLine($"{pair.Key}={pair.Value:X}");
            }
            //
            Offsets.Funcs.WarpPrep = FindAddressByPattern(Patterns.WarpPrep).ToInt64();
            Offsets.Funcs.BonfireWarp = FindAddressByPattern(Patterns.BonfireWarp).ToInt64();
            // Offsets.Funcs.ItemSpawn = FindAddressByPattern(Patterns.ItemSpawnFunc).ToInt64();
            // Offsets.Funcs.BreakAllObjects = FindAddressByPattern(Patterns.BreakAllObjects).ToInt64();
            // Offsets.Funcs.RestoreAllObjects = FindAddressByPattern(Patterns.RestoreAllObjects).ToInt64();
            Offsets.Funcs.SetEvent = FindAddressByPattern(Patterns.SetEvent).ToInt64();
            // Offsets.Funcs.Travel = FindAddressByPattern(Patterns.TravelFunc).ToInt64();
            // Offsets.Funcs.LevelUp = Offsets.Funcs.Travel - 0x720;
            // Offsets.Funcs.ReinforceWeapon = Offsets.Funcs.Travel - 0x1620;
            // Offsets.Funcs.InfuseWeapon = Offsets.Funcs.Travel - 0x1CB0;
            // Offsets.Funcs.Repair = Offsets.Funcs.Travel - 0x14C0;
            // Offsets.Funcs.Attunement = Offsets.Funcs.Travel - 0xB10;
            // Offsets.Funcs.AllotEstus = Offsets.Funcs.Travel - 0x2010;
            // Offsets.Funcs.Transpose = Offsets.Funcs.Travel - 0x1A10;
            // Offsets.Funcs.RegularShop = Offsets.Funcs.Travel - 0x1B50;
            // Offsets.Funcs.CombineMenuFlagAndEventFlag =
            //     FindAddressByPattern(Patterns.CombineMenuFlagAndEventFlag).ToInt64();


#if DEBUG
            Console.WriteLine($"GameManagerImp.Base: 0x{Offsets.GameManagerImp.Base.ToInt64():X}");
            // Console.WriteLine($"GameMan.Base: 0x{Offsets.GameMan.Base.ToInt64():X}");
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
            // Console.WriteLine($"Patches.NoLogo: 0x{Offsets.Patches.NoLogo.ToInt64():X}");
            // Console.WriteLine($"Patches.RepeatAct: 0x{Offsets.Patches.RepeatAct.ToInt64():X}");
            // Console.WriteLine($"Patches.GameSpeed: 0x{Offsets.Patches.GameSpeed.ToInt64():X}");
            // Console.WriteLine($"Patches.InfiniteDurability: 0x{Offsets.Patches.InfiniteDurability.ToInt64():X}");
            // Console.WriteLine($"Patches.PlayerSoundView: 0x{Offsets.Patches.PlayerSoundView.ToInt64():X}");
            // Console.WriteLine($"Patches.DebugFont: 0x{Offsets.Patches.DebugFont.ToInt64():X}");
            // Console.WriteLine($"Patches.NoRoll: 0x{Offsets.Patches.NoRoll.ToInt64():X}");
            // Console.WriteLine($"Patches.TargetingView: 0x{Offsets.Patches.DbgDrawFlag.ToInt64():X}");
            // Console.WriteLine($"Patches.FreeCam: 0x{Offsets.Patches.FreeCam.ToInt64():X}");
            //
           
            Console.WriteLine($"Hooks.SetAreaVariable: 0x{Offsets.Hooks.SetAreaVariable:X}");
            Console.WriteLine($"Hooks.CompareEventRandValue: 0x{Offsets.Hooks.CompareEventRandValue:X}");
            Console.WriteLine($"Hooks.HpWrite: 0x{Offsets.Hooks.HpWrite:X}");
            Console.WriteLine($"Hooks.OneShot: 0x{Offsets.Hooks.OneShot:X}");
            Console.WriteLine($"Hooks.WarpCoordWrite: 0x{Offsets.Hooks.WarpCoordWrite:X}");
            // Console.WriteLine($"Hooks.AddSubGoal: 0x{Offsets.Hooks.AddSubGoal:X}");
            // Console.WriteLine($"Hooks.InAirTimer: 0x{Offsets.Hooks.InAirTimer:X}");
            // Console.WriteLine($"Hooks.NoClipKeyboard: 0x{Offsets.Hooks.NoClipKeyboard:X}");
            // Console.WriteLine($"Hooks.NoClipTriggers: 0x{Offsets.Hooks.NoClipTriggers:X}");
            // Console.WriteLine($"Hooks.NoClipTriggers2: 0x{Offsets.Hooks.NoClipTriggers2:X}");
            // Console.WriteLine($"Hooks.NoClipUpdateCoords: 0x{Offsets.Hooks.NoClipUpdateCoords:X}");
            // Console.WriteLine($"Hooks.CameraUpLimit: 0x{Offsets.Hooks.CameraUpLimit:X}");
            // Console.WriteLine($"Hooks.ItemLotBase: 0x{Offsets.Hooks.ItemLotBase:X}");
            //
            Console.WriteLine($"Funcs.WarpPrep: 0x{Offsets.Funcs.WarpPrep:X}");
            Console.WriteLine($"Funcs.BonfireWarp: 0x{Offsets.Funcs.BonfireWarp:X}");
            // Console.WriteLine($"Funcs.ItemSpawn: 0x{Offsets.Funcs.ItemSpawn:X}");
            Console.WriteLine($"Funcs.SetEvent: 0x{Offsets.Funcs.SetEvent:X}");
            // Console.WriteLine($"Funcs.Travel: 0x{Offsets.Funcs.Travel:X}");
            // Console.WriteLine($"Funcs.ReinforceWeapon: 0x{Offsets.Funcs.ReinforceWeapon:X}");
            // Console.WriteLine($"Funcs.AllotEstus: 0x{Offsets.Funcs.AllotEstus:X}");
            // Console.WriteLine($"Funcs.Attunement: 0x{Offsets.Funcs.Attunement:X}");
            // Console.WriteLine($"Funcs.RegularShop: 0x{Offsets.Funcs.RegularShop:X}");
            // Console.WriteLine($"Funcs.Transpose: 0x{Offsets.Funcs.Transpose:X}");
            // Console.WriteLine($"Funcs.CombineMenuFlagAndEventFlag: 0x{Offsets.Funcs.CombineMenuFlagAndEventFlag:X}");
            // Console.WriteLine($"Funcs.BreakAllObjects: 0x{Offsets.Funcs.BreakAllObjects:X}");
            // Console.WriteLine($"Funcs.RestoreAllObjects: 0x{Offsets.Funcs.RestoreAllObjects:X}");
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