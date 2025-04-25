using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using SilkySouls2.ViewModels;
using Xceed.Wpf.Toolkit;

namespace SilkySouls2.Views
{
    public partial class PlayerTab
    {
        private readonly PlayerViewModel _playerViewModel;
        
        public PlayerTab(PlayerViewModel playerViewModel)
        {
            InitializeComponent();
            _playerViewModel = playerViewModel;
            DataContext = _playerViewModel;
        }
        
        private void SetRtsrClick(object sender, RoutedEventArgs e)
        {
            _playerViewModel.SetHp(1);
        }

        private void SetMaxHpClick(object sender, RoutedEventArgs e)
        {
            _playerViewModel.SetMaxHp();
        }

        private void HealthUpDown_Loaded(object sender, RoutedEventArgs e)
        {
            if (!(sender is IntegerUpDown upDown)) return;
            if (upDown.Template.FindName("PART_TextBox", upDown) is TextBox textBox)
            {
                textBox.GotFocus += PauseUpdates_GotFocus;
            }
            var spinner = upDown.Template.FindName("PART_Spinner", upDown);
            if (spinner == null) return;

            var type = spinner.GetType();

            var incField = type.GetField("_increaseButton", BindingFlags.Instance | BindingFlags.NonPublic);
            var decField = type.GetField("_decreaseButton", BindingFlags.Instance | BindingFlags.NonPublic);

            if (incField?.GetValue(spinner) is ButtonBase incBtn)
                incBtn.Click += SpinnerSetHp;

            if (decField?.GetValue(spinner) is ButtonBase decBtn)
                decBtn.Click += SpinnerSetHp;
        }


        private void PauseUpdates_GotFocus(object sender, RoutedEventArgs e)
        {
            _playerViewModel.PauseUpdates();
        }

        private void SpinnerSetHp(object sender, RoutedEventArgs e)
        {
            _playerViewModel.PauseUpdates();
            if (HealthUpDown.Value.HasValue)
            {
                _playerViewModel.SetHp(HealthUpDown.Value.Value);
            }

            _playerViewModel.ResumeUpdates();
        }

        private void HealthUpDown_LostFocus(object sender, RoutedEventArgs e)
        {
            if (HealthUpDown.Value.HasValue)
            {
                _playerViewModel.SetHp(HealthUpDown.Value.Value);
            }

            _playerViewModel.ResumeUpdates();
        }

        private void HealthUpDown_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter && e.Key != Key.Return) return;
            if (HealthUpDown.Value.HasValue)
            {
                _playerViewModel.SetHp(HealthUpDown.Value.Value);
            }

            Focus();

            e.Handled = true;
        }
    }
}