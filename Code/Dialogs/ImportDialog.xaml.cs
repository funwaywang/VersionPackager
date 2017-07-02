using System;
using System.Collections.Generic;
using System.IO;
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
    public partial class ImportDialog : Window
    {
        private System.Windows.Forms.FolderBrowserDialog ImportFolderDialog;

        public static readonly DependencyProperty ImportArgsProperty = DependencyProperty.Register("ImportFolderArgs", typeof(ImportFolderArgs), typeof(ImportDialog));
        public static readonly DependencyProperty ErrorMessageProperty = DependencyProperty.Register("ErrorMessage", typeof(string), typeof(ImportDialog));
        public static readonly DependencyProperty CurrentFolderProperty = DependencyProperty.Register("CurrentFolder", typeof(PackageFolder), typeof(ImportDialog));

        public ImportDialog()
        {
            InitializeComponent();
        }

        public ImportFolderArgs ImportArgs
        {
            get => (ImportFolderArgs)GetValue(ImportArgsProperty);
            set => SetValue(ImportArgsProperty, value);
        }

        public PackageFolder CurrentFolder
        {
            get => (PackageFolder)GetValue(CurrentFolderProperty);
            set => SetValue(CurrentFolderProperty, value);
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

        private void BtnImportFolder_Click(object sender, RoutedEventArgs e)
        {
            ErrorMessage = null;
            if (ImportArgs == null)
                return;

            // validation
            if (ImportArgs.WithRoot)
            {
                var name = string.IsNullOrEmpty(ImportArgs.Rename) ? System.IO.Path.GetFileName(ImportArgs.Source) : ImportArgs.Rename;
                if (CurrentFolder.Items.Any(it => StringComparer.OrdinalIgnoreCase.Equals(it.Name, name)))
                {
                    ErrorMessage = "There is same name object is exists";
                    return;
                }
            }

            //
            string[] includes = null;
            string[] excludes = null;
            if (ImportArgs.IncludeFileTypes != null)
            {
                includes = ImportArgs.IncludeFileTypes.Split(' ').Where(ext => !string.IsNullOrEmpty(ext)).ToArray();
            }
            if (ImportArgs.ExcludeFileTypes != null)
            {
                excludes = ImportArgs.ExcludeFileTypes.Split(' ').Where(ext => !string.IsNullOrEmpty(ext)).ToArray();
            }
            var folder = ImportFolder(ImportArgs.Source, includes, excludes, ImportArgs.WithHiddenFiles, ImportArgs.WithEmptyFolders);
            if (folder != null)
            {
                if (ImportArgs.WithRoot)
                {
                    if (!string.IsNullOrEmpty(ImportArgs.Rename))
                    {
                        folder.Name = ImportArgs.Rename;
                    }
                    if (CurrentFolder.Items.Any(it => StringComparer.OrdinalIgnoreCase.Equals(it.Name, folder.Name)))
                    {
                        ErrorMessage = "There is same name object is exists";
                        return;
                    }
                    CurrentFolder.Items.Add(folder);
                }
                else
                {
                    foreach (var item in folder.Items)
                    {
                        if (CurrentFolder.Items.Any(it => StringComparer.OrdinalIgnoreCase.Equals(it.Name, item.Name)))
                            continue;
                        CurrentFolder.Items.Add(item);
                    }
                }

                DialogResult = true;
                Close();
            }
        }

        private PackageFolder ImportFolder(string path, string[] includes, string[] excludes, bool withHiddenFiles, bool withEmptyFolders)
        {
            if (!Directory.Exists(path))
            {
                ErrorMessage = "Folder not exists";
                return null;
            }

            var folder = new PackageFolder()
            {
                Name = System.IO.Path.GetFileName(path)
            };
            var dirs = Directory.GetDirectories(path);
            foreach (var dir in dirs)
            {
                var dirInfo = new DirectoryInfo(dir);
                if (!withHiddenFiles && (dirInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                    continue;

                var subFolder = ImportFolder(dir, includes, excludes, withHiddenFiles, withEmptyFolders);
                if (subFolder != null && (withEmptyFolders || subFolder.Items.Any()))
                {
                    folder.Items.Add(subFolder);
                }
            }

            var files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                if (!withHiddenFiles && (fileInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                    continue;
                if (includes != null && includes.Any() && !includes.Contains(fileInfo.Extension, StringComparer.OrdinalIgnoreCase))
                    continue;
                if (!string.IsNullOrEmpty(fileInfo.Extension) && excludes != null && excludes.Any() && excludes.Contains(fileInfo.Extension, StringComparer.OrdinalIgnoreCase))
                    continue;

                var f = new PackageFile()
                {
                    Name = fileInfo.Name,
                    Path = file,
                };

                folder.Items.Add(f);
            }

            return folder;
        }

        private void BtnBrowseImportSource_Click(object sender, RoutedEventArgs e)
        {
            if (ImportFolderDialog == null)
            {
                ImportFolderDialog = new System.Windows.Forms.FolderBrowserDialog()
                {
                    Description = Lang.GetText("Please select a folder to Import"),
                    SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    ShowNewFolderButton = false
                };
            }

            if (ImportArgs != null)
            {
                ImportFolderDialog.SelectedPath = ImportArgs.Source;
                var owner = new WindowWrapper(this);
                if (ImportFolderDialog.ShowDialog(owner) == System.Windows.Forms.DialogResult.OK)
                {
                    ImportArgs.Source = ImportFolderDialog.SelectedPath;
                }
            }
        }
    }
}
