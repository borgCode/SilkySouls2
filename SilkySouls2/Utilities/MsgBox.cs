// 

using SilkySouls2.Views.Windows;

namespace SilkySouls2.Utilities
{
    /// <summary>
    /// Static helper class to show custom message boxes from anywhere in the application.
    /// </summary>
    public static class MsgBox
    {
        /// <summary>
        /// Shows a message box with only an OK button.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="title"></param>
        public static void Show(string message, string title = "Message")
        {
            var box = new CustomMessageBox(message, showCancel: false, title);
            box.ShowDialog();
        }

        /// <summary>
        /// Shows a message box with OK and Cancel buttons.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="title"></param>
        /// <returns>True if OK was clicked, false if Cancel was clicked.</returns>
        public static bool ShowOkCancel(string message, string title = "Message")
        {
            var box = new CustomMessageBox(message, showCancel: true, title);
            box.ShowDialog();
            return box.Result;
        }
    }
}