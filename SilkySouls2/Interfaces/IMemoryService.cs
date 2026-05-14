// 

using System;
using System.Diagnostics;

namespace SilkySouls2.Interfaces
{
    public interface IMemoryService
    {
        void Dispose();

        public bool IsAttached { get; }
        public Process? TargetProcess { get; }
        public nint BaseAddress { get; }
        public int ModuleMemorySize { get; }

        T Read<T>(nint addr) where T : unmanaged;
        
        void Write<T>(IntPtr addr, T value) where T : unmanaged;
        void Write(IntPtr addr, bool value);
        
        nint ReadPointer(nint addr);
        byte[] ReadBytes(nint addr, int size);
        
        void WriteBytes(IntPtr addr, byte[] val);
        void WriteWString(nint addr, string value, int maxChars = 32);
        void SetBitValue(nint addr, int flagMask, bool setValue);
        bool IsBitSet(nint addr, int flagMask);

        void RunPersistentThread(IntPtr address);
        nint FollowPointers(nint baseAddress, int[] offsets, bool readFinalPtr);
        
        uint RunThread(nint address, uint timeout = uint.MaxValue);
        bool RunThreadAndWaitForCompletion(nint address, uint timeout = uint.MaxValue);
        
        void AllocateAndExecute(byte[] shellcode);
        void AllocCodeCave();

        public IntPtr GetProcAddress(string moduleName, string procName);
        bool InjectDll(string dllPath);
        void Detach();
        void StartAutoAttach();
    }
}