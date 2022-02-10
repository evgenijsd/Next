using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Next2.Controls
{
    public partial class CalendarGrid : ContentView
    {
        public CalendarGrid()
        {
            InitializeComponent();
        }

        #region -- Public Properties --

        #endregion

        #region -- Private Helpers --

        private int[] CreateArrayOfDays(int year, int month)
        {
            DateTime dt = new DateTime(year, month, 1);
            int currentMonthindex = (int)dt.DayOfWeek;
            int nextmonthIndex = (int)dt.AddMonths(1).DayOfWeek + 35;
            int previousMonthLastDate = dt.AddDays(-1).Day;

            int[] result = new int[42];
            result[currentMonthindex] = 1;
            result[nextmonthIndex] = 1;

            for (int i = currentMonthindex - 1; i >= 0; i--)
            {
                result[i] = previousMonthLastDate--;
            }

            int enumer = 1;
            for (int i = nextmonthIndex + 1; i < result.Length; i++)
            {
                result[i] = enumer++;
            }

            enumer = 1;
            for (int i = currentMonthindex; i <= nextmonthIndex; i++)
            {
                result[i] = enumer++;
            }

            return result;
        }

        #endregion

    }
}