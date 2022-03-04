using AutoMapper;
using Next2.Enums;
using Next2.Models;
using Next2.Services.Order;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;

namespace Next2.ViewModels
{
    public class OrderRegistrationViewModel : BaseViewModel
    {
        private readonly IOrderService _orderService;

        private bool _isInited;

        private ICommand _tapCheckedCommand;
        private ICommand _tapDeleteCommand;
        private ICommand _tapItemCommand;

        public OrderRegistrationViewModel(
            INavigationService navigationService,
            IOrderService orderService)
            : base(navigationService)
        {
            _orderService = orderService;

            CurrentState = LayoutState.Loading;

            _tapCheckedCommand = new AsyncCommand<SeatBindableModel>(OnTapCheckedCommandAsync, allowsMultipleExecutions: false);
            _tapDeleteCommand = new AsyncCommand<SeatBindableModel>(OnTapDeleteCommandAsync, allowsMultipleExecutions: false);
            _tapItemCommand = new AsyncCommand<SeatBindableModel>(OnTapItemCommandAsync, allowsMultipleExecutions: true);

            RefreshTablesAsync();

            List<EOrderType> enums = new (Enum.GetValues(typeof(EOrderType)).Cast<EOrderType>());

            OrderTypes = new (enums.Select(x => new OrderTypeBindableModel
            {
                OrderType = x,
                Text = LocalizationResourceManager.Current[x.ToString()],
            }));
        }

        #region -- Public properties --

        public LayoutState CurrentState { get; set; }

        public FullOrderBindableModel CurrentOrder { get; set; } = new();

        public ObservableCollection<OrderTypeBindableModel> OrderTypes { get; set; } = new();

        public OrderTypeBindableModel SelectedOrderType { get; set; }

        public ObservableCollection<TableBindableModel> Tables { get; set; } = new();

        public TableBindableModel SelectedTable { get; set; }

        public int NumberOfSeats { get; set; } = 0;

        public bool IsSideMenuVisible { get; set; } = true;

        public bool IsOrderWithTax { get; set; } = true;

        private ICommand _goBackCommand;
        public ICommand GoBackCommand => _goBackCommand ??= new Command(OnGoBackCommand);

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

        #endregion

        #region -- Overrides --

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            RefreshCurrentOrderAsync();
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            switch (args.PropertyName)
            {
                case nameof(SelectedTable):
                    if (_isInited)
                    {
                        _orderService.CurrentOrder.Table = SelectedTable;
                    }

                    break;
                case nameof(SelectedOrderType):
                    if (_isInited)
                    {
                        _orderService.CurrentOrder.OrderType = SelectedOrderType.OrderType;
                    }

                    break;
                case nameof(NumberOfSeats):
                    if (_isInited)
                    {
                        if (NumberOfSeats > CurrentOrder.Seats.Count)
                        {
                            _orderService.AddSeatInCurrentOrderAsync();
                            AddSeatsCommandsAsync();
                        }
                    }

                    break;
            }
        }

        #endregion

        #region -- Public helpers --

        public async Task RefreshCurrentOrderAsync()
        {
            CurrentOrder = _orderService.CurrentOrder;

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
                seat.TapDeleteCommand = _tapDeleteCommand;
                seat.TapItemCommand = _tapItemCommand;
            }
        }

        private void OnGoBackCommand()
        {
            IsSideMenuVisible = true;
            CurrentState = LayoutState.Loading;
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

        private async Task OnTapDeleteCommandAsync(SeatBindableModel seat)
        {
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

            IsSideMenuVisible = false;
            CurrentState = LayoutState.Success;
        }

        private async Task RefreshTablesAsync()
        {
            var availableTablesResult = await _orderService.GetAvailableTablesAsync();

            if (availableTablesResult.IsSuccess)
            {
                MapperConfiguration mapperConfig = new(cfg => cfg.CreateMap<TableModel, TableBindableModel>());
                Mapper mapper = new(mapperConfig);
                var tableBindableModels = mapper.Map<IEnumerable<TableModel>, ObservableCollection<TableBindableModel>>(availableTablesResult.Result);

                Tables = new(tableBindableModels);

                SelectedTable = Tables[0];
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