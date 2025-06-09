using System.Collections.Generic;

namespace SilkySouls2.Models
{
    public class ItemTemplate
    {
        public string ItemName { get; set; }
        public string Infusion { get; set; } = "Normal";
        public int Upgrade { get; set; }
        public int Quantity { get; set; } = 1;
    }

    public class LoadoutTemplate
    {
        public string Name { get; set; }
        public List<ItemTemplate> Items { get; set; }
    }

    public static class LoadoutTemplates
    {
        public static readonly List<LoadoutTemplate> All = new List<LoadoutTemplate> { };
    }

}