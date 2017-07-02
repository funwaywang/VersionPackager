using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;

namespace VPackager
{
    public class WindowWrapper : System.Windows.Forms.IWin32Window
    {
        public WindowWrapper(IntPtr handle)
        {
            _hwnd = handle;
        }

        public WindowWrapper(Window window)
        {
            _hwnd = new WindowInteropHelper(window).Handle;
        }

        public IntPtr Handle
        {
            get { return _hwnd; }
        }

        private IntPtr _hwnd;
    }

    public class PopupIWin32 : System.Windows.Forms.IWin32Window
    {
        Popup Popup;

        public PopupIWin32(Popup popup)
        {
            Popup = popup;
        }

        public IntPtr Handle
        {
            get
            {
                var source = System.Windows.PresentationSource.FromVisual(Popup.Child) as HwndSource;
                if (source != null)
                    return source.Handle;
                else
                    return IntPtr.Zero;
            }
        }
    }
}
