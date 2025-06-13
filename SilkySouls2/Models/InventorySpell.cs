using System;

namespace SilkySouls2.Models
{
    public class InventorySpell
    {
        public InventorySpell(int spellId, bool isEquipped, IntPtr entryAddress)
        {
            Id = spellId;
            IsEquipped = isEquipped;
            EntryAddress = entryAddress;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsEquipped { get; set; }
        public IntPtr EntryAddress { get; set; }
    }
}