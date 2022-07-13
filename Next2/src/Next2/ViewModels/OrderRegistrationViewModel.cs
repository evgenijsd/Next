using AutoMapper;
using Next2.Enums;
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
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;
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

            CurrentState = ENewOrderViewState.InProgress;

            _seatSelectionCommand = new AsyncCommand<SeatGroupBindableModel>(OnSeatSelectionCommandAsync, allowsMultipleExecutions: false);
            _deleteSeatCommand = new AsyncCommand<SeatGroupBindableModel>(OnDeleteSeatCommandAsync, allowsMultipleExecutions: false);
            _removeOrderCommand = new AsyncCommand(OnRemoveOrderCommandAsync, allowsMultipleExecutions: false);
            _selectDishCommand = new AsyncCommand<DishBindableModel>(OnSelectDishCommandAsync, allowsMultipleExecutions: false);
        }

        #region -- Public properties --

        public bool IsClockRunning { get; set; }

        public ENewOrderViewState CurrentState { get; set; }

        public FullOrderBindableModel CurrentOrder { get; set; } = new();

        public string PopUpInfo => string.Format(LocalizationResourceManager.Current["TheOrderWasPlacedTo"], CurrentOrder.Number);

        public ObservableCollection<OrderTypeBindableModel> OrderTypes { get; set; } = new();

        public OrderTypeBindableModel SelectedOrderType { get; set; }

        public DishBindableModel? SelectedDish { get; set; }

        public ObservableCollection<SeatGroupBindableModel> SelectedDishesForSeats { get; set; } = new();

        public SeatBindableModel SeatWithSelectedDish { get; set; }

        public ObservableCollection<TableBindableModel> Tables { get; set; } = new();

        public TableBindableModel SelectedTable { get; set; } = new();

        public int NumberOfSeats { get; set; }

        public bool IsOrderWithTax { get; set; } = true;

        public bool IsSideMenuVisible { get; set; } = true;

        public bool IsOrderSavedNotificationVisible { get; set; }

        public bool IsOrderSavingAndPaymentEnabled { get; set; }

        private ICommand _closeEditStateCommand;
        public ICommand CloseEditStateCommand => _closeEditStateCommand ??= new AsyncCommand(OnCloseEditStateCommandAsync, allowsMultipleExecutions: false);

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

            if (parameters.ContainsKey(Constants.Navigations.TAX_OFF))
            {
                await RemoveTax();
            }

            if (CurrentOrder.TaxCoefficient == 0)
            {
                IsOrderWithTax = false;
            }

            if (parameters.TryGetValue(Constants.Navigations.BONUS, out FullOrderBindableModel currentOrder))
            {
                await UpdateOrderWithBonusAsync(currentOrder);
                await RefreshCurrentOrderAsync();
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
                        await UpdateDishGroups();
                        await _orderService.UpdateCurrentOrderAsync();
                    }

                    break;
                case nameof(SelectedOrderType):
                    _orderService.CurrentOrder.OrderType = SelectedOrderType.OrderType;
                    await UpdateDishGroups();
                    await _orderService.UpdateCurrentOrderAsync();
                    break;
                case nameof(NumberOfSeats):
                    if (NumberOfSeats <= SelectedTable.SeatNumbers && CurrentOrder.Seats.Count != NumberOfSeats)
                    {
                        IsOrderSavedNotificationVisible = false;

                        await _orderService.AddSeatInCurrentOrderAsync();
                        AddSeatsAdditional();
                        await UpdateDishGroups();
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
                        await UpdateDishGroups();
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

        public async Task UpdateOrderWithBonusAsync(FullOrderBindableModel currentOrder)
        {
            CurrentOrder = currentOrder;
            _orderService.CurrentOrder = CurrentOrder;

            var currentSeatNumber = _orderService.CurrentSeat != null
                ? _orderService.CurrentSeat.SeatNumber
                : CurrentOrder.Seats.FirstOrDefault().SeatNumber;

            _orderService.CurrentSeat = _orderService.CurrentOrder.Seats?.FirstOrDefault(x => x.SeatNumber == currentSeatNumber);
            SeatWithSelectedDish = currentOrder.Seats.FirstOrDefault(row => row.SelectedItem != null);

            await RefreshCurrentOrderAsync();
            await _orderService.UpdateCurrentOrderAsync();
        }

        public async Task RemoveTax()
        {
            IsOrderWithTax = false;

            if (!IsOrderWithTax)
            {
                CurrentOrder.TaxCoefficient = 0;
                _orderService.UpdateTotalSum(CurrentOrder);

                await _orderService.UpdateCurrentOrderAsync();
                await RefreshCurrentOrderAsync();
            }
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

        public async Task LaunchRefreshOrder()
        {
            await RefreshCurrentOrderAsync();
        }

        #endregion

        #region -- Private helpers --

        private async Task RefreshCurrentOrderAsync()
        {
            IsOrderSavedNotificationVisible = false;

            CurrentOrder = _orderService.CurrentOrder;

            _firstSeat = CurrentOrder.Seats.FirstOrDefault();

            var seatNumber = SelectedDish?.SeatNumber ?? 0;
            _seatWithSelectedDish = CurrentOrder.Seats.FirstOrDefault(x => x.SeatNumber == seatNumber);

            _isAnyDishChosen = CurrentOrder.Seats.Any(x => x.SelectedDishes.Any());

            _firstNotEmptySeat = CurrentOrder.Seats.FirstOrDefault(x => x.SelectedDishes.Any());

            SelectedOrderType = OrderTypes.FirstOrDefault(row => row.OrderType == CurrentOrder.OrderType);
            NumberOfSeats = CurrentOrder.Seats.Count;

            await UpdateDishGroups();

            await RefreshTablesAsync();
        }

        private Task UpdateDishGroups()
        {
            SelectedDishesForSeats = new();
            var selectedDish = SelectedDish = null;

            foreach (var seat in _orderService.CurrentOrder.Seats)
            {
                int index = 0;
                bool isSelectedDishes = seat.SelectedDishes.Any();
                var selectedDishes = isSelectedDishes
                    ? seat.SelectedDishes.ToList()
                    : new() { new(), };
                int indexSelected = isSelectedDishes
                    ? seat.SelectedDishes.IndexOf(seat.SelectedItem ?? new())
                    : -1;

                foreach (var dish in selectedDishes)
                {
                    dish.Index = index;
                    dish.SeatNumber = seat.SeatNumber;
                    dish.IsSelectedSeat = seat.Checked;
                    dish.DishSelectionCommand = _selectDishCommand;

                    if (index == indexSelected || (seat.Checked && index == 0))
                    {
                        selectedDish = dish;
                    }

                    index++;
                }

                SelectedDishesForSeats.Add(new(seat.SeatNumber, selectedDishes));
                var seatGroup = SelectedDishesForSeats.Last();
                seatGroup.Checked = seat.Checked;
                seatGroup.IsFirstSeat = seat.IsFirstSeat;
                SelectedDish = selectedDish;

                if (seat.Checked && seat.SelectedItem is null)
                {
                    selectedDish = null;
                }

                SelectedDish = selectedDish;
                seatGroup.SeatSelectionCommand = _seatSelectionCommand;
                seatGroup.SeatDeleteCommand = _deleteSeatCommand;
                seatGroup.RemoveOrderCommand = _removeOrderCommand;
            }

            if (SelectedDish is null)
            {
                CurrentState = ENewOrderViewState.Default;
                IsSideMenuVisible = true;
            }

            return Task.CompletedTask;
        }

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

        private void AddSeatsAdditional()
        {
            foreach (var seat in CurrentOrder.Seats)
            {
                if (seat.SelectedItem is not null)
                {
                    seat.SelectedItem = null;
                }
            }
        }

        private Task DeleteSeatsAdditional()
        {
            SelectedDish = null;
            return UpdateDishGroups();
        }

        private Task OnCloseEditStateCommandAsync()
        {
            if (SelectedDish is not null)
            {
                SelectedDish = null;

                foreach (var item in CurrentOrder.Seats)
                {
                    item.SelectedItem = null;
                }
            }

            if (CurrentState == ENewOrderViewState.Edit)
            {
                CurrentState = ENewOrderViewState.Default;
            }

            IsSideMenuVisible = true;

            return UpdateDishGroups();
        }

        private Task OnSeatSelectionCommandAsync(SeatGroupBindableModel? seatGroup)
        {
            if (CurrentOrder is not null && CurrentOrder.Seats is not null)
            {
                var seatNumber = seatGroup?.SeatNumber ?? 0;
                var seat = CurrentOrder.Seats.FirstOrDefault(x => x.SeatNumber == seatNumber);

                if (seat is not null)
                {
                    seat.Checked = true;

                    if (_seatWithSelectedDish is not null && _seatWithSelectedDish.SeatNumber != seatNumber)
                    {
                        _seatWithSelectedDish.SelectedItem = null;
                        SelectedDish = null;
                    }

                    foreach (var item in CurrentOrder.Seats)
                    {
                        if (item.SeatNumber != seat.SeatNumber)
                        {
                            item.Checked = false;
                        }
                    }

                    _orderService.CurrentSeat = seat;
                }
            }

            return UpdateDishGroups();
        }

        private async Task OnSelectDishCommandAsync(DishBindableModel? dish)
        {
            if (CurrentOrder is not null && CurrentOrder.Seats is not null)
            {
                var seatNumber = dish?.SeatNumber ?? 0;
                var seat = CurrentOrder.Seats.FirstOrDefault(x => x.SeatNumber == seatNumber);

                if (seat is not null && CurrentOrder.Seats.IndexOf(seat) != -1 && SelectedDish is not null)
                {
                    seat.SelectedItem = seat.SelectedDishes.FirstOrDefault(x => x.Index == (dish?.Index ?? -1));
                    _seatWithSelectedDish = seat;
                    seat.Checked = true;

                    foreach (var item in CurrentOrder.Seats)
                    {
                        if (item.SeatNumber != seat.SeatNumber)
                        {
                            item.SelectedItem = null;
                            item.Checked = false;
                        }
                    }

                    _orderService.CurrentSeat = seat;
                    await UpdateDishGroups();

                    if (dish?.Id != Guid.Empty)
                    {
                        if (App.IsTablet)
                        {
                            CurrentState = ENewOrderViewState.Edit;
                            IsSideMenuVisible = false;
                        }
                        else
                        {
                            await _navigationService.NavigateAsync(nameof(EditPage));
                        }
                    }
                }
            }
        }

        private Task OnDeleteSeatCommandAsync(SeatGroupBindableModel? seat) => DeleteSeatAsync(seat);

        private Task OnDeleteLastSeatCommandAsync()
        {
            var lastSeat = SelectedDishesForSeats.LastOrDefault();

            return lastSeat is null
                ? Task.CompletedTask
                : DeleteSeatAsync(lastSeat);
        }

        private async Task DeleteSeatAsync(SeatGroupBindableModel? seatGroup)
        {
            var seat = CurrentOrder.Seats.FirstOrDefault(x => x.SeatNumber == seatGroup?.SeatNumber);

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
                    await SelectSeatAsync(_firstSeat);

                    if (!_isAnyDishChosen)
                    {
                        await OnCloseEditStateCommandAsync();
                    }
                }
            }

            await UpdateDishGroups();

            if (!App.IsTablet && !CurrentOrder.Seats.Any())
            {
                await _navigationService.GoBackAsync();
            }
        }

        private async void CloseDeleteSeatDialogCallback(IDialogParameters dialogResult)
        {
            if (dialogResult is not null
                && dialogResult.TryGetValue(Constants.DialogParameterKeys.ACTION_ON_DISHES, out EActionOnDishes actionOnDishes)
                && dialogResult.TryGetValue(Constants.DialogParameterKeys.REMOVAL_SEAT, out SeatBindableModel removalSeat))
            {
                if (actionOnDishes is EActionOnDishes.DeleteDishes)
                {
                    var deleteDishesResult = await _orderService.DeleteSeatFromCurrentOrder(removalSeat);

                    if (deleteDishesResult.IsSuccess)
                    {
                        IsOrderSavingAndPaymentEnabled = CurrentOrder.Seats.Any(x => x.SelectedDishes.Any());

                        await SelectSeatAsync(_firstSeat);

                        await OnCloseEditStateCommandAsync();
                    }
                }
                else if (actionOnDishes is EActionOnDishes.RedirectDishes
                    && dialogResult.TryGetValue(Constants.DialogParameterKeys.DESTINATION_SEAT_NUMBER, out int destinationSeatNumber))
                {
                    var redirectDishesResult = await _orderService.RedirectDishesFromSeatInCurrentOrder(removalSeat, destinationSeatNumber);

                    if (redirectDishesResult.IsSuccess)
                    {
                        var deleteSeatResult = await _orderService.DeleteSeatFromCurrentOrder(removalSeat);

                        if (deleteSeatResult.IsSuccess)
                        {
                            var updatedDestinationSeatNumber = (destinationSeatNumber < removalSeat.SeatNumber)
                                ? destinationSeatNumber
                                : destinationSeatNumber - 1;

                            var destinationSeat = CurrentOrder.Seats.FirstOrDefault(x => x.SeatNumber == updatedDestinationSeatNumber);

                            await SelectSeatAsync(destinationSeat);

                            if (App.IsTablet && CurrentState == ENewOrderViewState.Edit)
                            {
                                destinationSeat.SelectedItem = SelectedDish = destinationSeat.SelectedDishes.FirstOrDefault();
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

            await UpdateDishGroups();
            await _orderService.UpdateCurrentOrderAsync();
        }

        private Task SelectSeatAsync(SeatBindableModel seatToBeSelected)
        {
            foreach (var seat in CurrentOrder.Seats)
            {
                seat.Checked = false;
            }

            seatToBeSelected.Checked = true;

            DeleteSeatsAdditional();

            return RefreshCurrentOrderAsync();
        }

        private async Task OnRemoveOrderCommandAsync()
        {
            bool isAnyDishesInOrder = CurrentOrder.Seats.Any(x => x.SelectedDishes.Any());

            if (!isAnyDishesInOrder)
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
                        await OnCloseEditStateCommandAsync();
                    }

                    await PopupNavigation.PopAllAsync();
                }
                else
                {
                    await PopupNavigation.PopAsync();
                }
            }

            if (!App.IsTablet && !CurrentOrder.Seats.Any())
            {
                await _navigationService.GoBackAsync();
            }

            await UpdateDishGroups();
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
                await RemoveTax();
            }
            else
            {
                await _navigationService.NavigateAsync(nameof(TaxRemoveConfirmPage), useModalNavigation: true);
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

                    await OnCloseEditStateCommandAsync();
                }
            }
            else
            {
                await ResponseToBadRequestAsync(updateOrderResult.Exception.Message);
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
            else
            {
                await PopupNavigation.PopAsync();
            }
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
                { Constants.DialogParameterKeys.DESCRIPTION, LocalizationResourceManager.Current["ThisDishWillBeRemoved"] },
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
                        _orderService.UpdateTotalSum(CurrentOrder);

                        await RefreshCurrentOrderAsync();

                        if (CurrentState == ENewOrderViewState.Edit)
                        {
                            if (_seatWithSelectedDish.SelectedDishes.Any())
                            {
                                _seatWithSelectedDish.SelectedItem = _seatWithSelectedDish.SelectedDishes.FirstOrDefault();
                            }
                            else if (_isAnyDishChosen)
                            {
                                foreach (var seat in CurrentOrder.Seats)
                                {
                                    seat.SelectedItem = null;
                                }

                                _firstNotEmptySeat.SelectedItem = _firstNotEmptySeat.SelectedDishes.FirstOrDefault();
                                await UpdateDishGroups();
                            }
                            else
                            {
                                if (App.IsTablet)
                                {
                                    await OnCloseEditStateCommandAsync();
                                }
                            }
                        }

                        await _orderService.UpdateCurrentOrderAsync();
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
                CurrentState = ENewOrderViewState.Default;

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