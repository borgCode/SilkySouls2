using System;
using SilkySouls2.Memory;
using static SilkySouls2.Memory.Offsets;

namespace SilkySouls2.Services
{
    public class SettingsService
    {
        private readonly MemoryIo _memoryIo;
        
        public SettingsService(MemoryIo memoryIo)
        {
            _memoryIo = memoryIo;
        }
        
        public void Quitout() =>
            _memoryIo.WriteByte((IntPtr)_memoryIo.ReadInt64(GameManagerImp.Base) + GameManagerImp.Offsets.MenuKick, 6);

    }
}