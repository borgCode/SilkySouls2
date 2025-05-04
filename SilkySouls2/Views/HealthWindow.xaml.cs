using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using SilkySouls2.Utilities;

namespace SilkySouls2.Views
{
    public partial class HealthWindow : Window
    {
        public HealthWindow()
        {
            InitializeComponent();
            
            MouseLeftButtonDown += (s, e) => DragMove();
            Background = new SolidColorBrush(
                Color.FromArgb(128, 0, 0, 0));
            
            Loaded += (s, e) =>
            {
                if (SettingsManager.Default.HealthWindowLeft > 0)
                    Left = SettingsManager.Default.HealthWindowLeft;
        
                if (SettingsManager.Default.HealthWindowTop > 0)
                    Top = SettingsManager.Default.HealthWindowTop;
                
                IntPtr hwnd = new WindowInteropHelper(this).Handle;
                User32.SetTopmost(hwnd);

                if (Application.Current.MainWindow != null)
                {
                    Application.Current.MainWindow.Closing += (sender, args) => { Close(); };
                }
            };
        }
        
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            
            SettingsManager.Default.HealthWindowLeft = Left;
            SettingsManager.Default.HealthWindowTop = Top;
            SettingsManager.Default.Save();
        }
    }
}