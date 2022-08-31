using Next2.Enums;
using Next2.Services.Authentication;
using Next2.Services.Employees;
using Next2.Services.Notifications;
using Next2.Services.Order;
using Next2.Views.Mobile;
using Next2.Views.Tablet.Dialogs;
using Prism.Navigation;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Next2.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private readonly IOrderService _orderService;
        private readonly IEmployeesService _employeesService;

        private CancellationTokenSource _cancelTokenSource = new();
        private CancellationToken _token = new();

        public SettingsViewModel(
            INavigationService navigationService,
            IAuthenticationService authenticationService,
            INotificationsService notificationsService,
            PrintReceiptViewModel printReceiptViewModel,
            IOrderService orderService,
            IEmployeesService employeesService)
            : base(navigationService, authenticationService, notificationsService)
        {
            _orderService = orderService;
            _employeesService = employeesService;

            PrintReceiptViewModel = printReceiptViewModel;
        }

        #region -- Public properties --

        public bool IsLoading { get; set; }

        public string Title { get; set; } = LocalizationResourceManager.Current["Settings"];

        public ESettingsPageState PageState { get; set; } = ESettingsPageState.Default;

        public PrintReceiptViewModel PrintReceiptViewModel { get; set; }

        private ICommand? _logOutCommand;
        public ICommand LogOutCommand => _logOutCommand ??= new AsyncCommand(OnLogOutCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _programDeviceCommand;
        public ICommand ProgramDeviceCommand => _programDeviceCommand ??= new AsyncCommand(OnProgramDeviceCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _changeStateCommand;
        public ICommand ChangeStateCommand => _changeStateCommand ??= new AsyncCommand<ESettingsPageState>(OnChangeStateCommand, allowsMultipleExecutions: false);

        private ICommand? _reassignTableCommand;
        public ICommand ReassignTableCommand => _reassignTableCommand ??= new AsyncCommand(OnReassignTableCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Overrides --

        public override void OnAppearing()
        {
            base.OnAppearing();

            _cancelTokenSource = new CancellationTokenSource();
            _token = _cancelTokenSource.Token;
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();

            _cancelTokenSource.Cancel();
            _cancelTokenSource.Dispose();
        }

        #endregion

        #region -- Private helpers --

        private Task OnProgramDeviceCommandAsync()
        {
            PopupPage page = new ProgramDeviceDialog(CloseProgramDeviceDialogCallback);
            return PopupNavigation.PushAsync(page);
        }

        private async void CloseProgramDeviceDialogCallback(IDialogParameters dialogResult)
        {
            await PopupNavigation.PopAsync();
        }

        private Task OnChangeStateCommand(ESettingsPageState state)
        {
            if (!IsLoading)
            {
                PageState = state;
            }

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
                ? new Next2.Views.Tablet.Dialogs.ConfirmDialog(dialogParameters, CloseLogOutConfirmationDialogCallback)
                : new Next2.Views.Mobile.Dialogs.ConfirmDialog(dialogParameters, CloseLogOutConfirmationDialogCallback);

            return PopupNavigation.PushAsync(confirmDialog);
        }

        private async void CloseLogOutConfirmationDialogCallback(IDialogParameters dialogResult)
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

        private async Task OnReassignTableCommandAsync()
        {
            if (!IsLoading)
            {
                if (IsInternetConnected)
                {
                    IsLoading = true;

                    var resultOfGettingEmployees = await _employeesService.GetEmployeesAsync();

                    if (resultOfGettingEmployees.IsSuccess)
                    {
                        var dialogParameters = new DialogParameters { { Constants.DialogParameterKeys.EMPLOYEES, resultOfGettingEmployees.Result } };

                        PopupPage confirmDialog = App.IsTablet
                            ? new Next2.Views.Tablet.Dialogs.TableReassignmentDialog(dialogParameters, CloseReassignTableDialogCallback)
                            : new Next2.Views.Mobile.Dialogs.TableReassignmentDialog(dialogParameters, CloseReassignTableDialogCallback);

                        await PopupNavigation.PushAsync(confirmDialog);

                        IsLoading = false;
                    }
                    else
                    {
                        await ResponseToUnsuccessfulRequestAsync(resultOfGettingEmployees.Exception?.Message);
                    }
                }
                else
                {
                    await _notificationsService.ShowNoInternetConnectionDialogAsync();
                }
            }
        }

        private async void CloseReassignTableDialogCallback(IDialogParameters dialogResult)
        {
            await _notificationsService.CloseAllPopupAsync();

            if (IsInternetConnected)
            {
                if (dialogResult.TryGetValue(Constants.DialogParameterKeys.ACCEPT, out bool isTableReassignAccepted)
                    && isTableReassignAccepted
                    && dialogResult.TryGetValue(Constants.DialogParameterKeys.ORDERS_ID, out IEnumerable<Guid> ordersId)
                    && ordersId is not null
                    && dialogResult.TryGetValue(Constants.DialogParameterKeys.EMPLOYEE_ID, out string employeeIdToAssignTo)
                    && employeeIdToAssignTo is not null)
                {
                    IsLoading = true;

                    var resultOfUpdatingOrders = await _orderService.UpdateOrdersAsync(ordersId, employeeIdToAssignTo);

                    if (!resultOfUpdatingOrders.IsSuccess && !_token.IsCancellationRequested)
                    {
                        await ResponseToUnsuccessfulRequestAsync(resultOfUpdatingOrders.Exception?.Message);
                    }

                    IsLoading = false;
                }
            }
            else
            {
                await _notificationsService.ShowNoInternetConnectionDialogAsync();
            }
        }

        #endregion
    }
}
