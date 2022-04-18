using Next2.Controls;
using Next2.Services.CustomersService;
using Next2.ViewModels.Dialogs;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Pages;
using System;
using Xamarin.Forms;

namespace Next2.Views.Tablet.Dialogs
{
    public partial class CustomerAddDialog : PopupPage
    {
        public CustomerAddDialog(DialogParameters param, Action<IDialogParameters> requestClose, ICustomersService customersService)
        {
            InitializeComponent();
            BindingContext = new CustomerAddViewModel(param, requestClose, customersService);

            mailWarningLabel.TextColor = (Color)App.Current.Resources["TextAndBackgroundColor_i3"];
            nameWarningLabel.TextColor = (Color)App.Current.Resources["TextAndBackgroundColor_i3"];
            phoneWarningLabel.TextColor = (Color)App.Current.Resources["TextAndBackgroundColor_i3"];
        }

        #region -- Private helpers --

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
                if (entry.Text?.Length == Constants.Limits.PHONE_LENGTH || entry?.Text == string.Empty)
                {
                    phoneFrame.BorderColor = (Color)App.Current.Resources["TextAndBackgroundColor_i2"];
                    phoneWarningLabel.TextColor = (Color)App.Current.Resources["TextAndBackgroundColor_i3"];
                }
                else
                {
                    phoneFrame.BorderColor = phoneWarningLabel.TextColor = (Color)App.Current.Resources["IndicationColor_i3"];
                }
            }
        }

        private void OnMailEntryUnfocused(object sender, EventArgs arg)
        {
            if (Device.RuntimePlatform == Device.Android)
            {
                nameEntryBlock.IsVisible = true;
            }

            if (sender is HideClipboardEntry entry && entry != null && entry.Text != null)
            {
                if (entry.IsValid || entry.Text == string.Empty)
                {
                    mailFrame.BorderColor = (Color)App.Current.Resources["TextAndBackgroundColor_i2"];
                    mailWarningLabel.TextColor = (Color)App.Current.Resources["TextAndBackgroundColor_i3"];
                }
                else
                {
                    mailFrame.BorderColor = mailWarningLabel.TextColor = (Color)App.Current.Resources["IndicationColor_i3"];
                }
            }
        }

        #endregion
    }
}