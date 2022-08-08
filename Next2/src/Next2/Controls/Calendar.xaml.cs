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

        public List<Month> Months { get; set; }

        public List<Year> Years { get; set; }

        public Month? Month => Months is not null && Months.Any()
            ? Months[SelectedMonth - 1]
            : null;

        public Day SelectedDay { get; set; }

        private ICommand? _selectYearCommand;
        public ICommand SelectYearCommand => _selectYearCommand ??= new Command(OnSelectYearCommand);

        #endregion

        #region -- Overrides --

        protected override void OnPropertyChanged(string? propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == nameof(SelectedDay) && dropdownFrame.IsVisible)
            {
                dropdownFrame.IsVisible = false;
                yearDropdownFrame.BackgroundColor = (Color)App.Current.Resources["TextAndBackgroundColor_i4"];
                yearDropdownIcon.Source = "ic_arrow_down_primary_24x24";
            }
            else if (propertyName == nameof(SelectedYear))
            {
                if (SelectedYear.YearValue > DateTime.Now.Year && !_isFutureYearSelected)
                {
                    _isFutureYearSelected = true;
                    SelectedYear = Years.FirstOrDefault(x => x.YearValue == DateTime.Now.Year);
                    yearsCollectionView.SelectedItem = null;
                }

                _isFutureYearSelected = false;
            }
        }

        #endregion

        #region -- Private helpers --

        private void OnSelectYearCommand() => dropdownFrame.IsVisible = false;

        private void OnYearDropDownTapped(object sender, EventArgs arg)
        {
            if (!dropdownFrame.IsVisible)
            {
                dropdownFrame.IsVisible = true;
                yearsCollectionView.ScrollTo(SelectedYear, -1, ScrollToPosition.Center, false);
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