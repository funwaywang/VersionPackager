using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPackager
{
    public class ImportFolderArgs : INotifyPropertyChanged
    {
        string _Error;

        public event PropertyChangedEventHandler PropertyChanged;

        public ImportFolderArgs()
        {
            WithRoot = true;
            WithHiddenFiles = false;
            WithEmptyFolders = false;
        }

        public string Source { get; set; }

        public string Rename { get; set; }

        public bool WithHiddenFiles { get; set; }

        public bool WithEmptyFolders { get; set; }

        public bool WithRoot { get; set; }

        public string IncludeFileTypes { get; set; }

        public string ExcludeFileTypes { get; set; }

        public string Error
        {
            get { return _Error; }
            set
            {
                if (_Error != value)
                {
                    _Error = value;
                    OnPropertyChanged("Error");
                }
            }
        }

        void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
