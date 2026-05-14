using System;

namespace SilkySouls2.GameIds
{
    public class SpEffect(
        int effectId,
        int quantity,
        float floatValue,
        byte effectType,
        byte param1,
        byte param2,
        byte param3)
    {
        public readonly int EffectId = effectId;
        public readonly int Quantity = quantity;
        public readonly float FloatValue = floatValue;
        public readonly byte EffectType = effectType;
        public readonly byte Param1 = param1;
        public readonly byte Param2 = param2;
        public readonly byte Param3 = param3;

        public byte[] ToBytes()
        {
            var buf = new byte[0x10];
            BitConverter.GetBytes(EffectId).CopyTo(buf, 0x0);
            BitConverter.GetBytes(Quantity).CopyTo(buf, 0x4);
            BitConverter.GetBytes(FloatValue).CopyTo(buf, 0x8);
            buf[0xC] = EffectType;
            buf[0xD] = Param1;
            buf[0xE] = Param2;
            buf[0xF] = Param3;
            return buf;
        }

        public static readonly SpEffect RestoreHumanity = new(
            60151000, 1, -1.0f, 0x19, 0x02, 0x00, 0x00
        );

        public static readonly SpEffect BonfireRest = new(
            110000010, 1, -1.0f, 0x19, 0x02, 0x00, 0x00
        );
    }
}