using System;
using System.Threading;
using System.Threading.Tasks;
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
        private readonly PlayerService _playerService;
        
        
        public TravelService(MemoryIo memoryIo, HookManager hookManager, PlayerService playerService)
        {
            _memoryIo = memoryIo;
            _hookManager = hookManager;
            _playerService = playerService;
        }

        public void Warp(WarpLocation location, bool isRestOnWarpEnabled)
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
                var emptySpace = CodeCaveOffsets.Base + (int)CodeCaveOffsets.BonfireWarp.EmptySpace;
                var bonfireIdLoc = CodeCaveOffsets.Base + (int)CodeCaveOffsets.BonfireWarp.BonfireId;
                codeLoc = CodeCaveOffsets.Base + (int)CodeCaveOffsets.BonfireWarp.WarpCode;

                _memoryIo.WriteInt32(bonfireIdLoc, location.BonfireId);

                if (GameVersion.Current.Edition == GameEdition.Scholar)
                {
                    warpBytes = AsmLoader.GetAsmBytes("BonfireWarp64");
                    var bytes = BitConverter.GetBytes(eventWarpEntity.ToInt64());
                    Array.Copy(bytes, 0, warpBytes, 0x1D + 2, 8);

                    AsmHelper.WriteRelativeOffsets(warpBytes, new[]
                    {
                        (codeLoc.ToInt64() + 0x4, emptySpace.ToInt64(), 7, 0x4 + 3),
                        (codeLoc.ToInt64() + 0xB, bonfireIdLoc.ToInt64(), 7, 0xB + 3),
                        (codeLoc.ToInt64() + 0x18, warpPrep, 5, 0x18 + 1),
                        (codeLoc.ToInt64() + 0x27, emptySpace.ToInt64(), 7, 0x27 + 3),
                        (codeLoc.ToInt64() + 0x2E, actualWarp, 5, 0x2E + 1)
                    });
                }
                else
                {
                    warpBytes = AsmLoader.GetAsmBytes("BonfireWarp32");
                    AsmHelper.WriteAbsoluteAddresses32(warpBytes, new []
                    {
                        (bonfireIdLoc.ToInt64(), 0x5 + 2),
                        (emptySpace.ToInt64(), 0xB + 2),
                        (warpPrep, 0x12 + 1),
                        (emptySpace.ToInt64(), 0x1C + 2),
                        (eventWarpEntity.ToInt64(), 0x23 + 1),
                        (actualWarp, 0x28 + 1)
                    });
                }
            }
            else
            {
                var paramsLoc = CodeCaveOffsets.Base + (int)CodeCaveOffsets.EventWarp.Params;
                _memoryIo.WriteInt32(paramsLoc, 4);
                _memoryIo.WriteInt32(paramsLoc +0x4, 5); 
                _memoryIo.WriteInt32(paramsLoc + 0x8, location.BonfireId);
                _memoryIo.WriteInt32(paramsLoc + 0xC, -1);
                _memoryIo.WriteInt32(paramsLoc + 0x18, location.EventObjId);

                codeLoc = CodeCaveOffsets.Base + (int)CodeCaveOffsets.EventWarp.Code;

                if (GameVersion.Current.Edition == GameEdition.Scholar)
                {
                    warpBytes = AsmLoader.GetAsmBytes("EventWarp64");
                    var bytes = BitConverter.GetBytes(eventWarpEntity.ToInt64());
                    Array.Copy(bytes, 0, warpBytes, 0x7 + 2, 8);
                    AsmHelper.WriteRelativeOffsets(warpBytes, new[]
                    {
                        (codeLoc.ToInt64() + 0x11, paramsLoc.ToInt64(), 7, 0x11 + 3),
                        (codeLoc.ToInt64() + 0x18, actualWarp, 5, 0x18 + 1)
                    });
                }
                else
                {
                    warpBytes = AsmLoader.GetAsmBytes("EventWarp32");
                    AsmHelper.WriteAbsoluteAddresses32(warpBytes, new []
                    {
                        (eventWarpEntity.ToInt64(), 1),
                        (paramsLoc.ToInt64(), 0x5 + 2),
                        (actualWarp, 0xC + 1)
                    });
                }
                
            }

            _memoryIo.WriteBytes(codeLoc, warpBytes);
            _memoryIo.RunThread(codeLoc);
            if (isRestOnWarpEnabled) _playerService.SetSpEffect(GameIds.SpEffects.SpEffectData.BonfireRest);
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

            if (GameVersion.Current.Edition == GameEdition.Scholar)
            {
                var codeBytes = AsmLoader.GetAsmBytes("WarpCoordWrite64");
                var bytes = BitConverter.GetBytes(HkHardwareInfo.Base.ToInt64());
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
            }
            else
            {
                var codeBytes = AsmLoader.GetAsmBytes("WarpCoordWrite32");
                var jmpBytes = AsmHelper.GetJmpOriginOffsetBytes(hook, 7, code + 0x6C);
                Array.Copy(jmpBytes, 0, codeBytes, 0x66 + 1, 4);
                AsmHelper.WriteAbsoluteAddresses32(codeBytes, new []
                {
                    (GameManagerImp.Base.ToInt64(), 0x8 + 1),
                    (coordsLoc.ToInt64(), 0x32 + 3),
                    (coordsLoc.ToInt64() + 0x10, 0x3D + 3),
                    (coordsLoc.ToInt64() + 0x20, 0x48 + 3),
                    (coordsLoc.ToInt64() + 0x30, 0x53 + 3)
                });
                _memoryIo.WriteBytes(code, codeBytes);
            }
            
            {
                int start = Environment.TickCount;
                while (!IsLoadingScreen() && Environment.TickCount - start < 10000)
                    Thread.Sleep(50);
            }

            _hookManager.InstallHook(code.ToInt64(), hook,
                GameVersion.Current.Edition == GameEdition.Scholar
                    ? new byte[] { 0x0F, 0x5C, 0xC2, 0x0F, 0x29, 0x47, 0x50 }
                    : new byte[] { 0x0F, 0x5C, 0xC1, 0x0F, 0x29, 0x46, 0x40 });

            {
                int start = Environment.TickCount;
                while (IsLoadingScreen() && Environment.TickCount - start < 10000)
                    Thread.Sleep(50);
            }
            Task.Delay(200).Wait();
            _hookManager.UninstallHook(code.ToInt64());
        }

        public bool IsLoadingScreen() =>
            _memoryIo.ReadUInt8((IntPtr)_memoryIo.ReadInt64(GameManagerImp.Base) +
                                GameManagerImp.Offsets.LoadingFlag) == 1;

        public void UnlockAllBonfires()
        {
            var func = Funcs.UnlockBonfire;
            var bonfireManager = _memoryIo.FollowPointers(GameManagerImp.Base, new[]
            {
                GameManagerImp.Offsets.EventManager,
                GameManagerImp.EventManagerOffsets.EventBonfireManager
            }, true);
            
            var bytes = AsmLoader.GetAsmBytes("UnlockAllBonfires");
            AsmHelper.WriteAbsoluteAddresses64(bytes, new []
            {
               
                (bonfireManager.ToInt64(), 2),
                (func,   0xA + 2),
            });
            
            _memoryIo.AllocateAndExecute(bytes);
        }
    }
}