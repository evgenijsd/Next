using System;
using System.Globalization;
using Xamarin.Forms;
using System.Collections;
using System.Text;
using Xamarin.CommunityToolkit.Helpers;

namespace Next2.Converters
{
    public class TableNumberToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return GetTableName((int)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        #region -- Private helpers --

        private string GetTableName(int table)
        {
            string result = $"{LocalizationResourceManager.Current["Table"]} {table}";

            if (table == 0)
            {
                result = $"{LocalizationResourceManager.Current["AllTables"]}";
            }

            return result;
        }

        #endregion
    }
}
