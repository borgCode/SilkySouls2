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
            
            if (Application.Current.MainWindow != null)
            {
                Application.Current.MainWindow.Closing += (sender, args) => { Close(); };
            }
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            
            if (DataContext is UtilityViewModel viewModel)
            {
                var border = sender as Border;
                var spell = border.DataContext as InventorySpell;
                viewModel.HandleSpellAttune(spell);
            }
            
        }

        private void UnequipSpell_Click(object sender, MouseButtonEventArgs e)
        {
            var textBlock = sender as TextBlock;

            if (textBlock?.DataContext is EquippedSpell spell)
            {
                var viewModel = this.DataContext as UtilityViewModel;
                int index = viewModel.EquippedSpells.IndexOf(spell);
                viewModel.HandleUnAttune(index);
            }
        }
    }
}