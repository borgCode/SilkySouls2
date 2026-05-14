using System;

namespace SilkySouls2.Utilities
{
    public static class AsmHelper
    {
        
        public static int GetRelOffset(long srcInstrAddr, long targetAddr, int instrLength = 0)
            => (int)(targetAddr - (srcInstrAddr + instrLength));

        public static byte[] GetRelOffsetBytes(long srcInstrAddr, long targetAddr, int instrLength = 0)
            => BitConverter.GetBytes(GetRelOffset(srcInstrAddr, targetAddr, instrLength));

        public static void WriteRelativeOffsets(byte[] bytes,
            (nint baseAddr, nint targetAddr, int size, int destinationIndex)[] offsets)
        {
            foreach (var (baseAddr, targetAddr, size, destinationIndex) in offsets)
            {
                var relativeBytes = GetRelOffsetBytes(baseAddr, targetAddr, size);
                Array.Copy(relativeBytes, 0, bytes, destinationIndex, 4);
            }
        }

        public static void WriteRelativeOffset(byte[] buffer, nint instructionAddress, nint targetAddress, int instructionLength, int writeOffset)
        {
            var relativeBytes = GetRelOffsetBytes(instructionAddress, targetAddress, instructionLength);
            Array.Copy(relativeBytes, 0, buffer, writeOffset, 4);
        }

        public static byte[] GetJmpOriginOffsetBytes(nint hookLocation, int originalInstrLen, IntPtr customCodeEnd)
            => BitConverter.GetBytes((int)(hookLocation + originalInstrLen - customCodeEnd));

        public static void WriteJumpOffsets(byte[] bytes,
            (nint hookLocation, int originalInstrLen, IntPtr customCodeAddr, int destinationIndex)[] jumpOffsets)
        {
            foreach (var (hookLocation, originalInstrLen, customCodeAddr, destinationIndex) in jumpOffsets)
            {
                var originOffsetBytes = GetJmpOriginOffsetBytes(hookLocation, originalInstrLen, customCodeAddr + 5);
                Array.Copy(originOffsetBytes, 0, bytes, destinationIndex, 4);
            }
        }
        
        public static byte[] GetAbsAddressBytes(nint address)
            => BitConverter.GetBytes(address);
        
        /// <summary>
        /// Writes a single 64-bit absolute address into a byte buffer.
        /// </summary>
        /// <param name="buffer">The byte array to write into.</param>
        /// <param name="address">The absolute address to write.</param>
        /// <param name="writeOffset">Index in the buffer to write the 8-byte address.</param>
        public static void WriteAbsoluteAddress64(byte[] buffer, nint address, int writeOffset)
        {
            var addressBytes = GetAbsAddressBytes(address);
            Array.Copy(addressBytes, 0, buffer, writeOffset, 8);
        }
        
        public static void WriteAbsoluteAddresses64(byte[] bytes, (nint address, int destinationIndex)[] addresses)
        {
            foreach (var (address, destinationIndex) in addresses)
            {
                var addressBytes = GetAbsAddressBytes(address);
                Array.Copy(addressBytes, 0, bytes, destinationIndex, 8);
            }
        }
        
        public static void WriteAbsoluteAddresses32(byte[] bytes, (nint address, int destinationIndex)[] addresses)
        {
            foreach (var (address, destinationIndex) in addresses)
            {
                var addressBytes = BitConverter.GetBytes((int)address);
                Array.Copy(addressBytes, 0, bytes, destinationIndex, 4);
            }
        }

        public static void WriteAbsoluteAddress32(byte[] buffer, nint address, int writeOffset)
        {
            var addressBytes = BitConverter.GetBytes((int)address);
            Array.Copy(addressBytes, 0, buffer, writeOffset, 4);
        }
        
        public static void WriteImmediateDwords(byte[] bytes, (int value, int destinationIndex)[] immediates)
        {
            foreach (var (value, destinationIndex) in immediates)
            {
                var valueBytes = BitConverter.GetBytes(value);
                Array.Copy(valueBytes, 0, bytes, destinationIndex, 4);
            }
        }
        
        /// <summary>
        /// Writes a single 32-bit immediate value into a byte buffer.
        /// </summary>
        /// <param name="buffer">The byte array to write into.</param>
        /// <param name="value">The 32-bit value to write.</param>
        /// <param name="writeOffset">Index in the buffer to write the 4-byte value.</param>
        public static void WriteImmediateDword(byte[] buffer, int value, int writeOffset)
        {
            var valueBytes = BitConverter.GetBytes(value);
            Array.Copy(valueBytes, 0, buffer, writeOffset, 4);
        }

        public static byte[] BuildNearCall(nint srcAddr, nint target)
        {
            var buf = new byte[5];
            buf[0] = 0xE8;
            BitConverter.GetBytes((int)(target - (srcAddr + 5))).CopyTo(buf, 1);
            return buf;
        }
    }
}