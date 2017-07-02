using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace VPackager
{
    public partial class App : Application
    {
        public const string Name = "VPackager";

        public static string ApplicationDataDirectory
        {
            get => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Name);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var res = GetResourceStream(new Uri("/Resources/zh-CN.xml", UriKind.Relative));
            Lang.Current = Lang.LoadXml(res.Stream);

            Settings.Default.Load();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            Settings.Default.Save();
        }
    }
}
