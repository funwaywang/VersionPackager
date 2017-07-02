using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VPackager
{
    public class PackageViewModel : DependencyObject
    {
        public static readonly DependencyProperty OutputDirectoryProperty = DependencyProperty.Register("OutputDirectory", typeof(string), typeof(PackageViewModel));
        public static readonly DependencyProperty OutputFileProperty = DependencyProperty.Register("OutputFile", typeof(string), typeof(PackageViewModel));
        public static readonly DependencyProperty VersionPatternProperty = DependencyProperty.Register("VersionPattern", typeof(string), typeof(PackageViewModel));
        public static readonly DependencyProperty BuildAllInOnePackageProperty = DependencyProperty.Register("BuildAllInOnePackage", typeof(bool), typeof(PackageViewModel));

        public string OutputDirectory
        {
            get => (string)GetValue(OutputDirectoryProperty);
            set => SetValue(OutputDirectoryProperty, value);
        }

        public string OutputFile
        {
            get => (string)GetValue(OutputFileProperty);
            set => SetValue(OutputFileProperty, value);
        }

        public string VersionPattern
        {
            get => (string)GetValue(VersionPatternProperty);
            set => SetValue(VersionPatternProperty, value);
        }

        public bool BuildAllInOnePackage
        {
            get => (bool)GetValue(BuildAllInOnePackageProperty);
            set => SetValue(BuildAllInOnePackageProperty, value);
        }

        public void LoadFrom(Package package)
        {
            if (package != null)
            {
                OutputDirectory = package.OutputDirectory;
                OutputFile = package.OutputFile;
                VersionPattern = package.VersionPattern;
                BuildAllInOnePackage = package.BuildAllInOnePackage;
            }
            else
            {
                OutputDirectory = null;
                OutputFile = null;
                VersionPattern = null;
                BuildAllInOnePackage = false;
            }
        }

        public void SaveTo(Package package)
        {
            if (package != null)
            {
                package.OutputDirectory = OutputDirectory;
                package.OutputFile = OutputFile;
                package.VersionPattern = VersionPattern;
                package.BuildAllInOnePackage = BuildAllInOnePackage;
            }
        }
    }
}
