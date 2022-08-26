using Next2.Enums;
using Next2.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Next2.Controls
{
    public partial class Calendar : StackLayout
    {
        private bool _isFutureYearSelected;
        private bool _isDaySelecting;
        private bool _isFirstOpemDropdown = true;

        public Calendar()
        {
            InitializeComponent();

            int monthNumber = 1;
            Months = new(DateTimeFormatInfo.CurrentInfo.MonthNames
                .Where(x => x != string.Empty)
                .Select(y => new Month { Name = y, Number = monthNumber++ }));

            SelectedMonth = DateTime.Now.Month;

            Years = new ();

            for (int i = Constants.Limits.MIN_YEAR; i <= Constants.Limits.MAX_YEAR; i++)
            {
                Years.Add(new Year() { YearValue = i, Opacity = i <= DateTime.Now.Year ? 1 : 0.32 });
            }

            SelectedYear = Years.FirstOrDefault(x => x.YearValue == DateTime.Now.Year - 1);

            SetMarginToDropdownFrame();
        }

        #region -- Public properties --

        public static readonly BindableProperty DropdownListBackgroundColorProperty = BindableProperty.Create(
            propertyName: nameof(DropdownListBackgroundColor),
            returnType: typeof(Color),
            defaultValue: Color.FromHex("#34374C"),
            declaringType: typeof(Calendar),
            defaultBindingMode: BindingMode.OneWay);

        public Color DropdownListBackgroundColor
        {
            get => (Color)GetValue(DropdownListBackgroundColorProperty);
            set => SetValue(DropdownListBackgroundColorProperty, value);
        }

        public static readonly BindableProperty DropdownHeaderDroppedBackgroundColorProperty = BindableProperty.Create(
            propertyName: nameof(DropdownHeaderDroppedBackgroundColor),
            returnType: typeof(Color),
            defaultValue: Color.FromHex("#252836"),
            declaringType: typeof(Calendar),
            defaultBindingMode: BindingMode.OneWay);

        public Color DropdownHeaderDroppedBackgroundColor
        {
            get => (Color)GetValue(DropdownHeaderDroppedBackgroundColorProperty);
            set => SetValue(DropdownHeaderDroppedBackgroundColorProperty, value);
        }

        public static readonly BindableProperty MonthFontFamilyProperty = BindableProperty.Create(
            propertyName: nameof(MonthFontFamily),
            returnType: typeof(string),
            defaultValue: "Barlow-Bold",
            declaringType: typeof(Calendar),
            defaultBindingMode: BindingMode.OneWay);

        public string MonthFontFamily
        {
            get => (string)GetValue(MonthFontFamilyProperty);
            set => SetValue(MonthFontFamilyProperty, value);
        }

        public static readonly BindableProperty DropdownListFontFamilyProperty = BindableProperty.Create(
            propertyName: nameof(DropdownListFontFamily),
            returnType: typeof(string),
            defaultValue: "Barlow-Regular",
            declaringType: typeof(Calendar),
            defaultBindingMode: BindingMode.OneWay);

        public string DropdownListFontFamily
        {
            get => (string)GetValue(DropdownListFontFamilyProperty);
            set => SetValue(DropdownListFontFamilyProperty, value);
        }

        public static readonly BindableProperty TitleProperty = BindableProperty.Create(
           propertyName: nameof(Title),
           returnType: typeof(string),
           declaringType: typeof(Calendar),
           defaultValue: string.Empty,
           defaultBindingMode: BindingMode.OneWay);

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
            defaultBindingMode: BindingMode.OneWay);

        public int OffsetYears
        {
            get => (int)GetValue(OffsetYearsProperty);
            set => SetValue(OffsetYearsProperty, value);
        }

        public static readonly BindableProperty DropdownHeaderFontSizeProperty = BindableProperty.Create(
           propertyName: nameof(DropdownHeaderFontSize),
           returnType: typeof(double),
           defaultValue: 18d,
           declaringType: typeof(Calendar),
           defaultBindingMode: BindingMode.OneWay);

        public double DropdownHeaderFontSize
        {
            get => (double)GetValue(DropdownHeaderFontSizeProperty);
            set => SetValue(DropdownHeaderFontSizeProperty, value);
        }

        public static readonly BindableProperty MonthFontSizeProperty = BindableProperty.Create(
           propertyName: nameof(MonthFontSize),
           returnType: typeof(double),
           defaultValue: 26d,
           declaringType: typeof(Calendar),
           defaultBindingMode: BindingMode.OneWay);

        public double MonthFontSize
        {
            get => (double)GetValue(MonthFontSizeProperty);
            set => SetValue(MonthFontSizeProperty, value);
        }

        public static readonly BindableProperty MonthStepperIconSizeProperty = BindableProperty.Create(
           propertyName: nameof(MonthStepperIconSize),
           returnType: typeof(double),
           defaultValue: 35.25d,
           declaringType: typeof(Calendar),
           defaultBindingMode: BindingMode.OneWay);

        public double MonthStepperIconSize
        {
            get => (double)GetValue(MonthStepperIconSizeProperty);
            set => SetValue(MonthStepperIconSizeProperty, value);
        }

        public static readonly BindableProperty DayFontSizeProperty = BindableProperty.Create(
           propertyName: nameof(DayFontSize),
           returnType: typeof(double),
           defaultValue: 20d,
           declaringType: typeof(Calendar),
           defaultBindingMode: BindingMode.OneWay);

        public double DayFontSize
        {
            get => (double)GetValue(DayFontSizeProperty);
            set => SetValue(DayFontSizeProperty, value);
        }

        public static readonly BindableProperty DropdownHeaderHeightRequestProperty = BindableProperty.Create(
           propertyName: nameof(DropdownHeaderHeightRequest),
           returnType: typeof(double),
           defaultValue: 36d,
           declaringType: typeof(Calendar),
           defaultBindingMode: BindingMode.OneWay);

        public double DropdownHeaderHeightRequest
        {
            get => (double)GetValue(DropdownHeaderHeightRequestProperty);
            set => SetValue(DropdownHeaderHeightRequestProperty, value);
        }

        public static readonly BindableProperty DropdownWidthRequestProperty = BindableProperty.Create(
           propertyName: nameof(DropdownWidthRequest),
           returnType: typeof(double),
           defaultValue: 120d,
           declaringType: typeof(Calendar),
           defaultBindingMode: BindingMode.OneWay);

        public double DropdownWidthRequest
        {
            get => (double)GetValue(DropdownWidthRequestProperty);
            set => SetValue(DropdownWidthRequestProperty, value);
        }

        public static readonly BindableProperty DropdownHeaderWidthRequestProperty = BindableProperty.Create(
           propertyName: nameof(DropdownHeaderWidthRequest),
           returnType: typeof(double),
           defaultValue: 120d,
           declaringType: typeof(Calendar),
           defaultBindingMode: BindingMode.OneWay);

        public double DropdownHeaderWidthRequest
        {
            get => (double)GetValue(DropdownHeaderWidthRequestProperty);
            set => SetValue(DropdownHeaderWidthRequestProperty, value);
        }

        public List<Month> Months { get; set; }

        public ObservableCollection<Year> Years { get; set; }

        public Month? Month => Months is not null && Months.Any()
            ? Months[SelectedMonth - 1]
            : null;

        public Day SelectedDay { get; set; }

        private ICommand? _selectYearCommand;
        public ICommand SelectYearCommand => _selectYearCommand ??= new Command<object>(OnSelectYearCommand);

        private ICommand? _rightMonthTapCommand;
        public ICommand RightMonthTapCommand => _rightMonthTapCommand ??= new AsyncCommand(OnRightMonthTapCommandAsync);

        private ICommand? _leftMonthTapCommand;
        public ICommand LeftMonthTapCommand => _leftMonthTapCommand ??= new AsyncCommand(OnLeftMonthTapCommandAsync);

        private ICommand? _yearDropdownTapCommand;
        public ICommand YearDropdownTapCommand => _yearDropdownTapCommand ??= new AsyncCommand(OnYearDropdownTapCommandAsync);

        #endregion

        #region -- Overrides --

        protected override async void OnPropertyChanged(string? propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            switch (propertyName)
            {
                case nameof(SelectedDate):

                    if (SelectedDate is not null && !_isDaySelecting)
                    {
                        SelectedYear = Years.FirstOrDefault(x => x.YearValue == SelectedDate.Value.Year);
                        SelectedMonth = SelectedDate.Value.Month;

                        await CreateSelectedDay(true);
                    }

                    break;

                case nameof(OffsetYears):

                    var currentDate = DateTime.Now;

                    if (currentDate.Year + OffsetYears > Constants.Limits.MAX_YEAR)
                    {
                        OffsetYears = Constants.Limits.MAX_YEAR - currentDate.Year;
                    }

                    var firstYear = Years.FirstOrDefault(x => x.YearValue == currentDate.Year);
                    var index = Years.IndexOf(firstYear);

                    for (int i = index; i <= index + OffsetYears; i++)
                    {
                        Years[i].Opacity = 1;
                    }

                    break;

                case nameof(SelectedDay):

                    if (dropdownFrame.IsVisible)
                    {
                        dropdownFrame.IsVisible = false;
                    }

                    break;

                case nameof(SelectedYear):

                    currentDate = DateTime.Now;

                    if (SelectedYear.YearValue > currentDate.Year + OffsetYears && !_isFutureYearSelected)
                    {
                        _isFutureYearSelected = true;
                        SelectedYear = Years.FirstOrDefault(x => x.YearValue == currentDate.Year + OffsetYears);
                        yearsCollectionView.SelectedItem = null;
                    }

                    _isFutureYearSelected = false;

                    await CreateSelectedDay();

                    break;

                case nameof(SelectedMonth):

                    await CreateSelectedDay();

                    break;

                case nameof(DropdownHeaderHeightRequest):

                    SetMarginToDropdownFrame();

                    break;
            }
        }

        #endregion

        #region -- Private helpers --

        private void OnSelectYearCommand(object sender)
        {
            if (sender is Year year && year.YearValue <= DateTime.Now.Year)
            {
                SelectedYear = year;
            }

            dropdownFrame.IsVisible = false;
        }

        private void SetMarginToDropdownFrame()
        {
            if (App.IsTablet)
            {
                dropdownFrame.Margin = new Thickness() { Right = 0, Top = DropdownHeaderHeightRequest + 2, Left = 0, Bottom = 0 };
            }
            else
            {
                dropdownFrame.Margin = new Thickness() { Right = 0, Top = DropdownHeaderHeightRequest + 2, Left = 0, Bottom = 0 };
            }
        }

        private void RaiseSelectedDay()
        {
            var tmp = SelectedDay;
            SelectedDay = new();
            SelectedDay = tmp;
        }

        private async Task CreateSelectedDay(bool isAwait = false)
        {
            if (SelectedDate is not null && !_isDaySelecting)
            {
                _isDaySelecting = true;

                SelectedDay = new Day
                {
                    DayOfMonth = SelectedDate.Value.Day.ToString(),
                    State = EDayState.DayMonth,
                };

                if (Device.RuntimePlatform == Device.iOS)
                {
                    if (isAwait)
                    {
                        await Task.Delay(333);
                    }

                    RaiseSelectedDay();
                }

                _isDaySelecting = false;
            }
        }

        private async Task OnYearDropdownTapCommandAsync()
        {
            dropdownFrame.IsVisible = !dropdownFrame.IsVisible;

            if (dropdownFrame.IsVisible)
            {
                if (Device.RuntimePlatform == Device.iOS)
                {
                    if (_isFirstOpemDropdown)
                    {
                        await Task.Delay(200);
                        _isFirstOpemDropdown = false;
                    }
                }

                yearsCollectionView.ScrollTo(SelectedYear, -1, ScrollToPosition.Center, false);
            }

            //return Task.CompletedTask;
        }

        private Task OnRightMonthTapCommandAsync()
        {
            if (SelectedMonth == 12)
            {
                SelectedMonth = 1;
            }
            else
            {
                SelectedMonth++;
            }

            return Task.CompletedTask;
        }

        private Task OnLeftMonthTapCommandAsync()
        {
            if (SelectedMonth == 1)
            {
                SelectedMonth = 12;
            }
            else
            {
                SelectedMonth--;
            }

            return Task.CompletedTask;
        }

        #endregion
    }
}