using Next2.Services.Authentication;
using Next2.Views.Mobile.Dialogs;
using Prism.Navigation;
using Prism.Services.Dialogs;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private IAuthenticationService _authenticationService;
        public SettingsViewModel(
            INavigationService navigationService,
            IAuthenticationService authenticationService)
            : base(navigationService)
        {
            Text = "Settings";
            _authenticationService = authenticationService;
        }

        #region -- Public properties --

        public string? Text { get; set; }

        private ICommand _logOutCommand;
        public ICommand LogOutCommand => _logOutCommand ??= new AsyncCommand(OnLogOutCommandAsync);

        #endregion

        #region -- Private helpers --

        private async Task OnLogOutCommandAsync()
        {
            var param = new DialogParameters();

            string okButton = LocalizationResourceManager.Current["LogOut(ApperCase)"];

            string cancelButton = LocalizationResourceManager.Current["Cancel"];

            param.Add(Constants.DialogParameterKeys.OK_BUTTON_TEXT, $"{okButton}");

            param.Add(Constants.DialogParameterKeys.CANCEL_BUTTON_TEXT, $"{cancelButton}");
            if (App.IsTablet)
            {
                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(new Next2.Views.Tablet.Dialogs.LogOutAlertView(param, CloseDialogCallback));
            }
            else
            {
                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(new Next2.Views.Mobile.Dialogs.LogOutAlertView(param, CloseDialogCallback));
            }
        }

        private async void CloseDialogCallback(IDialogParameters dialogResult)
        {
            bool result = (bool)dialogResult?[Constants.DialogParameterKeys.ACCEPT];

            if (result)
            {
                _authenticationService.LogOut();

                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAsync();

                var navigationParameters = new NavigationParameters
                {
                    { "IsLastUserLoggedOut", result },
                };

                await _navigationService.GoBackToRootAsync(navigationParameters);
            }
            else
            {
                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAsync();
            }
        }

        #endregion
    }
}
