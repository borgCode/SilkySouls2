using System;

namespace SilkySouls2.Models
{
    public class InventorySpell(int spellId, bool isEquipped, IntPtr entryAddress, int slotReq)
    {
        public int Id { get; set; } = spellId;
        public string Name { get; set; }
        public bool IsEquipped { get; set; } = isEquipped;
        public IntPtr EntryAddress { get; set; } = entryAddress;
        public int SlotReq { get; set; } = slotReq;
        public SpellType Type { get; set; }
    }
}