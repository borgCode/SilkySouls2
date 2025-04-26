using SilkySouls2.Memory;
using SilkySouls2.Utilities;
using static SilkySouls2.Memory.Offsets;

namespace SilkySouls2.Services
{
    public class UtilityService
    {
        private readonly MemoryIo _memoryIo;
        private readonly HookManager _hookManager;
        public UtilityService(MemoryIo memoryIo, HookManager hookManager)
        {
            _memoryIo = memoryIo;
            _hookManager = hookManager;
        }
        
        public void SetEventOn(long gameId)
        {
            var eventFlagMan = _memoryIo.FollowPointers(GameManagerImp.Base, new[]
            {
                GameManagerImp.Offsets.EventManager,
                GameManagerImp.EventManagerOffsets.EventFlagManager
            }, true);

            var setEventFunc = Funcs.SetEvent;
            var bytes = AsmLoader.GetAsmBytes("SetEventOn");
            AsmHelper.WriteAbsoluteAddresses(bytes, new []
            {
                (eventFlagMan.ToInt64(), 0x0 +2),
                (gameId, 0xA + 2), 
                (setEventFunc, 0x1A + 2)
            });
            
            _memoryIo.AllocateAndExecute(bytes);
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
            var eventFlagMan = _memoryIo.FollowPointers(GameManagerImp.Base, new[]
            {
                GameManagerImp.Offsets.EventManager,
                GameManagerImp.EventManagerOffsets.EventFlagManager
            }, true);

            var setEventFunc = Funcs.SetEvent;
            var bytes = AsmLoader.GetAsmBytes("SetEventOff");
            AsmHelper.WriteAbsoluteAddresses(bytes, new []
            {
                (eventFlagMan.ToInt64(), 0x0 +2),
                (gameId, 0xA + 2), 
                (setEventFunc, 0x1A + 2)
            });
            
            _memoryIo.AllocateAndExecute(bytes);
        }

        public void SetMultipleEventOff(long[] gameIds)
        {
            foreach (var gameId in gameIds)
            {
                SetEventOff(gameId);
            }
        }
    }
}