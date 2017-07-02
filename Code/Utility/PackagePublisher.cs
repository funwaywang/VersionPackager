using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace VPackager
{
    public class PackagePublisher
    {
        private bool IsZipStreamModel;
        private ZipArchive Zip;
        private XmlDocument VersionXmlDocument;

        public Package Package { get; private set; }

        public PackagePublisher(Package package)
        {
            Package = package;
        }

        public List<string> Errors { get; private set; } = new List<string>();

        public string RootDirectory { get; private set; }

        public string VersionFileName { get; private set; }

        public void Publish(string directory, string filename)
        {
            if (Package == null)
                throw new NullReferenceException("Package");

            Package.OutputDirectory = directory ?? throw new ArgumentNullException("directory");
            Package.OutputFile = filename;
            RootDirectory = directory;
            VersionFileName = filename;

            if (Package.BuildAllInOnePackage)
            {
                var zipfile = Path.Combine(directory, Path.GetFileNameWithoutExtension(filename) + ".zip");
                PublishToZip(zipfile);
            }
            else
            {
                PublishToDirectory("");
            }
        }

        private void PublishToZip(string zipfile)
        {
            IsZipStreamModel = true;

            var tempfile = Path.GetTempFileName();
            try
            {
                using (var fs = new FileStream(tempfile, FileMode.Create, FileAccess.Write))
                {
                    using (var zip = new ZipArchive(fs, ZipArchiveMode.Create))
                    {
                        Zip = zip;
                        PublishToDirectory("");
                        Zip = null;
                    }
                }
                
                EnsureDirectoryCreated(Path.GetDirectoryName(zipfile));
                if (File.Exists(zipfile))
                    File.Delete(zipfile);
                File.Move(tempfile, zipfile);

                if (VersionXmlDocument != null)
                {
                    var versionFile = Path.Combine(RootDirectory, VersionFileName);
                    using (var fs = new FileStream(versionFile, FileMode.Create, FileAccess.Write))
                    {
                        VersionXmlDocument.Save(fs);
                    }
                }
            }
            finally
            {
                if (File.Exists(tempfile))
                {
                    File.Delete(tempfile);
                }
            }
        }

        private void PublishToDirectory(string directory)
        {
            string version = GenerateVersion(Package.VersionPattern);

            //
            VersionXmlDocument = new XmlDocument();
            VersionXmlDocument.LoadXml("<?xml version='1.0' encoding='utf-8'?><package/>");
            VersionXmlDocument.DocumentElement.SetAttribute("version", version);

            var items = VersionXmlDocument.CreateElement("items");
            VersionXmlDocument.DocumentElement.AppendChild(items);

            PublishPackageVersion(Package, items, directory);

            using (var stream = GetTargetStream(directory, VersionFileName))
            {
                VersionXmlDocument.Save(stream);
                stream.Close();
            }
        }

        private void PublishPackageVersion(IPackageItemContainer folder, XmlElement parentElement, string relativePath)
        {
            var directory = Path.Combine(RootDirectory, relativePath.Replace("/", "\\"));
            if (!IsZipStreamModel && !EnsureDirectoryCreated(directory))
            {
                return;
            }

            var itemNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            var folders = folder.Items.OfType<PackageFolder>();
            foreach (var fd in folders)
            {
                var folderName = string.IsNullOrEmpty(fd.TransitName) ? fd.Name : fd.TransitName;
                if (itemNames.Contains(folderName))
                {
                    Errors.Add(string.Format(Lang._("The existence of items in folder \"{0}\" with the same name \"{1}\""), relativePath, folderName));
                    continue;
                }
                itemNames.Add(folderName);

                var node = parentElement.OwnerDocument.CreateElement("folder");
                node.SetAttribute("name", fd.Name);
                if (!string.IsNullOrEmpty(fd.TransitName))
                    node.SetAttribute("transit_name", fd.TransitName);
                parentElement.AppendChild(node);

                PublishPackageVersion(fd, node, relativePath + fd.Name + "/");
            }

            var files = folder.Items.OfType<PackageFile>();
            foreach (var file in files)
            {
                if (file == null || string.IsNullOrEmpty(file.Path))
                    continue;
                if (!File.Exists(file.Path))
                {
                    Errors.Add(string.Format(Lang._("File \"{0}\" not exists"), file.Path));
                    continue;
                }

                var filename = string.IsNullOrEmpty(file.TransitName) ? file.Name : file.TransitName;
                if (itemNames.Contains(filename))
                {
                    Errors.Add(string.Format(Lang._("The existence of items in folder \"{0}\" with the same name \"{1}\""), relativePath, filename));
                    continue;
                }

                itemNames.Add(filename);
                if (IsZipStreamModel)
                {
                    var entry = Zip.CreateEntry(Path.Combine(relativePath, filename));
                    using (var stream = entry.Open())
                    {
                        using (var source = new FileStream(file.Path, FileMode.Open, FileAccess.Read))
                        {
                            source.CopyTo(stream);
                        }
                    }
                }
                else
                {
                    var destFileName = Path.Combine(directory, filename);
                    if (File.Exists(destFileName))
                    {
                        try
                        {
                            File.Delete(destFileName);
                        }
                        catch (Exception ex)
                        {
                            Errors.Add(string.Format(Lang._("Delete File \"{0}\" Failed"), file.Path) + "\n" + ex.Message);
                            continue;
                        }
                    }
                    File.Copy(file.Path, destFileName);
                }

                //
                var node = parentElement.OwnerDocument.CreateElement("file");
                node.SetAttribute("name", file.Name);
                node.SetAttribute("hash", file.CreateHash());
                if (!string.IsNullOrEmpty(file.TransitName))
                    node.SetAttribute("transit_name", file.TransitName);
                if (!string.IsNullOrEmpty(file.Version))
                    node.SetAttribute("version", file.Version);
                if (file.Action != FileAction.Default)
                    node.SetAttribute("action", file.Action.ToString());
                if (file.CompareMode != FileCompareMode.Default)
                    node.SetAttribute("compare_mode", file.CompareMode.ToString());
                parentElement.AppendChild(node);

                //
                itemNames.Add(filename);
            }
        }

        private Stream GetTargetStream(string directory, string filename)
        {
            if (IsZipStreamModel)
            {
                if (Zip != null)
                {
                    var entry = Zip.CreateEntry(Path.Combine(directory, filename));
                    return entry.Open();
                }
                else
                {
                    throw new Exception("Zip Archive was closed");
                }
            }
            else
            {
                var path = Path.Combine(Path.Combine(RootDirectory, directory ?? "/"), filename);
                if (EnsureDirectoryCreated(Path.GetDirectoryName(path)))
                    return new FileStream(path, FileMode.Create, FileAccess.Write);
                else
                    return null;
            }
        }

        private static string GenerateVersion(string versionPattern)
        {
            if (string.IsNullOrEmpty(versionPattern))
                return null;

            versionPattern = versionPattern.Replace("{YM}", DateTime.Today.ToString("yyMM"));
            versionPattern = versionPattern.Replace("{MD}", DateTime.Today.ToString("MMdd"));
            versionPattern = versionPattern.Replace("{HM}", DateTime.Now.ToString("HHmm"));
            versionPattern = versionPattern.Replace("{Y}", DateTime.Today.ToString("yy"));

            versionPattern = versionPattern.Replace("{YY}", DateTime.Today.ToString("yy"));
            versionPattern = versionPattern.Replace("{MM}", DateTime.Today.ToString("MM"));
            versionPattern = versionPattern.Replace("{DD}", DateTime.Today.ToString("dd"));
            versionPattern = versionPattern.Replace("{HH}", DateTime.Now.ToString("hh"));
            versionPattern = versionPattern.Replace("{mm}", DateTime.Now.ToString("mm"));
            versionPattern = versionPattern.Replace("{SS}", DateTime.Now.ToString("ss"));

            return versionPattern;
        }

        private bool EnsureDirectoryCreated(string directory)
        {
            if (Directory.Exists(directory))
                return true;

            try
            {
                Directory.CreateDirectory(directory);
                return true;
            }
            catch (Exception ex)
            {
                Errors.Add(string.Format(Lang._("Create Directory \"{0}\" Failed"), directory) + "\n" + ex.Message);
                return false;
            }
        }
    }
}
