using System;
using System.Windows;

namespace ACAD.Apparel.Notches.Plugin
{
    internal static class ExceptionHandler
    {
        internal static void ExecuteSafe(Action action)
        {
            try
            {
                action();
            }
            catch (Exception unhandledException)
            {
                // Instead of trying to bury the error in a log we'd better be sure user knows about it
                MessageBox.Show(
                    messageBoxText: unhandledException.ToString(),
                    caption: "Unhandled exception in Nothes plugin (copy error text using Ctrl + C)",
                    button: MessageBoxButton.OK,
                    icon: MessageBoxImage.Error);
            }
        }
    }
}
