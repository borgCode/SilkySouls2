using System.Diagnostics.CodeAnalysis;
using SilkySouls2.enums;
using SilkySouls2.Interfaces;
using SilkySouls2.Memory;
using SilkySouls2.Models;
using SilkySouls2.Utilities;
using static SilkySouls2.Memory.Offsets;

namespace SilkySouls2.Services
{

    public class TravelService(IMemoryService memoryService) : ITravelService
    {
        public void Warp(WarpEntry entry)
        {
            if (entry == null) return;

            var request = BuildRequest(entry);
            var requestLoc = CustomCodeOffsets.Base + (int)CustomCodeOffsets.Warp.Request;
            memoryService.Write(requestLoc, request);

            var warpManager = memoryService.FollowPointers(GameManagerImp.Base, [
                GameManagerImp.EventManager,
                GameManagerImp.EventManagerOffsets.EventWarpManager
            ], true);

            var code = CustomCodeOffsets.Base + (int)CustomCodeOffsets.Warp.Stub;
            if (PatchManager.IsScholar()) WriteScholarWarpCode(code, warpManager, requestLoc, Functions.RequestWarp);
            else WriteVanillaWarpCode(code, warpManager, requestLoc, Functions.RequestWarp);
            
            memoryService.RunThread(code);
            
        }


        [SuppressMessage("ReSharper", "PossibleInvalidOperationException")]
        private static WarpRequest BuildRequest(WarpEntry entry) => entry.Kind switch
        {
            WarpKind.Bonfire    => WarpRequest.ForBonfire(entry.BonfireId.Value, entry.MapId.Value),
            WarpKind.EventPoint => WarpRequest.ForEventPoint(entry.EventPointId.Value, entry.MapId.Value),
            WarpKind.Direct or WarpKind.DirectWithOffset
                                => WarpRequest.ForDirect(entry.Kind, entry.MapId.Value, entry.Pos, entry.Quat),
            _ => default
        };
        
        private void WriteScholarWarpCode(nint codeLoc, nint warpManager, nint requestLoc, nint requestWarp)
        {
            var bytes = AsmLoader.GetAsmBytes(AsmScript.WarpRequest64);
            AsmHelper.WriteAbsoluteAddresses64(bytes, [
                (warpManager, 0x06),
                (requestLoc,  0x10),
                (requestWarp,  0x1A)
            ]);
            memoryService.WriteBytes(codeLoc, bytes);
        }
        
        private void WriteVanillaWarpCode(nint codeLoc, nint warpManager, nint requestLoc,
            nint requestWarp)
        {
            var warpBytes = AsmLoader.GetAsmBytes(AsmScript.WarpRequest32);
            AsmHelper.WriteAbsoluteAddresses32(warpBytes, [
                (warpManager, 1),
                (requestLoc, 0x5 + 2),
                (requestWarp, 0xC + 1)
            ]);

            memoryService.WriteBytes(codeLoc, warpBytes);
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
