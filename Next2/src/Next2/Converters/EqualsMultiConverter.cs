using System;
using System.Globalization;
using Xamarin.Forms;

namespace Next2.Converters
{
    public class EqualsMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool result = true;

            if (values is not null)
            {
                foreach (var value in values)
                {
                    foreach (var value2 in values)
                    {
                        if (value?.ToString() != value2?.ToString())
                        {
                            result = false;

                            break;
                        }
                    }
                }
            }
            else
            {
                result = false;
            }

            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return targetTypes;
        }
    }
}
