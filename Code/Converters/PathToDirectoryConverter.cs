using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace VPackager
{
    public class PathToDirectoryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string vs)
            {
                if (string.IsNullOrEmpty(vs))
                    return string.Empty;
                else
                    return System.IO.Path.GetDirectoryName(vs);
            }
            else if (parameter is string ps)
            {
                if (string.IsNullOrEmpty(ps))
                    return string.Empty;
                else
                    return System.IO.Path.GetDirectoryName(ps);
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
