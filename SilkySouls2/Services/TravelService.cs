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

            nint codeLoc;

            if (location.EventObjId == 0)
            {
                codeLoc = CustomCodeOffsets.Base + (int)CustomCodeOffsets.BonfireWarp.WarpCode;
                WriteBonfireWarpCode(codeLoc, actualWarp, eventWarpEntity, location);
            }
            else
            {
                codeLoc = CustomCodeOffsets.Base + (int)CustomCodeOffsets.EventWarp.Code;
                WriteEventWarpCode(codeLoc, actualWarp, eventWarpEntity, location);
            }

            memoryService.RunThread(codeLoc);
            if (isRestOnWarpEnabled) playerService.SetSpEffect(SpEffect.BonfireRest);
            if (location.HasCoordinates) PerformCoordWrite(location);
        }

        private void WriteBonfireWarpCode(nint codeLoc, nint actualWarp, nint eventWarpEntity,
            WarpLocation location)
        {
            var bonfireIdLoc = CustomCodeOffsets.Base + (int)CustomCodeOffsets.BonfireWarp.BonfireId;
            memoryService.Write(bonfireIdLoc, location.BonfireId);

            if (PatchManager.IsScholar())
            {
                WriteScholarBonfireWarpCode(actualWarp, eventWarpEntity, codeLoc);
            }
            else
            {
                WriteVanillaBonfireWarpCode(actualWarp, eventWarpEntity, codeLoc);
            }
        }

        private void WriteScholarBonfireWarpCode(nint actualWarp, nint eventWarpEntity, nint codeLoc)
        {
            var warpPrep = Functions.WarpPrep;
            var emptySpace = CustomCodeOffsets.Base + (int)CustomCodeOffsets.BonfireWarp.Output;
            var bonfireIdLoc = CustomCodeOffsets.Base + (int)CustomCodeOffsets.BonfireWarp.BonfireId;

            var warpBytes = AsmLoader.GetAsmBytes(AsmScript.BonfireWarp64);
            AsmHelper.WriteAbsoluteAddress64(warpBytes, eventWarpEntity, 0x1D + 2);

            AsmHelper.WriteRelativeOffsets(warpBytes, [
                (codeLoc + 0x4, emptySpace, 7, 0x4 + 3),
                (codeLoc + 0xB, bonfireIdLoc, 7, 0xB + 3),
                (codeLoc + 0x18, warpPrep, 5, 0x18 + 1),
                (codeLoc + 0x27, emptySpace, 7, 0x27 + 3),
                (codeLoc + 0x2E, actualWarp, 5, 0x2E + 1)
            ]);

            memoryService.WriteBytes(codeLoc, warpBytes);
        }

        private void WriteVanillaBonfireWarpCode(nint actualWarp, nint eventWarpEntity, nint codeLoc)
        {
            var warpPrep = Functions.WarpPrep;
            var emptySpace = CustomCodeOffsets.Base + (int)CustomCodeOffsets.BonfireWarp.Output;
            var bonfireIdLoc = CustomCodeOffsets.Base + (int)CustomCodeOffsets.BonfireWarp.BonfireId;

            var warpBytes = AsmLoader.GetAsmBytes(AsmScript.BonfireWarp32);
            AsmHelper.WriteAbsoluteAddresses32(warpBytes, [
                (bonfireIdLoc, 0x5 + 2),
                (emptySpace, 0xB + 2),
                (warpPrep, 0x12 + 1),
                (emptySpace, 0x1C + 2),
                (eventWarpEntity, 0x23 + 1),
                (actualWarp, 0x28 + 1)
            ]);

            memoryService.WriteBytes(codeLoc, warpBytes);
        }

        private void WriteEventWarpCode(nint codeLoc, nint actualWarp, nint eventWarpEntity, WarpLocation location)
        {
            var paramsLoc = CustomCodeOffsets.Base + (int)CustomCodeOffsets.EventWarp.Params;
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

        private void WriteScholarEventWarpCode(nint codeLoc, nint actualWarp, nint eventWarpEntity,
            nint paramsLoc)
        {
            var warpBytes = AsmLoader.GetAsmBytes(AsmScript.EventWarp64);
            AsmHelper.WriteAbsoluteAddress64(warpBytes, eventWarpEntity, 0x7 + 2);
            AsmHelper.WriteRelativeOffsets(warpBytes, [
                (codeLoc + 0x11, paramsLoc, 7, 0x11 + 3),
                (codeLoc + 0x18, actualWarp, 5, 0x18 + 1)
            ]);
            memoryService.WriteBytes(codeLoc, warpBytes);
        }

        private void WriteVanillaEventWarpCode(nint codeLoc, nint actualWarp, nint eventWarpEntity,
            nint paramsLoc)
        {
            var warpBytes = AsmLoader.GetAsmBytes(AsmScript.EventWarp32);
            AsmHelper.WriteAbsoluteAddresses32(warpBytes, [
                (eventWarpEntity, 1),
                (paramsLoc, 0x5 + 2),
                (actualWarp, 0xC + 1)
            ]);

            memoryService.WriteBytes(codeLoc, warpBytes);
        }

        private void PerformCoordWrite(WarpLocation location)
        {
            var hook = Hooks.WarpCoordWrite;
            var coordsLoc = CustomCodeOffsets.Base + (int)CustomCodeOffsets.BonfireWarp.Coords;
            var code = CustomCodeOffsets.Base + (int)CustomCodeOffsets.BonfireWarp.CoordWrite;


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

            hookManager.InstallHook(code, hook,
                PatchManager.IsScholar()
                    ? [0x0F, 0x5C, 0xC2, 0x0F, 0x29, 0x47, 0x50]
                    : [0x0F, 0x5C, 0xC1, 0x0F, 0x29, 0x46, 0x40]);

            {
                int start = Environment.TickCount;
                while (IsLoadingScreen() && Environment.TickCount - start < 10000)
                    Thread.Sleep(50);
            }
            Task.Delay(200).Wait();
            hookManager.UninstallHook(code);
        }

        
        private void WriteScholarCoordWriteCode(nint hook, nint coordsLoc, nint code)
        {
            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.WarpCoordWrite64);
            AsmHelper.WriteAbsoluteAddress64(codeBytes, HkHardwareInfo.Base, 0x8 + 2);

            AsmHelper.WriteRelativeOffsets(codeBytes, [
                (code + 0x33, coordsLoc, 8, 0x33 + 4),
                (code + 0x40, coordsLoc + 0x10, 8, 0x40 + 4),
                (code + 0x4D, coordsLoc + 0x20, 8, 0x4D + 4),
                (code + 0x5A, coordsLoc + 0x30, 8, 0x5A + 4)
            ]);
            var jmpBytes = AsmHelper.GetJmpOriginOffsetBytes(hook, 7, code + 0x7C);
            Array.Copy(jmpBytes, 0, codeBytes, 0x77 + 1, 4);
            memoryService.WriteBytes(code, codeBytes);
        }

        private void WriteVanillaCoordWriteCode(nint hook, nint coordsLoc, nint code)
        {
            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.WarpCoordWrite32);
            var jmpBytes = AsmHelper.GetJmpOriginOffsetBytes(hook, 7, code + 0x6C);
            Array.Copy(jmpBytes, 0, codeBytes, 0x66 + 1, 4);
            AsmHelper.WriteAbsoluteAddresses32(codeBytes, [
                (GameManagerImp.Base, 0x8 + 1),
                (coordsLoc, 0x32 + 3),
                (coordsLoc + 0x10, 0x3D + 3),
                (coordsLoc + 0x20, 0x48 + 3),
                (coordsLoc + 0x30, 0x53 + 3)
            ]);
            memoryService.WriteBytes(code, codeBytes);
        }

        public bool IsLoadingScreen()
        {
            if (PatchManager.Current.Edition == GameEdition.Scholar)
            {
                return memoryService.Read<byte>(
                    memoryService.Read<nint>(GameManagerImp.Base) + GameManagerImp.LoadingFlag) == 1;
            }

            return memoryService.Read<byte>(
                memoryService.Read<int>(GameManagerImp.Base) + GameManagerImp.LoadingFlag) == 1;
        }

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
                    (bonfireManager, 2),
                    (func, 0xA + 2)
                ]);
                memoryService.AllocateAndExecute(bytes);
            }
            else
            {
                var bytes = AsmLoader.GetAsmBytes(AsmScript.UnlockAllBonfires32);
                AsmHelper.WriteAbsoluteAddresses32(bytes, [
                    (bonfireManager, 1),
                    (func, 0x5 + 1)
                ]);
                memoryService.AllocateAndExecute(bytes);
            }
        }
    }
}