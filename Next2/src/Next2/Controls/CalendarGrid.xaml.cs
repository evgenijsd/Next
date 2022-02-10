using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Next2.Controls
{
    public enum EDayState
    {
        Current,
        DayMonth,
        NoDayMonth,
    }

    public class DayModel
    {
        public int Day { get; set; }
        public EDayState State { get; set; }
    }

    public partial class CalendarGrid : ContentView
    {
        public CalendarGrid()
        {
            CreateArrayOfDays(1980, 3);
            InitializeComponent();
        }

        #region -- Public Properties --

        public ObservableCollection<DayModel> CategoriesItems { get; set; }

        public static readonly BindableProperty YearProperty = BindableProperty.Create(
            propertyName: nameof(Year),
            returnType: typeof(int),
            declaringType: typeof(CalendarGrid),
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
            declaringType: typeof(CalendarGrid),
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
            declaringType: typeof(CalendarGrid),
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
                CreateArrayOfDays(Year, Month);
            }

            if (propertyName == nameof(Month))
            {
                CreateArrayOfDays(Year, Month);
            }
        }

        #endregion

        #region -- Private Helpers --

        private Task OnSelectCommandAsync(object parameter)
        {
           return Task.CompletedTask;
        }

        private void CreateArrayOfDays(int year, int month)
        {
            DateTime dt = new DateTime(year, month, 1);
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

            CategoriesItems = new ();
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

                CategoriesItems.Add(new DayModel { Day = day, State = state, });
            }
        }

        #endregion

    }
}