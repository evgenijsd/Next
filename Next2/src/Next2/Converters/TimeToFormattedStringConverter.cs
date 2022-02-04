using System;
using System.Globalization;
using Xamarin.Forms;

namespace Next2.Converters
{
    public class TimeToFormattedStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string result = string.Empty;

            if (value != null)
            {
                if (parameter == null)
                {
                    parameter = Constants.LONG_DATE_FORMAT;
                }

                try
                {
                    result = ((DateTime)value).ToString((string)parameter);
                }
                catch (Exception)
                {
                    result = "Incorrect date format";
                }
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
