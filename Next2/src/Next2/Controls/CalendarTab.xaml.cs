using Next2.Models;
using System;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Forms;

namespace Next2.Controls
{
    public partial class CalendarTab : Grid
    {
        private const int MIN_YEAR = 1900;
        private const int MAX_YEAR = 2100;
        private bool _isFutureYearSelected = false;
        public CalendarTab()
        {
            InitializeComponent();
            Months = new()
            {
                new MonthModel() { Id = 1, MonthName = "January" },
                new MonthModel() { Id = 2, MonthName = "February" },
                new MonthModel() { Id = 3, MonthName = "March" },
                new MonthModel() { Id = 4, MonthName = "April" },
                new MonthModel() { Id = 5, MonthName = "May" },
                new MonthModel() { Id = 6, MonthName = "June" },
                new MonthModel() { Id = 7, MonthName = "July" },
                new MonthModel() { Id = 8, MonthName = "August" },
                new MonthModel() { Id = 9, MonthName = "September" },
                new MonthModel() { Id = 10, MonthName = "October" },
                new MonthModel() { Id = 11, MonthName = "November" },
                new MonthModel() { Id = 12, MonthName = "December" },
            };

            SelectedMonth = DateTime.Now.Month;
            Month = Months[SelectedMonth - 1];

            Years = new List<Years>();

            for (int i = MIN_YEAR; i < MAX_YEAR; i++)
            {
                Years.Add(new Years() { Id = i - 1900, Year = i, Opacity = i <= DateTime.Now.Year ? 1 : 0.32 });
            }

            SelectedYear = Years.FirstOrDefault(x => x.Year == DateTime.Now.Year);
        }

        #region -- Public Properties --

        public List<MonthModel> Months { get; set; }

        public List<Years> Years { get; set; }

        public MonthModel Month { get; set; }

        public Day SelectedDay { get; set; }

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
            returnType: typeof(Years),
            declaringType: typeof(Calendar),
            defaultBindingMode: BindingMode.TwoWay);

        public Years SelectedYear
        {
            get => (Years)GetValue(SelectedYearProperty);
            set => SetValue(SelectedYearProperty, value);
        }

        #endregion

        #region -- Overrides --

        protected override void OnPropertyChanged(string? propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == nameof(SelectedDay))
            {
                if (dropdownFrame.IsVisible)
                {
                    dropdownFrame.IsVisible = false;
                    yearDropdownFrame.BackgroundColor = (Color)App.Current.Resources["TextAndBackgroundColor_i4"];
                    yearDropdownIcon.Source = "ic_arrow_down_primary_24x24";
                }
            }

            if (propertyName == nameof(SelectedYear))
            {
                if (SelectedYear.Year > DateTime.Now.Year && !_isFutureYearSelected)
                {
                    _isFutureYearSelected = true;
                    SelectedYear = Years.FirstOrDefault(x => x.Year == DateTime.Now.Year);
                    yearsCollectionView.SelectedItem = null;
                }

                _isFutureYearSelected = false;
            }
        }

        #endregion

        #region -- Private Helpers --

        private void OnCalendarGridPropertyChanged(object sender, System.EventArgs arg)
        {
            if (Month is not null)
            {
                if (sender is CalendarGridCollectionView collectionView && collectionView.Month != Month?.Id)
                {
                    if (collectionView.Month > Month?.Id)
                    {
                        Month = Months[SelectedMonth - 1];
                        if (SelectedMonth == 1)
                        {
                            SelectedMonth = 12;
                        }

                        Month = Months[SelectedMonth - 1];
                    }
                    else
                    {
                        if (SelectedMonth == 12)
                        {
                            SelectedMonth = 1;
                        }

                        Month = Months[SelectedMonth - 1];
                    }
                }
            }
        }

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
                SelectedYear.Year++;
            }
            else
            {
                SelectedMonth++;
            }

            Month = Months[SelectedMonth - 1];
        }

        private void OnLeftMonthButtonTapped(object? sender, EventArgs? arg)
        {
            if (SelectedMonth == 1)
            {
                SelectedYear.Year--;
                SelectedMonth = 12;
            }
            else
            {
                SelectedMonth--;
            }

            Month = Months[SelectedMonth - 1];
        }

        #endregion

    }
}