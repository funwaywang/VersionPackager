using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Data;

namespace VPackager
{
    public interface IPackageItemContainer
    {
        ObservableCollection<PackageItem> Items { get; }
    }

    public abstract class PackageItem : INotifyPropertyChanged
    {
        string _Name;
        string _TransitName;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Name
        {
            get { return _Name; }
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        public string TransitName
        {
            get { return _TransitName; }
            set
            {
                if (_TransitName != value)
                {
                    _TransitName = value;
                    OnPropertyChanged("TransitName");
                }
            }
        }

        public virtual bool IsFolder
        {
            get { return false; }
        }

        public PackageFolder Parent { get; internal set; }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public partial class PackageFolder : PackageItem, IPackageItemContainer
    {
        bool _IsExpanded;
        bool _IsSelected;

        public PackageFolder()
        {
            Items = new ObservableCollection<PackageItem>();
            Items.CollectionChanged += Items_CollectionChanged;
            
            Folders = new CollectionViewSource() { Source = Items }.View;
            Folders.Filter = (item => item is PackageFolder);

            IsExpanded = true;
        }

        public ObservableCollection<PackageItem> Items { get; private set; }

        public ICollectionView Folders { get; private set; }

        public bool IsExpanded
        {
            get => _IsExpanded;
            set
            {
                if (_IsExpanded != value)
                {
                    _IsExpanded = value;
                    OnPropertyChanged("IsExpanded");
                }
            }
        }

        public bool IsSelected
        {
            get => _IsSelected;
            set
            {
                if (_IsSelected != value)
                {
                    _IsSelected = value;
                    OnPropertyChanged("IsSelected");
                }
            }
        }

        public override bool IsFolder
        {
            get
            {
                return true;
            }
        }

        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems.OfType<PackageItem>())
                {
                    if (item.Parent == this)
                        item.Parent = null;
                }
            }

            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems.OfType<PackageItem>())
                {
                    item.Parent = this;
                }
            }
        }
    }

    public enum FileAction
    {
        Default,
        UpdateIfDifferent,
        UpdateIfNew,
        UpdateIfNotExists,
        UpdateForce,
        Delete,
    }

    public enum FileCompareMode
    {
        Default,
        Hash,
        StaticHash,
        Version,
        BuildInVersion,
    }

    public class PackageFile : PackageItem
    {
        string _Path;
        string _Identity;
        string _Version;
        FileAction _Action;
        FileCompareMode _CompareMode;

        public string Path
        {
            get { return _Path; }
            set
            {
                if (_Path != value)
                {
                    _Path = value;
                    OnPropertyChanged("Path");
                }
            }
        }

        public string Location
        {
            get
            {
                if (string.IsNullOrEmpty(Path))
                    return string.Empty;
                else
                    return System.IO.Path.GetDirectoryName(Path);
            }
        }

        public string Identity
        {
            get { return _Identity; }
            set
            {
                if (_Identity != value)
                {
                    _Identity = value;
                    OnPropertyChanged("Identity");
                }
            }
        }

        public string Version
        {
            get { return _Version; }
            set
            {
                if (_Version != value)
                {
                    _Version = value;
                    OnPropertyChanged("Version");
                }
            }
        }

        public FileAction Action
        {
            get { return _Action; }
            set
            {
                if (_Action != value)
                {
                    _Action = value;
                    OnPropertyChanged("Action");
                }
            }
        }

        public FileCompareMode CompareMode
        {
            get { return _CompareMode; }
            set
            {
                if (_CompareMode != value)
                {
                    _CompareMode = value;
                    OnPropertyChanged("CompareMode");
                }
            }
        }

        public string CreateHash()
        {
            if (!string.IsNullOrEmpty(Path) && File.Exists(Path))
            {
                using (var md5 = MD5.Create())
                {
                    using (var stream = File.OpenRead(Path))
                    {
                        var buffer = md5.ComputeHash(stream);
                        return BitConverter.ToString(buffer).Replace("-", "");
                    }
                }
            }

            return null;
        }
    }
}
