using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace VPackager
{
    public partial class NewFolderDialog : Window
    {
        public static readonly DependencyProperty FolderNameProperty = DependencyProperty.Register("FolderName", typeof(string), typeof(NewFolderDialog));
        public static readonly DependencyProperty ErrorMessageProperty = DependencyProperty.Register("ErrorMessage", typeof(string), typeof(NewFolderDialog));

        public event ValidatingEventHandler Validating;

        public NewFolderDialog()
        {
            InitializeComponent();
        }

        public string FolderName
        {
            get => (string)GetValue(FolderNameProperty);
            set => SetValue(FolderNameProperty, value);
        }

        public string ErrorMessage
        {
            get => (string)GetValue(ErrorMessageProperty);
            set => SetValue(ErrorMessageProperty, value);
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            ErrorMessage = null;

            if (string.IsNullOrEmpty(FolderName))
            {
                ErrorMessage = Lang.GetText("Please input new folder name");
                TxbFolderName.Focus();
                return;
            }

            FolderName = FolderName.Trim();
            if (!StringHelper.IsRightFileName(FolderName))
            {
                ErrorMessage = Lang.GetText("Invalid Folder Name");
                return;
            }

            var args = new ValidatingEventArgs();
            Validating?.Invoke(this, args);
            if (!args.IsOK)
            {
                ErrorMessage = Lang.GetText(args.ErrorMessage);
                return;
            }

            DialogResult = true;
            Close();
        }
    }
}
