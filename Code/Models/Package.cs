using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Xml;

namespace VPackager
{
    public class Package : PackageFolder
    {
        string _OutputDirectory;
        string _OutputFile;
        string _VersionPattern;
        bool _BuildAllInOnePackage;

        public Package()
        {
            Name = "Package";
        }

        public string FileName { get; set; }

        public string OutputDirectory
        {
            get => _OutputDirectory;
            set
            {
                if(_OutputDirectory != value)
                {
                    _OutputDirectory = value;
                    OnPropertyChanged("OutputDirectory");
                }
            }
        }

        public string OutputFile
        {
            get => _OutputFile;
            set
            {
                if (_OutputFile != value)
                {
                    _OutputFile = value;
                    OnPropertyChanged("OutputFile");
                }
            }
        }

        public string VersionPattern
        {
            get => _VersionPattern;
            set
            {
                if (_VersionPattern != value)
                {
                    _VersionPattern = value;
                    OnPropertyChanged("VersionPattern");
                }
            }
        }

        public bool BuildAllInOnePackage
        {
            get => _BuildAllInOnePackage;
            set
            {
                if(_BuildAllInOnePackage != value)
                {
                    _BuildAllInOnePackage = value;
                    OnPropertyChanged("BuildAllInOnePackage");
                }
            }
        }

        public static Package Load(string filename)
        {
            if (string.IsNullOrEmpty(filename) || !File.Exists(filename))
                throw new ArgumentException("filename");

            using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                var package = Load(stream);
                if(package != null)
                {
                    package.FileName = filename;
                    package.Name = Path.GetFileName(filename);
                }

                return package;
            }
        }

        public static Package Load(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException();
            if (!stream.CanRead)
                throw new Exception("Stream Can't Read");

            var dom = new XmlDocument();
            dom.Load(stream);

            return Load(dom);
        }

        public static Package Load(XmlDocument dom)
        {
            if (dom == null)
                throw new ArgumentNullException();

            if (dom.DocumentElement == null || dom.DocumentElement.Name != "package")
                return null;

            var package = new Package();
            package.OutputDirectory = dom.DocumentElement.GetAttribute("output_dir");
            package.OutputFile = dom.DocumentElement.GetAttribute("output_file");
            package.VersionPattern = dom.DocumentElement.GetAttribute("version_pattern");
            package.BuildAllInOnePackage = StringHelper.GetBoolDefault(dom.DocumentElement.GetAttribute("build_all_in_one_package"));

            if (dom.DocumentElement.SelectSingleNode("items") is XmlElement itemsNode)
            {
                var items = LoadItems(itemsNode);
                if (items != null)
                {
                    foreach (var item in items)
                    {
                        package.Items.Add(item);
                    }
                }
            }

            return package;
        }

        static PackageItem[] LoadItems(XmlElement element)
        {
            var list = new List<PackageItem>();

            var folderNodes = element.SelectNodes("folder");
            foreach (XmlElement fn in folderNodes)
            {
                var folder = new PackageFolder()
                {
                    Name = fn.GetAttribute("name"),
                    TransitName = fn.GetAttribute("transit_name"),
                    IsExpanded = StringHelper.GetBoolDefault(fn.GetAttribute("is_expanded"))
                };

                if (fn.HasChildNodes)
                {
                    var items = LoadItems(fn);
                    if (items != null)
                    {
                        foreach (var item in items)
                        {
                            folder.Items.Add(item);
                        }
                    }
                }

                list.Add(folder);
            }

            var fileNodes = element.SelectNodes("file");
            foreach (XmlElement fn in fileNodes)
            {
                var item = new PackageFile()
                {
                    Name = fn.GetAttribute("name"),
                    Path = fn.GetAttribute("path"),
                    TransitName = fn.GetAttribute("transit_name"),
                    Action = GetEnumValue(fn.GetAttribute("action"), FileAction.Default),
                    CompareMode = GetEnumValue(fn.GetAttribute("compare_mode"), FileCompareMode.Default),
                    Version = fn.GetAttribute("version")
                };
                list.Add(item);
            }

            return list.ToArray();
        }

        static T GetEnumValue<T>(string text, T defaultValue)
            where T : struct
        {
            if (Enum.TryParse(text, out T r))
                return r;
            else
                return defaultValue;
        }

        public void Save(string fileName)
        {
            if (fileName == null)
                throw new ArgumentNullException();
            if (fileName == string.Empty)
                throw new Exception("Invalid File Name");

            var xmlDom = new XmlDocument();
            Save(xmlDom);

            using (var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                xmlDom.Save(stream);
                FileName = fileName;
            }
        }

        public void Save(XmlDocument dom)
        {
            if (dom == null)
                throw new ArgumentNullException();

            dom.LoadXml("<?xml version='1.0' encoding='utf-8'?><package/>");

            var root = dom.DocumentElement;
            root.SetAttribute("output_dir", OutputDirectory);
            root.SetAttribute("output_file", OutputFile);
            root.SetAttribute("version_pattern", VersionPattern);
            root.SetAttribute("build_all_in_one_package", BuildAllInOnePackage.ToString());

            //
            var items = dom.CreateElement("items");
            Save(dom, items, Items);
            root.AppendChild(items);
        }

        static void Save(XmlDocument dom, XmlElement parentElement, IEnumerable<PackageItem> items)
        {
            foreach (var folder in items.OfType<PackageFolder>())
            {
                var node = dom.CreateElement("folder");
                node.SetAttribute("name", folder.Name);

                if (!string.IsNullOrEmpty(folder.TransitName))
                {
                    node.SetAttribute("transit_name", folder.TransitName);
                }

                if (folder.IsExpanded)
                {
                    node.SetAttribute("is_expanded", folder.IsExpanded.ToString());
                }

                parentElement.AppendChild(node);

                Save(dom, node, folder.Items);
            }

            foreach (var file in items.OfType<PackageFile>())
            {
                var node = dom.CreateElement("file");
                node.SetAttribute("name", file.Name);
                if (!string.IsNullOrEmpty(file.TransitName))
                    node.SetAttribute("transit_name", file.TransitName);

                node.SetAttribute("path", file.Path);
                if (!string.IsNullOrEmpty(file.Version))
                    node.SetAttribute("version", file.Version);
                if (file.Action != FileAction.Default)
                    node.SetAttribute("action", file.Action.ToString());
                if (file.CompareMode != FileCompareMode.Default)
                    node.SetAttribute("compare_mode", file.CompareMode.ToString());

                parentElement.AppendChild(node);
            }
        }
    }
}
