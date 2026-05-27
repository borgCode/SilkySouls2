// 

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using SilkySouls2.Utilities;

namespace SilkySouls2.Views.Windows
{
    public partial class CustomMessageBox : Window
    {
        public string SelectedButton { get; private set; } = string.Empty;

        public CustomMessageBox(string message, string title = "Message", params string[] buttons)
        {
            InitializeComponent();
            MessageText.Text = message;
            TitleText.Text = title;

            if (buttons.Length == 0)
                buttons = ["OK"];

            foreach (var label in buttons)
            {
                var button = new Button
                {
                    Content = label,
                    Width = 75,
                    Height = 26,
                    Margin = new Thickness(0, 0, 8, 0)
                };

                button.Click += (_, _) =>
                {
                    SelectedButton = label;
                    Close();
                };

                ButtonPanel.Children.Add(button);
            }
        
            Loaded += (s, e) =>
            {
                IntPtr hwnd = new WindowInteropHelper(this).Handle;
                User32.SetTopmost(hwnd);

                if (Application.Current.MainWindow != null)
                {
                    Application.Current.MainWindow.Closing += (sender, args) => { Close(); };
                }
            
            };
        }
        
        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}