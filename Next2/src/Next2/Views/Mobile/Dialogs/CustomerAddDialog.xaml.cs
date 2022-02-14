using Next2.ViewModels.Dialogs;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Pages;
using System;
using Next2.Models;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Linq;

namespace Next2.Views.Mobile.Dialogs
{
    public enum ETabState
    {
        Birthday,
        Info,
    }

    public partial class CustomerAddDialog : PopupPage
    {
        public CustomerAddDialog(DialogParameters param, Action<IDialogParameters> requestClose)
        {
            InitializeComponent();
            State = ETabState.Info;
            State = ETabState.Birthday;
            Months = new ()
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
            SelectedMonth = 3;

            Years = new List<YearModel>();
            for (int i = 1900; i < 2100; i++)
            {
                Years.Add(new YearModel() { Id = i - 1900, Year = i });
            }
        }

        #region -- Public Properties --

        public ETabState State { get; set; }

        public List<MonthModel> Months { get; set; }

        public List<YearModel> Years { get; set; }

        public static readonly BindableProperty SelectedMonthProperty = BindableProperty.Create(
            propertyName: nameof(SelectedMonth),
            returnType: typeof(int),
            declaringType: typeof(CustomerAddDialog),
            defaultValue: 3,
            defaultBindingMode: BindingMode.TwoWay);

        public int SelectedMonth
        {
            get => (int)GetValue(SelectedMonthProperty);
            set => SetValue(SelectedMonthProperty, value);
        }

        public static readonly BindableProperty SelectedYearProperty = BindableProperty.Create(
            propertyName: nameof(SelectedYear),
            returnType: typeof(int),
            declaringType: typeof(CustomerAddDialog),
            defaultValue: 2022,
            defaultBindingMode: BindingMode.TwoWay);

        public int SelectedYear
        {
            get => (int)GetValue(SelectedYearProperty);
            set => SetValue(SelectedYearProperty, value);
        }

        #endregion

        #region -- Private Helpers --

        private void OnSelectionChanged(object sender, System.EventArgs arg)
        {
            if (sender is CollectionView collection && collection.SelectedItem != null)
            {
                if (collection.SelectedItem is YearModel year)
                {
                    SelectedYear = year.Year;
                }
            }
        }

        private void OnYearDropDownTapped(object sender, System.EventArgs arg)
        {
            if (dropdownFrame.IsVisible == false)
            {
                yearsCollectionView.SelectedItem = Years.FirstOrDefault(x => x.Year == SelectedYear);
                yearsCollectionView.ScrollTo(yearsCollectionView.SelectedItem, -1, ScrollToPosition.Center, false);
                dropdownFrame.IsVisible = true;
                yearDropdownFrame.BackgroundColor = Color.FromHex("#252836");
                yearDropdownIcon.Source = "ic_arrow_up_24x24";
            }
            else
            {
                dropdownFrame.IsVisible = false;
                yearDropdownFrame.BackgroundColor = Color.FromHex("#2E3143");
                yearDropdownIcon.Source = "ic_arrow_down_primary_24x24";
            }
        }

        private void OnRightMonthButtonTapped(object sender, System.EventArgs arg)
        {
            if (SelectedMonth == 12)
            {
                SelectedMonth = 1;
            }
            else
            {
                SelectedMonth++;
            }

            monthLabel.Text = Months[SelectedMonth - 1].MonthName;
        }

        private void OnLeftMonthButtonTapped(object sender, System.EventArgs arg)
        {
            if (SelectedMonth == 1)
            {
                SelectedMonth = 12;
            }
            else
            {
                SelectedMonth--;
            }

            monthLabel.Text = Months[SelectedMonth - 1].MonthName;
        }

        private void OnButtonTapped(object sender, System.EventArgs arg)
        {
            if (sender is Frame frame)
            {
                if (frame == frame1)
                {
                    frame1.BackgroundColor = Color.FromHex("#AB3821");
                    frame2.BackgroundColor = Color.FromHex("#34374C");
                    underLine1.BackgroundColor = Color.FromHex("#F45E49");
                    underLine2.BackgroundColor = Color.FromHex("#424861");
                    State = ETabState.Info;
                }

                if (frame == frame2)
                {
                    frame2.BackgroundColor = Color.FromHex("#AB3821");
                    frame1.BackgroundColor = Color.FromHex("#34374C");
                    underLine2.BackgroundColor = Color.FromHex("#F45E49");
                    underLine1.BackgroundColor = Color.FromHex("#424861");
                    State = ETabState.Birthday;
                }
            }
        }

        #endregion

    }
}