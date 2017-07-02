using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;

namespace VPackager
{
    [ValueConversion(typeof(string), typeof(string))]
    public class TextLocalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is string)
            {
                return Lang.GetText((string)parameter);
            }
            else if(value is string)
            {
                return Lang.GetText((string)value);
            }
            else if(value != null)
            {
                return Lang.GetText(value.ToString());
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(TextBlock), typeof(string))]
    public class LocalTextBlockConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is string)
            {
                return new TextBlock() { Text = Lang.GetText((string)parameter) };
            }
            else if (value is string)
            {
                return new TextBlock() { Text = Lang.GetText((string)value) };
            }
            else if (value != null)
            {
                return new TextBlock() { Text = Lang.GetText(value.ToString()) };
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
