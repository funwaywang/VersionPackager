using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPackager
{
    public class NewFolderArgs : INotifyPropertyChanged
    {
        string _Error;
        string _FolderName;

        public event PropertyChangedEventHandler PropertyChanged;

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

        public string FolderName
        {
            get
            {
                return _FolderName;
            }

            set
            {
                if (_FolderName != value)
                {
                    _FolderName = value;
                    OnPropertyChanged("FolderName");
                }
            }
        }

        void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
