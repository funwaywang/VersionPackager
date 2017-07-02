using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VPackager;

namespace System.Windows
{
    public static class MessageBoxExtensions
    {
        public static void ShowException(this FrameworkElement source, Exception ex)
        {
            var message = ex.Message;
            if (message != null)
                message = message.Trim();
            message = Lang.GetText(message);

            var window = Window.GetWindow(source);
            if(window != null)
                MessageBox.Show(window, message, Lang.GetText("Exception"), MessageBoxButton.OK, MessageBoxImage.Warning);
            else
                MessageBox.Show(message, Lang.GetText("Exception"), MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public static bool ShowConfirmBox(this FrameworkElement source, string message, string caption = null)
        {
            var dialog = new ConfirmDialog()
            {
                Message = message,
                Owner = Window.GetWindow(source)
            };

            if (caption == null)
            {
                dialog.Title = Lang.GetText(App.Name);
            }

            return dialog.ShowDialog() == true;
        }

        public static void ShowInformationBox(this FrameworkElement source, string message, string caption = null)
        {
            var dialog = new InformationDialog()
            {
                Message = message,
                Owner = Window.GetWindow(source)
            };

            if (caption == null)
            {
                dialog.Title = Lang.GetText(App.Name);
            }

            dialog.ShowDialog();
        }

        public static void ShowErrorMessageBox(this FrameworkElement source, string message, string caption = null)
        {
            if (string.IsNullOrEmpty(message))
                message = Lang.GetText("Unknown Error");
            else
                message = Lang.GetText(message);

            var dialog = new InformationDialog()
            {
                Message = message,
                Owner = Window.GetWindow(source)
            };

            if (caption == null)
            {
                dialog.Title = Lang.GetText(App.Name);
            }

            dialog.ShowDialog();
        }
    }
}
