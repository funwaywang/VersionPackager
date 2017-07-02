using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace VPackager
{
    public class Settings : INotifyPropertyChanged
    {
        int _RecentFilesCount;

        public static readonly Settings Default = new Settings();
        public event PropertyChangedEventHandler PropertyChanged;

        private Settings()
        {
            RecentFiles = new ObservableCollection<string>();
            RecentFiles.CollectionChanged += RecentFiles_CollectionChanged;
        }

        public string FileName
        {
            get => Path.Combine(App.ApplicationDataDirectory, "Settings.xml");
        }

        public string LastPackage { get; set; }

        public ObservableCollection<string> RecentFiles { get; private set; }

        public int RecentFilesCount
        {
            get => _RecentFilesCount;
            private set
            {
                if (_RecentFilesCount != value)
                {
                    _RecentFilesCount = value;
                    OnPropertyChanged("RecentFilesCount");
                }
            }
        }

        public void Load()
        {
            if (string.IsNullOrEmpty(FileName) || !File.Exists(FileName))
                return;

            XmlDocument dom = new XmlDocument();
            try
            {
                using (var fs = new FileStream(FileName, FileMode.Open, FileAccess.Read))
                {
                    dom.Load(fs);
                }
            }
            catch
            {
                return;
            }

            if (dom.DocumentElement.Name == "settings")
            {
                foreach (var item in dom.DocumentElement.SelectNodes("item").OfType<XmlElement>())
                {
                    switch (item.GetAttribute("name"))
                    {
                        case "LastPackage":
                            LastPackage = item.InnerText;
                            break;
                        case "RectentFiles":
                            RecentFiles.Clear();
                            foreach (var rp in item.ChildNodes.OfType<XmlElement>())
                            {
                                var file = rp.InnerText;
                                if (!string.IsNullOrEmpty(file))
                                {
                                    RecentFiles.Add(file);
                                }
                            }
                            break;
                    }
                }
            }
        }

        public void Save()
        {
            if (string.IsNullOrEmpty(FileName))
                return;

            XmlDocument dom = new XmlDocument();
            dom.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\"?><settings/>");

            var lastPackage = dom.CreateElement("item");
            lastPackage.SetAttribute("name", "LastPackage");
            lastPackage.InnerText = LastPackage;
            dom.DocumentElement.AppendChild(lastPackage);

            var recentfiles = dom.CreateElement("item");
            recentfiles.SetAttribute("name", "RectentFiles");
            foreach (var file in RecentFiles)
            {
                var fileElement = dom.CreateElement("file");
                fileElement.InnerText = file;
                recentfiles.AppendChild(fileElement);
            }
            dom.DocumentElement.AppendChild(recentfiles);

            var dir = Path.GetDirectoryName(FileName);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            try
            {
                using (var fs = new FileStream(FileName, FileMode.Create, FileAccess.Write))
                {
                    dom.Save(fs);
                }
            }
            catch
            {
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void RecentFiles_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RecentFilesCount = RecentFiles.Count;
        }

        public void AddRecentFile(string filename)
        {
            var fs = RecentFiles.Where(f => StringComparer.OrdinalIgnoreCase.Equals(f, filename)).ToArray();
            if (fs != null)
            {
                foreach (var f in fs)
                {
                    RecentFiles.Remove(f);
                }
            }

            RecentFiles.Insert(0, filename);
            LastPackage = filename;
        }

        public void RemoveRecentFile(string filename)
        {
            var fs = RecentFiles.Where(f => StringComparer.OrdinalIgnoreCase.Equals(f, filename)).ToArray();
            if (fs != null)
            {
                foreach (var f in fs)
                {
                    RecentFiles.Remove(f);
                }
            }

            if (StringComparer.OrdinalIgnoreCase.Equals(Default.LastPackage, filename))
            {
                LastPackage = null;
            }
        }
    }
}
