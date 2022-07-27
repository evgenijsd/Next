using System;
using System.Globalization;
using Xamarin.Forms;

namespace Next2.Converters
{
    public class EqualsMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool isObjectsEqual = true;

            if (values is not null && values[0] is not null)
            {
                var firstValue = values[0].ToString();

                foreach (var value in values)
                {
                    if (value?.ToString() != firstValue)
                    {
                        isObjectsEqual = false;

                        break;
                    }
                }
            }
            else
            {
                isObjectsEqual = false;
            }

            return isObjectsEqual;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return targetTypes;
        }
    }
}
