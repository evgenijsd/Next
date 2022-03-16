using Next2.Enums;
using Next2.Models;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Contracts;
using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Next2.ViewModels.Dialogs
{
    public class DeleteOrderViewModel : BindableBase
    {
        private readonly IPopupNavigation _popupNavigation;

        public DeleteOrderViewModel(IPopupNavigation popupNavigation, DialogParameters param, Action<IDialogParameters> requestClose)
        {
            _popupNavigation = popupNavigation;
            LoadPageData(param);
            RequestClose = requestClose;
            CancelCommand = new Command(() => RequestClose(null));
            DeleteOrderCommand = new AsyncCommand(OnDeleteOrderCommand, allowsMultipleExecutions: false);
        }

        #region -- Public properties --

        public int OrderId { get; set; }

        public ObservableCollection<SeatModel> Seats { get; set; } = new ();

        public bool IsOrderDetailsDisplayed { get; set; }

        public Action<IDialogParameters> RequestClose;

        public ICommand CancelCommand { get; }

        public ICommand DeleteOrderCommand { get; }

        private ICommand _showHideOrderDetailsCommand;
        public ICommand ShowHideOrderDetailsCommand => _showHideOrderDetailsCommand ??= new Command(OnShowHideOrderDetailsCommand);

        #endregion

        #region -- Private helpers --

        private void LoadPageData(IDialogParameters dialogParameters)
        {
            if (dialogParameters is not null
                && dialogParameters.TryGetValue(Constants.DialogParameterKeys.ORDER_ID, out int orderId))
            {
                if (dialogParameters.TryGetValue(Constants.DialogParameterKeys.SEATS, out IEnumerable<SeatModel> seats))
                {
                    OrderId = orderId;

                    if (seats is not null)
                    {
                        Seats = new (seats);
                    }
                }
            }
        }

        private void OnShowHideOrderDetailsCommand()
        {
            IsOrderDetailsDisplayed = !IsOrderDetailsDisplayed;
        }

        private Task OnDeleteOrderCommand()
        {
            bool isAccepted = false;

            var dialogParameters = new DialogParameters
            {
                { Constants.DialogParameterKeys.CONFIRM_MODE, EConfirmMode.Attention },
                { Constants.DialogParameterKeys.TITLE, LocalizationResourceManager.Current["AreYouSure"] },
                { Constants.DialogParameterKeys.DESCRIPTION, LocalizationResourceManager.Current["OrderWillBeRemoved"] },
                { Constants.DialogParameterKeys.CANCEL_BUTTON_TEXT, LocalizationResourceManager.Current["Cancel"] },
                { Constants.DialogParameterKeys.OK_BUTTON_TEXT, LocalizationResourceManager.Current["Remove"] },
            };

            PopupPage confirmDialog = App.IsTablet
                ? new Next2.Views.Tablet.Dialogs.ConfirmDialog(dialogParameters, CloseDialogCallback)
                : new Next2.Views.Mobile.Dialogs.ConfirmDialog(dialogParameters, CloseDialogCallback);

            return _popupNavigation.PushAsync(confirmDialog);
        }

        private async void CloseDialogCallback(IDialogParameters dialogResult)
        {
            if (dialogResult is not null && dialogResult.TryGetValue(Constants.DialogParameterKeys.ACCEPT, out bool isAccepted))
            {
                var dialogParameters = new DialogParameters { { Constants.DialogParameterKeys.ACCEPT, isAccepted } };

                RequestClose(dialogParameters);
            }

            await _popupNavigation.PopAsync();
        }

        #endregion
    }
}