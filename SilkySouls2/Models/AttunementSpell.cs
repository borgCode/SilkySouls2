namespace SilkySouls2.Models
{
    public class AttunementSpell
    {
        public AttunementSpell(int spellId, string name, SpellType type)
        {
            Id = spellId;
            Name = name;   
            Type = type;   
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public SpellType Type { get; set; }
    }
    
    
    public enum SpellType
    {
        Sorcery,
        Miracle,
        Pyromancy,
        Hex
    }
}