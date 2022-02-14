using Next2.ViewModels.Dialogs;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Pages;
using System;
using Next2.Models;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using System.Collections.ObjectModel;

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
            BindingContext = new CustomerInfoViewModel(param, requestClose);
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
            ScrollPosition = 3;
        }

        public ETabState State { get; set; }

       // public List<YearModel> Years { get; set; }
        public List<MonthModel> Months { get; set; }

        public List<YearModel> Years { get; set; }

        public static readonly BindableProperty ScrollPositionProperty = BindableProperty.Create(
            propertyName: nameof(ScrollPosition),
            returnType: typeof(int),
            declaringType: typeof(CustomerAddDialog),
            defaultValue: 3,
            defaultBindingMode: BindingMode.TwoWay);

        public int ScrollPosition
        {
            get => (int)GetValue(ScrollPositionProperty);
            set => SetValue(ScrollPositionProperty, value);
        }

        public void OnYearDropDownTapped(object sender, System.EventArgs arg)
        {
            var years2 = new List<YearModel>();
            for (int i = 1900; i < 2100; i++)
            {
                years2.Add(new YearModel() { Id = i - 1900, Year = i });
            }

            Years = new (years2);

            if (dropdownFrame.IsVisible == false)
            {
                dropdownFrame.IsVisible = true;
            }
            else
            {
                dropdownFrame.IsVisible = false;
            }
        }

        public void OnRightMonthButtonTapped(object sender, System.EventArgs arg)
        {
            if (ScrollPosition == 12)
            {
                ScrollPosition = 1;
            }
            else
            {
                ScrollPosition++;
            }

            monthLabel.Text = Months[ScrollPosition - 1].MonthName;
        }

        public void OnLeftMonthButtonTapped(object sender, System.EventArgs arg)
        {
            if (ScrollPosition == 1)
            {
                ScrollPosition = 12;
            }
            else
            {
                ScrollPosition--;
            }

            monthLabel.Text = Months[ScrollPosition - 1].MonthName;
        }

        public void OnButtonTapped(object sender, System.EventArgs arg)
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
    }
}