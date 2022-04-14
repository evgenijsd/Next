using Next2.Enums;
using Next2.Helpers;
using Next2.Models;
using Next2.Resources.Strings;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Xamarin.Forms;

namespace Next2.Controls
{
    public partial class CalendarTab2 : Grid
    {
        private bool _isFutureYearSelected = false;

        public CalendarTab2()
        {
            InitializeComponent();

            int monthNumber = 1;
            Months = new(DateTimeFormatInfo.CurrentInfo.MonthNames
                .Where(x => x != string.Empty)
                .Select(y => new Month { Name = y, Number = monthNumber++ }));

            SelectedMonth = DateTime.Now.Month;

            Years = new List<Year>();

            for (int i = Constants.Limits.MIN_YEAR; i < Constants.Limits.MAX_YEAR; i++)
            {
                Years.Add(new Year() { YearValue = i, Opacity = i <= DateTime.Now.Year ? 1 : 0.32 });
            }

            SelectedYear = Years.FirstOrDefault(x => x.YearValue == DateTime.Now.Year);
        }

        #region -- Public properties --

        public List<Month> Months { get; set; }

        public List<Year> Years { get; set; }

        public Month? Month => Months is not null && Months.Any()
            ? Months[SelectedMonth - 1]
            : null;

        public Day SelectedDay { get; set; }

        public static readonly BindableProperty SelectedDateProperty = BindableProperty.Create(
            propertyName: nameof(SelectedDate),
            returnType: typeof(DateTime?),
            declaringType: typeof(CalendarTab2),
            defaultValue: null,
            defaultBindingMode: BindingMode.TwoWay);

        public DateTime? SelectedDate
        {
            get => (DateTime?)GetValue(SelectedDateProperty);
            set => SetValue(SelectedDateProperty, value);
        }

        public static readonly BindableProperty SelectedEndDateProperty = BindableProperty.Create(
            propertyName: nameof(SelectedEndDate),
            returnType: typeof(DateTime?),
            declaringType: typeof(CalendarTab2),
            defaultBindingMode: BindingMode.TwoWay);

        public DateTime? SelectedEndDate
        {
            get => (DateTime?)GetValue(SelectedEndDateProperty);
            set => SetValue(SelectedEndDateProperty, value);
        }

        public static readonly BindableProperty TitleProperty = BindableProperty.Create(
            propertyName: nameof(Title),
            returnType: typeof(string),
            declaringType: typeof(CalendarTab2),
            defaultValue: Strings.Birthday,
            defaultBindingMode: BindingMode.TwoWay);

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly BindableProperty SelectedMonthProperty = BindableProperty.Create(
            propertyName: nameof(SelectedMonth),
            returnType: typeof(int),
            declaringType: typeof(CalendarTab2),
            defaultValue: 3,
            defaultBindingMode: BindingMode.TwoWay);

        public int SelectedMonth
        {
            get => (int)GetValue(SelectedMonthProperty);
            set => SetValue(SelectedMonthProperty, value);
        }

        public static readonly BindableProperty OffsetYearsProperty = BindableProperty.Create(
            propertyName: nameof(OffsetYears),
            returnType: typeof(int),
            declaringType: typeof(CalendarTab2),
            defaultValue: 0,
            defaultBindingMode: BindingMode.TwoWay);

        public int OffsetYears
        {
            get => (int)GetValue(OffsetYearsProperty);
            set => SetValue(OffsetYearsProperty, value);
        }

        public static readonly BindableProperty SelectedYearProperty = BindableProperty.Create(
            propertyName: nameof(SelectedYear),
            returnType: typeof(Year),
            declaringType: typeof(CalendarTab2),
            defaultBindingMode: BindingMode.TwoWay);

        public Year SelectedYear
        {
            get => (Year)GetValue(SelectedYearProperty);
            set => SetValue(SelectedYearProperty, value);
        }

        #endregion

        #region -- Overrides --

        protected override void OnPropertyChanged(string? propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == nameof(SelectedEndDate) && SelectedEndDate is not null)
            {
                SelectedYear = Years.FirstOrDefault(x => x.YearValue == SelectedEndDate.Value.Year);
                SelectedMonth = SelectedEndDate.Value.Month;
                SelectedDay = new Day
                {
                    DayOfMonth = SelectedEndDate.Value.Day.ToString(),
                    State = EDayState.DayMonth,
                };
            }

            if (propertyName == nameof(OffsetYears))
            {
                if (DateTime.Now.Year + OffsetYears > Constants.Limits.MAX_YEAR)
                {
                    OffsetYears = Constants.Limits.MAX_YEAR - DateTime.Now.Year;
                }

                for (int i = DateTime.Now.Year; i <= DateTime.Now.Year + OffsetYears; i++)
                {
                    var year = Years.FirstOrDefault(x => x.YearValue == i);
                    year.Opacity = 1;
                }
            }

            if (propertyName == nameof(SelectedMonth) || propertyName == nameof(SelectedYear))
            {
                if (SelectedEndDate is not null && SelectedYear.YearValue == SelectedEndDate.Value.Year && SelectedMonth == SelectedEndDate.Value.Month)
                {
                    SelectedDay = new Day
                    {
                        DayOfMonth = SelectedEndDate.Value.Day.ToString(),
                        State = EDayState.DayMonth,
                    };
                }
                else
                {
                    SelectedDay = new();
                }
            }

            if (propertyName == nameof(SelectedDay) && dropdownFrame.IsVisible)
            {
                dropdownFrame.IsVisible = false;
                yearDropdownFrame.BackgroundColor = (Color)App.Current.Resources["TextAndBackgroundColor_i4"];
                yearDropdownIcon.Source = "ic_arrow_down_primary_24x24";
            }
            else if (propertyName == nameof(SelectedYear))
            {
                dropdownFrame.IsVisible = false;
                yearDropdownFrame.BackgroundColor = (Color)App.Current.Resources["TextAndBackgroundColor_i4"];
                yearDropdownIcon.Source = "ic_arrow_down_primary_24x24";

                if (SelectedYear.YearValue > DateTime.Now.Year + OffsetYears && !_isFutureYearSelected)
                {
                    _isFutureYearSelected = true;
                    SelectedYear = Years.FirstOrDefault(x => x.YearValue == DateTime.Now.Year + OffsetYears);
                    yearsCollectionView.SelectedItem = null;
                }

                _isFutureYearSelected = false;
            }
        }

        #endregion

        #region -- Private helpers --

        private void OnYearDropDownTapped(object sender, EventArgs arg)
        {
            if (!dropdownFrame.IsVisible)
            {
                yearsCollectionView.ScrollTo(SelectedYear, -1, ScrollToPosition.Center, false);
                dropdownFrame.IsVisible = true;
                yearDropdownFrame.BackgroundColor = (Color)App.Current.Resources["TextAndBackgroundColor_i5"];
                yearDropdownIcon.Source = "ic_arrow_up_24x24";
            }
            else
            {
                dropdownFrame.IsVisible = false;
                yearDropdownFrame.BackgroundColor = (Color)App.Current.Resources["TextAndBackgroundColor_i4"];
                yearDropdownIcon.Source = "ic_arrow_down_primary_24x24";
            }
        }

        private void OnRightMonthButtonTapped(object? sender, EventArgs? arg)
        {
            if (SelectedMonth == 12)
            {
                SelectedMonth = 1;
            }
            else
            {
                SelectedMonth++;
            }
        }

        private void OnLeftMonthButtonTapped(object? sender, EventArgs? arg)
        {
            if (SelectedMonth == 1)
            {
                SelectedMonth = 12;
            }
            else
            {
                SelectedMonth--;
            }
        }

        #endregion
    }
}