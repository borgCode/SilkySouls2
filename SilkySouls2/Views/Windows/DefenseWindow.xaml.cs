﻿using System.Windows;
using System.Windows.Input;
using SilkySouls2.Utilities;

namespace SilkySouls2.Views.Windows
{
    public partial class DefenseWindow : Window
    {
        public DefenseWindow()
        {
            InitializeComponent();
            
            if (Application.Current.MainWindow != null)
            {
                Application.Current.MainWindow.Closing += (sender, args) => { Close(); };
            }
            
            Loaded += (s, e) =>
            {
                if (SettingsManager.Default.DefenseWindowLeft > 0)
                    Left = SettingsManager.Default.DefenseWindowLeft;
        
                if (SettingsManager.Default.DefenseWindowTop > 0)
                    Top = SettingsManager.Default.DefenseWindowTop;
            };
        }
        
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);


            SettingsManager.Default.DefenseWindowLeft = Left;
            SettingsManager.Default.DefenseWindowTop = Top;
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