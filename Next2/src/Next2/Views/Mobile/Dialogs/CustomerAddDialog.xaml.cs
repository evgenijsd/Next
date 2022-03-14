using Next2.Controls;
using Next2.ENums;
using Next2.Services.CustomersService;
using Next2.ViewModels.Dialogs;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Pages;
using System;
using Xamarin.Forms;

namespace Next2.Views.Mobile.Dialogs
{
    public partial class CustomerAddDialog : PopupPage
    {
        public CustomerAddDialog(DialogParameters param, Action<IDialogParameters> requestClose, ICustomersService customersService)
        {
            InitializeComponent();
            BindingContext = new CustomerAddViewModel(param, requestClose, customersService);

            State = ETabState.Info;

            mailWarningLabel.TextColor = (Color)App.Current.Resources["TextAndBackgroundColor_i4"];
            nameWarningLabel.TextColor = (Color)App.Current.Resources["TextAndBackgroundColor_i4"];
            phoneWarningLabel.TextColor = (Color)App.Current.Resources["TextAndBackgroundColor_i4"];
        }

        #region -- Public Properties --

        public ETabState State { get; set; }

        #endregion

        #region -- Private Helpers --

        private void OnMailEntryFocused(object sender, EventArgs arg)
        {
            if (Device.RuntimePlatform == Device.Android)
            {
                nameEntryBlock.IsVisible = false;
            }
        }

        private void OnPhoneEntryUnfocused(object sender, EventArgs arg)
        {
            if (sender is Entry entry && entry?.Text != null)
            {
                var isValid = entry.Text?.Length == 10;
                phoneFrame.BorderColor = isValid || entry?.Text == string.Empty ? (Color)App.Current.Resources["TextAndBackgroundColor_i2"] : (Color)App.Current.Resources["IndicationColor_i3"];
                phoneWarningLabel.TextColor = isValid || entry?.Text == string.Empty ? (Color)App.Current.Resources["TextAndBackgroundColor_i4"] : (Color)App.Current.Resources["IndicationColor_i3"];
            }
        }

        private void OnMailEntryUnfocused(object sender, EventArgs arg)
        {
            nameEntryBlock.IsVisible = true;

            if (sender is CustomEntry entry && entry != null && entry.Text != null)
            {
                mailFrame.BorderColor = entry.IsValid || entry.Text == string.Empty ? (Color)App.Current.Resources["TextAndBackgroundColor_i2"] : (Color)App.Current.Resources["IndicationColor_i3"];
                mailWarningLabel.TextColor = entry.IsValid || entry.Text == string.Empty ? (Color)App.Current.Resources["TextAndBackgroundColor_i4"] : (Color)App.Current.Resources["IndicationColor_i3"];
            }
        }

        private void OnButtonTapped(object sender, System.EventArgs arg)
        {
            if (sender is Frame frame)
            {
                if (frame == infoButtonFrame)
                {
                    infoButtonFrame.BackgroundColor = (Color)App.Current.Resources["AppColor_i4"];
                    birthdayButtonFrame.BackgroundColor = (Color)App.Current.Resources["TextAndBackgroundColor_i3"];
                    underLine1.BackgroundColor = (Color)App.Current.Resources["AppColor_i1"];
                    underLine2.BackgroundColor = (Color)App.Current.Resources["TextAndBackgroundColor_i2"];
                    State = ETabState.Info;
                    nameEntry.IsEnabled = true;
                    phoneEntry.IsEnabled = true;
                    mailEntry.IsEnabled = true;
                }

                if (frame == birthdayButtonFrame)
                {
                    phoneEntry.IsEnabled = false;
                    mailEntry.IsEnabled = false;
                    nameEntry.IsEnabled = false;
                    birthdayButtonFrame.BackgroundColor = (Color)App.Current.Resources["AppColor_i4"];
                    infoButtonFrame.BackgroundColor = (Color)App.Current.Resources["TextAndBackgroundColor_i3"];
                    underLine2.BackgroundColor = (Color)App.Current.Resources["AppColor_i1"];
                    underLine1.BackgroundColor = (Color)App.Current.Resources["TextAndBackgroundColor_i2"];
                    State = ETabState.Birthday;
                }
            }
        }

        #endregion

    }
}