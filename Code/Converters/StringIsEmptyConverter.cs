using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace VPackager
{
    [ValueConversion(typeof(string), typeof(bool))]
    public class StringIsEmptyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string vs)
            {
                return string.IsNullOrEmpty(vs);
            }
            else if (parameter is string ps)
            {
                return string.IsNullOrEmpty(ps);
            }
            else
            {
                return true;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
