using AutoMapper;
using Next2.Enums;
using Next2.Helpers;
using Next2.Models;
using Next2.Services.Authentication;
using Next2.Services.Order;
using Next2.Services.UserService;
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
        private readonly ICommand _setSelectionCommand;

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

            _seatSelectionCommand = new AsyncCommand<SeatBindableModel>(OnSeatSelectionCommandAsync, allowsMultipleExecutions: false);
            _deleteSeatCommand = new AsyncCommand<SeatBindableModel>(OnDeleteSeatCommandAsync, allowsMultipleExecutions: false);
            _setSelectionCommand = new AsyncCommand<SeatBindableModel>(OnSetSelectionCommandAsync, allowsMultipleExecutions: false);
        }

        #region -- Public properties --

        public FullOrderBindableModel CurrentOrder { get; set; } = new ();

        public ObservableCollection<OrderTypeBindableModel> OrderTypes { get; set; } = new ();

        public OrderTypeBindableModel SelectedOrderType { get; set; }

        public ObservableCollection<TableBindableModel> Tables { get; set; } = new ();

        public TableBindableModel SelectedTable { get; set; } = new ();

        public int NumberOfSeats { get; set; }

        public bool IsOrderWithTax { get; set; } = true;

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

        #endregion

        #region -- Overrides --

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
                        await _orderService.AddSeatInCurrentOrderAsync();
                        await AddSeatsCommandsAsync();
                    }

                    break;
                case nameof(IsOrderWithTax):
                    _orderService.CurrentOrder.Total = _orderService.CurrentOrder.SubTotal;
                    break;
            }
        }

        #endregion

        #region -- Public helpers --

        public void InitOrderTypes()
        {
            List<EOrderType> enums = new (Enum.GetValues(typeof(EOrderType)).Cast<EOrderType>());

            OrderTypes = new (enums.Select(x => new OrderTypeBindableModel
            {
                OrderType = x,
                Text = LocalizationResourceManager.Current[x.ToString()],
            }));
        }

        public async Task RefreshCurrentOrderAsync()
        {
            CurrentOrder = _orderService.CurrentOrder;

            // value for testing
            CurrentOrder.CustomerName = "Martin Levin";

            await AddSeatsCommandsAsync();

            SelectedTable = Tables.FirstOrDefault(row => row.Id == CurrentOrder.Table.Id);
            SelectedOrderType = OrderTypes.FirstOrDefault(row => row.OrderType == CurrentOrder.OrderType);
            NumberOfSeats = CurrentOrder.Seats.Count;
        }

        #endregion

        #region -- Private helpers --

        private async Task AddSeatsCommandsAsync()
        {
            foreach (var seat in CurrentOrder.Seats)
            {
                seat.SeatSelectionCommand = _seatSelectionCommand;
                seat.SeatDeleteCommand = _deleteSeatCommand;
                seat.SetSelectionCommand = _setSelectionCommand;
            }
        }

        private async Task OnSeatSelectionCommandAsync(SeatBindableModel seat)
        {
            if(CurrentOrder.Seats is not null)
            {
                seat.Checked = true;

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
                }
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
                        NumberOfSeats = CurrentOrder.Seats.Count;
                    }
                }
                else if (actionOnSets is EActionOnSets.RedirectSets
                    && dialogResult.TryGetValue(Constants.DialogParameterKeys.DESTINATION_SEAT_NUMBER, out int destinationSeatNumber))
                {
                    var resirectSetsResult = await _orderService.RedirectSetsFromSeatInCurrentOrder(removalSeat, destinationSeatNumber);

                    if (resirectSetsResult.IsSuccess)
                    {
                        await RefreshCurrentOrderAsync();

                        var deleteSeatResult = await _orderService.DeleteSeatFromCurrentOrder(removalSeat);

                        if (deleteSeatResult.IsSuccess)
                        {
                            NumberOfSeats = CurrentOrder.Seats.Count;
                        }
                    }
                }
            }

            await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAsync();
        }

        private async Task OnSetSelectionCommandAsync(SeatBindableModel seat)
        {
            if (CurrentOrder.Seats is not null)
            {
                foreach (var item in CurrentOrder.Seats)
                {
                    if (item.Id != seat.Id)
                    {
                        item.SelectedItem = null;
                    }
                }
            }
        }

        private async Task RefreshTablesAsync()
        {
            var availableTablesResult = await _orderService.GetFreeTablesAsync();

            if (availableTablesResult.IsSuccess)
            {
                var tableBindableModels = _mapper.Map<IEnumerable<TableModel>, ObservableCollection<TableBindableModel>>(availableTablesResult.Result);

                Tables = new (tableBindableModels);
            }
        }

        private Task OnOpenHoldSelectionCommandAsync()
        {
            return Task.CompletedTask;
        }

        private async Task OnOpenDiscountSelectionCommandAsync()
        {
            var parameters = new NavigationParameters { { Constants.Navigations.CURRENT_ORDER, CurrentOrder } };
            await _navigationService.NavigateAsync(nameof(TabletViews.BonusPage), parameters);
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
        }

        private Task OnOrderCommandAsync()
        {
            return Task.CompletedTask;
        }

        private Task OnTabCommandAsync()
        {
            return Task.CompletedTask;
        }

        private Task OnPayCommandAsync()
        {
            return Task.CompletedTask;
        }

        #endregion
    }
}