using System;
using SilkySouls2.Memory;
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

        public void BonfireWarp(int bonfireId)
        {
            var warpPrep = Funcs.WarpPrep;
            var actualWarp = Funcs.BonfireWarp;
            //Param 1 needs to point to some empty space
            var emptySpace = CodeCaveOffsets.Base + (int)CodeCaveOffsets.BonfireWarp.EmptySpace; 
            var bonfireIdLoc = CodeCaveOffsets.Base + (int)CodeCaveOffsets.BonfireWarp.BonfireId;
            var codeLoc = CodeCaveOffsets.Base + (int)CodeCaveOffsets.BonfireWarp.Code;

            var eventWarpEntity = _memoryIo.FollowPointers(GameManagerImp.Base, new[]
            {
                GameManagerImp.Offsets.EventManager,
                GameManagerImp.EventManagerOffsets.WarpEventEntity
            } ,true);
            
            _memoryIo.WriteInt32(bonfireIdLoc, bonfireId);

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
        }
    }
}