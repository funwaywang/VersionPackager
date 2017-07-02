using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VPackager
{
    public class PackageFolderViewModel : DependencyObject
    {
        public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(PackageFolderViewModel));
        public static readonly DependencyProperty TransitNameProperty = DependencyProperty.Register("TransitName", typeof(string), typeof(PackageFolderViewModel));

        public string Name
        {
            get => (string)GetValue(NameProperty);
            set => SetValue(NameProperty, value);
        }

        public string TransitName
        {
            get => (string)GetValue(TransitNameProperty);
            set => SetValue(TransitNameProperty, value);
        }

        public void LoadFrom(PackageFolder folder)
        {
            if (folder != null)
            {
                Name = folder.Name;
                TransitName = folder.TransitName;
            }
            else
            {
                Name = null;
                TransitName = null;
            }
        }

        public void SaveTo(PackageFolder folder)
        {
            if (folder != null)
            {
                folder.Name = Name;
                folder.TransitName = TransitName;
            }
        }
    }
}
