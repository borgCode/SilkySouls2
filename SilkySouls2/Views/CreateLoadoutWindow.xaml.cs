using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using SilkySouls2.Models;

namespace SilkySouls2.Views
{
    public partial class CreateLoadoutWindow : Window
    {
        public CreateLoadoutWindow()
        {
            InitializeComponent();
        }

        public CreateLoadoutWindow(ObservableCollection<string> categories,
            Dictionary<string, ObservableCollection<Item>> itemsByCategory,
            Dictionary<string, LoadoutTemplate> loadoutTemplatesByName,
            Dictionary<string, LoadoutTemplate> customLoadoutTemplates, string[] infusionNames)
        {
            
        }
    }
}