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
    public partial class FilePropertyDialog : Window
    {
        public static readonly DependencyProperty FileItemProperty = DependencyProperty.Register("FileItem", typeof(PackageFile), typeof(FilePropertyDialog));
        public static readonly DependencyProperty ErrorMessageProperty = DependencyProperty.Register("ErrorMessage", typeof(string), typeof(FilePropertyDialog));

        public FilePropertyDialog()
        {
            InitializeComponent();
        }

        public FileAction[] AllFileActions
        {
            get
            {
                return Enum.GetValues(typeof(FileAction)).OfType<FileAction>().ToArray();
            }
        }

        public FileCompareMode[] AllCompareModes
        {
            get
            {
                return Enum.GetValues(typeof(FileCompareMode)).OfType<FileCompareMode>().ToArray();
            }
        }

        public PackageFile FileItem
        {
            get => (PackageFile)GetValue(FileItemProperty);
            set => SetValue(FileItemProperty, value);
        }

        public string ErrorMessage
        {
            get => (string)GetValue(ErrorMessageProperty);
            set => SetValue(ErrorMessageProperty, value);
        }

        public PackageFileViewModel Model { get; private set; } = new PackageFileViewModel();

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == FileItemProperty)
            {
                Model.LoadFrom(e.NewValue as PackageFile);
            }
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if(FileItem != null)
            {
                Model.SaveTo(FileItem);

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
