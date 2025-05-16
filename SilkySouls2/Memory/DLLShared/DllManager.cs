using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using SilkySouls2.Memory.Draw;

namespace SilkySouls2.Memory.DLLShared
{
    public class DllManager
    {
        private readonly MemoryIo _memoryIo;
        private readonly string _dllPath;
        
        private static readonly int NumDrawFlags = Enum.GetValues(typeof(DrawType)).Length;
        private static readonly int NumAddresses = Enum.GetValues(typeof(SharedMemAddr)).Length;
        
        private static readonly int DrawFlagsSize = NumDrawFlags * sizeof(bool);
        private static readonly int AddressesSize = NumAddresses * sizeof(long);
        
        private static readonly int TotalSize = DrawFlagsSize + AddressesSize;

        private MemoryMappedFile _sharedMemory;
        private MemoryMappedViewAccessor _viewAccessor;
        private bool _isInjected;
        
        public DllManager(MemoryIo memoryIo)
        {
            _memoryIo = memoryIo;
            _dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "DLL", "SilkyDll.dll");
        }
        
        public void Inject()
        {
            if (_isInjected) return;
            
            SetAddress(SharedMemAddr.GameManagerImp, Offsets.GameManagerImp.Base.ToInt64());
            SetAddress(SharedMemAddr.ParamLookUp, Offsets.Funcs.ParamLookUp);
            SetAddress(SharedMemAddr.SetRenderTargets, Offsets.Funcs.SetRenderTargets);
            SetAddress(SharedMemAddr.CreateSoundEvent, Offsets.Funcs.CreateSoundEvent);
            
            Console.WriteLine($"Injecting DLL from: {_dllPath}");
            _isInjected = _memoryIo.InjectDll(_dllPath);
            Console.WriteLine($"Injection {(_isInjected ? "successful" : "failed")}");
        }
        
        
        public void AllocateDllVars()
        {
            _sharedMemory = MemoryMappedFile.CreateOrOpen(
                "DrawControl",
                TotalSize,
                MemoryMappedFileAccess.ReadWrite);

            _viewAccessor = _sharedMemory.CreateViewAccessor();
            
            for (int i = 0; i < NumDrawFlags; i++)
            {
                _viewAccessor.Write(i * sizeof(bool), false);
            }
            
            // for (int i = 0; i < NumFeatureFlags; i++)
            // {
            //     _viewAccessor.Write(FeatureFlagsOffset + (i * sizeof(bool)), false);
            // }
            //
            // _viewAccessor.Write(SpeedhackParamOffset, 1.0f);
            //
            // for (int i = 0; i < NumAddresses; i++)
            // {
            //     _viewAccessor.Write(AddressesOffset + (i * sizeof(long)), (long)0);
            // }
        }

        public void ToggleRender(DrawType drawType, bool value)
        {
            if (_viewAccessor == null) return;
            if (!_isInjected) Inject();
            
            int offset = (int)drawType * sizeof(bool);
            _viewAccessor.Write(offset, value);
        }

        // public void ToggleFeature(FeatureType featureType, bool value)
        // {
        //     if (_viewAccessor == null) return;
        //     if (!_isInjected) Inject();
        //
        //     int offset = FeatureFlagsOffset + ((int)featureType * sizeof(bool));
        //     _viewAccessor.Write(offset, value);
        // }
        //
        // public void SetSpeedhackFactor(float factor) => _viewAccessor.Write(SpeedhackParamOffset, factor);
        //
        public void SetAddress(SharedMemAddr addrType, long address)
        {
            if (_viewAccessor == null)
            {
                Console.WriteLine("Shared memory not initialized");
                return;
            }
            
            int offset = DrawFlagsSize + ((int)addrType * sizeof(long));
            _viewAccessor.Write(offset, address);
            Console.WriteLine($"{addrType} address set to: 0x{address:X16}");
        }
    }
}