using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using SilkySouls2.Interfaces;
using SilkySouls2.Services;

namespace SilkySouls2.Memory.DLLShared
{
    public class DllManager(IMemoryService memoryService)
    {
        private readonly string _drawScholarDllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "DLL", "DrawScholar.dll");
        private readonly string _speedScholarDllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "DLL", "SilkySpeed.dll");
        private readonly string _drawVanillaDllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "DLL", "DrawVanilla.dll");
        private readonly string _speedVanillaDllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "DLL", "SilkySpeed32.dll");
        
        private static readonly int NumDrawFlags = Enum.GetValues(typeof(DrawType)).Length;
        private static readonly int NumAddresses = Enum.GetValues(typeof(SharedMemAddr)).Length;
        
        private static readonly int DrawFlagsSize = NumDrawFlags * sizeof(bool);
        private static int AddressesSize => NumAddresses * (PatchManager.IsScholar() ? 8 : 4);
        private static int TotalSize => DrawFlagsSize + AddressesSize;

        private MemoryMappedFile _drawSharedMemory;
        private MemoryMappedViewAccessor _drawViewAccessor;
        
        private MemoryMappedFile _speedSharedMemory;
        private MemoryMappedViewAccessor _speedViewAccessor;
        
        private bool _drawIsInjected;
        private bool _speedIsInjected;

        public void InjectDrawDll()
        {
            if (_drawIsInjected) return;
            
            SetAddress(SharedMemAddr.GameManagerImp, Offsets.GameManagerImp.Base.ToInt64());
            SetAddress(SharedMemAddr.ParamLookup, Offsets.Functions.ParamLookup);
            if (PatchManager.IsScholar())
            {
                SetAddress(SharedMemAddr.SetRenderTargets, Offsets.Functions.SetRenderTargets);
            }
            else
            {
                SetAddress(SharedMemAddr.SetRenderTargets, Offsets.Functions.SetDepthStencilSurface);
            }

            SetAddress(SharedMemAddr.CreateSoundEvent, Offsets.Functions.CreateSoundEvent);
            SetAddress(SharedMemAddr.GetEyePosition, Offsets.Functions.GetEyePosition);
            string dllPath = PatchManager.IsScholar() ? _drawScholarDllPath : _drawVanillaDllPath;
            _drawIsInjected = memoryService.InjectDll(dllPath);
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

        public void SetAddress(SharedMemAddr addrType, long address)
        {
            if (_drawViewAccessor == null)
            {
                Console.WriteLine("Shared memory not initialized");
                return;
            }

            int pointerSize = PatchManager.Current.Edition == GameEdition.Scholar ? 8 : 4;
            int offset = DrawFlagsSize + ((int)addrType * pointerSize);

            if (pointerSize == 8) _drawViewAccessor.Write(offset, address);
            else _drawViewAccessor.Write(offset, (int)address);
            
            Console.WriteLine($"{addrType} address set to: 0x{address:X16}");
        }

        public void InjectSpeedDll()
        {
            if (_speedIsInjected) return;
            string dllPath = PatchManager.Current.Edition == GameEdition.Scholar ? _speedScholarDllPath : _speedVanillaDllPath;
            _speedIsInjected = memoryService.InjectDll(dllPath);
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

        public void ResetState()
        {
            _drawIsInjected = false;
            _speedIsInjected = false;
        }

        public void TestInject32()
        {
            memoryService.InjectDll(_drawVanillaDllPath);
        }

        public bool IsSpeedInjected() => _speedIsInjected;
    }
}