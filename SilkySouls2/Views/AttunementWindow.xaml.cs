using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SilkySouls2.Models;
using SilkySouls2.ViewModels;


namespace SilkySouls2.Views
{
    public partial class AttunementWindow : Window
    {
        public AttunementWindow()
        {
            InitializeComponent();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            
            if (DataContext is UtilityViewModel viewModel)
            {
                var border = sender as Border;
                var spell = border.DataContext as InventorySpell;
                viewModel.HandleSpellAttune(spell.EntryAddress);
            }
            
        }
    }
}