using System;
using System.Text.RegularExpressions;

namespace Next2.Helpers
{
    public static class Filters
    {
        public static string ApplyNumberFilter(string text)
        {
            var result = text;

            try
            {
                Regex regexNumber = new(Constants.Validators.NUMBER);
                result = regexNumber.Replace(text, string.Empty);
            }
            catch (Exception)
            {
            }

            return result;
        }

        public static string ApplyNameFilter(string text)
        {
            var result = text;

            try
            {
                Regex regexName = new(Constants.Validators.NAME);
                Regex regexNumber = new(Constants.Validators.NUMBER);
                Regex regexText = new(Constants.Validators.TEXT);

                result = regexText.Replace(text, string.Empty);

                result = Regex.IsMatch(result, Constants.Validators.CHECK_NUMBER)
                    ? regexNumber.Replace(result, string.Empty)
                    : regexName.Replace(result, string.Empty);
            }
            catch (Exception)
            {
            }

            return result;
        }
    }
}
