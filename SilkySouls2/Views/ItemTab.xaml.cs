using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SilkySouls2.ViewModels;

namespace SilkySouls2.Views
{
    public partial class ItemTab
    {
        private readonly ItemViewModel _itemViewModel;
        public ItemTab(ItemViewModel itemViewModel)
        {
            InitializeComponent();
            _itemViewModel = itemViewModel;
            DataContext = _itemViewModel;
        }

        private void SpawnButton_Click(object sender, RoutedEventArgs e) => _itemViewModel.SpawnItem();

        private void AutoSpawn_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void MassSpawn_Click(object sender, RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void LoadPreset_Click(object sender, RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }
    }
}