using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace VPackager
{
    public partial class MainWindow : Window
    {
        private System.Windows.Forms.FolderBrowserDialog ImportFolderDialog;
        private ImportFolderArgs LastImportFolderArgs;

        public static readonly DependencyProperty IsModifiedProperty = DependencyProperty.Register("IsModified", typeof(bool), typeof(MainWindow));
        public static readonly DependencyProperty CurrentPackageProperty = DependencyProperty.Register("CurrentPackage", typeof(Package), typeof(MainWindow));
        public static readonly DependencyProperty CurrentFolderProperty = DependencyProperty.Register("CurrentFolder", typeof(PackageFolder), typeof(MainWindow));
        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(PackageItem), typeof(MainWindow));
        
        public MainWindow()
        {
            Packages = new ObservableCollection<Package>();

            //
            InitializeComponent();
        }

        public ObservableCollection<Package> Packages { get; private set; }

        public Package CurrentPackage
        {
            get { return (Package)GetValue(CurrentPackageProperty); }
            set { SetValue(CurrentPackageProperty, value); }
        }

        public PackageFolder CurrentFolder
        {
            get { return (PackageFolder)GetValue(CurrentFolderProperty); }
            set { SetValue(CurrentFolderProperty, value); }
        }

        public bool IsModified
        {
            get { return (bool)GetValue(IsModifiedProperty); }
            set { SetValue(IsModifiedProperty, value); }
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

        public PackageItem SelectedItem
        {
            get => (PackageItem)GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            Lang.Localize(this);
        }

        private bool OpenProjectFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName) || !File.Exists(fileName))
                return false;

            try
            {
                var package = Package.Load(fileName);
                if (package != null)
                {
                    CurrentPackage = package;
                    Settings.Default.AddRecentFile(fileName);
                    IsModified = false;
                    return true;
                }
            }
            catch (Exception ex)
            {
                this.ShowException(ex);
            }

            return false;
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if(e.Property == CurrentPackageProperty)
            {
                OnCurrentPackageChanged(e);
            }
            else if(e.Property == CurrentFolderProperty)
            {
                OnCurrentFolderChanged(e);
            }
        }

        private void OnCurrentFolderChanged(DependencyPropertyChangedEventArgs e)
        {
            if(e.OldValue is PackageFolder oldFolder)
            {
                oldFolder.IsSelected = false;
            }

            if(e.NewValue is PackageFolder folder)
            {
                folder.IsSelected = true;

                while (folder != null)
                {
                    folder.IsExpanded = true;
                    folder = folder.Parent;
                }
            }
        }

        private void OnCurrentPackageChanged(DependencyPropertyChangedEventArgs e)
        {
            if(e.OldValue is Package oldPackage)
            {
                oldPackage.PropertyChanged -= CurrentPackage_PropertyChanged;
            }

            Packages.Clear();
            if (CurrentPackage != null)
            {
                Packages.Add(CurrentPackage);
                CurrentFolder = CurrentPackage;
                CurrentPackage.PropertyChanged += CurrentPackage_PropertyChanged;

                if (!string.IsNullOrEmpty(CurrentPackage.FileName) && new Uri(CurrentPackage.FileName).IsAbsoluteUri)
                    Environment.CurrentDirectory = System.IO.Path.GetDirectoryName(CurrentPackage.FileName);
                else
                    Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            }
            else
            {
                CurrentFolder = null;
            }
        }

        private void CurrentPackage_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            IsModified = true;
        }

        private void ImportFolder_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CurrentPackage != null;
        }

        private void ImportFolder_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (CurrentFolder == null)
                return;

            if(ImportFolderDialog == null)
            {
                ImportFolderDialog = new System.Windows.Forms.FolderBrowserDialog()
                {
                    Description = Lang.GetText("Please select a folder to Import"),
                    SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    ShowNewFolderButton = false
                };
            }

            if (ImportFolderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var args = new ImportFolderArgs();
                if(LastImportFolderArgs != null)
                {
                    args.IncludeFileTypes = LastImportFolderArgs.IncludeFileTypes;
                    args.ExcludeFileTypes = LastImportFolderArgs.ExcludeFileTypes;
                    args.WithHiddenFiles = LastImportFolderArgs.WithHiddenFiles;
                    args.WithEmptyFolders = LastImportFolderArgs.WithEmptyFolders;
                    args.WithRoot = LastImportFolderArgs.WithRoot;
                }

                args.Source = ImportFolderDialog.SelectedPath;

                var dialog = new ImportDialog()
                {
                    Owner = this,
                    CurrentFolder = CurrentFolder,
                    ImportArgs = args
                };

                if(dialog.ShowDialog() == true)
                {
                    LastImportFolderArgs = dialog.ImportArgs;
                    IsModified = true;
                }
            }
        }

        private void CreateFolder_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CurrentPackage != null;
        }

        private void CreateFolder_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (CurrentFolder == null)
                return;

            var dialog = new NewFolderDialog()
            {
                Owner = this
            };
            dialog.Validating += Dialog_Validating;

            if(dialog.ShowDialog() == true)
            {
                var folder = new PackageFolder()
                {
                    Name = dialog.FolderName
                };
                CurrentFolder.Items.Add(folder);
                
                IsModified = true;
            }
        }

        private void Dialog_Validating(object sender, ValidatingEventArgs args)
        {
            if (sender is NewFolderDialog dialog)
            {
                if (CurrentFolder != null && CurrentFolder.Items.Any(it => StringComparer.OrdinalIgnoreCase.Equals(it.Name, dialog.FolderName)))
                {
                    args.ErrorMessage = "The same name object already exists";
                    args.IsOK = false;
                }
            }
        }

        private void AddFiles_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CurrentFolder != null;
        }

        private void AddFiles_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (CurrentFolder == null)
                return;

            var dialog = new OpenFileDialog()
            {
                Filter = string.Format("{0}|*.*", Lang._("All Files")),
                Multiselect = true
            };

            if (dialog.ShowDialog(this) == true)
            {
                foreach (var file in dialog.FileNames)
                {
                    if (CurrentFolder.Items.Any(it => StringComparer.OrdinalIgnoreCase.Equals(it.Name, System.IO.Path.GetFileName(file))))
                        continue;

                    CurrentFolder.Items.Add(new PackageFile()
                    {
                        Name = System.IO.Path.GetFileName(file),
                        Path = file
                    });
                }
                IsModified = true;
            }
        }

        private void Generate_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CurrentPackage != null;
        }

        private void Generate_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (CurrentPackage == null)
                return;

            var outputDirectory = CurrentPackage.OutputDirectory;
            if (string.IsNullOrEmpty(outputDirectory))
            {
                var dialog = new System.Windows.Forms.FolderBrowserDialog();
                if (!string.IsNullOrEmpty(CurrentPackage.OutputDirectory))
                    dialog.SelectedPath = CurrentPackage.OutputDirectory;
                dialog.Description = Lang._("Select a folder to publish this package");
                if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    return;

                outputDirectory = dialog.SelectedPath;
            }

            var outputFile = CurrentPackage.OutputFile;
            if (string.IsNullOrEmpty(outputFile))
            {
                outputFile = "Version.xml";
            }

            var publisher = new PackagePublisher(CurrentPackage);
            try
            {
                publisher.Publish(outputDirectory, outputFile);
            }
            catch (Exception ex)
            {
                this.ShowException(ex);
                return;
            }

            if (publisher.Errors.Count > 0)
            {
                this.ShowErrorMessageBox(StringHelper.JoinArray(publisher.Errors));
                return;
            }

            this.ShowInformationBox(Lang._("Published success") + "\n" + outputDirectory);
        }

        private void Options_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CurrentPackage != null;
        }

        private void Options_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (CurrentPackage == null)
                return;

            var dialog = new PackagePropertyDialog()
            {
                Owner = this,
                Package = CurrentPackage
            };

            if(dialog.ShowDialog() == true)
            {
                IsModified = true;
            }
        }

        private void New_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void New_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CurrentPackage = new Package();
            IsModified = true;
        }

        private void Open_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is string fileName)
            {
                OpenProjectFile(fileName);
            }
            else
            {
                var dialog = new OpenFileDialog()
                {
                    Filter = string.Format("{0}|*.vpp", Lang._("Version Package Project"))
                };
                if (dialog.ShowDialog(this) == true)
                {
                    OpenProjectFile(dialog.FileName);
                }
            }
        }

        private void Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CurrentPackage != null && IsModified;
        }

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (CurrentPackage == null)
                return;

            if (string.IsNullOrEmpty(CurrentPackage.FileName) || !File.Exists(CurrentPackage.FileName) || new FileInfo(CurrentPackage.FileName).IsReadOnly)
            {
                SaveAs_Executed(sender, e);
                return;
            }

            try
            {
                CurrentPackage.Save(CurrentPackage.FileName);
                IsModified = false;
            }
            catch (Exception ex)
            {
                this.ShowException(ex);
            }
        }

        private void SaveAs_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CurrentPackage != null;
        }

        private void SaveAs_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new SaveFileDialog()
            {
                Filter = string.Format("{0}|*.vpp", Lang._("Version Package Project")),
                FileName = CurrentPackage.FileName
            };

            if (dialog.ShowDialog(this) == true)
            {
                try
                {
                    CurrentPackage.Save(dialog.FileName);
                    IsModified = false;
                }
                catch (Exception ex)
                {
                    this.ShowException(ex);
                    return;
                }

                Settings.Default.AddRecentFile(dialog.FileName);
            }
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if(e.NewValue is PackageFolder)
            {
                CurrentFolder = (PackageFolder)e.NewValue;
            }
            else
            {
                CurrentFolder = null;
            }
        }
        
        private T FindAncestor<T>(FrameworkElement element)
            where T : FrameworkElement
        {
            if (element == null)
                return null;

            while(element != null && !(element is T))
            {
                element = element.Parent as FrameworkElement;
            }

            return element as T;
        }

        private void ItemRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedItem is PackageFolder folder)
            {
                CurrentFolder = folder;
            }
            else if (SelectedItem is PackageFile file)
            {
                ShowFileProperty(file);
            }
        }

        private void ShowFileProperty(PackageFile file)
        {
            var dialog = new FilePropertyDialog()
            {
                Owner = this,
                FileItem = file
            };

            if (dialog.ShowDialog() == true)
            {
                IsModified = true;
            }
        }

        private void ShowFolderProperty(PackageFolder folder)
        {
            var dialog = new FolderPropertyDialog()
            {
                Owner = this,
                Folder = folder
            };

            if (dialog.ShowDialog() == true)
            {
                IsModified = true;
            }
        }

        private void TurnRelativePath_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = GridItems.SelectedItems != null 
                && GridItems.SelectedItems.OfType<PackageItem>().Any(it => !it.IsFolder);
        }

        private void TurnRelativePath_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(CurrentPackage.FileName))
            {
                this.ShowErrorMessageBox("Please save project first");
                return;
            }

            var basePath = System.IO.Path.GetDirectoryName(CurrentPackage.FileName);
            foreach (var item in GridItems.SelectedItems.OfType<PackageFile>())
            {
                item.Path = PathHelper.ToRelativePath(basePath, item.Path);
            }
            IsModified = true;
        }

        private void TurnAbsolutePath_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = GridItems.SelectedItems != null
                && GridItems.SelectedItems.OfType<PackageItem>().Any(it => !it.IsFolder);
        }

        private void TurnAbsolutePath_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(CurrentPackage.FileName))
            {
                this.ShowErrorMessageBox("Please save project first");
                return;
            }

            var basePath = System.IO.Path.GetDirectoryName(CurrentPackage.FileName);
            foreach (var item in GridItems.SelectedItems.OfType<PackageFile>())
            {
                item.Path = PathHelper.ToAbsolutePath(basePath, item.Path);
            }
            IsModified = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(Settings.Default.LastPackage) && File.Exists(Settings.Default.LastPackage))
            {
                OpenProjectFile(Settings.Default.LastPackage);
            }
        }

        private void DeleteFolder_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = TreeViewFolders != null && TreeViewFolders.SelectedItem is PackageFolder folder && folder.Parent != null;
        }

        private void DeleteFolder_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (TreeViewFolders != null && TreeViewFolders.SelectedItem is PackageFolder folder && folder.Parent != null)
            {
                if (folder.Items.Count > 0)
                {
                    if (!this.ShowConfirmBox("The folder is not empty, Are you sure want to delete it?"))
                        return;
                }

                folder.Parent.Items.Remove(folder);
                CurrentFolder = folder.Parent ?? CurrentPackage;
                IsModified = true;
            }
        }

        private void DeleteItems_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = GridItems != null && GridItems.SelectedItems.Count > 0;
        }

        private void DeleteItems_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (CurrentFolder != null && GridItems != null && GridItems.SelectedItems.Count > 0)
            {
                if (!this.ShowConfirmBox("Are you sure want to delete the selected items?"))
                    return;

                var items = GridItems.SelectedItems.OfType<PackageItem>().ToArray();
                foreach (var item in items)
                {
                    CurrentFolder.Items.Remove(item);
                }
                IsModified = true;
            }
        }

        private void OpenRecent_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.Parameter is string fileName && File.Exists(fileName);
        }

        private void OpenRecent_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if(e.Parameter is string fileName && File.Exists(fileName))
            {
                OpenProjectFile(fileName);
            }
        }

        private void FolderProperty_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CurrentFolder != null;
        }

        private void FolderProperty_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if(CurrentFolder != null)
            {
                ShowFolderProperty(CurrentFolder);
            }
        }

        private void GridProperty_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SelectedItem != null;
        }

        private void GridProperty_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (SelectedItem is PackageFolder folder)
            {
                ShowFolderProperty(folder);
            }
            else if (SelectedItem is PackageFile file)
            {
                ShowFileProperty(file);
            }
        }
        
        private void Minimize_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (WindowState != WindowState.Minimized)
            {
                WindowState = WindowState.Minimized;
            }
        }

        private void Maximize_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
            else
                WindowState = WindowState.Maximized;
        }

        private void CloseWindow_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }
    }
}
