using System;
using System.Collections.Generic;
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
    public partial class FolderPropertyDialog : Window
    {
        public static readonly DependencyProperty FolderProperty = DependencyProperty.Register("Folder", typeof(PackageFolder), typeof(FolderPropertyDialog));
        public static readonly DependencyProperty ErrorMessageProperty = DependencyProperty.Register("ErrorMessage", typeof(string), typeof(FolderPropertyDialog));

        public FolderPropertyDialog()
        {
            InitializeComponent();
        }

        public PackageFolderViewModel Model { get; private set; } = new PackageFolderViewModel();

        public PackageFolder Folder
        {
            get => (PackageFolder)GetValue(FolderProperty);
            set => SetValue(FolderProperty, value);
        }

        public string ErrorMessage
        {
            get => (string)GetValue(ErrorMessageProperty);
            set => SetValue(ErrorMessageProperty, value);
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == FolderProperty)
            {
                Model.LoadFrom(e.NewValue as PackageFolder);
            }
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if(Folder != null)
            {
                Model.SaveTo(Folder);

                DialogResult = true;
                Close();
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
