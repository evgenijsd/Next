using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms.Xaml;
using Next2.ENums;
using Next2.Models;

namespace Next2.Controls
{
    public class CalendarGridCollectionView : CollectionView
    {
        public CalendarGridCollectionView()
        {
            Year = 2022;
            Month = 3;
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
            defaultValue: 2022,
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
            defaultValue: 3,
            defaultBindingMode: BindingMode.TwoWay);

        public int Month
        {
            get => (int)GetValue(MonthProperty);
            set => SetValue(MonthProperty, value);
        }

        public static readonly BindableProperty SelectedDayProperty = BindableProperty.Create(
            propertyName: nameof(SelectedDay),
            returnType: typeof(int),
            declaringType: typeof(CalendarGridCollectionView),
            defaultValue: 0,
            defaultBindingMode: BindingMode.TwoWay);

        public int SelectedDay
        {
            get => (int)GetValue(SelectedDayProperty);
            set => SetValue(SelectedDayProperty, value);
        }

        private ICommand _selectCommand;
        public ICommand SelectCommand => _selectCommand ??= new AsyncCommand<object>(OnSelectCommandAsync);

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
                    if (selectedDay.State == ENums.EDayState.NoDayMonth || selectedDay.State == ENums.EDayState.NameOfDay)
                    {
                        SelectedItem = null;
                    }
                }
            }
        }

        #endregion

        #region -- Private Helpers --

        private Task OnSelectCommandAsync(object parameter)
        {
            return Task.CompletedTask;
        }

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
