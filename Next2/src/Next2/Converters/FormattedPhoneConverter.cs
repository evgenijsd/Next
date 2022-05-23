using System;
using System.Globalization;
using Xamarin.Forms;
using System.Collections;
using System.Text;

namespace Next2.Converters
{
    public class FormattedPhoneConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var phone = value as string;

            if (!string.IsNullOrEmpty(phone))
            {
                value = GetPhoneFormat(phone);
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        #region -- Private helpers --

        private string GetPhoneFormat(string? phone)
        {
            StringBuilder result = new();
            int interval = 3;
            int index = 0;

            for (int i = 0; i < phone?.Length; i++)
            {
                result.Append(phone[i]);

                if ((i + 1) % interval == 0)
                {
                    result.Append("-");
                    index = result.Length - 1;
                }
            }

            if (index + interval != result.Length)
            {
                result.Remove(index, 1);
            }

            return result.ToString();
        }

        #endregion
    }
}
