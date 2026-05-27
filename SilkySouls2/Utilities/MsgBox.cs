//

using System;
using System.Collections.Generic;
using System.Windows;
using SilkySouls2.Views.Windows;

namespace SilkySouls2.Utilities
{
    public static class MsgBox
    {
        private static T OnUiThread<T>(Func<T> func)
        {
            var dispatcher = Application.Current?.Dispatcher;
            if (dispatcher == null || dispatcher.CheckAccess()) return func();
            return dispatcher.Invoke(func);
        }

        private static void OnUiThread(Action action) => OnUiThread<object>(() => { action(); return null; });

        public static string ShowChoice(string message, string title, params string[] buttons) => OnUiThread(() =>
        {
            var box = new CustomMessageBox(message, title, buttons);
            box.ShowDialog();
            return box.SelectedButton;
        });

        public static void Show(string message, string title = "Message") =>
            ShowChoice(message, title, "OK");

        public static bool ShowOkCancel(string message, string title = "Message") =>
            ShowChoice(message, title, "OK", "Cancel") == "OK";
        
        
        public static string ShowInput(string prompt, string defaultValue = "", string title = "Input") => OnUiThread(() =>
        {
            var box = new InputBox(prompt, defaultValue, title);
            box.ShowDialog();
            return box.Result ? box.InputValue : string.Empty;
        });

        public static Dictionary<string, string> ShowInputs(InputField[] fields, string title = "Input") => OnUiThread(() =>
        {
            var box = new InputBox(fields, title);
            box.ShowDialog();
            return box.Result ? box.GetValues() : null;
        });
    }
}