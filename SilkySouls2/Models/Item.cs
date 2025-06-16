namespace SilkySouls2.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int StackSize { get; set; }
        public int InfuseId { get; set; }
        public int MaxUpgrade { get; set; }
        public float Durability { get; set; }
        public string CategoryName { get; set; }
        
    }
}