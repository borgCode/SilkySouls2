using System;
using System.IO;
using System.IO.MemoryMappedFiles;

namespace SilkySouls2.Memory.DLLShared
{
    public class DllManager
    {
        private readonly MemoryIo _memoryIo;
        private readonly string _drawDllPath;
        private readonly string _speedDllPath;
        
        private static readonly int NumDrawFlags = Enum.GetValues(typeof(DrawType)).Length;
        private static readonly int NumAddresses = Enum.GetValues(typeof(SharedMemAddr)).Length;
        
        private static readonly int DrawFlagsSize = NumDrawFlags * sizeof(bool);
        private static readonly int AddressesSize = NumAddresses * sizeof(long);
        
        private static readonly int TotalSize = DrawFlagsSize + AddressesSize;

        private MemoryMappedFile _drawSharedMemory;
        private MemoryMappedViewAccessor _drawViewAccessor;
        
        private MemoryMappedFile _speedSharedMemory;
        private MemoryMappedViewAccessor _speedViewAccessor;
        
        private bool _drawIsInjected;
        private bool _speedIsInjected;
        
        public DllManager(MemoryIo memoryIo)
        {
            _memoryIo = memoryIo;
            _drawDllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "DLL", "SilkyDll.dll");
            _speedDllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "DLL", "SilkySpeed.dll");
        }
        
        public void InjectDrawDll()
        {
            if (_drawIsInjected) return;
            
            SetAddress(SharedMemAddr.GameManagerImp, Offsets.GameManagerImp.Base.ToInt64());
            SetAddress(SharedMemAddr.ParamLookUp, Offsets.Funcs.ParamLookUp);
            SetAddress(SharedMemAddr.SetRenderTargets, Offsets.Funcs.SetRenderTargets);
            SetAddress(SharedMemAddr.CreateSoundEvent, Offsets.Funcs.CreateSoundEvent);
            SetAddress(SharedMemAddr.RenderChrModel, Offsets.Funcs.ChrCtrlUpdate);
            _drawIsInjected = _memoryIo.InjectDll(_drawDllPath);
        }
        
        public void CreateDrawSharedMem()
        {
            _drawSharedMemory = MemoryMappedFile.CreateOrOpen(
                "DrawControl",
                TotalSize,
                MemoryMappedFileAccess.ReadWrite);

            _drawViewAccessor = _drawSharedMemory.CreateViewAccessor();
            
            for (int i = 0; i < NumDrawFlags; i++)
            {
                _drawViewAccessor.Write(i * sizeof(bool), false);
            }
        }
        
        public void InjectSpeedDll()
        {
            if (_speedIsInjected) return;
            _speedIsInjected = _memoryIo.InjectDll(_speedDllPath);

        }
        
        public void CreateSpeedSharedMem()
        {
            _speedSharedMemory = MemoryMappedFile.CreateOrOpen(
                "SpeedhackSharedMem",
                sizeof(double),
                MemoryMappedFileAccess.ReadWrite);

            _speedViewAccessor = _speedSharedMemory.CreateViewAccessor();
            
            _speedViewAccessor.Write(0, 1.0);
        }
        
        public void SetSpeed(double speed)
        {
            if (!_speedIsInjected)
            {
                InjectSpeedDll();
            }
            
            if (_speedViewAccessor == null)
            {
                Console.WriteLine("Speed shared memory not initialized");
                return;
            }
            
            _speedViewAccessor.Write(0, speed);
        }

        public void ToggleRender(DrawType drawType, bool value)
        {
            if (_drawViewAccessor == null) return;
            if (!_drawIsInjected) InjectDrawDll();
            
            int offset = (int)drawType * sizeof(bool);
            _drawViewAccessor.Write(offset, value);
        }
        
        public void SetAddress(SharedMemAddr addrType, long address)
        {
            if (_drawViewAccessor == null)
            {
                Console.WriteLine("Shared memory not initialized");
                return;
            }
            
            int offset = DrawFlagsSize + ((int)addrType * sizeof(long));
            _drawViewAccessor.Write(offset, address);
            Console.WriteLine($"{addrType} address set to: 0x{address:X16}");
        }
    }
}