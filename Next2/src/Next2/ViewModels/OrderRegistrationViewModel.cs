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
using Next2.Services.UserService;
using Next2.Views.Mobile;
using Prism.Events;
using Prism.Navigation;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Contracts;
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
        private readonly IUserService _userService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IMenuService _menuService;
        private readonly IBonusesService _bonusesService;

        private readonly ICommand _seatSelectionCommand;
        private readonly ICommand _deleteSeatCommand;
        private readonly ICommand _removeOrderCommand;
        private readonly ICommand _setSelectionCommand;

        private SeatBindableModel _firstSeat;
        private SeatBindableModel _firstNotEmptySeat;
        private SeatBindableModel _seatWithSelectedDish;
        private EOrderStatus _orderPaymentStatus;
        private bool _isAnySetChosen;
        private bool _isAnyUpDateForCurrentSet = true;

        public OrderRegistrationViewModel(
            INavigationService navigationService,
            IEventAggregator eventAggregator,
            IMapper mapper,
            IOrderService orderService,
            IUserService userService,
            IBonusesService bonusesService,
            IAuthenticationService authenticationService,
            IMenuService menuService)
            : base(navigationService)
        {
            _eventAggregator = eventAggregator;
            _mapper = mapper;
            _orderService = orderService;
            _userService = userService;
            _authenticationService = authenticationService;
            _menuService = menuService;
            _bonusesService = bonusesService;

            _orderPaymentStatus = EOrderStatus.Closed;

            CurrentState = LayoutState.Loading;

            _seatSelectionCommand = new AsyncCommand<SeatBindableModel>(OnSeatSelectionCommandAsync, allowsMultipleExecutions: false);
            _deleteSeatCommand = new AsyncCommand<SeatBindableModel>(OnDeleteSeatCommandAsync, allowsMultipleExecutions: false);
            _removeOrderCommand = new AsyncCommand(OnRemoveOrderCommandAsync, allowsMultipleExecutions: false);
            _setSelectionCommand = new AsyncCommand<SeatBindableModel>(OnSetSelectionCommandAsync, allowsMultipleExecutions: false);
        }

        #region -- Public properties --

        public LayoutState CurrentState { get; set; }

        public FullOrderBindableModel CurrentOrder { get; set; } = new();

        public string PopUpInfo => string.Format(LocalizationResourceManager.Current["TheOrderWasPlacedTo"], CurrentOrder.Id);

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
        public ICommand OrderCommand => _orderCommand ??= new AsyncCommand<bool>(OnOrderCommandAsync, allowsMultipleExecutions: false);

        private ICommand _tabCommand;
        public ICommand TabCommand => _tabCommand ??= new AsyncCommand<bool>(OnTabCommandAsync, allowsMultipleExecutions: false);

        private ICommand _payCommand;
        public ICommand PayCommand => _payCommand ??= new AsyncCommand(OnPayCommandAsync, allowsMultipleExecutions: false);

        private ICommand _deleteLastSeatCommand;
        public ICommand DeleteLastSeatCommand => _deleteLastSeatCommand ??= new AsyncCommand(OnDeleteLastSeatCommandAsync, allowsMultipleExecutions: false);

        private ICommand _hideOrderNotificationCommand;
        public ICommand HideOrderNotificationCommnad => _hideOrderNotificationCommand ??= new AsyncCommand(OnHideOrderNotificationCommnadAsync, allowsMultipleExecutions: false);

        private ICommand _goToOrderTabs;
        public ICommand GoToOrderTabs => _goToOrderTabs ??= new AsyncCommand(OnGoToOrderTabsCommandAsync, allowsMultipleExecutions: false);

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
            }

            if (parameters.ContainsKey(Constants.Navigations.SET_MODIFIED))
            {
                _isAnyUpDateForCurrentSet = true;
            }

            if (CurrentOrder.TaxCoefficient == 0)
            {
                IsOrderWithTax = false;
            }
        }

        public override async Task InitializeAsync(INavigationParameters parameters)
        {
            base.InitializeAsync(parameters);

            InitOrderTypes();
            await RefreshTablesAsync();
            await RefreshCurrentOrderAsync();
        }

        protected override async void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            switch (args.PropertyName)
            {
                case nameof(SelectedTable):
                    if (SelectedTable is not null)
                    {
                        _orderService.CurrentOrder.Table = _mapper.Map<TableBindableModel, SimpleTableModelDTO>(SelectedTable);
                    }

                    break;
                case nameof(SelectedOrderType):
                    _orderService.CurrentOrder.OrderType = SelectedOrderType.OrderType;
                    break;
                case nameof(NumberOfSeats):
                    if (NumberOfSeats <= SelectedTable.SeatNumbers && CurrentOrder.Seats.Count != NumberOfSeats)
                    {
                        IsOrderSavedNotificationVisible = false;
                        await _orderService.AddSeatInCurrentOrderAsync();
                        AddSeatsCommands();
                    }

                    break;
                case nameof(CurrentOrder):
                    IsOrderWithTax = CurrentOrder.TaxCoefficient > 0;
                    break;
                case nameof(IsOrderWithTax):
                    if (!IsOrderWithTax)
                    {
                        CurrentOrder.TotalPrice = CurrentOrder.Coupon != null || CurrentOrder.Discount != null
                            ? (decimal)CurrentOrder.DiscountPrice
                            : (decimal)CurrentOrder.SubTotalPrice;
                        CurrentOrder.PriceTax = 0;
                    }

                    break;
            }
        }

        #endregion

        #region -- Public helpers --

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

            _isAnySetChosen = CurrentOrder.Seats.Any(x => x.SelectedDishes.Any());

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
                    var tableBindableModels = _mapper.Map<ObservableCollection<TableBindableModel>>(freeTablesResult.Result);

                    Tables = new(tableBindableModels);

                    SelectedTable = SelectedTable.IsAvailable
                        ? SelectedTable
                        : Tables.FirstOrDefault();
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
                seat.SetSelectionCommand = _setSelectionCommand;
            }
        }

        private void DeleteSeatsCommands()
        {
            foreach (var seat in CurrentOrder.Seats)
            {
                seat.SeatSelectionCommand = null;
                seat.SeatDeleteCommand = null;
                seat.SetSelectionCommand = null;
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
                    NumberOfSeats = CurrentOrder.Seats.Count;

                    foreach (var item in CurrentOrder.Seats)
                    {
                        item.Checked = false;
                    }

                    _firstSeat.Checked = true;
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
                        RefreshCurrentOrderAsync();

                        NumberOfSeats = CurrentOrder.Seats.Count;

                        if (!_isAnySetChosen)
                        {
                            OnGoBackCommand();
                        }
                        else
                        {
                            if (App.IsTablet)
                            {
                                _firstSeat.Checked = true;
                            }
                            else
                            {
                                DeleteSeatsCommands();

                                foreach (var item in CurrentOrder.Seats)
                                {
                                    item.Checked = false;
                                }

                                _firstSeat.Checked = true;

                                RefreshCurrentOrderAsync();
                            }
                        }
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
                            DeleteSeatsCommands();

                            var updatedDestinationSeatNumber = (destinationSeatNumber < removalSeat.SeatNumber) ? destinationSeatNumber : destinationSeatNumber - 1;

                            var destinationSeat = CurrentOrder.Seats.FirstOrDefault(x => x.SeatNumber == updatedDestinationSeatNumber);

                            foreach (var item in CurrentOrder.Seats)
                            {
                                item.Checked = false;
                            }

                            destinationSeat.Checked = true;
                            if (CurrentState == LayoutState.Success)
                            {
                                SelectedDish = destinationSeat.SelectedItem = destinationSeat.SelectedDishes.FirstOrDefault();
                            }

                            RefreshCurrentOrderAsync();
                        }
                    }
                }
            }

            await PopupNavigation.PopAsync();

            if (!App.IsTablet && !CurrentOrder.Seats.Any())
            {
                await _navigationService.GoBackAsync();
            }
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
                List<SeatBindableModel> seats = CurrentOrder.Seats.ToList();

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

                    await PopupNavigation.PopAsync();
                }
            }

            await PopupNavigation.PopAsync();

            if (!App.IsTablet && !CurrentOrder.Seats.Any())
            {
                await _navigationService.GoBackAsync();
            }
        }

        private async Task RemoveOrderAsync()
        {
            CurrentOrder.OrderStatus = EOrderStatus.Deleted;
            var updateRes = await _orderService.UpdateOrderAsync(CurrentOrder.ToUpdateOrderCommand());
            if (updateRes.IsSuccess)
            {
                var result = await _orderService.CreateNewCurrentOrderAsync();

                if (result.IsSuccess)
                {
                    InitOrderTypes();
                    await RefreshTablesAsync();
                    await RefreshCurrentOrderAsync();
                }
            }

            //var orders = await _orderService.GetOrdersAsync();
            //foreach (var order in orders.Result)
            //{
            //    try
            //    {
            //        var fullorder = await _orderService.GetOrderByIdAsync(order.Id);

            //        if (fullorder.Result.Number != 1851 && fullorder.Result.TotalPrice != 0)
            //        {
            //            var or = fullorder.Result.ToUpdateOrderCommand();
            //            or.OrderStatus = EOrderStatus.Pending;
            //            var res = await _orderService.UpdateOrderAsync(or);
            //        }
            //    }
            //    catch (Exception)
            //    {
            //    }
            //}
        }

        private async Task OnSetSelectionCommandAsync(SeatBindableModel seat)
        {
            if (CurrentOrder.Seats is not null && CurrentOrder.Seats.IndexOf(seat) != -1 && seat.SelectedItem is not null)
            {
                foreach (var item in CurrentOrder.Seats)
                {
                    if (item.Id != seat.Id)
                    {
                        item.SelectedItem = null;
                    }
                    else
                    {
                        item.SelectedItem = seat.SelectedItem;
                    }
                }

                SelectedDish = seat.SelectedItem;

                if (App.IsTablet)
                {
                    CurrentState = LayoutState.Success;
                    Thread.Sleep(100); // It suspend the thread to hide unwanted animation
                    IsSideMenuVisible = true;
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

        private async Task OnOpenDiscountSelectionCommandAsync()
        {
            _eventAggregator.GetEvent<AddBonusToCurrentOrderEvent>().Subscribe(BonusEventCommand);

            var parameters = new NavigationParameters { { Constants.Navigations.CURRENT_ORDER, CurrentOrder } };
            await _navigationService.NavigateAsync(nameof(TabletViews.BonusPage), parameters);
        }

        private void BonusEventCommand(FullOrderBindableModel currentOrder)
        {
            CurrentOrder = currentOrder;
            _orderService.CurrentOrder = CurrentOrder;

            _eventAggregator.GetEvent<AddBonusToCurrentOrderEvent>().Unsubscribe(BonusEventCommand);
        }

        private async Task OnRemoveTaxFromOrderCommandAsync()
        {
            var user = await _userService.GetUserById(_authenticationService.AuthorizedUserId);

            if (user.IsSuccess)
            {
                if (user.Result.UserType != EUserType.Admin)
                {
                    _eventAggregator.GetEvent<TaxRemovedEvent>().Subscribe(OnTaxEvent);

                    await _navigationService.NavigateAsync(nameof(Views.Mobile.TaxRemoveConfirmPage), useModalNavigation: App.IsTablet);
                }
                else if (user.Result.UserType == EUserType.Admin)
                {
                    IsOrderWithTax = false;
                    CurrentOrder.TaxCoefficient = 0;
                }
            }
        }

        private void OnTaxEvent(bool isOrderWithTax)
        {
            _eventAggregator.GetEvent<TaxRemovedEvent>().Unsubscribe(OnTaxEvent);

            IsOrderWithTax = isOrderWithTax;

            if (!IsOrderWithTax)
            {
                CurrentOrder.TaxCoefficient = 0;
            }
        }

        private async Task OnOrderCommandAsync(bool isTab)
        {
            List<SeatModel> seats = new();

            bool isAllSeatSaved = true;

            //foreach (var seat in CurrentOrder.Seats)
            //{
            //    if (seat.Sets.Any())
            //    {
            //        var sets = new List<SetModel>(seat.Sets.Select(x => new SetModel
            //        {
            //            Id = x.Id,
            //            SubcategoryId = x.SubcategoryId,
            //            Title = x.Title,
            //            Price = x.Portion.Price,
            //            ImagePath = x.ImagePath,
            //        }));

            //        var newSeat = new SeatModel
            //        {
            //            OrderId = CurrentOrder.Id,
            //            SeatNumber = seat.SeatNumber,
            //            Sets = sets,
            //        };

            //        var isSuccessSeatResult = await _orderService.AddSeatAsync(newSeat);

            //        await RefreshTablesAsync();

            //        if (!isSuccessSeatResult.IsSuccess)
            //        {
            //            isAllSeatSaved = !isAllSeatSaved;
            //            break;
            //        }
            //    }
            //}

            //if (isAllSeatSaved)
            //{
            //    CurrentOrder.PaymentStatus = _orderPaymentStatus;

            //    var config = new MapperConfiguration(cfg => cfg.CreateMap<FullOrderBindableModel, OrderModel>()
            //    .ForMember(x => x.TableNumber, s => s.MapFrom(x => x.Table.TableNumber))
            //    .ForMember(x => x.Customer, s => s.MapFrom(x => x.Customer)));

            //    var mapper = new Mapper(config);

            //    var order = mapper.Map<FullOrderBindableModel, OrderModel>(CurrentOrder);

            //    var isSuccessOrderResult = await _orderService.AddOrderAsync(order);

            //    if (isSuccessOrderResult.IsSuccess)
            //    {
            //        IsOrderSavedNotificationVisible = true;
            //        CurrentOrder.Seats = new();
            //        await _orderService.CreateNewOrderAsync();
            //    }
            //}
        }

        private async Task OnTabCommandAsync(bool isTab)
        {
            var parameters = new DialogParameters
            {
                { Constants.DialogParameterKeys.TITLE, LocalizationResourceManager.Current["NeedCard"] },
                { Constants.DialogParameterKeys.DESCRIPTION, LocalizationResourceManager.Current["SwipeTheCard"] },
                { Constants.DialogParameterKeys.CANCEL_BUTTON_TEXT, LocalizationResourceManager.Current["Cancel"] },
                { Constants.DialogParameterKeys.OK_BUTTON_TEXT, LocalizationResourceManager.Current["Complete"] },
            };

            PopupPage confirmDialog = App.IsTablet ?
                new Next2.Views.Tablet.Dialogs.MovedOrderToOrderTabsDialog(parameters, CloseMovedOrderDialogCallbackAsync) :
                new Next2.Views.Mobile.Dialogs.MovedOrderToOrderTabsDialog(parameters, CloseMovedOrderDialogCallbackAsync);

            await PopupNavigation.PushAsync(confirmDialog);
        }

        private async void CloseMovedOrderDialogCallbackAsync(IDialogParameters dialogResult)
        {
            if (dialogResult is not null && dialogResult.TryGetValue(Constants.DialogParameterKeys.ACCEPT, out bool isMovedOrderAccepted) && isMovedOrderAccepted)
            {
                if (isMovedOrderAccepted)
                {
                    await OnOrderCommandAsync(true);
                }
            }

            await PopupNavigation.PopAsync();
        }

        private async Task OnOpenModifyCommandAsync()
        {
            await _navigationService.NavigateAsync(nameof(Views.Tablet.ModificationsPage));
        }

        private async Task OnOpenRemoveCommandAsync()
        {
            var parameters = new DialogParameters
            {
                { Constants.DialogParameterKeys.TITLE, LocalizationResourceManager.Current["AreYouSure"] },
                { Constants.DialogParameterKeys.DESCRIPTION, LocalizationResourceManager.Current["ThisSetWillBeRemoved"] },
                { Constants.DialogParameterKeys.CANCEL_BUTTON_TEXT, LocalizationResourceManager.Current["Cancel"] },
                { Constants.DialogParameterKeys.OK_BUTTON_TEXT, LocalizationResourceManager.Current["Remove"] },
            };

            PopupPage confirmDialog = App.IsTablet
                ? new Next2.Views.Tablet.Dialogs.ConfirmDialog(parameters, CloseDeleteSetDialogCallbackAsync)
                : new Next2.Views.Mobile.Dialogs.ConfirmDialog(parameters, CloseDeleteSetDialogCallbackAsync);

            await PopupNavigation.PushAsync(confirmDialog);
        }

        private async void CloseDeleteSetDialogCallbackAsync(IDialogParameters dialogResult)
        {
            if (dialogResult is not null && dialogResult.TryGetValue(Constants.DialogParameterKeys.ACCEPT, out bool isDishRemovingAccepted))
            {
                if (isDishRemovingAccepted)
                {
                    var result = await _orderService.DeleteDishFromCurrentSeat();

                    if (result.IsSuccess)
                    {
                        RefreshCurrentOrderAsync();

                        CurrentOrder = await _bonusesService.СalculationBonusAsync(CurrentOrder);
                        if (CurrentState == LayoutState.Success)
                        {
                            if (_seatWithSelectedDish.SelectedDishes.Any())
                            {
                                SelectedDish = _seatWithSelectedDish.SelectedItem = _seatWithSelectedDish.SelectedDishes.FirstOrDefault();
                            }
                            else if (_isAnySetChosen)
                            {
                                foreach (var set in CurrentOrder.Seats)
                                {
                                    set.SelectedItem = null;
                                }

                                SelectedDish = _firstNotEmptySeat.SelectedItem = _firstNotEmptySeat.SelectedDishes.FirstOrDefault();
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

            await PopupNavigation.PopAsync();
        }

        private Task OnPayCommandAsync()
        {
            string path = App.IsTablet
                ? nameof(Views.Tablet.PaymentPage)
                : nameof(Views.Mobile.PaymentPage);

            return _navigationService.NavigateAsync(path);
        }

        private Task OnHideOrderNotificationCommnadAsync()
        {
            RefreshCurrentOrderAsync();

            return Task.CompletedTask;
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
                await _navigationService.NavigateAsync(nameof(OrderTabsPage));
            }

            _eventAggregator.GetEvent<OrderSelectedEvent>().Publish(CurrentOrder.Id);
            _eventAggregator.GetEvent<OrderMovedEvent>().Publish(true);

            RefreshCurrentOrderAsync();
        }

        #endregion
    }
}