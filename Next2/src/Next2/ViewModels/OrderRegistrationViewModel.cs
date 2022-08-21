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
using Next2.Services.Notifications;
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
        private readonly IMenuService _menuService;
        private readonly IBonusesService _bonusesService;

        private readonly ICommand _seatSelectionCommand;
        private readonly ICommand _deleteSeatCommand;
        private readonly ICommand _removeOrderCommand;
        private readonly ICommand _selectDishCommand;

        private FullOrderBindableModel _tempCurrentOrder = new();
        private SeatBindableModel? _firstSeat;
        private SeatBindableModel _firstNotEmptySeat = new();
        private SeatBindableModel? _seatWithSelectedDish;
        private DishBindableModel? _rememberPositionSelection;

        private ELoadingState _externalPageLoadStatus;

        private bool _isAnyDishChosen;

        public OrderRegistrationViewModel(
            INavigationService navigationService,
            IAuthenticationService authenticationService,
            INotificationsService notificationsService,
            IEventAggregator eventAggregator,
            IMapper mapper,
            IOrderService orderService,
            IBonusesService bonusesService,
            IMenuService menuService)
            : base(navigationService, authenticationService, notificationsService)
        {
            _eventAggregator = eventAggregator;
            _mapper = mapper;
            _orderService = orderService;
            _menuService = menuService;
            _bonusesService = bonusesService;

            CurrentState = ENewOrderViewState.InProgress;

            _eventAggregator.GetEvent<LoadingStateEvent>().Subscribe(GetLoadingStateOfExternalPage);

            _seatSelectionCommand = new AsyncCommand<DishesGroupedBySeat>(OnSeatSelectionCommandAsync, allowsMultipleExecutions: false);
            _deleteSeatCommand = new AsyncCommand<DishesGroupedBySeat>(OnDeleteSeatCommandAsync, allowsMultipleExecutions: false);
            _removeOrderCommand = new AsyncCommand(OnRemoveOrderCommandAsync, allowsMultipleExecutions: false);
            _selectDishCommand = new AsyncCommand<DishBindableModel>(OnSelectDishCommandAsync, allowsMultipleExecutions: false);
        }

        #region -- Public properties --

        public bool IsClockRunning { get; set; }

        private ENewOrderViewState _currentState;
        public ENewOrderViewState CurrentState
        {
            get => _currentState;
            set
            {
                _eventAggregator.GetEvent<NewOrderStateChanging>().Publish(value);

                SetProperty(ref _currentState, value);
            }
        }

        public FullOrderBindableModel CurrentOrder { get; set; } = new();

        public ObservableCollection<OrderTypeBindableModel> OrderTypes { get; set; } = new();

        public OrderTypeBindableModel SelectedOrderType { get; set; }

        public DishBindableModel? SelectedDish { get; set; }

        public ObservableCollection<DishesGroupedBySeat> DishesGroupedBySeats { get; set; } = new();

        public SeatBindableModel SeatWithSelectedDish { get; set; }

        public ObservableCollection<TableBindableModel> Tables { get; set; } = new();

        public TableBindableModel SelectedTable { get; set; } = new();

        public int NumberOfSeats { get; set; }

        public bool IsOrderWithTax { get; set; } = true;

        public bool IsSideMenuVisible { get; set; } = true;

        public ENotificationType OrderNotificationStatus { get; set; } = ENotificationType.None;

        public bool IsOrderSavingAndPaymentEnabled { get; set; }

        private ICommand? _closeEditStateCommand;
        public ICommand CloseEditStateCommand => _closeEditStateCommand ??= new AsyncCommand(OnCloseEditStateCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _openModifyCommand;
        public ICommand OpenModifyCommand => _openModifyCommand ??= new AsyncCommand(OnOpenModifyCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _openRemoveCommand;
        public ICommand OpenRemoveCommand => _openRemoveCommand ??= new AsyncCommand(OnOpenRemoveCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _openHoldSelectionCommand;
        public ICommand OpenHoldSelectionCommand => _openHoldSelectionCommand ??= new AsyncCommand(OnOpenHoldSelectionCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _openDiscountSelectionCommand;
        public ICommand OpenDiscountSelectionCommand => _openDiscountSelectionCommand ??= new AsyncCommand(OnOpenDiscountSelectionCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _removeTaxFromOrderCommand;
        public ICommand RemoveTaxFromOrderCommand => _removeTaxFromOrderCommand ??= new AsyncCommand(OnRemoveTaxFromOrderCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _orderCommand;
        public ICommand OrderCommand => _orderCommand ??= new AsyncCommand(OnOrderCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _tabCommand;
        public ICommand TabCommand => _tabCommand ??= new AsyncCommand(OnTabCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _payCommand;
        public ICommand PayCommand => _payCommand ??= new AsyncCommand(OnPayCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _deleteLastSeatCommand;
        public ICommand DeleteLastSeatCommand => _deleteLastSeatCommand ??= new AsyncCommand(OnDeleteLastSeatCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _hideOrderNotificationCommand;
        public ICommand HideOrderNotificationCommnad => _hideOrderNotificationCommand ??= new AsyncCommand(OnHideOrderNotificationCommnadAsync, allowsMultipleExecutions: false);

        private ICommand? _goToOrderTabsCommand;
        public ICommand GoToOrderTabsCommand => _goToOrderTabsCommand ??= new AsyncCommand(OnGoToOrderTabsCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _addNewSeatCommand;
        public ICommand AddNewSeatCommand => _addNewSeatCommand ??= new AsyncCommand(OnAddNewSeatCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Overrides --

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            if (!App.IsTablet)
            {
                CurrentOrder = _orderService.CurrentOrder;
                IsOrderSavingAndPaymentEnabled = CurrentOrder.Seats.Any(x => x.SelectedDishes.Any());

                await UpdateDishGroupsAsync();
            }

            if (parameters.ContainsKey(Constants.Navigations.DISH_MODIFIED))
            {
                await RefreshCurrentOrderAsync();
            }

            if (parameters.ContainsKey(Constants.Navigations.TAX_OFF))
            {
                await RemoveTaxAsync();
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
                    await SelectTable();

                    break;
                case nameof(SelectedOrderType):
                    await SelectOrderType();

                    break;
                case nameof(CurrentOrder):
                    IsOrderWithTax = CurrentOrder.TaxCoefficient > 0;

                    break;
                case nameof(IsOrderWithTax):
                    UpdateTotalOrderPrice();

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
            if (IsInternetConnected)
            {
                _tempCurrentOrder = _mapper.Map<FullOrderBindableModel>(_orderService.CurrentOrder);

                CurrentOrder = currentOrder;
                _orderService.CurrentOrder = CurrentOrder;

                var currentSeatNumber = _orderService.CurrentSeat != null
                    ? _orderService.CurrentSeat.SeatNumber
                    : CurrentOrder.Seats.FirstOrDefault().SeatNumber;

                _orderService.CurrentSeat = _orderService.CurrentOrder?.Seats?.FirstOrDefault(x => x.SeatNumber == currentSeatNumber);
                SeatWithSelectedDish = currentOrder.Seats.FirstOrDefault(row => row.SelectedItem != null);

                var resultOfUpdatingOrder = await _orderService.UpdateCurrentOrderAsync();

                if (!resultOfUpdatingOrder.IsSuccess)
                {
                    _orderService.CurrentOrder = _tempCurrentOrder;

                    await RefreshCurrentOrderAsync();

                    await ResponseToUnsuccessfulRequestAsync(resultOfUpdatingOrder.Exception?.Message);
                }
            }
            else
            {
                await _notificationsService.ShowNoInternetConnectionDialogAsync();
            }
        }

        public async Task RemoveTaxAsync()
        {
            if (IsInternetConnected)
            {
                _tempCurrentOrder = _mapper.Map<FullOrderBindableModel>(_orderService.CurrentOrder);

                IsOrderWithTax = false;

                CurrentOrder.TaxCoefficient = 0;

                _orderService.UpdateTotalSum(CurrentOrder);

                var resultOfUpdatingOrder = await _orderService.UpdateCurrentOrderAsync();

                if (!resultOfUpdatingOrder.IsSuccess)
                {
                    IsOrderWithTax = true;

                    _orderService.CurrentOrder = _tempCurrentOrder;

                    await ResponseToUnsuccessfulRequestAsync(resultOfUpdatingOrder.Exception?.Message);
                }

                await RefreshCurrentOrderAsync();
            }
            else
            {
                await _notificationsService.ShowNoInternetConnectionDialogAsync();
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

        public async Task RefreshCurrentOrderAsync()
        {
            if (IsInternetConnected)
            {
                OrderNotificationStatus = ENotificationType.None;

                CurrentOrder = _orderService.CurrentOrder;

                IsOrderSavingAndPaymentEnabled = CurrentOrder.Seats.Any(x => x.SelectedDishes.Any());

                _firstSeat = CurrentOrder.Seats.FirstOrDefault();

                var seatNumber = SelectedDish?.SeatNumber ?? 0;

                _seatWithSelectedDish = CurrentOrder.Seats.FirstOrDefault(x => x.SeatNumber == seatNumber);

                _isAnyDishChosen = CurrentOrder.Seats.Any(x => x.SelectedDishes.Any());

                _firstNotEmptySeat = CurrentOrder.Seats.FirstOrDefault(x => x.SelectedDishes.Any());

                SelectedOrderType = OrderTypes.FirstOrDefault(row => row.OrderType == CurrentOrder.OrderType);

                NumberOfSeats = CurrentOrder.Seats.Count;

                await UpdateDishGroupsAsync();

                await RefreshTablesAsync();
            }
            else
            {
                await _notificationsService.ShowNoInternetConnectionDialogAsync();
            }
        }

        #endregion

        #region -- Private helpers --

        private Task UpdateDishGroupsAsync()
        {
            var updatedDishesGroupedBySeats = new ObservableCollection<DishesGroupedBySeat>();

            var selectedDish = SelectedDish = null;

            foreach (var seat in _orderService.CurrentOrder.Seats)
            {
                bool isSelectedDishes = seat.SelectedDishes.Any();
                var selectedDishes = isSelectedDishes
                    ? seat.SelectedDishes.ToList()
                    : new() { new(), };

                var dishFirst = selectedDishes.FirstOrDefault();

                foreach (var dish in selectedDishes)
                {
                    dish.SeatNumber = seat.SeatNumber;
                    dish.IsSeatSelected = seat.Checked;
                    dish.SelectDishCommand = _selectDishCommand;

                    if (dish == seat.SelectedItem || (seat.Checked && dish == dishFirst))
                    {
                        selectedDish = dish;
                    }
                }

                updatedDishesGroupedBySeats.Add(new(seat.SeatNumber, selectedDishes));

                var seatGroup = updatedDishesGroupedBySeats.Last();

                seatGroup.Checked = seat.Checked;
                seatGroup.IsFirstSeat = seat.IsFirstSeat;
                SelectedDish = selectedDish;

                if (App.IsTablet && _rememberPositionSelection is not null && SelectedDish is null)
                {
                    SelectedDish = _rememberPositionSelection;
                }

                if (seat.Checked && seat.SelectedItem is null)
                {
                    selectedDish = null;
                }

                SelectedDish = selectedDish;

                seatGroup.SelectSeatCommand = _seatSelectionCommand;
                seatGroup.DeleteSeatCommand = _deleteSeatCommand;
                seatGroup.RemoveOrderCommand = _removeOrderCommand;
            }

            DishesGroupedBySeats = updatedDishesGroupedBySeats;

            if (SelectedDish is null)
            {
                CurrentState = ENewOrderViewState.Default;
                IsSideMenuVisible = true;
            }
            else if (!App.IsTablet)
            {
                SelectedDish = null;
            }

            return Task.CompletedTask;
        }

        private async Task RefreshTablesAsync()
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
                    Tables = new(Tables.OrderBy(x => x?.TableNumber));
                }
            }
            else if (_externalPageLoadStatus == ELoadingState.Completed)
            {
                await ResponseToUnsuccessfulRequestAsync(freeTablesResult.Exception?.Message);
            }
        }

        private Task OnCloseEditStateCommandAsync()
        {
            if (SelectedDish is not null)
            {
                _rememberPositionSelection = SelectedDish;
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

            return UpdateDishGroupsAsync();
        }

        private async Task OnSeatSelectionCommandAsync(DishesGroupedBySeat? seatGroup)
        {
            if (IsInternetConnected)
            {
                _rememberPositionSelection = null;
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
                        if (item.SeatNumber != seatNumber)
                        {
                            item.Checked = false;
                        }
                    }

                    _orderService.CurrentSeat = seat;

                    await UpdateDishGroupsAsync();
                }
            }
            else
            {
                await _notificationsService.ShowNoInternetConnectionDialogAsync();
            }
        }

        private async Task OnSelectDishCommandAsync(DishBindableModel? dish)
        {
            if (CurrentOrder is not null && CurrentOrder.Seats is not null)
            {
                _rememberPositionSelection = null;
                var seatNumber = dish?.SeatNumber ?? 0;
                var seat = CurrentOrder.Seats.FirstOrDefault(x => x.SeatNumber == seatNumber);

                if (seat is not null && CurrentOrder.Seats.IndexOf(seat) != -1 && SelectedDish is not null)
                {
                    seat.SelectedItem = seat.SelectedDishes.FirstOrDefault(x => x == dish);
                    _seatWithSelectedDish = seat;
                    seat.Checked = true;

                    foreach (var item in CurrentOrder.Seats)
                    {
                        if (item.SeatNumber != seatNumber)
                        {
                            item.SelectedItem = null;
                            item.Checked = false;
                        }
                    }

                    _orderService.CurrentSeat = seat;

                    await UpdateDishGroupsAsync();

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

        private Task OnDeleteSeatCommandAsync(DishesGroupedBySeat? seat) => DeleteSeatAsync(seat);

        private Task OnDeleteLastSeatCommandAsync()
        {
            var lastSeat = DishesGroupedBySeats.LastOrDefault();

            return lastSeat is null
                ? Task.CompletedTask
                : DeleteSeatAsync(lastSeat);
        }

        private async Task DeleteSeatAsync(DishesGroupedBySeat? seatGroup)
        {
            if (IsInternetConnected)
            {
                _tempCurrentOrder = _mapper.Map<FullOrderBindableModel>(_orderService.CurrentOrder);
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

                await UpdateDishGroupsAsync();

                if (!App.IsTablet && !CurrentOrder.Seats.Any())
                {
                    await _navigationService.GoBackAsync();
                }
            }
            else
            {
                await _notificationsService.ShowNoInternetConnectionDialogAsync();
            }
        }

        private async void CloseDeleteSeatDialogCallback(IDialogParameters dialogResult)
        {
            var isDeletedSets = false;
            var isRedirectedSets = false;
            var isDeletedSeat = false;

            if (dialogResult.TryGetValue(Constants.DialogParameterKeys.ACTION_ON_DISHES, out EActionOnDishes actionOnDishes)
                && dialogResult.TryGetValue(Constants.DialogParameterKeys.REMOVAL_SEAT, out SeatBindableModel removalSeat))
            {
                if (actionOnDishes is EActionOnDishes.DeleteDishes)
                {
                    var deleteDishesResult = await _orderService.DeleteSeatFromCurrentOrder(removalSeat);

                    if (deleteDishesResult.IsSuccess)
                    {
                        isDeletedSets = true;

                        IsOrderSavingAndPaymentEnabled = CurrentOrder.Seats.Any(x => x.SelectedDishes.Any());

                        await SelectSeatAsync(_firstSeat);

                        await OnCloseEditStateCommandAsync();
                    }
                    else
                    {
                        await _notificationsService.ShowSomethingWentWrongDialogAsync();
                    }
                }
                else if (actionOnDishes is EActionOnDishes.RedirectDishes
                    && dialogResult.TryGetValue(Constants.DialogParameterKeys.DESTINATION_SEAT_NUMBER, out int destinationSeatNumber))
                {
                    var redirectSetsResult = await _orderService.RedirectDishesFromSeatInCurrentOrder(removalSeat, destinationSeatNumber);

                    if (redirectSetsResult.IsSuccess)
                    {
                        isRedirectedSets = true;

                        var deleteSeatResult = await _orderService.DeleteSeatFromCurrentOrder(removalSeat);

                        if (deleteSeatResult.IsSuccess)
                        {
                            isDeletedSeat = true;

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

                    if (!isRedirectedSets || !isDeletedSeat)
                    {
                        await _notificationsService.ShowSomethingWentWrongDialogAsync();
                    }
                }
            }

            await _notificationsService.CloseAllPopupAsync();

            if (!App.IsTablet && !CurrentOrder.Seats.Any())
            {
                await _navigationService.GoBackAsync();
            }

            if (isDeletedSets || (isRedirectedSets && isDeletedSeat))
            {
                var resultOfUpdatingOrder = await _orderService.UpdateCurrentOrderAsync();

                if (!resultOfUpdatingOrder.IsSuccess)
                {
                    await ResponseToUnsuccessfulRequestAsync(resultOfUpdatingOrder.Exception?.Message);

                    _orderService.CurrentOrder = _tempCurrentOrder;

                    await UpdateDishGroupsAsync();
                }
            }
        }

        private Task SelectSeatAsync(SeatBindableModel? seatToBeSelected)
        {
            foreach (var seat in CurrentOrder.Seats)
            {
                seat.Checked = false;
                seat.SelectedItem = null;
            }

            if (seatToBeSelected is not null)
            {
                seatToBeSelected.Checked = true;
            }

            SelectedDish = null;
            _rememberPositionSelection = null;

            return RefreshCurrentOrderAsync();
        }

        private async Task OnRemoveOrderCommandAsync()
        {
            if (IsInternetConnected)
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
            else
            {
                await _notificationsService.ShowNoInternetConnectionDialogAsync();
            }
        }

        private async void CloseDeleteOrderDialogCallbackAsync(IDialogParameters parameters)
        {
            if (parameters.TryGetValue(Constants.DialogParameterKeys.ACCEPT, out bool isOrderDeletionConfirmationRequestCalled))
            {
                if (isOrderDeletionConfirmationRequestCalled)
                {
                    await _notificationsService.CloseAllPopupAsync();

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
                await _notificationsService.CloseAllPopupAsync();
            }
        }

        private async void CloseOrderDeletionConfirmationDialogCallback(IDialogParameters parameters)
        {
            if (parameters.TryGetValue(Constants.DialogParameterKeys.ACCEPT, out bool isOrderRemovingAccepted))
            {
                if (isOrderRemovingAccepted)
                {
                    await _notificationsService.CloseAllPopupAsync();
                    await RemoveOrderAsync();

                    if (App.IsTablet)
                    {
                        await OnCloseEditStateCommandAsync();
                    }
                }
                else
                {
                    await _notificationsService.CloseAllPopupAsync();
                }
            }

            await UpdateDishGroupsAsync();
        }

        private async Task RemoveOrderAsync()
        {
            if (IsInternetConnected)
            {
                _tempCurrentOrder = _mapper.Map<FullOrderBindableModel>(_orderService.CurrentOrder);

                CurrentOrder.OrderStatus = EOrderStatus.Deleted;
                CurrentOrder.Close = DateTime.Now;

                var resultOfUpdatingCurrentOrder = await _orderService.UpdateCurrentOrderAsync();

                if (resultOfUpdatingCurrentOrder.IsSuccess)
                {
                    var resultOfSettingEmptyCurrentOrder = await _orderService.SetEmptyCurrentOrderAsync();
                    _rememberPositionSelection = null;

                    if (resultOfSettingEmptyCurrentOrder.IsSuccess)
                    {
                        OrderNotificationStatus = ENotificationType.OrderRemoved;

                        IsOrderSavingAndPaymentEnabled = false;

                        CurrentOrder.Seats = new();

                        if (App.IsTablet)
                        {
                            await OnCloseEditStateCommandAsync();
                        }
                    }
                    else
                    {
                        await _notificationsService.ShowSomethingWentWrongDialogAsync();
                    }
                }
                else
                {
                    await ResponseToUnsuccessfulRequestAsync(resultOfUpdatingCurrentOrder.Exception?.Message);

                    _orderService.CurrentOrder = _tempCurrentOrder;

                    await RefreshCurrentOrderAsync();
                }
            }
            else
            {
                await _notificationsService.ShowNoInternetConnectionDialogAsync();
            }
        }

        private Task OnOpenHoldSelectionCommandAsync()
        {
            return Task.CompletedTask;
        }

        private Task OnOpenDiscountSelectionCommandAsync()
        {
            var parameters = new NavigationParameters { { Constants.Navigations.CURRENT_ORDER, CurrentOrder } };

            return IsInternetConnected
                ? _navigationService.NavigateAsync(nameof(TabletViews.BonusPage), parameters)
                : _notificationsService.ShowNoInternetConnectionDialogAsync();
        }

        private async Task OnRemoveTaxFromOrderCommandAsync()
        {
            if (IsInternetConnected)
            {
                var isUserAdmin = _authenticationService.IsUserAdmin;

                if (isUserAdmin)
                {
                    await RemoveTaxAsync();
                }
                else
                {
                    await _navigationService.NavigateAsync(nameof(TaxRemoveConfirmPage), useModalNavigation: true);
                }
            }
            else
            {
                await _notificationsService.ShowNoInternetConnectionDialogAsync();
            }
        }

        private async Task SaveCurrentOrderAsync(bool isTab)
        {
            if (IsInternetConnected)
            {
                CurrentOrder.IsTab = isTab;

                var resultOfUpdatingCurrentOrder = await _orderService.UpdateCurrentOrderAsync();

                if (resultOfUpdatingCurrentOrder.IsSuccess)
                {
                    var resultOfSettingEmptyCurrentOrder = await _orderService.SetEmptyCurrentOrderAsync();

                    if (resultOfSettingEmptyCurrentOrder.IsSuccess)
                    {
                        OrderNotificationStatus = ENotificationType.OrderSaved;

                        IsOrderSavingAndPaymentEnabled = false;

                        CurrentOrder.Seats = new();

                        await OnCloseEditStateCommandAsync();
                    }
                    else
                    {
                        CurrentOrder.IsTab = !isTab;

                        await ResponseToUnsuccessfulRequestAsync(resultOfSettingEmptyCurrentOrder.Exception?.Message);
                    }

                    await _notificationsService.CloseAllPopupAsync();
                }
                else
                {
                    await _notificationsService.CloseAllPopupAsync();

                    CurrentOrder.IsTab = !isTab;

                    await ResponseToUnsuccessfulRequestAsync(resultOfUpdatingCurrentOrder.Exception?.Message);
                }
            }
            else
            {
                await _notificationsService.CloseAllPopupAsync();

                await _notificationsService.ShowNoInternetConnectionDialogAsync();
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
            if (IsInternetConnected)
            {
                if (dialogResult.TryGetValue(Constants.DialogParameterKeys.ACCEPT, out bool isMovedOrderAccepted) && isMovedOrderAccepted)
                {
                    if (isMovedOrderAccepted)
                    {
                        await SaveCurrentOrderAsync(true);
                    }
                }
                else
                {
                    await _notificationsService.CloseAllPopupAsync();
                }
            }
            else
            {
                await _notificationsService.CloseAllPopupAsync();

                await _notificationsService.ShowNoInternetConnectionDialogAsync();
            }
        }

        private Task OnOpenModifyCommandAsync()
        {
            return IsInternetConnected
                ? _navigationService.NavigateAsync(nameof(ModificationsPage))
                : _notificationsService.ShowNoInternetConnectionDialogAsync();
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
            if (IsInternetConnected)
            {
                if (dialogResult.TryGetValue(Constants.DialogParameterKeys.ACCEPT, out bool isDishRemovingAccepted) && isDishRemovingAccepted)
                {
                    _tempCurrentOrder = _mapper.Map<FullOrderBindableModel>(_orderService.CurrentOrder);

                    var resultOfDeletingDishFromCurrentSeat = await _orderService.DeleteDishFromCurrentSeatAsync();

                    if (resultOfDeletingDishFromCurrentSeat.IsSuccess)
                    {
                        _orderService.UpdateTotalSum(CurrentOrder);

                        _rememberPositionSelection = null;
                        await RefreshCurrentOrderAsync();

                        if (CurrentState == ENewOrderViewState.Edit)
                        {
                            if (_seatWithSelectedDish is not null && _seatWithSelectedDish.SelectedDishes.Any())
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

                                _rememberPositionSelection = null;
                                await UpdateDishGroupsAsync();
                            }
                            else
                            {
                                if (App.IsTablet)
                                {
                                    await OnCloseEditStateCommandAsync();
                                }
                            }
                        }

                        var resultOfUpdatingCurrentOrder = await _orderService.UpdateCurrentOrderAsync();

                        if (!resultOfUpdatingCurrentOrder.IsSuccess)
                        {
                            _orderService.CurrentOrder = _tempCurrentOrder;

                            await ResponseToUnsuccessfulRequestAsync(resultOfUpdatingCurrentOrder.Exception?.Message);

                            await RefreshCurrentOrderAsync();
                        }
                    }
                    else
                    {
                        await _notificationsService.CloseAllPopupAsync();

                        await _notificationsService.ShowSomethingWentWrongDialogAsync();
                    }

                    if (!App.IsTablet)
                    {
                        await _navigationService.GoBackToRootAsync();
                    }
                }

                await _notificationsService.CloseAllPopupAsync();
            }
            else
            {
                await _notificationsService.CloseAllPopupAsync();

                await _notificationsService.ShowNoInternetConnectionDialogAsync();
            }
        }

        private Task OnPayCommandAsync()
        {
            return IsInternetConnected
                ? _navigationService.NavigateAsync(nameof(PaymentPage))
                : _notificationsService.ShowNoInternetConnectionDialogAsync();
        }

        private Task OnHideOrderNotificationCommnadAsync()
        {
            return App.IsTablet
                ? RefreshCurrentOrderAsync()
                : _navigationService.GoBackAsync();
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

        private async Task OnAddNewSeatCommandAsync()
        {
            if (IsInternetConnected)
            {
                NumberOfSeats++;

                if (NumberOfSeats <= SelectedTable.SeatNumbers && CurrentOrder.Seats.Count != NumberOfSeats)
                {
                    OrderNotificationStatus = ENotificationType.None;

                    var resultOfAddingSeatInOrder = await _orderService.AddSeatInCurrentOrderAsync();

                    if (resultOfAddingSeatInOrder.IsSuccess)
                    {
                        foreach (var seat in CurrentOrder.Seats)
                        {
                            if (seat.SelectedItem is not null)
                            {
                                seat.SelectedItem = null;
                            }
                        }

                        var resultOfUpdatingCurrentOrder = await _orderService.UpdateCurrentOrderAsync();

                        if (!resultOfUpdatingCurrentOrder.IsSuccess)
                        {
                            var deleteLastSeatResult = await _orderService.DeleteSeatFromCurrentOrder(CurrentOrder.Seats.Last());

                            if (deleteLastSeatResult.IsSuccess)
                            {
                                _orderService.CurrentOrder.Seats.LastOrDefault().Checked = true;

                                if (!_isAnyDishChosen)
                                {
                                    await OnCloseEditStateCommandAsync();
                                }
                            }

                            if (!App.IsTablet && !CurrentOrder.Seats.Any())
                            {
                                await _navigationService.GoBackAsync();
                            }

                            await ResponseToUnsuccessfulRequestAsync(resultOfUpdatingCurrentOrder.Exception?.Message);

                            await RefreshCurrentOrderAsync();
                        }

                        await UpdateDishGroupsAsync();
                    }
                    else
                    {
                        NumberOfSeats = CurrentOrder.Seats.Count;

                        await _notificationsService.ShowSomethingWentWrongDialogAsync();
                    }
                }
            }
            else
            {
                await _notificationsService.ShowNoInternetConnectionDialogAsync();
            }
        }

        private async Task SelectTable()
        {
            if (SelectedTable is not null && SelectedTable.TableNumber != CurrentOrder?.Table?.Number)
            {
                if (IsInternetConnected)
                {
                    var tempCurrentTableNumber = CurrentOrder?.Table?.Number;
                    _orderService.CurrentOrder.Table = _mapper.Map<SimpleTableModelDTO>(SelectedTable);

                    var resultOfUpdatingOrderTable = await _orderService.UpdateCurrentOrderAsync();

                    if (!resultOfUpdatingOrderTable.IsSuccess)
                    {
                        var tempCurrentTable = Tables.FirstOrDefault(row => row.TableNumber == tempCurrentTableNumber);

                        if (tempCurrentTable is not null)
                        {
                            _orderService.CurrentOrder.Table = _mapper.Map<SimpleTableModelDTO>(tempCurrentTable);
                            SelectedTable = tempCurrentTable;
                        }

                        await ResponseToUnsuccessfulRequestAsync(resultOfUpdatingOrderTable.Exception?.Message);
                    }

                    await UpdateDishGroupsAsync();
                }
                else
                {
                    SelectedTable = Tables.FirstOrDefault(row => row.TableNumber == CurrentOrder?.Table?.Number);

                    await _notificationsService.ShowNoInternetConnectionDialogAsync();
                }
            }
        }

        private async Task SelectOrderType()
        {
            if (SelectedOrderType?.OrderType != CurrentOrder?.OrderType)
            {
                if (IsInternetConnected)
                {
                    var tempCurrentOrderType = CurrentOrder?.OrderType;
                    CurrentOrder.OrderType = SelectedOrderType?.OrderType;

                    var resultOfUpdatingOrderType = await _orderService.UpdateCurrentOrderAsync();

                    if (!resultOfUpdatingOrderType.IsSuccess)
                    {
                        CurrentOrder.OrderType = tempCurrentOrderType;
                        SelectedOrderType = OrderTypes.FirstOrDefault(row => row.OrderType == tempCurrentOrderType);

                        await ResponseToUnsuccessfulRequestAsync(resultOfUpdatingOrderType.Exception?.Message);
                    }

                    await UpdateDishGroupsAsync();
                }
                else
                {
                    SelectedOrderType = OrderTypes.FirstOrDefault(row => row.OrderType == CurrentOrder?.OrderType);

                    await _notificationsService.ShowNoInternetConnectionDialogAsync();
                }
            }
        }

        private void UpdateTotalOrderPrice()
        {
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
            }
        }

        private void GetLoadingStateOfExternalPage(ELoadingState currentState)
        {
            _externalPageLoadStatus = currentState;
        }

        #endregion
    }
}