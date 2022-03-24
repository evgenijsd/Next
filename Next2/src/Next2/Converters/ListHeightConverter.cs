using System;
using System.Globalization;
using Xamarin.Forms;
using System.Collections;

namespace Next2.Converters
{
    public class ListHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int listHeight = 0;

            if (value is IList list && int.TryParse(parameter as string, out int itemHeight))
            {
                listHeight = list.Count * itemHeight;
            }

            return listHeight;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
