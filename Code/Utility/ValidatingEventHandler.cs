using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPackager
{
    public class ValidatingEventArgs : EventArgs
    {
        public bool IsOK { get; set; } = true;

        public string ErrorMessage { get; set; }
    }

    public delegate void ValidatingEventHandler(object sender, ValidatingEventArgs args);
}
