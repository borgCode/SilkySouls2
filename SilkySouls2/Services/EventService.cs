using SilkySouls2.Memory;
using SilkySouls2.Utilities;

namespace SilkySouls2.Services
{
    public class EventService
    {
        private readonly MemoryIo _memoryIo;

        public EventService(MemoryIo memoryIo)
        {
            _memoryIo = memoryIo;
   
        }


        public void SetEventOn(long gameId)
        {
            var eventFlagMan = _memoryIo.FollowPointers(Offsets.GameManagerImp.Base, new[]
            {
                Offsets.GameManagerImp.Offsets.EventManager,
                Offsets.GameManagerImp.EventManagerOffsets.EventFlagManager
            }, true);

            var setEventFunc = Offsets.Funcs.SetEvent;
            if (GameVersion.Current.Edition == GameEdition.Scholar)
            {
                var bytes = AsmLoader.GetAsmBytes("SetEventOn64");
                AsmHelper.WriteAbsoluteAddresses64(bytes, new[]
                {
                    (eventFlagMan.ToInt64(), 0x0 + 2),
                    (gameId, 0xA + 2),
                    (setEventFunc, 0x1A + 2)
                });

                _memoryIo.AllocateAndExecute(bytes);
            }
            else
            {
                var bytes = AsmLoader.GetAsmBytes("SetEventOn32");
                AsmHelper.WriteAbsoluteAddresses32(bytes, new[]
                {
                    (eventFlagMan.ToInt64(), 1),
                    (gameId, 0x7 + 1),
                    (setEventFunc, 0xC + 1)
                });

                _memoryIo.AllocateAndExecute(bytes);
            }
        }

        public void SetMultipleEventOn(long[] gameIds)
        {
            foreach (var gameId in gameIds)
            {
                SetEventOn(gameId);
            }
        }

        public void SetEventOff(long gameId)
        {
            var eventFlagMan = _memoryIo.FollowPointers(Offsets.GameManagerImp.Base, new[]
            {
                Offsets.GameManagerImp.Offsets.EventManager,
                Offsets.GameManagerImp.EventManagerOffsets.EventFlagManager
            }, true);

            var setEventFunc = Offsets.Funcs.SetEvent;

            if (GameVersion.Current.Edition == GameEdition.Scholar)
            {
                var bytes = AsmLoader.GetAsmBytes("SetEventOff64");
                AsmHelper.WriteAbsoluteAddresses64(bytes, new[]
                {
                    (eventFlagMan.ToInt64(), 0x0 + 2),
                    (gameId, 0xA + 2),
                    (setEventFunc, 0x1A + 2)
                });

                _memoryIo.AllocateAndExecute(bytes);
            }
            else
            {
                var bytes = AsmLoader.GetAsmBytes("SetEventOff32");
                AsmHelper.WriteAbsoluteAddresses32(bytes, new[]
                {
                    (eventFlagMan.ToInt64(), 1),
                    (gameId, 0x7 + 1),
                    (setEventFunc, 0xC + 1)
                });

                _memoryIo.AllocateAndExecute(bytes);
            }
        }

        public void SetMultipleEventOff(long[] gameIds)
        {
            foreach (var gameId in gameIds)
            {
                SetEventOff(gameId);
            }
        }

        public bool GetEvent(long gameId) =>
            GameVersion.Current.Edition == GameEdition.Scholar ? GetScholarEvent(gameId) : GetVanillaEvent(gameId);


        private bool GetScholarEvent(long gameId)
        {
            var result = CodeCaveOffsets.Base + CodeCaveOffsets.GetEventResult;
            var getEvent = Offsets.Funcs.GetEvent;
            var eventFlagMan = _memoryIo.FollowPointers(Offsets.GameManagerImp.Base, new[]
            {
                Offsets.GameManagerImp.Offsets.EventManager,
                Offsets.GameManagerImp.EventManagerOffsets.EventFlagManager
            }, true);
            
            var codeBytes = AsmLoader.GetAsmBytes("GetEvent64");
            AsmHelper.WriteAbsoluteAddresses64(codeBytes, new []
            {
                (eventFlagMan.ToInt64(), 0x0 + 2),
                (gameId, 0xA + 2),
                (getEvent, 0x14 + 2),
                (result.ToInt64(), 0x28 + 2)
            });
            _memoryIo.AllocateAndExecute(codeBytes);
            return _memoryIo.ReadUInt8(result) == 1;
        }

        private bool GetVanillaEvent(long gameId)
        {
            var result = CodeCaveOffsets.Base + CodeCaveOffsets.GetEventResult;
            var getEvent = Offsets.Funcs.GetEvent;
            var eventFlagMan = _memoryIo.FollowPointers(Offsets.GameManagerImp.Base, new[]
            {
                Offsets.GameManagerImp.Offsets.EventManager,
                Offsets.GameManagerImp.EventManagerOffsets.EventFlagManager
            }, true);
            
            var codeBytes = AsmLoader.GetAsmBytes("GetEvent32");
            AsmHelper.WriteAbsoluteAddresses32(codeBytes, new []
            {
                (eventFlagMan.ToInt64(), 0x0 + 1),
                (gameId, 0x5 + 1),
                (getEvent, 0xA + 1),
                (result.ToInt64(), 0x11 + 1)
            });
            _memoryIo.AllocateAndExecute(codeBytes);
            return _memoryIo.ReadUInt8(result) == 1;
        }
    }
}