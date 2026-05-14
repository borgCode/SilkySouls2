namespace SilkySouls2.enums
{
    public enum NpcMenuType : byte
    {
        SpellDecrementState = 0x01,        //Incense
        ShopBuy = 0x03,
        Repair = 0x04,
        Reinforce = 0x05,
        ChangeAttribute = 0x6,          //Infuse
        Trade = 0x0A,
        EquipmentPresent = 0x0B,        //Give Melfia Armor
        VowList = 0x12,                 //Covenants
        ReinforcePyro = 0x15,
        HeraldLevelUp = 0x19,
        ReallocateStats = 0x1A,
    }
}