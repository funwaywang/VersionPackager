using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VPackager
{
    public class PackageFileViewModel : DependencyObject
    {
        public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(PackageFileViewModel));
        public static readonly DependencyProperty TransitNameProperty = DependencyProperty.Register("TransitName", typeof(string), typeof(PackageFileViewModel));
        public static readonly DependencyProperty PathProperty = DependencyProperty.Register("Path", typeof(string), typeof(PackageFileViewModel));
        public static readonly DependencyProperty ActionProperty = DependencyProperty.Register("Action", typeof(FileAction), typeof(PackageFileViewModel));
        public static readonly DependencyProperty CompareModeProperty = DependencyProperty.Register("CompareMode", typeof(FileCompareMode), typeof(PackageFileViewModel));
        public static readonly DependencyProperty VersionProperty = DependencyProperty.Register("Version", typeof(string), typeof(PackageFileViewModel));

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

        public string Path
        {
            get => (string)GetValue(PathProperty);
            set => SetValue(PathProperty, value);
        }

        public FileAction Action
        {
            get => (FileAction)GetValue(ActionProperty);
            set => SetValue(ActionProperty, value);
        }

        public FileCompareMode CompareMode
        {
            get => (FileCompareMode)GetValue(CompareModeProperty);
            set => SetValue(CompareModeProperty, value);
        }

        public string Version
        {
            get => (string)GetValue(VersionProperty);
            set => SetValue(VersionProperty, value);
        }

        public void LoadFrom(PackageFile file)
        {
            if (file != null)
            {
                Name = file.Name;
                TransitName = file.TransitName;
                Path = file.Path;
                Action = file.Action;
                CompareMode = file.CompareMode;
                Version = file.Version;
            }
            else
            {
                Name = null;
                TransitName = null;
                Path = null;
                Action = FileAction.Default;
                CompareMode = FileCompareMode.Default;
                Version = null;
            }
        }

        public void SaveTo(PackageFile file)
        {
            if(file != null)
            {
                file.Name = Name;
                file.TransitName = TransitName;
                file.Path = Path;
                file.Action = Action;
                file.CompareMode = CompareMode;
                file.Version = Version;
            }
        }
    }
}
