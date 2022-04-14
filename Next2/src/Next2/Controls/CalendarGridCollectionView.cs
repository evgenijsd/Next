using Next2.Enums;
using Next2.Models;
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
            declaringType: typeof(CalendarGridCollectionView2),
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
                    }

                    break;

                case nameof(Month):
                    CreateArrayOfDays();
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
                    if (int.TryParse(selectedDay.DayOfMonth, out int numberOfselectedDay))
                    {
                        if (Year <= DateTime.Now.Year + OffsetYears)
                        {
                            SelectedDate = new DateTime(Year, Month, numberOfselectedDay);
                        }
                        else
                        {
                            SelectedItem = SelectedDate = null;
                        }
                    }
                }
                else if (selectedDay.State is EDayState.NoDayMonth or EDayState.NameOfDay)
                {
                    SelectedItem = SelectedDate = null;
                }
            }
        }

        private void CreateArrayOfDays()
        {
            DateTime dateTime = new DateTime(Year, Month, 1);
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

                Days.Add(new Day { DayOfMonth = day.ToString(), State = state, });
            }
        }

        #endregion
    }
}
