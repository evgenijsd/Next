using Next2.Enums;
using Next2.Models.API.DTO;
using Next2.Services.Authentication;
using Next2.Services.Employees;
using Next2.Services.Notifications;
using Next2.Services.Order;
using Next2.Views.Mobile;
using Prism.Navigation;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IEmployeesService _employeesService;

        private bool _isAnyPopUpNeeded = true;

        public SettingsViewModel(
            INavigationService navigationService,
            IAuthenticationService authenticationService,
            INotificationsService notificationsService,
            PrintReceiptViewModel printReceiptViewModel,
            IOrderService orderService,
            IEmployeesService employeesService)
            : base(navigationService)
        {
            _authenticationService = authenticationService;
            _orderService = orderService;
            _notificationsService = notificationsService;
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

        private ICommand? _changeStateCommand;
        public ICommand ChangeStateCommand => _changeStateCommand ??= new AsyncCommand<ESettingsPageState>(OnChangeStateCommand, allowsMultipleExecutions: false);

        private ICommand? _reassignTableCommand;
        public ICommand ReassignTableCommand => _reassignTableCommand ??= new AsyncCommand(OnReassignTableCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Overrides --

        public override void OnAppearing()
        {
            base.OnAppearing();

            _isAnyPopUpNeeded = true;
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();

            _isAnyPopUpNeeded = false;
        }

        #endregion

        #region -- Private helpers --

        private Task OnChangeStateCommand(ESettingsPageState state)
        {
            if (!IsLoading)
            {
                PageState = state;

                switch (state)
                {
                    case ESettingsPageState.Default:
                        Title = LocalizationResourceManager.Current["Settings"];
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

        private async Task OnReassignTableCommandAsync()
        {
            if (IsInternetConnected && !IsLoading)
            {
                IsLoading = true;

                var employees = await LoadAllEmployeesAsync();

                if (employees.Any())
                {
                    if (_isAnyPopUpNeeded)
                    {
                        var dialogParameters = new DialogParameters { { Constants.DialogParameterKeys.EMPLOYEES, employees } };

                        PopupPage confirmDialog = App.IsTablet
                            ? new Next2.Views.Tablet.Dialogs.TableReassignmentDialog(dialogParameters, CloseReassignTableDialogCallback)
                            : new Next2.Views.Mobile.Dialogs.TableReassignmentDialog(dialogParameters, CloseReassignTableDialogCallback);

                        await PopupNavigation.PushAsync(confirmDialog);
                    }

                    IsLoading = false;
                }
            }
            else
            {
                await _notificationsService.ShowInfoDialogAsync(
                    LocalizationResourceManager.Current["Error"],
                    LocalizationResourceManager.Current["NoInternetConnection"],
                    LocalizationResourceManager.Current["Ok"]);
            }
        }

        private async void CloseReassignTableDialogCallback(IDialogParameters dialogResult)
        {
            await _notificationsService.CloseAllPopupAsync();

            if (IsInternetConnected)
            {
                if (dialogResult is not null)
                {
                    if (dialogResult.TryGetValue(Constants.DialogParameterKeys.ACCEPT, out bool isTableReassignAccepted)
                        && isTableReassignAccepted
                        && dialogResult.TryGetValue(Constants.DialogParameterKeys.ORDERS_ID, out IEnumerable<Guid> ordersId)
                        && dialogResult.TryGetValue(Constants.DialogParameterKeys.EMPLOYEE_ID, out string employeeIdToAssignTo))
                    {
                        if (ordersId is not null && employeeIdToAssignTo is not null)
                        {
                            IsLoading = true;
                            await UpdateOrdersAsync(ordersId, employeeIdToAssignTo);
                            IsLoading = false;
                        }
                    }
                }
            }
            else
            {
                await _notificationsService.ShowInfoDialogAsync(
                    LocalizationResourceManager.Current["Error"],
                    LocalizationResourceManager.Current["NoInternetConnection"],
                    LocalizationResourceManager.Current["Ok"]);
            }
        }

        private async Task<IEnumerable<EmployeeModelDTO>> LoadAllEmployeesAsync()
        {
            IEnumerable<EmployeeModelDTO> employees = Enumerable.Empty<EmployeeModelDTO>();

            var resultOfGettingEmployees = await _employeesService.GetEmployeesAsync();

            if (resultOfGettingEmployees.IsSuccess)
            {
                if (resultOfGettingEmployees.Result is not null)
                {
                    employees = resultOfGettingEmployees.Result;
                }
                else if (_isAnyPopUpNeeded)
                {
                    await _notificationsService.ShowInfoDialogAsync(
                        LocalizationResourceManager.Current["Error"],
                        LocalizationResourceManager.Current["NoEmployeesWereFound"],
                        LocalizationResourceManager.Current["Ok"]);
                }
            }
            else
            {
                await _notificationsService.ResponseToBadRequestAsync(resultOfGettingEmployees.Exception?.Message);
            }

            return employees;
        }

        private async Task UpdateOrdersAsync(IEnumerable<Guid> ordersId, string employeeId)
        {
            var orders = await GetOrdersAsync(ordersId);

            if (orders.Count() == ordersId.Count())
            {
                foreach (var order in orders)
                {
                    order.EmployeeId = employeeId;

                    var resultOfUpdatingOrder = await _orderService.UpdateOrderAsync(order);

                    if (!resultOfUpdatingOrder.IsSuccess)
                    {
                        if (_isAnyPopUpNeeded)
                        {
                            await _notificationsService.ResponseToBadRequestAsync(resultOfUpdatingOrder.Exception?.Message);
                        }

                        break;
                    }
                }
            }
        }

        private async Task<IEnumerable<OrderModelDTO>> GetOrdersAsync(IEnumerable<Guid> ordersId)
        {
            var orderIdsNumber = ordersId.Count();

            OrderModelDTO[] orderModelsDTO = new OrderModelDTO[orderIdsNumber];

            List<Guid> orderIds = new(ordersId);

            for (int i = 0; i < orderIdsNumber; i++)
            {
                var resultOfGettingOrder = await _orderService.GetOrderByIdAsync(orderIds[i]);

                if (resultOfGettingOrder.IsSuccess)
                {
                    orderModelsDTO[i] = resultOfGettingOrder.Result;
                }
                else if (_isAnyPopUpNeeded)
                {
                    await _notificationsService.ResponseToBadRequestAsync(resultOfGettingOrder.Exception?.Message);
                    break;
                }
            }

            return orderModelsDTO;
        }

        #endregion
    }
}
