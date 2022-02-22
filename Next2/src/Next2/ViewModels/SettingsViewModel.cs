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
            _authenticationService = authenticationService;
        }

        #region -- Public properties --

        private ICommand _logOutCommand;
        public ICommand LogOutCommand => _logOutCommand ??= new AsyncCommand(OnLogOutCommandAsync);

        #endregion

        #region -- Private helpers --

        private async Task OnLogOutCommandAsync()
        {
            if (App.IsTablet)
            {
                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(new Next2.Views.Tablet.Dialogs.LogOutAlertView(null, CloseDialogCallback));
            }
            else
            {
                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(new Next2.Views.Mobile.Dialogs.LogOutAlertView(null, CloseDialogCallback));
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
                    { nameof(result), result },
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
