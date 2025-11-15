using System.Windows;
using System.Windows.Controls;
using SilkySouls2.ViewModels;

namespace SilkySouls2.Views.Tabs
{
    public partial class EnemyTab : UserControl
    {
        private readonly EnemyViewModel _enemyViewModel;

        public EnemyTab(EnemyViewModel enemyViewModel)
        {
            InitializeComponent();
            _enemyViewModel = enemyViewModel;
            DataContext = _enemyViewModel;
        }
        
        private void SpawnForlorn(object sender, RoutedEventArgs e)
        {
            _enemyViewModel.SpawnForlorn();
        }
    }
}