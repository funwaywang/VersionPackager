using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace VPackager
{
    [ValueConversion(typeof(IEnumerable), typeof(bool))]
    public class ListEmptyToBoolConverter : IValueConverter
    {
        public bool IsReverse { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is IEnumerable)
            {
                if (IsReverse)
                {
                    return ((IEnumerable)value).GetEnumerator().MoveNext();
                }
                else
                {
                    // list is empty
                    return !((IEnumerable)value).GetEnumerator().MoveNext();
                }
            }

            throw new InvalidCastException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
