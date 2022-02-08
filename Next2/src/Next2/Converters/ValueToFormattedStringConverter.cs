using System;
using System.Globalization;
using Xamarin.Forms;

namespace Next2.Converters
{
    public class ValueToFormattedStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                string re = string.Format(parameter as string, value);
                return re;
            }
            catch (Exception ex)
            {
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
