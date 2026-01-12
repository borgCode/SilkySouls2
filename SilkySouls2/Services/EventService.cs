using System;
using SilkySouls2.enums;
using SilkySouls2.Interfaces;
using SilkySouls2.Memory;
using SilkySouls2.Utilities;

namespace SilkySouls2.Services
{
    public class EventService(IMemoryService memoryService) : IEventService
    {
        public void SetEvent(long eventId, bool setVal)
        {
            var eventFlagMan = memoryService.FollowPointers(Offsets.GameManagerImp.Base, new[]
            {
                Offsets.GameManagerImp.EventManager,
                Offsets.GameManagerImp.EventManagerOffsets.EventFlagManager
            }, true);

            var setEventFunc = Offsets.Functions.SetEvent;
            if (PatchManager.Current.Edition == GameEdition.Scholar)
            {
                SetScholarEvent(eventFlagMan, setEventFunc, eventId, setVal);
            }
            else
            {
                SetVanillaEvent(eventFlagMan, setEventFunc, eventId, setVal);
                
            }
        }
        
        private void SetScholarEvent(IntPtr eventFlagMan, long setEventFunc, long eventId, bool setVal)
        {
            var bytes = AsmLoader.GetAsmBytes(AsmScript.SetEvent64);

            var setValBytes = BitConverter.GetBytes(setVal ? 1 : 0);
            Array.Copy(setValBytes, 0, bytes, 0x14 + 2, setValBytes.Length);
            AsmHelper.WriteAbsoluteAddresses64(bytes, [
                (eventFlagMan.ToInt64(), 0x0 + 2),
                (eventId, 0xA + 2),
                (setEventFunc, 0x1A + 2)
            ]);

            memoryService.AllocateAndExecute(bytes);
        }
        
        private void SetVanillaEvent(IntPtr eventFlagMan, long setEventFunc, long eventId, bool setVal)
        {
            var bytes = AsmLoader.GetAsmBytes(AsmScript.SetEvent32);
            
            var setValBytes = BitConverter.GetBytes(setVal ? 1 : 0);
            Array.Copy(setValBytes, 0, bytes, 0x14 + 2, setValBytes.Length);
            AsmHelper.WriteAbsoluteAddresses32(bytes, [
                (eventFlagMan.ToInt64(), 1),
                (eventId, 0x7 + 1),
                (setEventFunc, 0xC + 1)
            ]);

            memoryService.AllocateAndExecute(bytes);
        }

        public void SetMultipleEventOn(long[] gameIds)
        {
            foreach (var gameId in gameIds)
            {
                SetEvent(gameId, true);
            }
        }

        
        public bool GetEvent(long gameId) =>
            PatchManager.Current.Edition == GameEdition.Scholar ? GetScholarEvent(gameId) : GetVanillaEvent(gameId);


        private bool GetScholarEvent(long gameId)
        {
            var result = CodeCaveOffsets.Base + CodeCaveOffsets.GetEventResult;
            var getEvent = Offsets.Functions.GetEvent;
            var eventFlagMan = memoryService.FollowPointers(Offsets.GameManagerImp.Base, new[]
            {
                Offsets.GameManagerImp.EventManager,
                Offsets.GameManagerImp.EventManagerOffsets.EventFlagManager
            }, true);
            
            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.GetEvent64);
            AsmHelper.WriteAbsoluteAddresses64(codeBytes, [
                (eventFlagMan.ToInt64(), 0x0 + 2),
                (gameId, 0xA + 2),
                (getEvent, 0x14 + 2),
                (result.ToInt64(), 0x28 + 2)
            ]);
            memoryService.AllocateAndExecute(codeBytes);
            return memoryService.Read<byte>(result) == 1;
        }

        private bool GetVanillaEvent(long gameId)
        {
            var result = CodeCaveOffsets.Base + CodeCaveOffsets.GetEventResult;
            var getEvent = Offsets.Functions.GetEvent;
            var eventFlagMan = memoryService.FollowPointers(Offsets.GameManagerImp.Base, new[]
            {
                Offsets.GameManagerImp.EventManager,
                Offsets.GameManagerImp.EventManagerOffsets.EventFlagManager
            }, true);
            
            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.GetEvent32);
            AsmHelper.WriteAbsoluteAddresses32(codeBytes, [
                (eventFlagMan.ToInt64(), 0x0 + 1),
                (gameId, 0x5 + 1),
                (getEvent, 0xA + 1),
                (result.ToInt64(), 0x11 + 1)
            ]);
            memoryService.AllocateAndExecute(codeBytes);
            return memoryService.Read<byte>(result) == 1;
        }
    }
}