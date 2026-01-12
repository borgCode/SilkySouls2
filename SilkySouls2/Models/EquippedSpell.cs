namespace SilkySouls2.Models
{
    public class EquippedSpell(int spellId, int slot)
    {
        public int Id { get; set; } = spellId;
        public string Name { get; set; }
        public int Slot { get; set; } = slot;
    }
}