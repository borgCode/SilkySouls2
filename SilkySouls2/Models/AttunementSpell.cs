namespace SilkySouls2.Models
{
    public class AttunementSpell(int spellId, string name, SpellType type)
    {
        public int Id { get; set; } = spellId;
        public string Name { get; set; } = name;
        public SpellType Type { get; set; } = type;
    }
    
    
    public enum SpellType
    {
        Sorcery,
        Miracle,
        Pyromancy,
        Hex
    }
}