namespace SilkySouls2.Models
{
    public class EquippedSpell
    {
        public EquippedSpell(int spellId, int slot)
        {
            Id = spellId;
            Slot = slot;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public int Slot { get; set; }
    }
}