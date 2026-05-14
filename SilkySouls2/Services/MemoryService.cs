using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Timers;
using SilkySouls2.Interfaces;
using SilkySouls2.Memory;

namespace SilkySouls2.Services
{
    public class MemoryService : IMemoryService
    {
        public bool IsAttached { get; private set; }
        public Process? TargetProcess { get; private set; }
        public IntPtr ProcessHandle { get; private set; } = IntPtr.Zero;
        public nint BaseAddress { get; private set; }
        public int ModuleMemorySize { get; private set; }

        private const int ProcessVmRead = 0x0010;
        private const int ProcessVmWrite = 0x0020;
        private const int ProcessVmOperation = 0x0008;
        public const int ProcessQueryInformation = 0x0400;

        private const string ProcessName = "darksoulsii";
        private bool _disposed;

        private Timer _autoAttachTimer;

        public void StartAutoAttach()
        {
            _autoAttachTimer = new Timer(4000);
            _autoAttachTimer.Elapsed += (sender, e) => TryAttachToProcess();

            TryAttachToProcess();

            _autoAttachTimer.Start();
        }

        private void TryAttachToProcess()
        {
            if (ProcessHandle != IntPtr.Zero)
            {
                if (TargetProcess == null || TargetProcess.HasExited)
                {
                    Kernel32.CloseHandle(ProcessHandle);
                    ProcessHandle = IntPtr.Zero;
                    TargetProcess = null;
                    IsAttached = false;
                }

                return;
            }

            var processes = Process.GetProcessesByName(ProcessName);
            if (processes.Length > 0 && !processes[0].HasExited)
            {
                TargetProcess = processes[0];
                ProcessHandle = Kernel32.OpenProcess(
                    ProcessVmRead | ProcessVmWrite | ProcessVmOperation | ProcessQueryInformation,
                    false,
                    TargetProcess.Id);

                if (ProcessHandle == IntPtr.Zero)
                {
                    TargetProcess = null;
                    IsAttached = false;
                }
                else
                {
                    if (TargetProcess.MainModule != null)
                    {
                        BaseAddress = TargetProcess.MainModule.BaseAddress;
                        ModuleMemorySize = TargetProcess.MainModule.ModuleMemorySize;
                    }

                    IsAttached = true;
                }
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                if (_autoAttachTimer != null)
                {
                    _autoAttachTimer.Stop();
                    _autoAttachTimer.Dispose();
                    _autoAttachTimer = null;
                }

                if (ProcessHandle != IntPtr.Zero)
                {
                    Kernel32.CloseHandle(ProcessHandle);
                    ProcessHandle = IntPtr.Zero;
                    TargetProcess = null;
                    IsAttached = false;
                }

                _disposed = true;
            }

            GC.SuppressFinalize(this);
        }

        public T Read<T>(nint addr) where T : unmanaged
        {
            int size = Unsafe.SizeOf<T>();
            var bytes = ReadBytes(addr, size);
            return MemoryMarshal.Read<T>(bytes);
        }

        public void Write<T>(IntPtr addr, T value) where T : unmanaged
        {
            int size = Unsafe.SizeOf<T>();
            var bytes = new byte[size];
            MemoryMarshal.Write(bytes, ref value);
            WriteBytes(addr, bytes);
        }

        public void Write(IntPtr addr, bool value) =>
            Write(addr, value ? (byte)1 : (byte)0);

        ~MemoryService()
        {
            Dispose();
        }

        public bool IsBitSet(IntPtr addr, int flagMask)
        {
            byte currentByte = Read<byte>(addr);

            return (currentByte & flagMask) != 0;
        }

        public uint RunThread(IntPtr address, uint timeout = 0xFFFFFFFF)
        {
            IntPtr thread =
                Kernel32.CreateRemoteThread(ProcessHandle, IntPtr.Zero, 0, address, IntPtr.Zero, 0, IntPtr.Zero);
            var ret = Kernel32.WaitForSingleObject(thread, timeout);
            Kernel32.CloseHandle(thread);
            return ret;
        }

        public void RunPersistentThread(IntPtr address)
        {
            IntPtr thread =
                Kernel32.CreateRemoteThread(ProcessHandle, IntPtr.Zero, 0, address, IntPtr.Zero, 0, IntPtr.Zero);
            Kernel32.CloseHandle(thread);
        }

        public bool RunThreadAndWaitForCompletion(IntPtr address, uint timeout = 0xFFFFFFFF)
        {
            IntPtr thread =
                Kernel32.CreateRemoteThread(ProcessHandle, IntPtr.Zero, 0, address, IntPtr.Zero, 0, IntPtr.Zero);

            if (thread == IntPtr.Zero)
            {
                return false;
            }

            uint waitResult = Kernel32.WaitForSingleObject(thread, timeout);
            Kernel32.CloseHandle(thread);

            return waitResult == 0;
        }

        public void AllocateAndExecute(byte[] shellcode)
        {
            IntPtr allocatedMemory = Kernel32.VirtualAllocEx(ProcessHandle, IntPtr.Zero, (uint)shellcode.Length);

            if (allocatedMemory == IntPtr.Zero) return;

            WriteBytes(allocatedMemory, shellcode);
            bool executionSuccess = RunThreadAndWaitForCompletion(allocatedMemory);

            if (!executionSuccess) return;

            Kernel32.VirtualFreeEx(ProcessHandle, allocatedMemory, 0, 0x8000);
        }
        
        public nint ReadPointer(nint addr) =>
            PatchManager.Current.Edition == GameEdition.Scholar ? Read<nint>(addr) : Read<int>(addr);

        
        public byte[] ReadBytes(IntPtr addr, int size)
        {
            var array = new byte[size];
            var lpNumberOfBytesRead = 1;
            Kernel32.ReadProcessMemory(ProcessHandle, addr, array, size, ref lpNumberOfBytesRead);
            return array;
        }

        public void WriteBytes(IntPtr addr, byte[] val)
        {
            Kernel32.WriteProcessMemory(ProcessHandle, addr, val, val.Length, 0);
        }

        public void WriteWString(nint addr, string value, int maxChars = 32)
        {
            var charsToWrite = Math.Min(value.Length, maxChars - 1);
            var bytes = new byte[maxChars * 2];

            Encoding.Unicode.GetBytes(value, 0, charsToWrite, bytes, 0);

            WriteBytes(addr, bytes);
        }

        public void SetBitValue(IntPtr addr, int flagMask, bool setValue)
        {
            byte currentByte = Read<byte>(addr);
            byte modifiedByte;

            if (setValue)
                modifiedByte = (byte)(currentByte | flagMask);
            else
                modifiedByte = (byte)(currentByte & ~flagMask);
            Write(addr, modifiedByte);
        }



        public nint FollowPointers(nint baseAddress, int[] offsets, bool readFinalPtr)
        {
            var ptr = ReadPointer(baseAddress);

            for (int i = 0; i < offsets.Length - 1; i++)
                ptr = ReadPointer(ptr + offsets[i]);

            var finalAddress = ptr + offsets[offsets.Length - 1];

            return readFinalPtr ? ReadPointer(finalAddress) : finalAddress;
        }

        public void SetBitValue(IntPtr addr, byte flagMask, bool setValue)
        {
            byte currentByte = Read<byte>(addr);
            byte modifiedByte;

            if (setValue)
                modifiedByte = (byte)(currentByte | flagMask);
            else
                modifiedByte = (byte)(currentByte & ~flagMask);
            Write(addr, modifiedByte);
        }

        public bool IsBitSet(IntPtr addr, byte flagMask)
        {
            byte currentByte = Read<byte>(addr);

            return (currentByte & flagMask) != 0;
        }

        public void SetBit32(IntPtr addr, int bitPosition, bool setValue)
        {
            IntPtr wordAddr = IntPtr.Add(addr, (bitPosition / 32) * 4);

            int bitPos = bitPosition % 32;

            uint currentValue = Read<uint>(wordAddr);

            uint bitMask = 1u << bitPos;

            uint newValue = setValue
                ? currentValue | bitMask
                : currentValue & ~bitMask;

            Write(wordAddr, (int)newValue);
        }

        public void AllocCodeCave()
        {
            if (PatchManager.Current.Edition == GameEdition.Scholar)
            {
                IntPtr searchRangeStart = BaseAddress - 0x40000000;
                IntPtr searchRangeEnd = BaseAddress - 0x30000;
                uint codeCaveSize = 0x5000;

                for (IntPtr addr = searchRangeEnd; addr.ToInt64() > searchRangeStart.ToInt64(); addr -= 0x10000)
                {
                    var allocatedMemory = Kernel32.VirtualAllocEx(ProcessHandle, addr, codeCaveSize);

                    if (allocatedMemory != IntPtr.Zero)
                    {
                        CustomCodeOffsets.Base = allocatedMemory;
                        break;
                    }
                }
            }
            else
            {
                IntPtr moduleEnd = new IntPtr(BaseAddress + GetFileSize());
                IntPtr searchRangeStart = moduleEnd + 0x10000;
                IntPtr searchRangeEnd = BaseAddress + 0x7F000000;
                uint codeCaveSize = 0x4000;

                for (IntPtr addr = searchRangeStart; addr.ToInt64() < searchRangeEnd.ToInt64(); addr += 0x10000)
                {
                    var allocatedMemory = Kernel32.VirtualAllocEx(ProcessHandle, addr, codeCaveSize);
                    if (allocatedMemory != IntPtr.Zero)
                    {
                        CustomCodeOffsets.Base = allocatedMemory;
                        break;
                    }
                }
            }
        }

        public bool InjectDll(string dllPath)
        {
            if (!IsAttached || ProcessHandle == IntPtr.Zero)
            {
                Console.WriteLine("Not attached to process");
                return false;
            }

            if (!File.Exists(dllPath))
            {
                Console.WriteLine($"DLL not found: {dllPath}");
                return false;
            }

            try
            {
                string fullDllPath = Path.GetFullPath(dllPath);

                byte[] dllPathBytes = Encoding.Unicode.GetBytes(fullDllPath + "\0");
                IntPtr dllPathAddress = Kernel32.VirtualAllocEx(
                    ProcessHandle,
                    IntPtr.Zero,
                    (uint)dllPathBytes.Length);

                if (dllPathAddress == IntPtr.Zero)
                {
                    Console.WriteLine("Failed to allocate memory for DLL path");
                    return false;
                }

                WriteBytes(dllPathAddress, dllPathBytes);

                IntPtr loadLibraryAddr;
                if (PatchManager.Current.Edition == GameEdition.Scholar)
                    loadLibraryAddr = GetProcAddress("kernel32.dll", "LoadLibraryW");
                else loadLibraryAddr = (IntPtr)Read<int>(Offsets.LoadLibraryW);

                if (loadLibraryAddr == IntPtr.Zero)
                {
                    Console.WriteLine("Failed to get LoadLibraryW address");
                    Kernel32.VirtualFreeEx(ProcessHandle, dllPathAddress, 0, 0x8000); // MEM_RELEASE
                    return false;
                }

                IntPtr threadHandle = Kernel32.CreateRemoteThread(
                    ProcessHandle,
                    IntPtr.Zero,
                    0,
                    loadLibraryAddr,
                    dllPathAddress,
                    0,
                    IntPtr.Zero);

                if (threadHandle == IntPtr.Zero)
                {
                    Console.WriteLine("Failed to create remote thread");
                    Kernel32.VirtualFreeEx(ProcessHandle, dllPathAddress, 0, 0x8000);
                    return false;
                }

                uint result = Kernel32.WaitForSingleObject(threadHandle, 5000);
                Kernel32.CloseHandle(threadHandle);

                Kernel32.VirtualFreeEx(ProcessHandle, dllPathAddress, 0, 0x8000);

                return result == 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error injecting DLL: {ex.Message}");
                return false;
            }
        }

        public IntPtr GetProcAddress(string moduleName, string procName)
        {
            IntPtr moduleHandle = Kernel32.GetModuleHandle(moduleName);
            if (moduleHandle == IntPtr.Zero)
                return IntPtr.Zero;

            return Kernel32.GetProcAddress(moduleHandle, procName);
        }

        private long GetFileSize()
        {
            if (TargetProcess.MainModule == null) return 0;
            var fileInfo = new FileInfo(TargetProcess.MainModule.FileName);
            Console.WriteLine($"FileVersion: {TargetProcess.MainModule.FileVersionInfo.FileVersion}");
            return fileInfo.Length;
        }

        public void Detach()
        {
            if (ProcessHandle != IntPtr.Zero)
            {
                Kernel32.CloseHandle(ProcessHandle);
                ProcessHandle = IntPtr.Zero;
            }

            TargetProcess = null;
            IsAttached = false;
        }

    }
}