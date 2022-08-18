using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Contracts;
using Rg.Plugins.Popup.Pages;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.Helpers;

namespace Next2.Services.Notifications
{
    public class NotificationsService : INotificationsService
    {
        private readonly IPopupNavigation _popupNavigation;

        public NotificationsService(IPopupNavigation popupNavigation)
        {
            _popupNavigation = popupNavigation;
        }

        #region -- INotificationsService implementation --

        public async Task ResponseToBadRequestAsync(string? statusCode)
        {
            if (statusCode == Constants.StatusCode.SOCKET_CLOSED)
            {
                await ShowInfoDialogAsync(
                    LocalizationResourceManager.Current["Error"],
                    LocalizationResourceManager.Current["RequestTimedOut"],
                    LocalizationResourceManager.Current["Ok"]);
            }
            else
            {
                await ShowSomethingWentWrongDialogAsync();
            }
        }

        public Task ShowInfoDialogAsync(string titleText, string descriptionText, string okText)
        {
            var parameters = new DialogParameters
            {
                { Constants.DialogParameterKeys.TITLE, titleText },
                { Constants.DialogParameterKeys.DESCRIPTION,  descriptionText },
                { Constants.DialogParameterKeys.OK_BUTTON_TEXT, okText },
            };

            PopupPage infoDialog = App.IsTablet
                ? new Views.Tablet.Dialogs.InfoDialog(parameters, () => _popupNavigation.PopAsync())
                : new Views.Mobile.Dialogs.InfoDialog(parameters, () => _popupNavigation.PopAsync());

            return _popupNavigation.PushAsync(infoDialog);
        }

        public Task ShowNoInternetConnectionDialogAsync()
        {
            return ShowInfoDialogAsync(
                LocalizationResourceManager.Current["Error"],
                LocalizationResourceManager.Current["NoInternetConnection"],
                LocalizationResourceManager.Current["Ok"]);
        }

        public Task ShowSomethingWentWrongDialogAsync()
        {
            return ShowInfoDialogAsync(
                LocalizationResourceManager.Current["Error"],
                LocalizationResourceManager.Current["SomethingWentWrong"],
                LocalizationResourceManager.Current["Ok"]);
        }

        public async Task CloseAllPopupAsync()
        {
            if (_popupNavigation.PopupStack.Any())
            {
                await _popupNavigation.PopAllAsync();
            }
        }

        public async Task ClosePopupAsync()
        {
            if (_popupNavigation.PopupStack.Any())
            {
                await _popupNavigation.PopAsync();
            }
        }

        #endregion
    }
}
