using System;
using System.Globalization;
using Xamarin.Forms;

namespace Next2.Converters
{
    public class GuidToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Guid)value != Guid.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
