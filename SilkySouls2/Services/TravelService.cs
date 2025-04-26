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
            var warpPrep = Funcs.WarpPrep;
            var actualWarp = Funcs.BonfireWarp;
            //Param 1 needs to point to some empty space
            var emptySpace = CodeCaveOffsets.Base + (int)CodeCaveOffsets.BonfireWarp.EmptySpace; 
            var bonfireIdLoc = CodeCaveOffsets.Base + (int)CodeCaveOffsets.BonfireWarp.BonfireId;
            var codeLoc = CodeCaveOffsets.Base + (int)CodeCaveOffsets.BonfireWarp.WarpCode;

            var eventWarpEntity = _memoryIo.FollowPointers(GameManagerImp.Base, new[]
            {
                GameManagerImp.Offsets.EventManager,
                GameManagerImp.EventManagerOffsets.WarpEventEntity
            } ,true);
            
            _memoryIo.WriteInt32(bonfireIdLoc, location.BonfireId);

            var warpBytes = AsmLoader.GetAsmBytes("BonfireWarp");
            var bytes = BitConverter.GetBytes(eventWarpEntity.ToInt64());
            Array.Copy(bytes, 0, warpBytes, 0x1D + 2, 8);
            
            AsmHelper.WriteRelativeOffsets(warpBytes, new []
            {
                (codeLoc.ToInt64() + 0x4, emptySpace.ToInt64(), 7, 0x4 + 3),
                (codeLoc.ToInt64() + 0xB, bonfireIdLoc.ToInt64(), 7, 0xB + 3),
                (codeLoc.ToInt64() + 0x18, warpPrep, 5, 0x18 + 1),
                (codeLoc.ToInt64() + 0x27, emptySpace.ToInt64(), 7, 0x27 + 3),
                (codeLoc.ToInt64() + 0x2E, actualWarp, 5, 0x2E + 1)
            });
            
            _memoryIo.WriteBytes(codeLoc, warpBytes);
            _memoryIo.RunThread(codeLoc);

            if (location.HasCoordinates) PerformCoordWrite(location);
        }

        private void PerformCoordWrite(WarpLocation location)
        {
            var hook = Hooks.WarpCoordWrite;
            var coordsLoc = CodeCaveOffsets.Base + (int)CodeCaveOffsets.BonfireWarp.Coords;
            var code = CodeCaveOffsets.Base + (int)CodeCaveOffsets.BonfireWarp.CoordWrite;
            
            byte[] allCoordinateBytes = new byte[8 * sizeof(float)];
            
            for (int i = 0; i < 8; i++)
            {
                byte[] floatBytes = BitConverter.GetBytes(location.Coordinates[i]);
                Buffer.BlockCopy(floatBytes, 0, allCoordinateBytes, i * sizeof(float), sizeof(float));
            }
            _memoryIo.WriteBytes(coordsLoc, allCoordinateBytes);

            var codeBytes = AsmLoader.GetAsmBytes("WarpCoordWrite");
            AsmHelper.WriteRelativeOffsets(codeBytes, new []
            {
                (code.ToInt64(), coordsLoc.ToInt64(), 8, 0x0 + 4),
                (code.ToInt64() + 0x8, coordsLoc.ToInt64() + 0x10, 8, 0x8 + 4)
            });
            var jmpBytes = AsmHelper.GetJmpOriginOffsetBytes(hook, 8, code + 0x2E);
            Array.Copy(jmpBytes, 0, codeBytes, 0x29 + 1, 4);
            _memoryIo.WriteBytes(code, codeBytes);
            
            {
                int start = Environment.TickCount;
                while (_memoryIo.IsGameLoaded() && Environment.TickCount - start < 10000)
                    Thread.Sleep(50);
            }

            _hookManager.InstallHook(code.ToInt64(), hook,
                new byte[] { 0x48, 0x8B, 0x8C, 0x24, 0x80, 0x00, 0x00, 0x00 });
            
            {
                int start = Environment.TickCount;
                while (!_memoryIo.IsGameLoaded() && Environment.TickCount - start < 10000)
                    Thread.Sleep(50);
            }
            _hookManager.UninstallHook(code.ToInt64());
        }
    }
}