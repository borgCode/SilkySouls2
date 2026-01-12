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

        bool IsGameLoaded();
        
        T Read<T>(nint addr) where T : unmanaged;
        
        void Write<T>(IntPtr addr, T value) where T : unmanaged;
        void Write(IntPtr addr, bool value);
        
        int ReadInt32(nint addr);
        long ReadInt64(nint addr);
        string ReadString(nint addr, int maxLength = 32);
        byte[] ReadBytes(nint addr, int size);

        void WriteByte(IntPtr addr, int value);
        void WriteInt32(nint addr, int val);
        void WriteInt64(nint addr, long val);
        void WriteFloat(nint addr, float val);
        void WriteBytes(IntPtr addr, byte[] val);
        void SetBitValue(nint addr, int flagMask, bool setValue);
        bool IsBitSet(nint addr, int flagMask);

        void RunPersistentThread(IntPtr address);
        IntPtr FollowPointers(IntPtr baseAddress, int[] offsets, bool readFinalPtr);
        
        uint RunThread(nint address, uint timeout = uint.MaxValue);
        bool RunThreadAndWaitForCompletion(nint address, uint timeout = uint.MaxValue);
        
        void AllocateAndExecute(byte[] shellcode);
        void AllocCodeCave();

        public IntPtr GetProcAddress(string moduleName, string procName);
        bool InjectDll(string dllPath);
        void Detach();
        bool IsLoadingScreen();
        void StartAutoAttach();
    }
}