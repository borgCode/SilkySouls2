using System;
using System.Threading;
using SilkySouls2.Memory;
using SilkySouls2.Models;
using SilkySouls2.Utilities;
using static SilkySouls2.Memory.Offsets;

namespace SilkySouls2.Services
{
    public class TravelService
    {
        private readonly MemoryIo _memoryIo;
        private readonly HookManager _hookManager;

        public TravelService(MemoryIo memoryIo, HookManager hookManager)
        {
            _memoryIo = memoryIo;
            _hookManager = hookManager;
        }

        public void Warp(WarpLocation location)
        {
  
            var actualWarp = Funcs.BonfireWarp;
            var eventWarpEntity = _memoryIo.FollowPointers(GameManagerImp.Base, new[]
            {
                GameManagerImp.Offsets.EventManager,
                GameManagerImp.EventManagerOffsets.WarpEventEntity
            }, true);
            
            IntPtr codeLoc;
            byte[] warpBytes;
            if (location.EventObjId == 0)
            {
                var warpPrep = Funcs.WarpPrep;
                //Param 1 needs to point to some empty space
                var emptySpace = CodeCaveOffsets.Base + (int)CodeCaveOffsets.BonfireWarp.EmptySpace;
                var bonfireIdLoc = CodeCaveOffsets.Base + (int)CodeCaveOffsets.BonfireWarp.BonfireId;
                codeLoc = CodeCaveOffsets.Base + (int)CodeCaveOffsets.BonfireWarp.WarpCode;
                
                _memoryIo.WriteInt32(bonfireIdLoc, location.BonfireId);

                warpBytes = AsmLoader.GetAsmBytes("BonfireWarp");
                var bytes = BitConverter.GetBytes(eventWarpEntity.ToInt64());
                Array.Copy(bytes, 0, warpBytes, 0x24 + 2, 8);

                AsmHelper.WriteRelativeOffsets(warpBytes, new[]
                {
                    (codeLoc.ToInt64() + 0x4, emptySpace.ToInt64(), 7, 0x4 + 3),
                    (codeLoc.ToInt64() + 0xB, bonfireIdLoc.ToInt64(), 7, 0xB + 3),
                    (codeLoc.ToInt64() + 0x18, warpPrep, 5, 0x18 + 1),
                    (codeLoc.ToInt64() + 0x2E, emptySpace.ToInt64(), 7, 0x2E + 3),
                    (codeLoc.ToInt64() + 0x35, actualWarp, 5, 0x35 + 1)
                });
                
            }
            else
            {
                var paramsLoc = CodeCaveOffsets.Base + (int)CodeCaveOffsets.EventWarp.Params;
                _memoryIo.WriteInt32(paramsLoc, 4);
                _memoryIo.WriteInt32(paramsLoc + 0x8, location.BonfireId);
                _memoryIo.WriteInt32(paramsLoc + 0xC, -1);
                _memoryIo.WriteInt32(paramsLoc + 0x18, location.EventObjId);
                
                codeLoc = CodeCaveOffsets.Base + (int)CodeCaveOffsets.EventWarp.Code;
                warpBytes = AsmLoader.GetAsmBytes("EventWarp");
                var bytes = BitConverter.GetBytes(eventWarpEntity.ToInt64());
                Array.Copy(bytes, 0, warpBytes, 0x7 + 2, 8);
               AsmHelper.WriteRelativeOffsets(warpBytes, new []
               {
                   (codeLoc.ToInt64() + 0x11, paramsLoc.ToInt64(), 7, 0x11 + 3),
                   (codeLoc.ToInt64() + 0x18, actualWarp, 5, 0x18 + 1)
               });
               
            }
            
            _memoryIo.WriteBytes(codeLoc, warpBytes);
            _memoryIo.RunThread(codeLoc);

            if (location.HasCoordinates) PerformCoordWrite(location);
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

            _memoryIo.WriteBytes(coordsLoc, allCoordinateBytes);

            var codeBytes = AsmLoader.GetAsmBytes("WarpCoordWrite");
            var bytes = BitConverter.GetBytes(HkpPtrEntity.Base.ToInt64());
            Array.Copy(bytes, 0, codeBytes, 0x8 + 2, 8);

            AsmHelper.WriteRelativeOffsets(codeBytes, new[]
            {
                (code.ToInt64() + 0x33, coordsLoc.ToInt64(), 8, 0x33 + 4),
                (code.ToInt64() + 0x40, coordsLoc.ToInt64() + 0x10, 8, 0x40 + 4),
                (code.ToInt64() + 0x4D, coordsLoc.ToInt64() + 0x20, 8, 0x4D + 4),
                (code.ToInt64() + 0x5A, coordsLoc.ToInt64() + 0x30, 8, 0x5A + 4),
            });
            bytes = AsmHelper.GetJmpOriginOffsetBytes(hook, 7, code + 0x7C);
            Array.Copy(bytes, 0, codeBytes, 0x77 + 1, 4);
            _memoryIo.WriteBytes(code, codeBytes);

            {
                int start = Environment.TickCount;
                while (!IsLoadingScreen() && Environment.TickCount - start < 10000)
                    Thread.Sleep(50);
            }

            _hookManager.InstallHook(code.ToInt64(), hook,
                new byte[] { 0x0F, 0x5C, 0xC2, 0x0F, 0x29, 0x47, 0x50 });

            {
                int start = Environment.TickCount;
                while (IsLoadingScreen() && Environment.TickCount - start < 10000)
                    Thread.Sleep(50);
            }
            _hookManager.UninstallHook(code.ToInt64());
        }

        public bool IsLoadingScreen() =>
            _memoryIo.ReadUInt8((IntPtr)_memoryIo.ReadInt64(GameManagerImp.Base) +
                                GameManagerImp.Offsets.LoadingFlag) == 1;
    }
}