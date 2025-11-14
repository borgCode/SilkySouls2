using SilkySouls2.Memory;

namespace SilkySouls2.Services
{
    public class CmpEventRandValService
    {
        private readonly MemoryIo _memoryIo;
        private readonly HookManager _hookManager;

        private int count;

        public CmpEventRandValService(MemoryIo memoryIo, HookManager hookManager)
        {
            _memoryIo = memoryIo;
            _hookManager = hookManager;
            
        }
        
        
    }
}