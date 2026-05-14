// 

namespace SilkySouls2.Memory
{
    public class BitFlag(int offset, int mask)
    {
        public readonly int Offset = offset;
        public readonly int Mask = mask;
    }
}