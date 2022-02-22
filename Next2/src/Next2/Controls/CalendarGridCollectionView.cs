using Next2.ENums;
using Next2.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Next2.Controls
{
    public class CalendarGridCollectionView : CollectionView
    {
        public CalendarGridCollectionView()
        {
            CategoriesItems = new ();
            CreateArrayOfDays();
            this.ItemsSource = CategoriesItems;
            this.SelectionMode = SelectionMode.Single;
        }

        #region -- Public Properties --

        public ObservableCollection<DayModel> CategoriesItems { get; set; }

        public static readonly BindableProperty YearProperty = BindableProperty.Create(
            propertyName: nameof(Year),
            returnType: typeof(int),
            declaringType: typeof(CalendarGridCollectionView),
            defaultValue: DateTime.Now.Year,
            defaultBindingMode: BindingMode.TwoWay);

        public int Year
        {
            get => (int)GetValue(YearProperty);
            set => SetValue(YearProperty, value);
        }

        public static readonly BindableProperty MonthProperty = BindableProperty.Create(
            propertyName: nameof(Month),
            returnType: typeof(int),
            declaringType: typeof(CalendarGridCollectionView),
            defaultValue: DateTime.Now.Month,
            defaultBindingMode: BindingMode.TwoWay);

        public int Month
        {
            get => (int)GetValue(MonthProperty);
            set => SetValue(MonthProperty, value);
        }

        public static readonly BindableProperty SelectedDateProperty = BindableProperty.Create(
            propertyName: nameof(SelectedDate),
            returnType: typeof(DateTime?),
            declaringType: typeof(CalendarGridCollectionView),
            defaultBindingMode: BindingMode.TwoWay);

        public DateTime? SelectedDate
        {
            get => (DateTime?)GetValue(SelectedDateProperty);
            set => SetValue(SelectedDateProperty, value);
        }

        #endregion

        #region -- Overrides --

        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == nameof(Year))
            {
                CreateArrayOfDays();
            }

            if (propertyName == nameof(Month))
            {
                CreateArrayOfDays();
            }

            if (propertyName == nameof(SelectedItem))
            {
                if (SelectedItem is DayModel selectedDay)
                {
                    if (selectedDay.State == ENums.EDayState.NoDayMonth || selectedDay.State == ENums.EDayState.NameOfDay || Year > DateTime.Now.Year)
                    {
                        if (selectedDay.State == ENums.EDayState.NoDayMonth)
                        {
                            if (int.TryParse(selectedDay.Day, out int day) && day > 21)
                            {
                                if (Month == 1)
                                {
                                    Month = 12;
                                    Year--;
                                }
                                else
                                {
                                    Month--;
                                }
                            }
                            else
                            {
                                if (Month == 12)
                                {
                                    Month = 1;
                                    Year++;
                                }
                                else
                                {
                                    Month++;
                                }
                            }

                            SelectedItem = CategoriesItems.Where(x => x.Day == selectedDay.Day).FirstOrDefault();
                        }
                        else
                        {
                            SelectedItem = null;
                            SelectedDate = null;
                        }
                    }
                    else if (int.TryParse(selectedDay.Day, out int daySelected))
                    {
                        SelectedDate = new DateTime(Year, Month, daySelected);
                    }
                }
            }
        }

        #endregion

        #region -- Private Helpers --

        private void CreateArrayOfDays()
        {
            DateTime dt = new DateTime(Year, Month, 1);
            int currentMonthindex = (int)dt.DayOfWeek;
            var countDays = dt.AddMonths(1).Subtract(dt).Days;
            int previousMonthLastDate = dt.AddDays(-1).Day;

            int[] result = new int[42];
            result[currentMonthindex] = 1;
            for (int i = currentMonthindex - 1; i >= 0; i--)
            {
                result[i] = previousMonthLastDate--;
            }

            int enumer = 1;
            for (int i = currentMonthindex; i < result.Length; i++)
            {
                result[i] = enumer++;

                if (enumer == countDays + 1)
                {
                    enumer = 1;
                }
            }

            CategoriesItems.Clear();
            var namesOfDays = new string[] { "Sn", "Mn", "Tu", "Wn", "Th", "Fr", "St" };
            foreach (string name in namesOfDays)
            {
                CategoriesItems.Add(new DayModel { Day = name, State = EDayState.NameOfDay });
            }

            EDayState state = EDayState.NoDayMonth;
            bool isNewMonth = false;
            foreach (var day in result)
            {
                if (day == 1 && isNewMonth)
                {
                    state = EDayState.NoDayMonth;
                }

                if (day == 1 && !isNewMonth)
                {
                    state = EDayState.DayMonth;
                    isNewMonth = true;
                }

                CategoriesItems.Add(new DayModel { Day = day.ToString(), State = state, });
            }
        }

        #endregion
    }
}
