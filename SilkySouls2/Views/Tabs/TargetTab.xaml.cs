using System.Windows;
using System.Windows.Controls;
using SilkySouls2.ViewModels;

namespace SilkySouls2.Views.Tabs
{
    public partial class TargetTab
    {
        private readonly TargetViewModel _targetViewModel;

        public TargetTab(TargetViewModel targetViewModel)
        {
            InitializeComponent();
            _targetViewModel = targetViewModel;
            DataContext = _targetViewModel;
        }
        
        private void OnHealthButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string parameter = button.CommandParameter.ToString();
            int healthPercentage = int.Parse(parameter);
            _targetViewModel.SetTargetHealth(healthPercentage);
        }

        private void OpenDefenseWindow(object sender, RoutedEventArgs e) => _targetViewModel.OpenDefenseWindow();
    }
}