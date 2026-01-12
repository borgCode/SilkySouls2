using System;
using System.Threading;
using System.Threading.Tasks;
using SilkySouls2.enums;
using SilkySouls2.GameIds;
using SilkySouls2.Interfaces;
using SilkySouls2.Memory;
using SilkySouls2.Models;
using SilkySouls2.Utilities;
using static SilkySouls2.Memory.Offsets;

namespace SilkySouls2.Services
{
    public class TravelService(IMemoryService memoryService, HookManager hookManager, IPlayerService playerService) : ITravelService
    {
        public void Warp(WarpLocation location, bool isRestOnWarpEnabled)
        {
            var actualWarp = Functions.BonfireWarp;
            var eventWarpEntity = memoryService.FollowPointers(GameManagerImp.Base, new[]
            {
                GameManagerImp.EventManager,
                GameManagerImp.EventManagerOffsets.WarpEventEntity
            }, true);

            IntPtr codeLoc;

            if (location.EventObjId == 0)
            {
                codeLoc = CodeCaveOffsets.Base + (int)CodeCaveOffsets.BonfireWarp.WarpCode;
                WriteBonfireWarpCode(codeLoc, actualWarp, eventWarpEntity, location);
            }
            else
            {
                codeLoc = CodeCaveOffsets.Base + (int)CodeCaveOffsets.EventWarp.Code;
                WriteEventWarpCode(codeLoc, actualWarp, eventWarpEntity, location);
            }

            memoryService.RunThread(codeLoc);
            if (isRestOnWarpEnabled) playerService.SetSpEffect(SpEffect.BonfireRest);
            if (location.HasCoordinates) PerformCoordWrite(location);
        }

        private void WriteBonfireWarpCode(IntPtr codeLoc, long actualWarp, IntPtr eventWarpEntity,
            WarpLocation location)
        {
            var bonfireIdLoc = CodeCaveOffsets.Base + (int)CodeCaveOffsets.BonfireWarp.BonfireId;
            memoryService.WriteInt32(bonfireIdLoc, location.BonfireId);

            if (PatchManager.IsScholar())
            {
                WriteScholarBonfireWarpCode(actualWarp, eventWarpEntity, codeLoc);
            }
            else
            {
                WriteVanillaBonfireWarpCode(actualWarp, eventWarpEntity, codeLoc);
            }
        }

        private void WriteScholarBonfireWarpCode(long actualWarp, IntPtr eventWarpEntity, IntPtr codeLoc)
        {
            var warpPrep = Functions.WarpPrep;
            var emptySpace = CodeCaveOffsets.Base + (int)CodeCaveOffsets.BonfireWarp.Output;
            var bonfireIdLoc = CodeCaveOffsets.Base + (int)CodeCaveOffsets.BonfireWarp.BonfireId;

            var warpBytes = AsmLoader.GetAsmBytes(AsmScript.BonfireWarp64);
            var bytes = BitConverter.GetBytes(eventWarpEntity.ToInt64());
            Array.Copy(bytes, 0, warpBytes, 0x1D + 2, 8);

            AsmHelper.WriteRelativeOffsets(warpBytes, [
                (codeLoc.ToInt64() + 0x4, emptySpace.ToInt64(), 7, 0x4 + 3),
                (codeLoc.ToInt64() + 0xB, bonfireIdLoc.ToInt64(), 7, 0xB + 3),
                (codeLoc.ToInt64() + 0x18, warpPrep, 5, 0x18 + 1),
                (codeLoc.ToInt64() + 0x27, emptySpace.ToInt64(), 7, 0x27 + 3),
                (codeLoc.ToInt64() + 0x2E, actualWarp, 5, 0x2E + 1)
            ]);

            memoryService.WriteBytes(codeLoc, warpBytes);
        }

        private void WriteVanillaBonfireWarpCode(long actualWarp, IntPtr eventWarpEntity, IntPtr codeLoc)
        {
            var warpPrep = Functions.WarpPrep;
            var emptySpace = CodeCaveOffsets.Base + (int)CodeCaveOffsets.BonfireWarp.Output;
            var bonfireIdLoc = CodeCaveOffsets.Base + (int)CodeCaveOffsets.BonfireWarp.BonfireId;

            var warpBytes = AsmLoader.GetAsmBytes(AsmScript.BonfireWarp32);
            AsmHelper.WriteAbsoluteAddresses32(warpBytes, new[]
            {
                (bonfireIdLoc.ToInt64(), 0x5 + 2),
                (emptySpace.ToInt64(), 0xB + 2),
                (warpPrep, 0x12 + 1),
                (emptySpace.ToInt64(), 0x1C + 2),
                (eventWarpEntity.ToInt64(), 0x23 + 1),
                (actualWarp, 0x28 + 1)
            });

            memoryService.WriteBytes(codeLoc, warpBytes);
        }

        private void WriteEventWarpCode(IntPtr codeLoc, long actualWarp, IntPtr eventWarpEntity, WarpLocation location)
        {
            var paramsLoc = CodeCaveOffsets.Base + (int)CodeCaveOffsets.EventWarp.Params;
            memoryService.Write(paramsLoc, 4);
            memoryService.Write(paramsLoc + 0x4, 5);
            memoryService.Write(paramsLoc + 0x8, location.BonfireId);
            memoryService.Write(paramsLoc + 0xC, -1);
            memoryService.Write(paramsLoc + 0x18, location.EventObjId);

            if (PatchManager.IsScholar())
            {
                WriteScholarEventWarpCode(codeLoc, actualWarp, eventWarpEntity, paramsLoc);
            }
            else
            {
                WriteVanillaEventWarpCode(codeLoc, actualWarp, eventWarpEntity, paramsLoc);
            }
        }

        private void WriteScholarEventWarpCode(IntPtr codeLoc, long actualWarp, IntPtr eventWarpEntity,
            IntPtr paramsLoc)
        {
            var warpBytes = AsmLoader.GetAsmBytes(AsmScript.EventWarp64);
            var bytes = BitConverter.GetBytes(eventWarpEntity.ToInt64());
            Array.Copy(bytes, 0, warpBytes, 0x7 + 2, 8);
            AsmHelper.WriteRelativeOffsets(warpBytes, [
                (codeLoc.ToInt64() + 0x11, paramsLoc.ToInt64(), 7, 0x11 + 3),
                (codeLoc.ToInt64() + 0x18, actualWarp, 5, 0x18 + 1)
            ]);
            memoryService.WriteBytes(codeLoc, warpBytes);
        }

        private void WriteVanillaEventWarpCode(IntPtr codeLoc, long actualWarp, IntPtr eventWarpEntity,
            IntPtr paramsLoc)
        {
            var warpBytes = AsmLoader.GetAsmBytes(AsmScript.EventWarp32);
            AsmHelper.WriteAbsoluteAddresses32(warpBytes, [
                (eventWarpEntity.ToInt64(), 1),
                (paramsLoc.ToInt64(), 0x5 + 2),
                (actualWarp, 0xC + 1)
            ]);

            memoryService.WriteBytes(codeLoc, warpBytes);
        }

        private void PerformCoordWrite(WarpLocation location)
        {
            var hook = Hooks.WarpCoordWrite;
            var coordsLoc = CodeCaveOffsets.Base + (int)CodeCaveOffsets.BonfireWarp.Coords;
            var code = CodeCaveOffsets.Base + (int)CodeCaveOffsets.BonfireWarp.CoordWrite;


            byte[] allCoordinateBytes = new byte[16 * sizeof(float)];

            for (int i = 0; i < 16; i++)
            {
                byte[] floatBytes = BitConverter.GetBytes(location.Coordinates[i]);
                Buffer.BlockCopy(floatBytes, 0, allCoordinateBytes, i * sizeof(float), sizeof(float));
            }

            memoryService.WriteBytes(coordsLoc, allCoordinateBytes);

            if (PatchManager.IsScholar())
            {
                WriteScholarCoordWriteCode(hook, coordsLoc, code);
            }
            else
            {
                WriteVanillaCoordWriteCode(hook, coordsLoc, code);
            }

            {
                int start = Environment.TickCount;
                while (!IsLoadingScreen() && Environment.TickCount - start < 10000)
                    Thread.Sleep(50);
            }

            hookManager.InstallHook(code.ToInt64(), hook,
                PatchManager.IsScholar()
                    ? [0x0F, 0x5C, 0xC2, 0x0F, 0x29, 0x47, 0x50]
                    : [0x0F, 0x5C, 0xC1, 0x0F, 0x29, 0x46, 0x40]);

            {
                int start = Environment.TickCount;
                while (IsLoadingScreen() && Environment.TickCount - start < 10000)
                    Thread.Sleep(50);
            }
            Task.Delay(200).Wait();
            hookManager.UninstallHook(code.ToInt64());
        }

        
        private void WriteScholarCoordWriteCode(long hook, IntPtr coordsLoc, IntPtr code)
        {
            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.WarpCoordWrite64);
            var bytes = BitConverter.GetBytes(HkHardwareInfo.Base.ToInt64());
            Array.Copy(bytes, 0, codeBytes, 0x8 + 2, 8);

            AsmHelper.WriteRelativeOffsets(codeBytes, [
                (code.ToInt64() + 0x33, coordsLoc.ToInt64(), 8, 0x33 + 4),
                (code.ToInt64() + 0x40, coordsLoc.ToInt64() + 0x10, 8, 0x40 + 4),
                (code.ToInt64() + 0x4D, coordsLoc.ToInt64() + 0x20, 8, 0x4D + 4),
                (code.ToInt64() + 0x5A, coordsLoc.ToInt64() + 0x30, 8, 0x5A + 4)
            ]);
            bytes = AsmHelper.GetJmpOriginOffsetBytes(hook, 7, code + 0x7C);
            Array.Copy(bytes, 0, codeBytes, 0x77 + 1, 4);
            memoryService.WriteBytes(code, codeBytes);
        }
        
        private void WriteVanillaCoordWriteCode(long hook, IntPtr coordsLoc, IntPtr code)
        {
            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.WarpCoordWrite32);
            var jmpBytes = AsmHelper.GetJmpOriginOffsetBytes(hook, 7, code + 0x6C);
            Array.Copy(jmpBytes, 0, codeBytes, 0x66 + 1, 4);
            AsmHelper.WriteAbsoluteAddresses32(codeBytes, [
                (GameManagerImp.Base.ToInt64(), 0x8 + 1),
                (coordsLoc.ToInt64(), 0x32 + 3),
                (coordsLoc.ToInt64() + 0x10, 0x3D + 3),
                (coordsLoc.ToInt64() + 0x20, 0x48 + 3),
                (coordsLoc.ToInt64() + 0x30, 0x53 + 3)
            ]);
            memoryService.WriteBytes(code, codeBytes);
        }

        public bool IsLoadingScreen() =>
            memoryService.Read<byte>((IntPtr)memoryService.ReadInt64(GameManagerImp.Base) +
                                    GameManagerImp.LoadingFlag) == 1;

        public void UnlockAllBonfires()
        {
            var func = Functions.UnlockBonfire;
            var bonfireManager = memoryService.FollowPointers(GameManagerImp.Base, [
                GameManagerImp.EventManager,
                GameManagerImp.EventManagerOffsets.EventBonfireManager
            ], true);

            if (PatchManager.IsScholar())
            {
                var bytes = AsmLoader.GetAsmBytes(AsmScript.UnlockAllBonfires64);
                AsmHelper.WriteAbsoluteAddresses64(bytes, [
                    (bonfireManager.ToInt64(), 2),
                    (func, 0xA + 2)
                ]);
                memoryService.AllocateAndExecute(bytes);
            }
            else
            {
                var bytes = AsmLoader.GetAsmBytes(AsmScript.UnlockAllBonfires32);
                AsmHelper.WriteAbsoluteAddresses32(bytes, [
                    (bonfireManager.ToInt64(), 1),
                    (func, 0x5 + 1)
                ]);
                memoryService.AllocateAndExecute(bytes);
            }
        }
    }
}