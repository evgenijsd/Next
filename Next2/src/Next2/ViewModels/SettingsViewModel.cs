using Next2.Services.Authentication;
using Next2.Services.Order;
using Next2.Views.Mobile.Dialogs;
using Prism.Navigation;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Contracts;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _authenticationService;

        private readonly IPopupNavigation _popupNavigation;

        private readonly IOrderService _orderService;

        public SettingsViewModel(
            INavigationService navigationService,
            IAuthenticationService authenticationService,
            IPopupNavigation popupNavigation,
            IOrderService orderService)
            : base(navigationService)
        {
            _authenticationService = authenticationService;
            _popupNavigation = popupNavigation;
            _orderService = orderService;
        }

        #region -- Public properties --

        private ICommand _logOutCommand;
        public ICommand LogOutCommand => _logOutCommand ??= new AsyncCommand(OnLogOutCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Private helpers --

        private async Task OnLogOutCommandAsync()
        {
            if (App.IsTablet)
            {
                await _popupNavigation.PushAsync(new Next2.Views.Tablet.Dialogs.LogOutAlertView(null, CloseDialogCallback));
                await _orderService.CreateNewOrderAsync();
            }
            else
            {
                await _popupNavigation.PushAsync(new Next2.Views.Mobile.Dialogs.LogOutAlertView(null, CloseDialogCallback));
                await _orderService.CreateNewOrderAsync();
            }
        }

        private async void CloseDialogCallback(IDialogParameters dialogResult)
        {
            bool result = (bool)dialogResult?[Constants.DialogParameterKeys.ACCEPT];

            if (result)
            {
                _authenticationService.LogOut();

                await _popupNavigation.PopAsync();

                var navigationParameters = new NavigationParameters
                {
                    { nameof(result), result },
                };

                await _navigationService.GoBackToRootAsync(navigationParameters);
            }
            else
            {
                await _popupNavigation.PopAsync();
            }
        }

        #endregion
    }
}
