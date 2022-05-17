using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Next2.Helpers.Extensions
{
    public static class FormatPhoneNumberExtension
    {
        public static string FormatPhoneNumber(this string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
            {
                return phoneNumber;
            }

            Regex phoneParser = null;
            string format = string.Empty;

            switch (phoneNumber.Length)
            {
                case 5:
                    phoneParser = new Regex(@"(\d{3})(\d{2})");
                    format = "$1 $2";
                    break;

                case 6:
                    phoneParser = new Regex(@"(\d{2})(\d{2})(\d{2})");
                    format = "$1 $2 $3";
                    break;

                case 7:
                    phoneParser = new Regex(@"(\d{3})(\d{2})(\d{2})");
                    format = "$1 $2 $3";
                    break;

                case 8:
                    phoneParser = new Regex(@"(\d{4})(\d{2})(\d{2})");
                    format = "$1 $2 $3";
                    break;

                case 9:
                    phoneParser = new Regex(@"(\d{4})(\d{3})(\d{2})(\d{2})");
                    format = "$1 $2 $3 $4";
                    break;

                case 10:
                    phoneParser = new Regex(@"(\d{3})(\d{3})(\d{4})");
                    format = "$1-$2-$3";
                    break;

                case 11:
                    phoneParser = new Regex(@"(\d{4})(\d{3})(\d{2})(\d{2})");
                    format = "$1 $2 $3 $4";
                    break;

                default:
                    return phoneNumber;
            }

            return phoneParser.Replace(phoneNumber, format);
        }
    }
}
