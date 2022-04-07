﻿using Next2.ENums;
using Next2.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
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

        #region -- Public Properties --

        public ObservableCollection<Day> Days { get; set; }

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

            switch (propertyName)
            {
                case nameof(Year):
                    {
                        if (Year <= DateTime.Now.Year)
                        {
                            CreateArrayOfDays();
                        }
                    }

                    break;
                case nameof(Month):
                    {
                        CreateArrayOfDays();
                    }

                    break;
                case nameof(SelectedItem):
                    {
                        DaySelection();
                    }

                    break;
                default:
                    break;
            }
        }

        #endregion

        #region -- Private Helpers --

        private void DaySelection()
        {
            if (SelectedItem is Day selectedDay)
            {
                switch (selectedDay.State)
                {
                    case EDayState.DayMonth:
                        {
                            if (int.TryParse(selectedDay.DayOfMonth, out int daySelected))
                            {
                                if (Year <= DateTime.Now.Year)
                                {
                                    SelectedDate = new DateTime(Year, Month, daySelected);
                                }
                                else
                                {
                                    SelectedItem = null;
                                    SelectedDate = null;
                                }
                            }
                        }

                        break;
                    case EDayState.NoDayMonth:
                        {
                            SelectedItem = null;
                            SelectedDate = null;
                        }

                        break;
                    case EDayState.NameOfDay:
                        {
                            SelectedItem = null;
                            SelectedDate = null;
                        }

                        break;
                    default:
                        break;
                }
            }
        }

        private void CreateArrayOfDays()
        {
            DateTime dt = new DateTime(Year, Month, 1);
            int currentMonthindex = (int)dt.DayOfWeek;
            var countDays = dt.AddMonths(1).Subtract(dt).Days;
            int previousMonthLastDate = dt.AddDays(-1).Day;

            int[] arrayOfDays = new int[42];
            arrayOfDays[currentMonthindex] = 1;
            for (int i = currentMonthindex - 1; i >= 0; i--)
            {
                arrayOfDays[i] = previousMonthLastDate--;
            }

            int enumer = 1;
            for (int i = currentMonthindex; i < arrayOfDays.Length; i++)
            {
                arrayOfDays[i] = enumer++;

                if (enumer == countDays + 1)
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
            var namesOfDays = new string[] { "Sn", "Mn", "Tu", "Wn", "Th", "Fr", "St" };
            foreach (string name in namesOfDays)
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
                    if (state == EDayState.DayMonth)
                    {
                        state = EDayState.NoDayMonth;
                    }
                    else
                    {
                        state = EDayState.DayMonth;
                    }
                }

                Days.Add(new Day { DayOfMonth = day.ToString(), State = state, });
            }
        }

        #endregion
    }
}
