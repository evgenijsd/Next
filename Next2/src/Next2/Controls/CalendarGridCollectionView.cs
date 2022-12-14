using Next2.Enums;
using Next2.Helpers;
using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace Next2.Controls
{
    public class CalendarGridCollectionView : CollectionView
    {
        public CalendarGridCollectionView()
        {
            Days = new ();
            CreateArrayOfDays();
            ItemsSource = Days;
            SelectionMode = SelectionMode.Single;
            VerticalScrollBarVisibility = ScrollBarVisibility.Never;
            HorizontalScrollBarVisibility = ScrollBarVisibility.Never;
        }

        #region -- Public properties --

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

        public static readonly BindableProperty OffsetYearsProperty = BindableProperty.Create(
            propertyName: nameof(OffsetYears),
            returnType: typeof(int),
            declaringType: typeof(CalendarGridCollectionView),
            defaultValue: 0,
            defaultBindingMode: BindingMode.TwoWay);

        public int OffsetYears
        {
            get => (int)GetValue(OffsetYearsProperty);
            set => SetValue(OffsetYearsProperty, value);
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

        public static readonly BindableProperty SelectedStartDateProperty = BindableProperty.Create(
            propertyName: nameof(SelectedStartDate),
            returnType: typeof(DateTime?),
            declaringType: typeof(CalendarGridCollectionView),
            defaultBindingMode: BindingMode.TwoWay);

        public DateTime? SelectedStartDate
        {
            get => (DateTime?)GetValue(SelectedStartDateProperty);
            set => SetValue(SelectedStartDateProperty, value);
        }

        public ObservableCollection<Day> Days { get; set; }

        #endregion

        #region -- Overrides --

        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);

            switch (propertyName)
            {
                case nameof(Year):
                    if (Year <= DateTime.Now.Year + OffsetYears)
                    {
                        CreateArrayOfDays();
                        DaySelection();
                    }

                    break;

                case nameof(SelectedStartDate):
                    CreateArrayOfDays();
                    break;

                case nameof(Month):
                    CreateArrayOfDays();
                    DaySelection();
                    break;

                case nameof(SelectedItem):
                    DaySelection();
                    break;
            }
        }

        #endregion

        #region -- Private helpers --

        private void DaySelection()
        {
            if (SelectedItem is Day selectedDay)
            {
                if (selectedDay.State is EDayState.DayMonth)
                {
                    if (int.TryParse(selectedDay.DayOfMonth, out int numberOfSelectedDay))
                    {
                        if (numberOfSelectedDay <= DateTime.DaysInMonth(Year, Month))
                        {
                            var newSelectedDate = new DateTime(Year, Month, numberOfSelectedDay);

                            if (newSelectedDate <= DateTime.Now
                                || (OffsetYears > 0 && Year <= DateTime.Now.Year + OffsetYears))
                            {
                                SelectedDate = newSelectedDate;
                            }
                            else
                            {
                                SelectedItem = SelectedDate = null;
                            }
                        }
                    }
                }
                else
                {
                    SelectedItem = SelectedDate = null;
                }
            }
        }

        private void CreateArrayOfDays()
        {
            DateTime dateTime = new (Year, Month, 1);
            int currentMonthIndex = (int)dateTime.DayOfWeek;
            var dayCounter = dateTime.AddMonths(1).Subtract(dateTime).Days;
            int previousMonthLastDate = dateTime.AddDays(-1).Day;

            int[] arrayOfDays = new int[Constants.Limits.DAYS_IN_CALENDAR];
            arrayOfDays[currentMonthIndex] = 1;

            for (int i = currentMonthIndex - 1; i >= 0; i--)
            {
                arrayOfDays[i] = previousMonthLastDate--;
            }

            for (int enumer = 1, i = currentMonthIndex; i < arrayOfDays.Length; i++)
            {
                arrayOfDays[i] = enumer++;

                if (enumer == dayCounter + 1)
                {
                    enumer = 1;
                }
            }

            AddDayNames();

            AddDays(arrayOfDays);
        }

        private void AddDayNames()
        {
            Days.Clear();

            foreach (string name in new string[] { "Sn", "Mn", "Tu", "Wn", "Th", "Fr", "St" })
            {
                Days.Add(new Day { DayOfMonth = name, State = EDayState.NameOfDay });
            }
        }

        private void AddDays(int[] arrayOfDays)
        {
            EDayState state = EDayState.NoDayMonth;

            foreach (var day in arrayOfDays)
            {
                if (day == 1)
                {
                    state = state == EDayState.DayMonth
                        ? EDayState.NoDayMonth
                        : EDayState.DayMonth;
                }

                var saveState = state;

                if (SelectedStartDate is not null)
                {
                    var selectedStartDate = SelectedStartDate.Value;
                    var selectedYear = selectedStartDate.Year;
                    var selectedMonth = selectedStartDate.Month;

                    if (selectedYear > Year
                        || (selectedYear == Year && selectedMonth > Month)
                        || (selectedYear == Year && selectedMonth == Month && selectedStartDate.Day > day))
                    {
                        state = EDayState.NoDayMonth;
                    }
                }

                Days.Add(new Day { DayOfMonth = day.ToString(), State = state, });
                state = saveState;
            }
        }

        #endregion
    }
}
