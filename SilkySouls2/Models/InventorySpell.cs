using System;

namespace SilkySouls2.Models
{
    public class InventorySpell
    {
        public InventorySpell(int spellId, bool isEquipped, IntPtr entryAddress, int slotReq)
        {
            Id = spellId;
            IsEquipped = isEquipped;
            EntryAddress = entryAddress;
            SlotReq = slotReq;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsEquipped { get; set; }
        public IntPtr EntryAddress { get; set; }
        public int SlotReq { get; set; }
        public SpellType Type { get; set; }
    }
}