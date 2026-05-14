using SilkySouls2.enums;
using SilkySouls2.Interfaces;
using SilkySouls2.Memory;
using SilkySouls2.Utilities;
using static SilkySouls2.Memory.Offsets;

namespace SilkySouls2.Services
{
    public class EventService(IMemoryService memoryService) : IEventService
    {
        public void SetEvent(int eventId, bool setVal)
        {
            var eventFlagMan = memoryService.FollowPointers(GameManagerImp.Base, [
                GameManagerImp.EventManager,
                GameManagerImp.EventManagerOffsets.EventFlagManager
            ], true);

            var setEventFunc = Functions.SetEvent;
            if (PatchManager.Current.Edition == GameEdition.Scholar)
            {
                SetScholarEvent(eventFlagMan, setEventFunc, eventId, setVal);
            }
            else
            {
                SetVanillaEvent(eventFlagMan, setEventFunc, eventId, setVal);
            }
        }

        private void SetScholarEvent(nint eventFlagMan, nint setEventFunc, int eventId, bool setVal)
        {
            var bytes = AsmLoader.GetAsmBytes(AsmScript.SetEvent64);

            AsmHelper.WriteImmediateDword(bytes, setVal ? 1 : 0, 0x14 + 2);
            AsmHelper.WriteAbsoluteAddresses64(bytes, [
                (eventFlagMan, 0x0 + 2),
                (eventId, 0xA + 2),
                (setEventFunc, 0x1A + 2)
            ]);

            memoryService.AllocateAndExecute(bytes);
        }

        private void SetVanillaEvent(nint eventFlagMan, nint setEventFunc, int eventId, bool setVal)
        {
            var bytes = AsmLoader.GetAsmBytes(AsmScript.SetEvent32);

            bytes[0x5 + 1] = (byte)(setVal ? 1 : 0);
            AsmHelper.WriteAbsoluteAddresses32(bytes, [
                (eventFlagMan, 1),
                (eventId, 0x7 + 1),
                (setEventFunc, 0xC + 1)
            ]);

            memoryService.AllocateAndExecute(bytes);
        }

        public void SetMultipleEventOn(int[] gameIds)
        {
            foreach (var gameId in gameIds)
            {
                SetEvent(gameId, true);
            }
        }

        public bool GetEvent(int gameId)
        {
            var eventFlagMan = memoryService.FollowPointers(GameManagerImp.Base, [
                GameManagerImp.EventManager,
                GameManagerImp.EventManagerOffsets.EventFlagManager
            ], true);

            var getEventFunc = Functions.GetEvent;
            return PatchManager.Current.Edition == GameEdition.Scholar
                ? GetScholarEvent(eventFlagMan, getEventFunc, gameId)
                : GetVanillaEvent(eventFlagMan, getEventFunc, gameId);
        }

        private bool GetScholarEvent(nint eventFlagMan, nint getEvent, int gameId)
        {
            var result = CustomCodeOffsets.Base + CustomCodeOffsets.GetEventResult;

            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.GetEvent64);
            AsmHelper.WriteAbsoluteAddresses64(codeBytes, [
                (eventFlagMan, 0x0 + 2),
                (gameId, 0xA + 2),
                (getEvent, 0x14 + 2),
                (result, 0x28 + 2)
            ]);
            memoryService.AllocateAndExecute(codeBytes);
            return memoryService.Read<byte>(result) == 1;
        }

        private bool GetVanillaEvent(nint eventFlagMan, nint getEvent, int gameId)
        {
            var result = CustomCodeOffsets.Base + CustomCodeOffsets.GetEventResult;

            var codeBytes = AsmLoader.GetAsmBytes(AsmScript.GetEvent32);
            AsmHelper.WriteAbsoluteAddresses32(codeBytes, [
                (eventFlagMan, 0x0 + 1),
                (gameId, 0x5 + 1),
                (getEvent, 0xA + 1),
                (result, 0x11 + 1)
            ]);
            memoryService.AllocateAndExecute(codeBytes);
            return memoryService.Read<byte>(result) == 1;
        }
    }
}
