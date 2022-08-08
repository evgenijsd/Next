using Next2.Enums;
using Next2.Services.Authentication;
using Next2.Services.Notifications;
using Next2.Services.Order;
using Next2.Views.Mobile;
using Prism.Navigation;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Pages;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Next2.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IOrderService _orderService;
        private readonly INotificationsService _notificationsService;

        public SettingsViewModel(
            INavigationService navigationService,
            IAuthenticationService authenticationService,
            INotificationsService notificationsService,
            PrintReceiptViewModel printReceiptViewModel,
            IOrderService orderService)
            : base(navigationService)
        {
            _authenticationService = authenticationService;
            _orderService = orderService;
            _notificationsService = notificationsService;

            PrintReceiptViewModel = printReceiptViewModel;
        }

        #region -- Public properties --

        public string Title { get; set; } = LocalizationResourceManager.Current["Settings"];

        public ESettingsPageState PageState { get; set; } = ESettingsPageState.Default;

        public PrintReceiptViewModel PrintReceiptViewModel { get; set; }

        private ICommand? _logOutCommand;
        public ICommand LogOutCommand => _logOutCommand ??= new AsyncCommand(OnLogOutCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _changeStateCommand;
        public ICommand ChangeStateCommand => _changeStateCommand ??= new AsyncCommand<ESettingsPageState>(OnChangeStateCommand, allowsMultipleExecutions: false);

        #endregion

        #region -- Private helpers --

        private Task OnChangeStateCommand(ESettingsPageState state)
        {
            PageState = state;

            switch (state)
            {
                case ESettingsPageState.Default:
                    Title = LocalizationResourceManager.Current["Settings"];
                    break;
                case ESettingsPageState.ReAssignTable:
                    Title = LocalizationResourceManager.Current["ReAssignTable"];
                    break;
                case ESettingsPageState.BackOffice:
                    Title = LocalizationResourceManager.Current["BackOffice"];
                    break;
                case ESettingsPageState.PrintReceipt:
                    Title = LocalizationResourceManager.Current["PrintReceipt"];
                    break;
                case ESettingsPageState.ProgramDevice:
                    Title = LocalizationResourceManager.Current["ProgramDevice"];
                    break;
            }

            return Task.CompletedTask;
        }

        private Task OnLogOutCommandAsync()
        {
            var dialogParameters = new DialogParameters
            {
                { Constants.DialogParameterKeys.CONFIRM_MODE, EConfirmMode.Attention },
                { Constants.DialogParameterKeys.TITLE, LocalizationResourceManager.Current["AreYouSure"] },
                { Constants.DialogParameterKeys.DESCRIPTION, LocalizationResourceManager.Current["WantToLogOut"] },
                { Constants.DialogParameterKeys.CANCEL_BUTTON_TEXT, LocalizationResourceManager.Current["Cancel"] },
                { Constants.DialogParameterKeys.OK_BUTTON_TEXT, LocalizationResourceManager.Current["LogOut_UpperCase"] },
            };

            PopupPage confirmDialog = App.IsTablet
                ? new Next2.Views.Tablet.Dialogs.ConfirmDialog(dialogParameters, CloseDialogCallback)
                : new Next2.Views.Mobile.Dialogs.ConfirmDialog(dialogParameters, CloseDialogCallback);

            return PopupNavigation.PushAsync(confirmDialog);
        }

        private async void CloseDialogCallback(IDialogParameters dialogResult)
        {
            if (dialogResult.TryGetValue(Constants.DialogParameterKeys.ACCEPT, out bool result) && result)
            {
                await _notificationsService.CloseAllPopupAsync();

                var logoutResult = await _authenticationService.LogoutAsync();

                if (logoutResult.IsSuccess)
                {
                    _orderService.CurrentOrder = new();

                    var navigationParameters = new NavigationParameters
                        {
                            { Constants.Navigations.LOGOUT, true },
                        };

                    await _navigationService.NavigateAsync($"/{nameof(NavigationPage)}/{nameof(LoginPage)}");
                }
            }
            else
            {
                await _notificationsService.CloseAllPopupAsync();
            }
        }

        #endregion
    }
}
