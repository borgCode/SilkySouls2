// 

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
        public int EffectId = effectId;
        public int Quantity = quantity;
        public float FloatValue = floatValue;
        public byte EffectType = effectType;
        public byte Param1 = param1;
        public byte Param2 = param2;
        public byte Param3 = param3;

        public static readonly SpEffect RestoreHumanity = new SpEffect(
            60151000, 1, -1.0f, 0x19, 0x02, 0x00, 0x00
        );

        public static readonly SpEffect BonfireRest = new SpEffect(
            110000010, 1, -1.0f, 0x19, 0x02, 0x00, 0x00
        );
    }
}