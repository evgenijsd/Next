using AutoMapper;
using Next2.Enums;
using Next2.Enums;
using Next2.Helpers;
using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using Next2.Services.Authentication;
using Next2.Services.Order;
using Next2.Services.UserService;
using Next2.ViewModels.Mobile;
using Next2.Views;
using Next2.Views.Mobile;
using Next2.Views.Tablet;
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
using System.Threading.Tasks;
using System.Windows.Input;
using MobileViewModels = Next2.ViewModels.Mobile;
using MobileViews = Next2.Views.Mobile;
using TabletViewModels = Next2.ViewModels.Tablet;
using TabletViews = Next2.Views.Tablet;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using static Next2.Constants;

namespace Next2.ViewModels
{
    public class OrderRegistrationViewModel : BaseViewModel
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IPopupNavigation _popupNavigation;
        private readonly IMapper _mapper;

        private readonly IOrderService _orderService;
        private readonly IUserService _userService;
        private readonly IAuthenticationService _authenticationService;

        private readonly ICommand _seatSelectionCommand;
        private readonly ICommand _deleteSeatCommand;
        private readonly ICommand _removeOrderCommand;
        private readonly ICommand _setSelectionCommand;

        private SeatBindableModel _firstSeat;
        private TaxModel _tax;
        private SeatBindableModel _firstNotEmptySeat;
        private SeatBindableModel _seatWithSelectedSet;
        private EOrderPaymentStatus _orderPaymentStatus;
        private bool _isAnySetChosen;

        public OrderRegistrationViewModel(
            INavigationService navigationService,
            IEventAggregator eventAggregator,
            IPopupNavigation popupNavigation,
            IMapper mapper,
            IOrderService orderService,
            IUserService userService,
            IAuthenticationService authenticationService)
            : base(navigationService)
        {
            _popupNavigation = popupNavigation;
            _eventAggregator = eventAggregator;
            _mapper = mapper;
            _orderService = orderService;
            _userService = userService;
            _authenticationService = authenticationService;

            _orderPaymentStatus = EOrderPaymentStatus.None;

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
        public SetBindableModel? SelectedSet { get; set; }

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
        public ICommand OrderCommand => _orderCommand ??= new AsyncCommand<EOrderPaymentStatus>(OnOrderCommandAsync, allowsMultipleExecutions: false);

        private ICommand _tabCommand;
        public ICommand TabCommand => _tabCommand ??= new AsyncCommand<EOrderPaymentStatus>(OnTabCommandAsync, allowsMultipleExecutions: false);

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

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (!App.IsTablet)
            {
                foreach (var seat in CurrentOrder.Seats)
                {
                    seat.SelectedItem = null;
                }

                if (parameters.ContainsKey(Constants.Navigations.DELETE_SET))
                {
                    MessagingCenter.Subscribe<EditPageViewModel, SetBindableModel>(this, Constants.Navigations.SELECTED_SET, (sender, arg) =>
                    {
                        RecalculateOrderPriceBySet(arg);
                    });
                }
            }

            if (CurrentOrder.Tax.Value == 0)
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
                    _orderService.CurrentOrder.Table = SelectedTable;
                    break;
                case nameof(SelectedOrderType):
                    _orderService.CurrentOrder.OrderType = SelectedOrderType.OrderType;
                    break;
                case nameof(NumberOfSeats):
                    if (NumberOfSeats > CurrentOrder.Seats.Count)
                    {
                        IsOrderSavedNotificationVisible = false;
                        await _orderService.AddSeatInCurrentOrderAsync();
                        await AddSeatsCommandsAsync();
                    }

                    break;
                case nameof(IsOrderWithTax):
                    _orderService.CurrentOrder.Total = _orderService.CurrentOrder.BonusType != EBonusType.None ? _orderService.CurrentOrder.PriceWithBonus : _orderService.CurrentOrder.SubTotal;
                    _orderService.CurrentOrder.PriceTax = 0;
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

            _seatWithSelectedSet = CurrentOrder.Seats.FirstOrDefault(x => x.SelectedItem is not null);

            _isAnySetChosen = CurrentOrder.Seats.Any(x => x.Sets.Any());

            _firstNotEmptySeat = CurrentOrder.Seats.FirstOrDefault(x => x.Sets.Any());

            await AddSeatsCommandsAsync();

            SelectedTable = Tables.FirstOrDefault(row => row.Id == CurrentOrder.Table.Id);
            SelectedOrderType = OrderTypes.FirstOrDefault(row => row.OrderType == CurrentOrder.OrderType);
            NumberOfSeats = CurrentOrder.Seats.Count;
        }

        #endregion

        #region -- Private helpers --

        private async Task RefreshTablesAsync()
        {
            var availableTablesResult = await _orderService.GetFreeTablesAsync();

            if (availableTablesResult.IsSuccess)
            {
                var tableBindableModels = _mapper.Map<ObservableCollection<TableBindableModel>>(availableTablesResult.Result);

                Tables = new(tableBindableModels);
            }
        }

        private async Task AddSeatsCommandsAsync()
        {
            foreach (var seat in CurrentOrder.Seats)
            {
                seat.SeatSelectionCommand = _seatSelectionCommand;
                seat.SeatDeleteCommand = _deleteSeatCommand;
                seat.RemoveOrderCommand = _removeOrderCommand;
                seat.SetSelectionCommand = _setSelectionCommand;
            }
        }

        private async Task DeleteSeatsCommandsAsync()
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
            if (SelectedSet is not null)
            {
                foreach (var item in CurrentOrder.Seats)
                {
                    item.SelectedItem = null;
                }
            }

            IsSideMenuVisible = true;
            CurrentState = LayoutState.Loading;
        }

        private async Task OnSeatSelectionCommandAsync(SeatBindableModel seat)
        {
            if (CurrentOrder.Seats is not null)
            {
                seat.Checked = true;

                SelectedSeat = seat;

                foreach (var item in CurrentOrder.Seats)
                {
                    if (item.Id != seat.Id)
                    {
                        item.Checked = false;
                    }
                }

                _orderService.CurrentSeat = seat;
            }
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
            if (seat.Sets.Any())
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

                await _popupNavigation.PushAsync(deleteSeatDialog);
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
                        RecalculateOrderPriceBySeat(removalSeat);
                        await RefreshCurrentOrderAsync();

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
                                SelectedSet = _firstNotEmptySeat.SelectedItem = (CurrentState == LayoutState.Success) ? _firstNotEmptySeat.Sets.FirstOrDefault() : _firstNotEmptySeat.SelectedItem = null;
                            }
                            else
                            {
                                await DeleteSeatsCommandsAsync();

                                foreach (var item in CurrentOrder.Seats)
                                {
                                    item.Checked = false;
                                }

                                _firstSeat.Checked = true;

                                SelectedSet = _firstSeat.Sets.FirstOrDefault();

                                await RefreshCurrentOrderAsync();
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
                            await DeleteSeatsCommandsAsync();

                            var updatedDestinationSeatNumber = (destinationSeatNumber < removalSeat.SeatNumber) ? destinationSeatNumber : destinationSeatNumber - 1;

                            var destinationSeat = CurrentOrder.Seats.FirstOrDefault(x => x.SeatNumber == updatedDestinationSeatNumber);

                            foreach (var item in CurrentOrder.Seats)
                            {
                                item.Checked = false;
                            }

                            destinationSeat.Checked = true;
                            if (CurrentState == LayoutState.Success)
                            {
                                SelectedSet = destinationSeat.SelectedItem = destinationSeat.Sets.FirstOrDefault();
                            }

                            await RefreshCurrentOrderAsync();
                        }
                    }
                }
            }

            await _popupNavigation.PopAsync();

            if (!App.IsTablet && !CurrentOrder.Seats.Any())
            {
                await _navigationService.GoBackAsync();
            }
        }

        private async Task OnRemoveOrderCommandAsync()
        {
            List<SeatModel> seats = new();

            foreach (var seat in CurrentOrder.Seats)
            {
                if (seat.Sets.Any())
                {
                    var sets = new List<SetModel>(seat.Sets.Select(x => new SetModel
                    {
                        ImagePath = x.ImagePath,
                        Title = x.Title,
                        Price = x.Portion.Price,
                    }));

                    var newSeat = new SeatModel
                    {
                        SeatNumber = seat.SeatNumber,
                        Sets = sets,
                    };

                    seats.Add(newSeat);
                }
            }

            var param = new DialogParameters
            {
                { Constants.DialogParameterKeys.ORDER_NUMBER, CurrentOrder.OrderNumber },
                { Constants.DialogParameterKeys.SEATS, seats },
            };

            PopupPage removeOrderDialog = App.IsTablet
                ? new Views.Tablet.Dialogs.DeleteOrderDialog(param, CloseDeleteOrderDialogCallbackAsync)
                : new Views.Mobile.Dialogs.DeleteOrderDialog(param, CloseDeleteOrderDialogCallbackAsync);

            await _popupNavigation.PushAsync(removeOrderDialog);
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

                    await _popupNavigation.PushAsync(orderDeletionConfirmationDialog);
                }
            }
            else
            {
                await _popupNavigation.PopAsync();
            }
        }

        private async void CloseOrderDeletionConfirmationDialogCallback(IDialogParameters parameters)
        {
            if (parameters is not null && parameters.TryGetValue(Constants.DialogParameterKeys.ACCEPT, out bool isOrderRemovingAccepted))
            {
                if (isOrderRemovingAccepted)
                {
                    var result = await _orderService.CreateNewOrderAsync();

                    if (result.IsSuccess)
                    {
                        NumberOfSeats = 0;

                        if (App.IsTablet)
                        {
                            IsSideMenuVisible = true;
                            CurrentState = LayoutState.Loading;
                        }

                        await RefreshCurrentOrderAsync();
                    }

                    await _popupNavigation.PopAsync();
                }
            }

            await _popupNavigation.PopAsync();

            if (!App.IsTablet && !CurrentOrder.Seats.Any())
            {
                await _navigationService.GoBackAsync();
            }
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
                }

                SelectedSet = seat.SelectedItem;

                if (App.IsTablet)
                {
                    IsSideMenuVisible = false;
                    CurrentState = LayoutState.Success;
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

            _eventAggregator.GetEvent<AddBonusToCurrentOrderEvent>().Unsubscribe(BonusEventCommand);
        }

        private async Task OnRemoveTaxFromOrderCommandAsync()
        {
            var user = await _userService.GetUserById(_authenticationService.AuthorizedUserId);

            if (user.IsSuccess && (user.Result.UserType != EUserType.Admin))
            {
                _eventAggregator.GetEvent<TaxRemovedEvent>().Subscribe(OnTaxEvent);

                if (App.IsTablet)
                {
                    var parameters = new NavigationParameters { { Constants.Navigations.ADMIN, nameof(NewOrderView) } };
                    await _navigationService.NavigateAsync(nameof(NumericPage), parameters, useModalNavigation: true);
                }
                else
                {
                    var parameters = new NavigationParameters { { Constants.Navigations.ADMIN, nameof(OrderRegistrationPage) } };
                    await _navigationService.NavigateAsync(nameof(Views.Mobile.LoginPage), parameters);
                }
            }
            else
            {
                IsOrderWithTax = false;
            }
        }

        private void OnTaxEvent(bool isOrderWithTax)
        {
            _eventAggregator.GetEvent<TaxRemovedEvent>().Unsubscribe(OnTaxEvent);

            IsOrderWithTax = isOrderWithTax;

            if (!IsOrderWithTax)
            {
                CurrentOrder.Tax.Value = 0;
            }
        }

        private async Task OnOrderCommandAsync(EOrderPaymentStatus commandParameter)
        {
            _orderPaymentStatus = commandParameter;

            List<SeatModel> seats = new();

            bool isAllSeatSaved = true;

            foreach (var seat in CurrentOrder.Seats)
            {
                if (seat.Sets.Any())
                {
                    var sets = new List<SetModel>(seat.Sets.Select(x => new SetModel
                    {
                        Id = x.Id,
                        SubcategoryId = x.SubcategoryId,
                        Title = x.Title,
                        Price = x.Portion.Price,
                        ImagePath = x.ImagePath,
                    }));

                    var newSeat = new SeatModel
                    {
                        OrderId = CurrentOrder.Id,
                        SeatNumber = seat.SeatNumber,
                        Sets = sets,
                    };

                    var isSuccessSeatResult = await _orderService.AddSeatAsync(newSeat);

                    if (!isSuccessSeatResult.IsSuccess)
                    {
                        isAllSeatSaved = !isAllSeatSaved;
                        break;
                    }
                }
            }

            if (isAllSeatSaved)
            {
                CurrentOrder.PaymentStatus = _orderPaymentStatus;

                var config = new MapperConfiguration(cfg => cfg.CreateMap<FullOrderBindableModel, OrderModel>()
                .ForMember(x => x.TableNumber, s => s.MapFrom(x => x.Table.TableNumber))
                .ForMember(x => x.CustomerName, s => s.MapFrom(x => x.Customer.Name)));

                var mapper = new Mapper(config);

                var order = mapper.Map<FullOrderBindableModel, OrderModel>(CurrentOrder);

                var isSuccessOrderResult = await _orderService.AddOrderAsync(order);

                if (isSuccessOrderResult.IsSuccess)
                {
                    IsOrderSavedNotificationVisible = true;
                    CurrentOrder.Seats = new();
                    await _orderService.CreateNewOrderAsync();
                }
            }
        }

        private async Task OnTabCommandAsync(EOrderPaymentStatus commandParameter)
        {
            var parameters = new DialogParameters
            {
                { Constants.DialogParameterKeys.TITLE, LocalizationResourceManager.Current["NeedCard"] },
                { Constants.DialogParameterKeys.DESCRIPTION, LocalizationResourceManager.Current["SwipeTheCard"] },
                { Constants.DialogParameterKeys.CANCEL_BUTTON_TEXT, LocalizationResourceManager.Current["Cancel"] },
                { Constants.DialogParameterKeys.OK_BUTTON_TEXT, LocalizationResourceManager.Current["Complete"] },
                { Constants.DialogParameterKeys.ACTION_ON_ORDER, commandParameter },
            };

            PopupPage confirmDialog = new Next2.Views.Tablet.Dialogs.MovedOrderToOrderTabsDialog(parameters, CloseMovedOrderDialogCallbackAsync);

            await _popupNavigation.PushAsync(confirmDialog);
        }

        private async void CloseMovedOrderDialogCallbackAsync(IDialogParameters dialogResult)
        {
            if (dialogResult is not null && dialogResult.TryGetValue(Constants.DialogParameterKeys.ACCEPT, out bool isMovedOrderAccepted)
                && dialogResult.TryGetValue(Constants.DialogParameterKeys.ACTION_ON_ORDER, out EOrderPaymentStatus commandParameter))
            {
                if (isMovedOrderAccepted && commandParameter == EOrderPaymentStatus.InProgress)
                {
                    await OnOrderCommandAsync(commandParameter);
                }
            }

            await _popupNavigation.PopAsync();
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

            await _popupNavigation.PushAsync(confirmDialog);
        }

        private async void CloseDeleteSetDialogCallbackAsync(IDialogParameters dialogResult)
        {
            if (dialogResult is not null && dialogResult.TryGetValue(Constants.DialogParameterKeys.ACCEPT, out bool isSetRemovingAccepted))
            {
                if (isSetRemovingAccepted)
                {
                    var result = await _orderService.DeleteSetFromCurrentSeat();

                    if (SelectedSet is not null)
                    {
                        RecalculateOrderPriceBySet(SelectedSet);
                    }

                    if (result.IsSuccess)
                    {
                        await RefreshCurrentOrderAsync();

                        if (CurrentState == LayoutState.Success)
                        {
                            if (_seatWithSelectedSet.Sets.Any())
                            {
                                SelectedSet = _seatWithSelectedSet.SelectedItem = _seatWithSelectedSet.Sets.FirstOrDefault();
                            }
                            else if (_isAnySetChosen)
                            {
                                foreach (var set in CurrentOrder.Seats)
                                {
                                    set.SelectedItem = null;
                                }

                                SelectedSet = _firstNotEmptySeat.SelectedItem = _firstNotEmptySeat.Sets.FirstOrDefault();
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

            await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAsync();
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
            return RefreshCurrentOrderAsync();
        }

        private async Task OnGoToOrderTabsCommandAsync()
        {
            if (App.IsTablet)
            {
                IsSideMenuVisible = true;
                CurrentState = LayoutState.Loading;

                await RefreshCurrentOrderAsync();

                MessagingCenter.Send<MenuPageSwitchingMessage>(new(EMenuItems.OrderTabs), Constants.Navigations.SWITCH_PAGE);
            }
            else
            {
                await _navigationService.NavigateAsync(nameof(OrderTabsPage));
            }

            _eventAggregator.GetEvent<OrderSelectedEvent>().Publish(CurrentOrder.Id);
            _eventAggregator.GetEvent<OrderMovedEvent>().Publish(_orderPaymentStatus);
        }

        private void RecalculateOrderPriceBySet(SetBindableModel selectedSet)
        {
            var amoutToSubtract = selectedSet.Portion.Price;
            CurrentOrder.Total -= amoutToSubtract;
            CurrentOrder.SubTotal -= amoutToSubtract;

            if (!App.IsTablet)
            {
                MessagingCenter.Unsubscribe<EditPageViewModel, SetBindableModel>(this, Constants.Navigations.SELECTED_SET);
            }
        }

        private void RecalculateOrderPriceBySeat(SeatBindableModel seat)
        {
            float amoutToSubtract = 0;

            if (seat.Sets.Any())
            {
                foreach (var set in seat.Sets)
                {
                    amoutToSubtract += set.Portion.Price;
                }

                CurrentOrder.Total -= amoutToSubtract;
                CurrentOrder.SubTotal -= amoutToSubtract;
            }
        }

        #endregion
    }
}