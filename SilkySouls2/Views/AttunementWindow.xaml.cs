using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SilkySouls2.Models;
using SilkySouls2.Utilities;
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
            
            Loaded += (s, e) =>
            {
                if (SettingsManager.Default.AttunementWindowLeft > 0)
                    Left = SettingsManager.Default.AttunementWindowLeft;
        
                if (SettingsManager.Default.AttunementWindowTop > 0)
                    Top = SettingsManager.Default.AttunementWindowTop;
            };
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
        
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);


            SettingsManager.Default.AttunementWindowLeft = Left;
            SettingsManager.Default.AttunementWindowTop = Top;
            SettingsManager.Default.Save();
        }

        
        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (WindowState == WindowState.Maximized)
                    WindowState = WindowState.Normal;
                else
                    WindowState = WindowState.Maximized;
            }
            else
            {
                DragMove();
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;
        private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();
    }
}