using System;
using System.IO.MemoryMappedFiles;

namespace SilkySouls2.Memory.Draw
{
    public class DrawManager
    {
        
        private static readonly int NumFlags = Enum.GetValues(typeof(DrawType)).Length;
        private static readonly int NumAddresses = Enum.GetValues(typeof(SharedMemAddr)).Length;
        private static readonly int FlagsSize = NumFlags * sizeof(bool);
        private static readonly int AddressesSize = NumAddresses * sizeof(long);
        private static readonly int TotalSize = FlagsSize + AddressesSize;
        
        
        private MemoryMappedFile _sharedMemory;
        private MemoryMappedViewAccessor _viewAccessor;
        
        public void AllocateDllVars()
        {
            _sharedMemory = MemoryMappedFile.CreateOrOpen(
                "DrawControl",
                TotalSize,
                MemoryMappedFileAccess.ReadWrite);

            _viewAccessor = _sharedMemory.CreateViewAccessor();
            
            for (int i = 0; i < NumFlags; i++)
            {
                _viewAccessor.Write(i * sizeof(bool), false);
            }
            
            for (int i = 0; i < NumAddresses; i++)
            {
                _viewAccessor.Write(FlagsSize + (i * sizeof(long)), (long)0);
            }
        }

        public void ToggleRender(DrawType drawType, bool value)
        {
            if (_viewAccessor == null) 
            {
                Console.WriteLine("Shared memory not initialized");
                return;
            }

            int offset = (int)drawType * sizeof(bool);
            _viewAccessor.Write(offset, value);
            Console.WriteLine($"{drawType} rendering set to: {value}");
        }

        public void SetAddress(SharedMemAddr addrType, long address)
        {
            if (_viewAccessor == null)
            {
                Console.WriteLine("Shared memory not initialized");
                return;
            }

            int offset = FlagsSize + ((int)addrType * sizeof(long));
            _viewAccessor.Write(offset, address);
            Console.WriteLine($"{addrType} address set to: 0x{address:X16}");
        }
    }
}