using Next2.Controls;
using Next2.Enums;
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
        public CustomerAddDialog(
            DialogParameters param,
            Action<IDialogParameters> requestClose,
            ICustomersService customersService)
        {
            InitializeComponent();

            BindingContext = new CustomerAddViewModel(param, requestClose, customersService);
        }

        #region -- Private helpers --

        //private void OnPhoneEntryUnfocused(object sender, EventArgs arg)
        //{
        //    if (sender is Entry phoneEntry && phoneEntry.Text != null)
        //    {
        //        if (phoneEntry.Text.Length == Constants.Limits.PHONE_LENGTH || phoneEntry.Text == string.Empty)
        //        {
        //            phoneFrame.BorderColor = (Color)App.Current.Resources["TextAndBackgroundColor_i4"];
        //            phoneWarningLabel.TextColor = (Color)App.Current.Resources["TextAndBackgroundColor_i2"];
        //        }
        //        else
        //        {
        //            phoneFrame.BorderColor = phoneWarningLabel.TextColor = (Color)App.Current.Resources["IndicationColor_i3"];
        //        }
        //    }
        //}

        //private void OnMailEntryUnfocused(object sender, EventArgs arg)
        //{
        //    nameEntryBlock.IsVisible = true;

        //    if (sender is CustomEntry entry && entry.Text != null)
        //    {
        //        if (entry.IsValid || entry.Text == string.Empty)
        //        {
        //            mailFrame.BorderColor = (Color)App.Current.Resources["TextAndBackgroundColor_i2"];
        //            mailWarningLabel.TextColor = (Color)App.Current.Resources["TextAndBackgroundColor_i4"];
        //        }
        //        else
        //        {
        //            mailFrame.BorderColor = mailWarningLabel.TextColor = (Color)App.Current.Resources["IndicationColor_i3"];
        //        }
        //    }
        //}

        //private void OnButtonTapped(object sender, EventArgs arg)
        //{
        //    if (sender is Frame frame)
        //    {
        //        if (frame == infoButtonFrame)
        //        {
        //            infoButtonFrame.BackgroundColor = (Color)App.Current.Resources["AppColor_i4"];
        //            birthdayButtonFrame.BackgroundColor = (Color)App.Current.Resources["TextAndBackgroundColor_i3"];
        //            underLine1.BackgroundColor = (Color)App.Current.Resources["AppColor_i1"];
        //            underLine2.BackgroundColor = (Color)App.Current.Resources["TextAndBackgroundColor_i2"];
        //            nameEntry.IsEnabled = phoneEntry.IsEnabled = mailEntry.IsEnabled = true;
        //            State = EClientAdditionPageTab.Info;
        //        }
        //        else if (frame == birthdayButtonFrame)
        //        {
        //            birthdayButtonFrame.BackgroundColor = (Color)App.Current.Resources["AppColor_i4"];
        //            infoButtonFrame.BackgroundColor = (Color)App.Current.Resources["TextAndBackgroundColor_i3"];
        //            underLine2.BackgroundColor = (Color)App.Current.Resources["AppColor_i1"];
        //            underLine1.BackgroundColor = (Color)App.Current.Resources["TextAndBackgroundColor_i2"];
        //            phoneEntry.IsEnabled = mailEntry.IsEnabled = nameEntry.IsEnabled = false;
        //            State = EClientAdditionPageTab.Birthday;
        //        }
        //    }
        //}
        #endregion
    }
}