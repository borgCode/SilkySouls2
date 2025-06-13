namespace SilkySouls2.Models
{
    public class InventorySpell
    {
        public InventorySpell(int spellId, bool isEquipped)
        {
            Id = spellId;
            IsEquipped = isEquipped;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsEquipped { get; set; }
    }
}