using AutoMapper;
using Next2.Enums;
using Next2.Extensions;
using Next2.Helpers;
using Next2.Helpers.Events;
using Next2.Models;
using Next2.Models.API.DTO;
using Next2.Models.Bindables;
using Next2.Services.Authentication;
using Next2.Services.Bonuses;
using Next2.Services.Menu;
using Next2.Services.Order;
using Next2.Views.Mobile;
using Prism.Events;
using Prism.Navigation;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using TabletViews = Next2.Views.Tablet;

namespace Next2.ViewModels
{
    public class OrderRegistrationViewModel : BaseViewModel
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IMapper _mapper;

        private readonly IOrderService _orderService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IMenuService _menuService;
        private readonly IBonusesService _bonusesService;

        private readonly ICommand _seatSelectionCommand;
        private readonly ICommand _deleteSeatCommand;
        private readonly ICommand _removeOrderCommand;
        private readonly ICommand _selectDishCommand;

        private SeatBindableModel _firstSeat;
        private SeatBindableModel _firstNotEmptySeat;
        private SeatBindableModel _seatWithSelectedDish;
        private EOrderStatus _orderPaymentStatus;
        private bool _isAnyDishChosen;

        public OrderRegistrationViewModel(
            INavigationService navigationService,
            IEventAggregator eventAggregator,
            IMapper mapper,
            IOrderService orderService,
            IBonusesService bonusesService,
            IAuthenticationService authenticationService,
            IMenuService menuService)
            : base(navigationService)
        {
            _eventAggregator = eventAggregator;
            _mapper = mapper;
            _orderService = orderService;
            _authenticationService = authenticationService;
            _menuService = menuService;
            _bonusesService = bonusesService;

            _orderPaymentStatus = EOrderStatus.Closed;

            CurrentState = LayoutState.Loading;

            _seatSelectionCommand = new AsyncCommand<SeatBindableModel>(OnSeatSelectionCommandAsync, allowsMultipleExecutions: false);
            _deleteSeatCommand = new AsyncCommand<SeatBindableModel>(OnDeleteSeatCommandAsync, allowsMultipleExecutions: false);
            _removeOrderCommand = new AsyncCommand(OnRemoveOrderCommandAsync, allowsMultipleExecutions: false);
            _selectDishCommand = new AsyncCommand<SeatBindableModel>(OnSelectDishCommandAsync, allowsMultipleExecutions: false);
        }

        #region -- Public properties --

        public bool IsClockRunning { get; set; }

        public LayoutState CurrentState { get; set; }

        public FullOrderBindableModel CurrentOrder { get; set; } = new();

        public string PopUpInfo => string.Format(LocalizationResourceManager.Current["TheOrderWasPlacedTo"], CurrentOrder.Number);

        public ObservableCollection<OrderTypeBindableModel> OrderTypes { get; set; } = new();

        public OrderTypeBindableModel SelectedOrderType { get; set; }

        public DishBindableModel? SelectedDish { get; set; }

        public SeatBindableModel SelectedSeat { get; set; }

        public ObservableCollection<TableBindableModel> Tables { get; set; } = new();

        public TableBindableModel SelectedTable { get; set; } = new();

        public int NumberOfSeats { get; set; }

        public bool IsOrderWithTax { get; set; } = true;

        public bool IsSideMenuVisible { get; set; } = true;

        public bool IsOrderSavedNotificationVisible { get; set; }

        public bool IsOrderSavingAndPaymentEnabled { get; set; }

        private ICommand _goBackCommand;
        public ICommand GoBackCommand => _goBackCommand ??= new Command(OnGoBackCommand);

        private ICommand _openModifyCommand;
        public ICommand OpenModifyCommand => _openModifyCommand ??= new AsyncCommand(OnOpenModifyCommandAsync, allowsMultipleExecutions: false);

        private ICommand _openRemoveCommand;
        public ICommand OpenRemoveCommand => _openRemoveCommand ??= new AsyncCommand(OnOpenRemoveCommandAsync, allowsMultipleExecutions: false);

        private ICommand _openHoldSelectionCommand;
        public ICommand OpenHoldSelectionCommand => _openHoldSelectionCommand ??= new AsyncCommand(OnOpenHoldSelectionCommandAsync, allowsMultipleExecutions: false);

        private ICommand _openDiscountSelectionCommand;
        public ICommand OpenDiscountSelectionCommand => _openDiscountSelectionCommand ??= new AsyncCommand(OnOpenDiscountSelectionCommandAsync, allowsMultipleExecutions: false);

        private ICommand _removeTaxFromOrderCommand;
        public ICommand RemoveTaxFromOrderCommand => _removeTaxFromOrderCommand ??= new AsyncCommand(OnRemoveTaxFromOrderCommandAsync, allowsMultipleExecutions: false);

        private ICommand _orderCommand;
        public ICommand OrderCommand => _orderCommand ??= new AsyncCommand(OnOrderCommandAsync, allowsMultipleExecutions: false);

        private ICommand _tabCommand;
        public ICommand TabCommand => _tabCommand ??= new AsyncCommand(OnTabCommandAsync, allowsMultipleExecutions: false);

        private ICommand _payCommand;
        public ICommand PayCommand => _payCommand ??= new AsyncCommand(OnPayCommandAsync, allowsMultipleExecutions: false);

        private ICommand _deleteLastSeatCommand;
        public ICommand DeleteLastSeatCommand => _deleteLastSeatCommand ??= new AsyncCommand(OnDeleteLastSeatCommandAsync, allowsMultipleExecutions: false);

        private ICommand _hideOrderNotificationCommand;
        public ICommand HideOrderNotificationCommnad => _hideOrderNotificationCommand ??= new AsyncCommand(OnHideOrderNotificationCommnadAsync, allowsMultipleExecutions: false);

        private ICommand _goToOrderTabsCommand;
        public ICommand GoToOrderTabsCommand => _goToOrderTabsCommand ??= new AsyncCommand(OnGoToOrderTabsCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Overrides --

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            if (!App.IsTablet)
            {
                foreach (var seat in _orderService.CurrentOrder.Seats)
                {
                    seat.SelectedItem = null;
                }

                CurrentOrder = _orderService.CurrentOrder;
                IsOrderSavingAndPaymentEnabled = CurrentOrder.Seats.Any(x => x.SelectedDishes.Any());
            }

            if (parameters.ContainsKey(Constants.Navigations.DISH_MODIFIED))
            {
                await RefreshCurrentOrderAsync();
            }

            if (CurrentOrder.TaxCoefficient == 0)
            {
                IsOrderWithTax = false;
            }

            if (parameters.TryGetValue(Constants.Navigations.BONUS, out FullOrderBindableModel currentOrder))
            {
                await UpdateOrderWithBonusAsync(currentOrder);
            }
        }

        public override async Task InitializeAsync(INavigationParameters parameters)
        {
            await base.InitializeAsync(parameters);

            InitOrderTypes();
            await RefreshCurrentOrderAsync();
        }

        protected override async void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            switch (args.PropertyName)
            {
                case nameof(SelectedTable):
                    if (SelectedTable is not null && SelectedTable.TableNumber != CurrentOrder.Table?.Number)
                    {
                        _orderService.CurrentOrder.Table = _mapper.Map<SimpleTableModelDTO>(SelectedTable);
                        await _orderService.UpdateCurrentOrderAsync();
                    }

                    break;
                case nameof(SelectedOrderType):
                    _orderService.CurrentOrder.OrderType = SelectedOrderType.OrderType;
                    await _orderService.UpdateCurrentOrderAsync();
                    break;
                case nameof(NumberOfSeats):
                    if (NumberOfSeats <= SelectedTable.SeatNumbers && CurrentOrder.Seats.Count != NumberOfSeats)
                    {
                        IsOrderSavedNotificationVisible = false;

                        await _orderService.AddSeatInCurrentOrderAsync();
                        AddSeatsCommands();
                        await _orderService.UpdateCurrentOrderAsync();
                    }

                    break;
                case nameof(CurrentOrder):
                    IsOrderWithTax = CurrentOrder.TaxCoefficient > 0;

                    break;
                case nameof(IsOrderWithTax):
                    if (!IsOrderWithTax && CurrentOrder.DiscountPrice is not null && CurrentOrder.SubTotalPrice is not null)
                    {
                        if (CurrentOrder.Coupon != null || CurrentOrder.Discount != null)
                        {
                            CurrentOrder.TotalPrice = CurrentOrder.DiscountPrice is not null
                                ? (decimal)CurrentOrder.DiscountPrice
                                : 0;
                        }
                        else
                        {
                            CurrentOrder.TotalPrice = CurrentOrder.SubTotalPrice is not null
                                ? (decimal)CurrentOrder.SubTotalPrice
                                : 0;
                        }

                        CurrentOrder.PriceTax = 0;
                        await _orderService.UpdateCurrentOrderAsync();
                    }

                    break;

                case nameof(SelectedDish):
                    IsOrderSavingAndPaymentEnabled = CurrentOrder.Seats.Any(x => x.SelectedDishes.Any());
                    break;
            }
        }

        #endregion

        #region -- Public helpers --

        public Task UpdateOrderWithBonusAsync(FullOrderBindableModel currentOrder)
        {
            CurrentOrder = currentOrder;
            _orderService.CurrentOrder = CurrentOrder;

            var currentSeatNumber = _orderService?.CurrentSeat != null
                ? _orderService?.CurrentSeat.SeatNumber
                : CurrentOrder.Seats.FirstOrDefault().SeatNumber;

            _orderService.CurrentSeat = _orderService?.CurrentOrder?.Seats?.FirstOrDefault(x => x.SeatNumber == currentSeatNumber);

            return _orderService.UpdateCurrentOrderAsync();
        }

        public void InitOrderTypes()
        {
            List<EOrderType> enums = new(Enum.GetValues(typeof(EOrderType)).Cast<EOrderType>());

            OrderTypes = new(enums.Select(x => new OrderTypeBindableModel
            {
                OrderType = x,
                Text = LocalizationResourceManager.Current[x.ToString()],
            }));
        }

        public async Task RefreshCurrentOrderAsync()
        {
            IsOrderSavedNotificationVisible = false;

            CurrentOrder = _orderService.CurrentOrder;

            _firstSeat = CurrentOrder.Seats.FirstOrDefault();

            _seatWithSelectedDish = CurrentOrder.Seats.FirstOrDefault(x => x.SelectedItem is not null);

            SelectedDish = new();
            SelectedDish = _seatWithSelectedDish?.SelectedItem;

            _isAnyDishChosen = CurrentOrder.Seats.Any(x => x.SelectedDishes.Any());

            _firstNotEmptySeat = CurrentOrder.Seats.FirstOrDefault(x => x.SelectedDishes.Any());

            AddSeatsCommands();

            SelectedOrderType = OrderTypes.FirstOrDefault(row => row.OrderType == CurrentOrder.OrderType);
            NumberOfSeats = CurrentOrder.Seats.Count;

            await RefreshTablesAsync();
        }

        #endregion

        #region -- Private helpers --

        private async Task RefreshTablesAsync()
        {
            if (IsInternetConnected)
            {
                var freeTablesResult = await _orderService.GetFreeTablesAsync();

                if (freeTablesResult.IsSuccess)
                {
                    var tableBindableModels = _mapper.Map<ICollection<TableBindableModel>>(freeTablesResult.Result);

                    Tables = new(tableBindableModels);

                    SelectedTable = CurrentOrder.Table is null
                        ? Tables.FirstOrDefault()
                        : new()
                        {
                            Id = CurrentOrder.Table.Id,
                            SeatNumbers = CurrentOrder.Table.SeatNumbers,
                            TableNumber = CurrentOrder.Table.Number,
                        };

                    if (!tableBindableModels.Any(x => x.TableNumber == SelectedTable.TableNumber))
                    {
                        Tables.Add(SelectedTable);
                        Tables = new(Tables.OrderBy(x => x.TableNumber));
                    }
                }
            }
        }

        private void AddSeatsCommands()
        {
            foreach (var seat in CurrentOrder.Seats)
            {
                seat.SeatSelectionCommand = _seatSelectionCommand;
                seat.SeatDeleteCommand = _deleteSeatCommand;
                seat.RemoveOrderCommand = _removeOrderCommand;
                seat.DishSelectionCommand = _selectDishCommand;
            }
        }

        private void DeleteSeatsCommands()
        {
            foreach (var seat in CurrentOrder.Seats)
            {
                seat.SeatSelectionCommand = null;
                seat.SeatDeleteCommand = null;
                seat.DishSelectionCommand = null;
            }
        }

        private void OnGoBackCommand()
        {
            if (SelectedDish is not null)
            {
                foreach (var item in CurrentOrder.Seats)
                {
                    item.SelectedItem = null;
                }
            }

            CurrentState = LayoutState.Loading;
            Thread.Sleep(80); // It suspends the thread to hide unwanted animation
            IsSideMenuVisible = true;
        }

        private Task OnSeatSelectionCommandAsync(SeatBindableModel seat)
        {
            if (CurrentOrder.Seats is not null)
            {
                seat.Checked = true;

                SelectedSeat = seat;

                foreach (var item in CurrentOrder.Seats)
                {
                    if (item.SeatNumber != seat.SeatNumber)
                    {
                        item.Checked = false;
                    }
                }

                _orderService.CurrentSeat = seat;
            }

            return Task.CompletedTask;
        }

        private Task OnDeleteSeatCommandAsync(SeatBindableModel seat) => DeleteSeatAsync(seat);

        private Task OnDeleteLastSeatCommandAsync()
        {
            var lastSeat = CurrentOrder.Seats.LastOrDefault();

            return lastSeat is null
                ? Task.CompletedTask
                : DeleteSeatAsync(lastSeat);
        }

        private async Task DeleteSeatAsync(SeatBindableModel seat)
        {
            if (seat.SelectedDishes.Any())
            {
                IEnumerable<int> seatNumbersOfCurrentOrder = CurrentOrder.Seats.Select(x => x.SeatNumber);

                var param = new DialogParameters
                {
                    { Constants.DialogParameterKeys.REMOVAL_SEAT, seat },
                    { Constants.DialogParameterKeys.SEAT_NUMBERS_OF_CURRENT_ORDER, seatNumbersOfCurrentOrder },
                };

                PopupPage deleteSeatDialog = App.IsTablet
                    ? new Views.Tablet.Dialogs.DeleteSeatDialog(param, CloseDeleteSeatDialogCallback)
                    : new Views.Mobile.Dialogs.DeleteSeatDialog(param, CloseDeleteSeatDialogCallback);

                await PopupNavigation.PushAsync(deleteSeatDialog);
            }
            else
            {
                var deleteSeatResult = await _orderService.DeleteSeatFromCurrentOrder(seat);

                if (deleteSeatResult.IsSuccess)
                {
                    SelectSeat(_firstSeat);

                    if (!_isAnyDishChosen)
                    {
                        OnGoBackCommand();
                    }
                }
            }

            if (!App.IsTablet && !CurrentOrder.Seats.Any())
            {
                await _navigationService.GoBackAsync();
            }
        }

        private async void CloseDeleteSeatDialogCallback(IDialogParameters dialogResult)
        {
            if (dialogResult is not null
                && dialogResult.TryGetValue(Constants.DialogParameterKeys.ACTION_ON_SETS, out EActionOnSets actionOnSets)
                && dialogResult.TryGetValue(Constants.DialogParameterKeys.REMOVAL_SEAT, out SeatBindableModel removalSeat))
            {
                if (actionOnSets is EActionOnSets.DeleteSets)
                {
                    var deleteSetsResult = await _orderService.DeleteSeatFromCurrentOrder(removalSeat);

                    if (deleteSetsResult.IsSuccess)
                    {
                        IsOrderSavingAndPaymentEnabled = CurrentOrder.Seats.Any(x => x.SelectedDishes.Any());

                        SelectSeat(_firstSeat);

                        OnGoBackCommand();
                    }
                }
                else if (actionOnSets is EActionOnSets.RedirectSets
                    && dialogResult.TryGetValue(Constants.DialogParameterKeys.DESTINATION_SEAT_NUMBER, out int destinationSeatNumber))
                {
                    var resirectSetsResult = await _orderService.RedirectSetsFromSeatInCurrentOrder(removalSeat, destinationSeatNumber);

                    if (resirectSetsResult.IsSuccess)
                    {
                        var deleteSeatResult = await _orderService.DeleteSeatFromCurrentOrder(removalSeat);

                        if (deleteSeatResult.IsSuccess)
                        {
                            var updatedDestinationSeatNumber = (destinationSeatNumber < removalSeat.SeatNumber)
                                ? destinationSeatNumber
                                : destinationSeatNumber - 1;

                            var destinationSeat = CurrentOrder.Seats.FirstOrDefault(x => x.SeatNumber == updatedDestinationSeatNumber);

                            SelectSeat(destinationSeat);

                            if (App.IsTablet && CurrentState == LayoutState.Success)
                            {
                                SelectedDish = destinationSeat.SelectedItem = destinationSeat.SelectedDishes.FirstOrDefault();
                            }
                        }
                    }
                }
            }

            await PopupNavigation.PopAsync();

            if (!App.IsTablet && !CurrentOrder.Seats.Any())
            {
                await _navigationService.GoBackAsync();
            }

            await _orderService.UpdateCurrentOrderAsync();
        }

        private void SelectSeat(SeatBindableModel seatToBeSelected)
        {
            foreach (var seat in CurrentOrder.Seats)
            {
                seat.Checked = false;
            }

            seatToBeSelected.Checked = true;

            DeleteSeatsCommands();

            RefreshCurrentOrderAsync();
        }

        private async Task OnRemoveOrderCommandAsync()
        {
            bool isAnySetsInOrder = CurrentOrder.Seats.Any(x => x.SelectedDishes.Any());

            if (!isAnySetsInOrder)
            {
                await RemoveOrderAsync();

                if (!App.IsTablet)
                {
                    await _navigationService.GoBackAsync();
                }
            }
            else
            {
                List<SeatBindableModel> seats = CurrentOrder.Seats.Where(x => x.SelectedDishes.Any()).ToList();

                var param = new DialogParameters
                {
                    { Constants.DialogParameterKeys.ORDER_NUMBER, CurrentOrder.Number },
                    { Constants.DialogParameterKeys.SEATS, seats },
                    { Constants.DialogParameterKeys.TITLE, LocalizationResourceManager.Current["Remove"] },
                    { Constants.DialogParameterKeys.CANCEL_BUTTON_TEXT, LocalizationResourceManager.Current["Cancel"] },
                    { Constants.DialogParameterKeys.OK_BUTTON_TEXT, LocalizationResourceManager.Current["Remove"] },
                    { Constants.DialogParameterKeys.OK_BUTTON_BACKGROUND, Application.Current.Resources["IndicationColor_i3"] },
                    { Constants.DialogParameterKeys.OK_BUTTON_TEXT_COLOR, Application.Current.Resources["TextAndBackgroundColor_i1"] },
                };

                PopupPage removeOrderDialog = App.IsTablet
                    ? new Views.Tablet.Dialogs.OrderDetailDialog(param, CloseDeleteOrderDialogCallbackAsync)
                    : new Views.Mobile.Dialogs.OrderDetailDialog(param, CloseDeleteOrderDialogCallbackAsync);

                await PopupNavigation.PushAsync(removeOrderDialog);
            }
        }

        private async void CloseDeleteOrderDialogCallbackAsync(IDialogParameters parameters)
        {
            if (parameters is not null
                && parameters.TryGetValue(Constants.DialogParameterKeys.ACCEPT, out bool isOrderDeletionConfirmationRequestCalled))
            {
                if (isOrderDeletionConfirmationRequestCalled)
                {
                    var confirmDialogParameters = new DialogParameters
                    {
                        { Constants.DialogParameterKeys.CONFIRM_MODE, EConfirmMode.Attention },
                        { Constants.DialogParameterKeys.TITLE, LocalizationResourceManager.Current["AreYouSure"] },
                        { Constants.DialogParameterKeys.DESCRIPTION, LocalizationResourceManager.Current["OrderWillBeRemoved"] },
                        { Constants.DialogParameterKeys.CANCEL_BUTTON_TEXT, LocalizationResourceManager.Current["Cancel"] },
                        { Constants.DialogParameterKeys.OK_BUTTON_TEXT, LocalizationResourceManager.Current["Remove"] },
                    };

                    PopupPage orderDeletionConfirmationDialog = App.IsTablet
                        ? new Next2.Views.Tablet.Dialogs.ConfirmDialog(confirmDialogParameters, CloseOrderDeletionConfirmationDialogCallback)
                        : new Next2.Views.Mobile.Dialogs.ConfirmDialog(confirmDialogParameters, CloseOrderDeletionConfirmationDialogCallback);

                    await PopupNavigation.PushAsync(orderDeletionConfirmationDialog);
                }
            }
            else
            {
                await PopupNavigation.PopAsync();
            }
        }

        private async void CloseOrderDeletionConfirmationDialogCallback(IDialogParameters parameters)
        {
            if (parameters is not null && parameters.TryGetValue(Constants.DialogParameterKeys.ACCEPT, out bool isOrderRemovingAccepted))
            {
                if (isOrderRemovingAccepted)
                {
                    await RemoveOrderAsync();

                    if (App.IsTablet)
                    {
                        OnGoBackCommand();
                    }

                    await PopupNavigation.PopAsync();
                }
            }

            await PopupNavigation.PopAsync();

            if (!App.IsTablet && !CurrentOrder.Seats.Any())
            {
                await _navigationService.GoBackAsync();
            }

            await _orderService.UpdateCurrentOrderAsync();
        }

        private async Task RemoveOrderAsync()
        {
            CurrentOrder.OrderStatus = EOrderStatus.Deleted;
            CurrentOrder.Close = DateTime.Now;

            var updateOrderResult = await _orderService.UpdateCurrentOrderAsync();

            if (updateOrderResult.IsSuccess)
            {
                var resultOfSettingEmptyCurrentOrder = await _orderService.SetEmptyCurrentOrderAsync();

                if (resultOfSettingEmptyCurrentOrder.IsSuccess)
                {
                    InitOrderTypes();
                    await RefreshCurrentOrderAsync();
                }
            }
        }

        private async Task OnSelectDishCommandAsync(SeatBindableModel seat)
        {
            if (CurrentOrder.Seats is not null && CurrentOrder.Seats.IndexOf(seat) != -1 && seat.SelectedItem is not null)
            {
                foreach (var item in CurrentOrder.Seats)
                {
                    if (item.SeatNumber != seat.SeatNumber)
                    {
                        item.SelectedItem = null;
                    }
                }

                SelectedDish = seat.SelectedItem;

                if (App.IsTablet)
                {
                    CurrentState = LayoutState.Success;
                    Thread.Sleep(100); // It suspend the thread to hide unwanted animation
                    IsSideMenuVisible = false;
                }
                else
                {
                    await _navigationService.NavigateAsync(nameof(EditPage));
                }
            }
        }

        private Task OnOpenHoldSelectionCommandAsync()
        {
            return Task.CompletedTask;
        }

        private Task OnOpenDiscountSelectionCommandAsync()
        {
            var parameters = new NavigationParameters { { Constants.Navigations.CURRENT_ORDER, CurrentOrder } };

            return _navigationService.NavigateAsync(nameof(TabletViews.BonusPage), parameters);
        }

        private async Task OnRemoveTaxFromOrderCommandAsync()
        {
            var isUserAdmin = _authenticationService.IsUserAdmin;

            if (isUserAdmin)
            {
                IsOrderWithTax = false;
                CurrentOrder.TaxCoefficient = 0;
                CurrentOrder.UpdateTotalSum();

                await _orderService.UpdateCurrentOrderAsync();
            }
            else
            {
                _eventAggregator.GetEvent<TaxRemovedEvent>().Subscribe(OnTaxEvent);

                await _navigationService.NavigateAsync(nameof(TaxRemoveConfirmPage), useModalNavigation: App.IsTablet);
            }
        }

        private void OnTaxEvent(bool isOrderWithTax)
        {
            _eventAggregator.GetEvent<TaxRemovedEvent>().Unsubscribe(OnTaxEvent);

            IsOrderWithTax = isOrderWithTax;

            if (!IsOrderWithTax)
            {
                CurrentOrder.TaxCoefficient = 0;
                CurrentOrder.UpdateTotalSum();

                _orderService.UpdateCurrentOrderAsync().Await();
            }
        }

        private async Task SaveCurrentOrderAsync(bool isTab)
        {
            CurrentOrder.IsTab = isTab;

            var updateOrderResult = await _orderService.UpdateCurrentOrderAsync();

            if (updateOrderResult.IsSuccess)
            {
                var resultOfSettingEmptyCurrentOrder = await _orderService.SetEmptyCurrentOrderAsync();

                if (resultOfSettingEmptyCurrentOrder.IsSuccess)
                {
                    IsOrderSavedNotificationVisible = true;
                    IsOrderSavingAndPaymentEnabled = false;

                    CurrentOrder.Seats = new();
                    OnGoBackCommand();
                }
            }
        }

        private Task OnOrderCommandAsync()
        {
            return SaveCurrentOrderAsync(false);
        }

        private Task OnTabCommandAsync()
        {
            var parameters = new DialogParameters
            {
                { Constants.DialogParameterKeys.TITLE, LocalizationResourceManager.Current["NeedCard"] },
                { Constants.DialogParameterKeys.DESCRIPTION, LocalizationResourceManager.Current["SwipeTheCard"] },
                { Constants.DialogParameterKeys.CANCEL_BUTTON_TEXT, LocalizationResourceManager.Current["Cancel"] },
                { Constants.DialogParameterKeys.OK_BUTTON_TEXT, LocalizationResourceManager.Current["Complete"] },
            };

            PopupPage confirmDialog = App.IsTablet
                ? new Next2.Views.Tablet.Dialogs.MovedOrderToOrderTabsDialog(parameters, CloseMovedOrderDialogCallbackAsync)
                : new Next2.Views.Mobile.Dialogs.MovedOrderToOrderTabsDialog(parameters, CloseMovedOrderDialogCallbackAsync);

            return PopupNavigation.PushAsync(confirmDialog);
        }

        private async void CloseMovedOrderDialogCallbackAsync(IDialogParameters dialogResult)
        {
            if (dialogResult is not null && dialogResult.TryGetValue(Constants.DialogParameterKeys.ACCEPT, out bool isMovedOrderAccepted) && isMovedOrderAccepted)
            {
                if (isMovedOrderAccepted)
                {
                    await SaveCurrentOrderAsync(true);
                }
            }

            await PopupNavigation.PopAsync();
        }

        private Task OnOpenModifyCommandAsync()
        {
            return _navigationService.NavigateAsync(nameof(ModificationsPage));
        }

        private Task OnOpenRemoveCommandAsync()
        {
            var parameters = new DialogParameters
            {
                { Constants.DialogParameterKeys.TITLE, LocalizationResourceManager.Current["AreYouSure"] },
                { Constants.DialogParameterKeys.DESCRIPTION, LocalizationResourceManager.Current["ThisSetWillBeRemoved"] },
                { Constants.DialogParameterKeys.CANCEL_BUTTON_TEXT, LocalizationResourceManager.Current["Cancel"] },
                { Constants.DialogParameterKeys.OK_BUTTON_TEXT, LocalizationResourceManager.Current["Remove"] },
            };

            PopupPage confirmDialog = App.IsTablet
                ? new Next2.Views.Tablet.Dialogs.ConfirmDialog(parameters, CloseDeleteDishDialogCallbackAsync)
                : new Next2.Views.Mobile.Dialogs.ConfirmDialog(parameters, CloseDeleteDishDialogCallbackAsync);

            return PopupNavigation.PushAsync(confirmDialog);
        }

        private async void CloseDeleteDishDialogCallbackAsync(IDialogParameters dialogResult)
        {
            if (dialogResult is not null && dialogResult.TryGetValue(Constants.DialogParameterKeys.ACCEPT, out bool isDishRemovingAccepted))
            {
                if (isDishRemovingAccepted)
                {
                    var result = await _orderService.DeleteDishFromCurrentSeatAsync();

                    if (result.IsSuccess)
                    {
                        _bonusesService.СalculationBonus(CurrentOrder);

                        await RefreshCurrentOrderAsync();

                        await _orderService.UpdateCurrentOrderAsync();

                        if (CurrentState == LayoutState.Success)
                        {
                            if (_seatWithSelectedDish.SelectedDishes.Any())
                            {
                                _seatWithSelectedDish.SelectedItem = _seatWithSelectedDish.SelectedDishes.FirstOrDefault();
                            }
                            else if (_isAnyDishChosen)
                            {
                                foreach (var set in CurrentOrder.Seats)
                                {
                                    set.SelectedItem = null;
                                }

                                _firstNotEmptySeat.SelectedItem = _firstNotEmptySeat.SelectedDishes.FirstOrDefault();
                            }
                            else
                            {
                                if (App.IsTablet)
                                {
                                    OnGoBackCommand();
                                }
                            }
                        }
                    }

                    if (!App.IsTablet)
                    {
                        await _navigationService.GoBackToRootAsync();
                    }
                }
            }

            await PopupNavigation.PopAllAsync();
        }

        private Task OnPayCommandAsync()
        {
            return _navigationService.NavigateAsync(nameof(PaymentPage));
        }

        private Task OnHideOrderNotificationCommnadAsync()
        {
            return RefreshCurrentOrderAsync();
        }

        private async Task OnGoToOrderTabsCommandAsync()
        {
            if (App.IsTablet)
            {
                IsSideMenuVisible = true;
                CurrentState = LayoutState.Loading;

                MessagingCenter.Send<MenuPageSwitchingMessage>(new(EMenuItems.OrderTabs), Constants.Navigations.SWITCH_PAGE);
            }
            else
            {
                await _navigationService.NavigateAsync($"/{nameof(NavigationPage)}/{nameof(MenuPage)}/{nameof(OrderTabsPage)}");
            }

            _eventAggregator.GetEvent<OrderSelectedEvent>().Publish(CurrentOrder.Id);
            _eventAggregator.GetEvent<OrderMovedEvent>().Publish(CurrentOrder.IsTab);

            await RefreshCurrentOrderAsync();
        }

        #endregion
    }
}