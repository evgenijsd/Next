using AutoMapper;
using Next2.Enums;
using Next2.Models.Bindables;
using Next2.Services.Menu;
using Next2.Services.Notifications;
using Next2.Services.Order;
using Next2.Views.Mobile;
using Prism.Navigation;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Pages;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Next2.ViewModels.Mobile
{
    public class EditPageViewModel : BaseViewModel
    {
        private readonly IMapper _mapper;
        private readonly IOrderService _orderService;
        private readonly IMenuService _menuService;
        private readonly INotificationsService _notificationsService;

        private int _indexOfSeat;
        private bool _isModifiedDish;

        private FullOrderBindableModel _tempCurrentOrder;

        public EditPageViewModel(
            INavigationService navigationService,
            INotificationsService notificationsService,
            IOrderService orderService,
            IMenuService menuService,
            IMapper mapper)
          : base(navigationService)
        {
            _orderService = orderService;
            _notificationsService = notificationsService;
            _menuService = menuService;
            _mapper = mapper;

            Device.StartTimer(TimeSpan.FromSeconds(Constants.Limits.TIMER), OnTimerTick);
        }

        #region -- Public properties --

        public DishBindableModel? SelectedDish { get; set; }

        public TimeSpan TimerHoldSelectedDish { get; set; }

        private ICommand? _openModifyCommand;
        public ICommand OpenModifyCommand => _openModifyCommand ??= new AsyncCommand(OnOpenModifyCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _openRemoveCommand;
        public ICommand OpenRemoveCommand => _openRemoveCommand ??= new AsyncCommand(OnOpenRemoveCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _openHoldSelectionCommand;
        public ICommand OpenHoldSelectionCommand => _openHoldSelectionCommand ??= new AsyncCommand<DishBindableModel?>(OnOpenHoldSelectionCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _goBackCommand;
        public ICommand GoBackCommand => _goBackCommand ??= new AsyncCommand(OnGoBackCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Overrides --

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if (parameters.TryGetValue(Constants.Navigations.DISH_MODIFIED, out bool isModifiedDish))
            {
                _isModifiedDish = isModifiedDish;
            }

            var seat = _orderService.CurrentOrder.Seats.FirstOrDefault(row => row.SelectedItem != null);

            _indexOfSeat = _orderService.CurrentOrder.Seats.IndexOf(seat);

            SelectedDish = new();

            SelectedDish = _orderService.CurrentOrder.Seats[_indexOfSeat].SelectedItem;

            if (SelectedDish?.HoldTime is DateTime holdTime)
            {
                TimerHoldSelectedDish = holdTime.AddMinutes(1) - DateTime.Now;
            }
        }

        #endregion

        #region -- Private helpers --

        private bool OnTimerTick()
        {
            if (SelectedDish is not null && SelectedDish.HoldTime is DateTime holdTime)
            {
                TimerHoldSelectedDish = holdTime.AddMinutes(1) - DateTime.Now;
            }

            return true;
        }

        private async Task OnOpenHoldSelectionCommandAsync(DishBindableModel? dish)
        {
            if (dish is DishBindableModel selectedDish)
            {
                var param = new DialogParameters { { Constants.DialogParameterKeys.DISH, selectedDish } };

                PopupPage holdDishDialog = new Views.Mobile.Dialogs.HoldDishDialog(param, CloseHoldDishDialogCallback);

                await PopupNavigation.PushAsync(holdDishDialog);
            }
        }

        private async void CloseHoldDishDialogCallback(IDialogParameters parameters)
        {
            await _notificationsService.CloseAllPopupAsync();

            if (SelectedDish is not null)
            {
                if (parameters.TryGetValue(Constants.DialogParameterKeys.DISMISS, out bool isDismiss))
                {
                    SelectedDish.HoldTime = null;
                }

                if (parameters.TryGetValue(Constants.DialogParameterKeys.HOLD, out DateTime holdTime))
                {
                    SelectedDish.HoldTime = holdTime;
                    TimerHoldSelectedDish = holdTime.AddMinutes(1) - DateTime.Now;
                }
            }
        }

        private Task OnOpenModifyCommandAsync()
        {
            return IsInternetConnected
                ? _navigationService.NavigateAsync(nameof(ModificationsPage))
                : _notificationsService.ShowInfoDialogAsync(
                    LocalizationResourceManager.Current["Error"],
                    LocalizationResourceManager.Current["NoInternetConnection"],
                    LocalizationResourceManager.Current["Ok"]);
        }

        private Task OnOpenRemoveCommandAsync()
        {
            var confirmDialogParameters = new DialogParameters
            {
                { Constants.DialogParameterKeys.CONFIRM_MODE, EConfirmMode.Attention },
                { Constants.DialogParameterKeys.TITLE, LocalizationResourceManager.Current["AreYouSure"] },
                { Constants.DialogParameterKeys.DESCRIPTION, LocalizationResourceManager.Current["ThisDishWillBeRemoved"] },
                { Constants.DialogParameterKeys.CANCEL_BUTTON_TEXT, LocalizationResourceManager.Current["Cancel"] },
                { Constants.DialogParameterKeys.OK_BUTTON_TEXT, LocalizationResourceManager.Current["Remove"] },
            };

            PopupPage confirmDialog = new Views.Mobile.Dialogs.ConfirmDialog(confirmDialogParameters, CloseDeleteDishDialogCallbackAsync);

            return PopupNavigation.PushAsync(confirmDialog);
        }

        private async void CloseDeleteDishDialogCallbackAsync(IDialogParameters parameters)
        {
            if (IsInternetConnected)
            {
                await _notificationsService.CloseAllPopupAsync();

                if (parameters.TryGetValue(Constants.DialogParameterKeys.ACCEPT, out bool isDishRemovingAccepted))
                {
                    if (isDishRemovingAccepted)
                    {
                        _tempCurrentOrder = _mapper.Map<FullOrderBindableModel>(_orderService.CurrentOrder);

                        var result = await _orderService.DeleteDishFromCurrentSeatAsync();

                        if (result.IsSuccess)
                        {
                            var resultOfUpdatingOrder = await _orderService.UpdateCurrentOrderAsync();

                            if (resultOfUpdatingOrder.IsSuccess)
                            {
                                var navigationParameters = new NavigationParameters
                                {
                                    { nameof(Constants.Navigations.DELETE_DISH), Constants.Navigations.DELETE_DISH },
                                };

                                await _navigationService.GoBackAsync(navigationParameters);
                            }
                            else
                            {
                                _orderService.CurrentOrder = _tempCurrentOrder;

                                await _notificationsService.ResponseToBadRequestAsync(resultOfUpdatingOrder.Exception?.Message);
                            }
                        }
                        else
                        {
                            await _notificationsService.ShowInfoDialogAsync(
                                LocalizationResourceManager.Current["Error"],
                                LocalizationResourceManager.Current["SomethingWentWrong"],
                                LocalizationResourceManager.Current["Ok"]);
                        }
                    }
                }
            }
            else
            {
                await _notificationsService.CloseAllPopupAsync();

                await _notificationsService.ShowInfoDialogAsync(
                    LocalizationResourceManager.Current["Error"],
                    LocalizationResourceManager.Current["NoInternetConnection"],
                    LocalizationResourceManager.Current["Ok"]);
            }
        }

        private Task OnGoBackCommandAsync()
        {
            var navigationParam = new NavigationParameters();

            if (_isModifiedDish)
            {
                navigationParam.Add(Constants.Navigations.DISH_MODIFIED, _isModifiedDish);
            }

            return _navigationService.GoBackAsync(navigationParam);
        }

        #endregion
    }
}
