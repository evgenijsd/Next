using AutoMapper;
using Next2.Enums;
using Next2.Models;
using Next2.Services.Order;
using Prism.Navigation;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Contracts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels
{
    public class OrderRegistrationViewModel : BaseViewModel
    {
        private readonly IPopupNavigation _popupNavigation;
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;

        private ICommand _tapCheckedCommand;
        private ICommand _deleteSeatCommand;
        private ICommand _tapItemCommand;

        public OrderRegistrationViewModel(
            IPopupNavigation popupNavigation,
            INavigationService navigationService,
            IMapper mapper,
            IOrderService orderService)
            : base(navigationService)
        {
            _popupNavigation = popupNavigation;
            _orderService = orderService;
            _mapper = mapper;

            _tapCheckedCommand = new AsyncCommand<SeatBindableModel>(OnTapCheckedCommandAsync, allowsMultipleExecutions: false);
            _deleteSeatCommand = new AsyncCommand<SeatBindableModel>(OnDeleteSeatCommandAsync, allowsMultipleExecutions: false);
            _tapItemCommand = new AsyncCommand<SeatBindableModel>(OnTapItemCommandAsync, allowsMultipleExecutions: false);
        }

        #region -- Public properties --

        public FullOrderBindableModel CurrentOrder { get; set; } = new();

        public ObservableCollection<OrderTypeBindableModel> OrderTypes { get; set; } = new ();

        public OrderTypeBindableModel SelectedOrderType { get; set; }

        public ObservableCollection<TableBindableModel> Tables { get; set; } = new ();

        public TableBindableModel SelectedTable { get; set; }

        public int NumberOfSeats { get; set; }

        public bool IsOrderWithTax { get; set; } = true;

        private ICommand _openHoldSelectionCommand;
        public ICommand OpenHoldSelectionCommand => _openHoldSelectionCommand ??= new AsyncCommand(OnOpenHoldSelectionCommandAsync);

        private ICommand _openDiscountSelectionCommand;
        public ICommand OpenDiscountSelectionCommand => _openDiscountSelectionCommand ??= new AsyncCommand(OnOpenDiscountSelectionCommandAsync);

        private ICommand _removeTaxFromOrderCommand;
        public ICommand RemoveTaxFromOrderCommand => _removeTaxFromOrderCommand ??= new AsyncCommand(OnRemoveTaxFromOrderCommandAsync);

        private ICommand _orderCommand;
        public ICommand OrderCommand => _orderCommand ??= new AsyncCommand(OnOrderCommandAsync);

        private ICommand _tabCommand;
        public ICommand TabCommand => _tabCommand ??= new AsyncCommand(OnTabCommandAsync);

        private ICommand _payCommand;
        public ICommand PayCommand => _payCommand ??= new AsyncCommand(OnPayCommandAsync);

        private ICommand _deleteLastSeatCommand;
        public ICommand DeleteLastSeatCommand => _deleteLastSeatCommand ??= new AsyncCommand(OnDeleteLastSeatCommandAsync);

        #endregion

        #region -- Overrides --

        public override async Task InitializeAsync(INavigationParameters parameters)
        {
            base.InitializeAsync(parameters);

            InitOrderTypes();
            await RefreshTablesAsync();
            await RefreshCurrentOrderAsync();
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
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
                        _orderService.AddSeatInCurrentOrderAsync();
                        AddSeatsCommandsAsync();
                    }
                    else if (NumberOfSeats < CurrentOrder.Seats.Count)
                    {
                        var lastSeat = CurrentOrder.Seats.LastOrDefault();
                        DeleteSeat(lastSeat);
                    }

                    break;
            }
        }

        #endregion

        #region -- Public helpers --

        public void InitOrderTypes()
        {
            List<EOrderType> enums = new (Enum.GetValues(typeof(EOrderType)).Cast<EOrderType>());

            OrderTypes = new(enums.Select(x => new OrderTypeBindableModel
            {
                OrderType = x,
                Text = LocalizationResourceManager.Current[x.ToString()],
            }));
        }

        public async Task RefreshCurrentOrderAsync()
        {
            CurrentOrder = null;
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
                seat.TapCheckBoxCommand = _tapCheckedCommand;
                seat.TapDeleteCommand = _deleteSeatCommand;
                seat.TapItemCommand = _tapItemCommand;
            }
        }

        private async Task OnTapCheckedCommandAsync(SeatBindableModel seat)
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

        private async Task OnDeleteSeatCommandAsync(SeatBindableModel seat)
        {
            DeleteSeat(seat);
        }

        private async Task OnDeleteLastSeatCommandAsync()
        {
            var lastSeat = CurrentOrder.Seats.LastOrDefault();

            DeleteSeat(lastSeat);
        }

        private async void DeleteSeat(SeatBindableModel seat)
        {
            if (seat.Sets.Any())
            {
                IEnumerable<int> seatNumbers = CurrentOrder.Seats.Select(x => x.SeatNumber);

                var param = new DialogParameters
                {
                    { Constants.DialogParameterKeys.SEAT, seat },
                    { Constants.DialogParameterKeys.SEAT_NUMBERS, seatNumbers },
                };

                await _popupNavigation.PushAsync(App.IsTablet
                    ? new Views.Tablet.Dialogs.DeleteSeatDialog(param, CloseDeleteSeatDialogCallback)
                    : new Views.Mobile.Dialogs.DeleteSeatDialog(param, CloseDeleteSeatDialogCallback));
            }
            else
            {
                var deleteSeatResult = await _orderService.DeleteSeatFromCurrentOrder(seat.SeatNumber);

                if (deleteSeatResult.IsSuccess)
                {
                    NumberOfSeats = CurrentOrder.Seats.Count;
                }
            }
        }

        private async void CloseDeleteSeatDialogCallback(IDialogParameters dialogResult)
        {
            if (dialogResult is not null
                && dialogResult.TryGetValue(Constants.DialogParameterKeys.ACTION, out EActionWhenDeletingSeat action)
                && dialogResult.TryGetValue(Constants.DialogParameterKeys.SEAT, out SeatBindableModel removalSeat))
            {
                if (action is EActionWhenDeletingSeat.DeleteSets)
                {
                    var deleteSeatResult = await _orderService.DeleteSeatFromCurrentOrder(removalSeat.SeatNumber);

                    if (deleteSeatResult.IsSuccess)
                    {
                        NumberOfSeats = CurrentOrder.Seats.Count;
                    }
                }
                else if (action is EActionWhenDeletingSeat.RedirectSets
                    && dialogResult.TryGetValue(Constants.DialogParameterKeys.SEAT_NUMBER, out int destinationSeatNumber))
                {
                    var resirectSetsResult = await _orderService.RedirectSetsFromCurrentOrder(removalSeat.SeatNumber, destinationSeatNumber);

                    if (resirectSetsResult.IsSuccess)
                    {
                        await RefreshCurrentOrderAsync();

                        var deleteSeatResult = await _orderService.DeleteSeatFromCurrentOrder(removalSeat.SeatNumber);

                        if (deleteSeatResult.IsSuccess)
                        {
                            NumberOfSeats = CurrentOrder.Seats.Count;
                        }
                    }
                }
            }

            await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAsync();
        }

        private async Task OnTapItemCommandAsync(SeatBindableModel seat)
        {
            foreach (var item in CurrentOrder.Seats)
            {
                if (item.Id != seat.Id)
                {
                    item.SelectedItem = null;
                }
            }
        }

        private async Task RefreshTablesAsync()
        {
            var availableTablesResult = await _orderService.GetAvailableTablesAsync();

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

        private Task OnOpenDiscountSelectionCommandAsync()
        {
            return Task.CompletedTask;
        }

        private Task OnRemoveTaxFromOrderCommandAsync()
        {
            IsOrderWithTax = false;

            return Task.CompletedTask;
        }

        private Task OnOrderCommandAsync()
        {
            return Task.CompletedTask;
        }

        private Task OnTabCommandAsync()
        {
            return Task.CompletedTask;
        }

        private async Task OnPayCommandAsync()
        {
        }

        #endregion
    }
}