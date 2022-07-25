using Next2.Enums;
using Next2.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace Next2.Controls
{
    public partial class Calendar : Grid
    {
        private bool _isFutureYearSelected = false;

        public Calendar()
        {
            InitializeComponent();

            int monthNumber = 1;
            Months = new(DateTimeFormatInfo.CurrentInfo.MonthNames
                .Where(x => x != string.Empty)
                .Select(y => new Month { Name = y, Number = monthNumber++ }));

            SelectedMonth = DateTime.Now.Month;

            Years = new List<Year>();

            for (int i = Constants.Limits.MIN_YEAR; i <= Constants.Limits.MAX_YEAR; i++)
            {
                Years.Add(new Year() { YearValue = i, Opacity = i <= DateTime.Now.Year ? 1 : 0.32 });
            }

            SelectedYear = Years.FirstOrDefault(x => x.YearValue == DateTime.Now.Year - 1);
        }

        #region -- Public properties --

        public static readonly BindableProperty BackGroundColorProperty = BindableProperty.Create(
            propertyName: nameof(BackGroundColor),
            returnType: typeof(Color),
            defaultValue: Color.FromHex("#2E3143"),
            declaringType: typeof(Calendar),
            defaultBindingMode: BindingMode.TwoWay);
        public Color BackGroundColor
        {
            get => (Color)GetValue(BackGroundColorProperty);
            set => SetValue(BackGroundColorProperty, value);
        }

        public static readonly BindableProperty DropdownListBackGroundColorProperty = BindableProperty.Create(
            propertyName: nameof(DropdownListBackGroundColor),
            returnType: typeof(Color),
            defaultValue: Color.FromHex("#34374C"),
            declaringType: typeof(Calendar),
            defaultBindingMode: BindingMode.TwoWay);
        public Color DropdownListBackGroundColor
        {
            get => (Color)GetValue(DropdownListBackGroundColorProperty);
            set => SetValue(DropdownListBackGroundColorProperty, value);
        }

        public static readonly BindableProperty DropdownHeadDroppedBackGroundColorProperty = BindableProperty.Create(
            propertyName: nameof(DropdownHeadDroppedBackGroundColor),
            returnType: typeof(Color),
            defaultValue: Color.FromHex("#252836"),
            declaringType: typeof(Calendar),
            defaultBindingMode: BindingMode.TwoWay);
        public Color DropdownHeadDroppedBackGroundColor
        {
            get => (Color)GetValue(DropdownHeadDroppedBackGroundColorProperty);
            set => SetValue(DropdownHeadDroppedBackGroundColorProperty, value);
        }

        public static readonly BindableProperty MonthLabelFontFamilyProperty = BindableProperty.Create(
            propertyName: nameof(MonthLabelFontFamily),
            returnType: typeof(string),
            defaultValue: "Barlow-Bold",
            declaringType: typeof(Calendar),
            defaultBindingMode: BindingMode.TwoWay);
        public string MonthLabelFontFamily
        {
            get => (string)GetValue(MonthLabelFontFamilyProperty);
            set => SetValue(MonthLabelFontFamilyProperty, value);
        }

        public static readonly BindableProperty DropdownListLabelsFontFamilyProperty = BindableProperty.Create(
            propertyName: nameof(DropdownListLabelsFontFamily),
            returnType: typeof(string),
            defaultValue: "Barlow-Regular",
            declaringType: typeof(Calendar),
            defaultBindingMode: BindingMode.TwoWay);
        public string DropdownListLabelsFontFamily
        {
            get => (string)GetValue(DropdownListLabelsFontFamilyProperty);
            set => SetValue(DropdownListLabelsFontFamilyProperty, value);
        }

        public static readonly BindableProperty TitleProperty = BindableProperty.Create(
           propertyName: nameof(Title),
           returnType: typeof(string),
           declaringType: typeof(Calendar),
           defaultValue: string.Empty,
           defaultBindingMode: BindingMode.TwoWay);
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly BindableProperty SelectedDateProperty = BindableProperty.Create(
            propertyName: nameof(SelectedDate),
            returnType: typeof(DateTime?),
            declaringType: typeof(Calendar),
            defaultBindingMode: BindingMode.TwoWay);
        public DateTime? SelectedDate
        {
            get => (DateTime?)GetValue(SelectedDateProperty);
            set => SetValue(SelectedDateProperty, value);
        }

        public static readonly BindableProperty SelectedStartDateProperty = BindableProperty.Create(
            propertyName: nameof(SelectedStartDate),
            returnType: typeof(DateTime?),
            declaringType: typeof(Calendar),
            defaultValue: null,
            defaultBindingMode: BindingMode.TwoWay);

        public DateTime? SelectedStartDate
        {
            get => (DateTime?)GetValue(SelectedStartDateProperty);
            set => SetValue(SelectedStartDateProperty, value);
        }

        public static readonly BindableProperty SelectedMonthProperty = BindableProperty.Create(
            propertyName: nameof(SelectedMonth),
            returnType: typeof(int),
            declaringType: typeof(Calendar),
            defaultValue: 3,
            defaultBindingMode: BindingMode.TwoWay);
        public int SelectedMonth
        {
            get => (int)GetValue(SelectedMonthProperty);
            set => SetValue(SelectedMonthProperty, value);
        }

        public static readonly BindableProperty SelectedYearProperty = BindableProperty.Create(
            propertyName: nameof(SelectedYear),
            returnType: typeof(Year),
            declaringType: typeof(Calendar),
            defaultBindingMode: BindingMode.TwoWay);
        public Year SelectedYear
        {
            get => (Year)GetValue(SelectedYearProperty);
            set => SetValue(SelectedYearProperty, value);
        }

        public static readonly BindableProperty OffsetYearsProperty = BindableProperty.Create(
            propertyName: nameof(OffsetYears),
            returnType: typeof(int),
            declaringType: typeof(Calendar),
            defaultValue: 0,
            defaultBindingMode: BindingMode.TwoWay);

        public int OffsetYears
        {
            get => (int)GetValue(OffsetYearsProperty);
            set => SetValue(OffsetYearsProperty, value);
        }

        public static readonly BindableProperty DropdownHeadLabelFontSizeProperty = BindableProperty.Create(
           propertyName: nameof(DropdownHeadLabelFontSize),
           returnType: typeof(double),
           defaultValue: 18d,
           declaringType: typeof(Calendar),
           defaultBindingMode: BindingMode.TwoWay);

        public double DropdownHeadLabelFontSize
        {
            get => (double)GetValue(DropdownHeadLabelFontSizeProperty);
            set => SetValue(DropdownHeadLabelFontSizeProperty, value);
        }

        public static readonly BindableProperty MonthLabelFontSizeProperty = BindableProperty.Create(
           propertyName: nameof(MonthLabelFontSize),
           returnType: typeof(double),
           defaultValue: 26d,
           declaringType: typeof(Calendar),
           defaultBindingMode: BindingMode.TwoWay);

        public double MonthLabelFontSize
        {
            get => (double)GetValue(MonthLabelFontSizeProperty);
            set => SetValue(MonthLabelFontSizeProperty, value);
        }

        public static readonly BindableProperty MonthStepperIconScaleProperty = BindableProperty.Create(
           propertyName: nameof(MonthStepperIconScale),
           returnType: typeof(double),
           defaultValue: 0.6d,
           declaringType: typeof(Calendar),
           defaultBindingMode: BindingMode.TwoWay);

        public double MonthStepperIconScale
        {
            get => (double)GetValue(MonthStepperIconScaleProperty);
            set => SetValue(MonthStepperIconScaleProperty, value);
        }

        public static readonly BindableProperty DayLabelFontSizeProperty = BindableProperty.Create(
           propertyName: nameof(DayLabelFontSize),
           returnType: typeof(double),
           defaultValue: 20d,
           declaringType: typeof(Calendar),
           defaultBindingMode: BindingMode.TwoWay);

        public double DayLabelFontSize
        {
            get => (double)GetValue(DayLabelFontSizeProperty);
            set => SetValue(DayLabelFontSizeProperty, value);
        }

        public static readonly BindableProperty DropdownHeadHeightRequestProperty = BindableProperty.Create(
           propertyName: nameof(DropdownHeadHeightRequest),
           returnType: typeof(double),
           defaultValue: 45d,
           declaringType: typeof(Calendar),
           defaultBindingMode: BindingMode.TwoWay);

        public double DropdownHeadHeightRequest
        {
            get => (double)GetValue(DropdownHeadHeightRequestProperty);
            set => SetValue(DropdownHeadHeightRequestProperty, value);
        }

        public static readonly BindableProperty DropdownWidthRequestProperty = BindableProperty.Create(
           propertyName: nameof(DropdownWidthRequest),
           returnType: typeof(double),
           defaultValue: 122d,
           declaringType: typeof(Calendar),
           defaultBindingMode: BindingMode.TwoWay);

        public double DropdownWidthRequest
        {
            get => (double)GetValue(DropdownWidthRequestProperty);
            set => SetValue(DropdownWidthRequestProperty, value);
        }

        public static readonly BindableProperty DropdownHeadWidthRequestProperty = BindableProperty.Create(
           propertyName: nameof(DropdownHeadWidthRequest),
           returnType: typeof(double),
           defaultValue: 122d,
           declaringType: typeof(Calendar),
           defaultBindingMode: BindingMode.TwoWay);

        public double DropdownHeadWidthRequest
        {
            get => (double)GetValue(DropdownHeadWidthRequestProperty);
            set => SetValue(DropdownHeadWidthRequestProperty, value);
        }

        public List<Month> Months { get; set; }

        public List<Year> Years { get; set; }

        public Month? Month => Months is not null && Months.Any()
            ? Months[SelectedMonth - 1]
            : null;

        public Day SelectedDay { get; set; }

        private ICommand _selectYearCommand;
        public ICommand SelectYearCommand => _selectYearCommand ??= new Command(() => dropdownFrame.IsVisible = false);

        #endregion

        #region -- Overrides --

        protected override void OnPropertyChanged(string? propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == nameof(SelectedDate) && SelectedDate is not null)
            {
                SelectedYear = Years.FirstOrDefault(x => x.YearValue == SelectedDate.Value.Year);
                SelectedMonth = SelectedDate.Value.Month;
                SelectedDay = new Day
                {
                    DayOfMonth = SelectedDate.Value.Day.ToString(),
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
                if (SelectedDate is not null && SelectedYear?.YearValue == SelectedDate.Value.Year && SelectedMonth == SelectedDate.Value.Month)
                {
                    SelectedDay = new Day
                    {
                        DayOfMonth = SelectedDate.Value.Day.ToString(),
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
            }
            else if (propertyName == nameof(SelectedYear))
            {
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
            dropdownFrame.IsVisible = !dropdownFrame.IsVisible;

            if (dropdownFrame.IsVisible)
            {
                yearsCollectionView.ScrollTo(SelectedYear, -1, ScrollToPosition.Center, false);
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