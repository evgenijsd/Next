using Next2.Enums;
using Next2.Extensions;
using Next2.Models;
using Next2.Services.Order;
using Prism.Navigation;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels
{
    public class NewOrderViewModel : BaseViewModel
    {
        private readonly IOrderService _orderService;

        public NewOrderViewModel(
            INavigationService navigationService,
            IOrderService orderService)
            : base(navigationService)
        {
            _orderService = orderService;
        }

        #region -- Public properties --

        public int NewOrderId { get; set; }

        public EOrderType OrderType { get; set; } = EOrderType.DineIn;

        public ObservableCollection<TableBindableModel> Tables { get; set; } = new ();

        public TableBindableModel SelectedTable { get; set; } = new ();

        public int NumberOfSeats { get; set; } = 0;

        public bool IsOrderWithTax { get; set; } = true;

        public float Tax { get; set; } = 3;

        public float SubTotal { get; set; } = 24;

        public float Total { get; set; } = 27;

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

        //for testing
        private ICommand _selectTableCommand;
        public ICommand SelectTableCommand => _selectTableCommand ??= new AsyncCommand<TableBindableModel>(OnSelectTableCommandAsync);

        private ICommand _payCommand;
        public ICommand PayCommand => _payCommand ??= new AsyncCommand(OnPayCommandAsync);

        #endregion

        #region -- Overrides --

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            await Task.WhenAll(
                RefreshOrderId(),
                RefreshTables());
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);

            Tables.Clear();
        }

        public override async void OnAppearing()
        {
            base.OnAppearing();

            await Task.WhenAll(
                RefreshOrderId(),
                RefreshTables());
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();

            Tables.Clear();
        }

        #endregion

        #region -- Private helpers --

        private float CalculateTax()
        {
            return Total / 100 * Constants.TAX_PERCENTAGE;
        }

        private async Task RefreshOrderId()
        {
            var orderResult = await _orderService.GetNewOrderIdAsync();

            if (orderResult.IsSuccess)
            {
                NewOrderId = orderResult.Result;
            }
        }

        private async Task RefreshTables()
        {
            var availableTablesResult = await _orderService.GetAvailableTables();

            if (availableTablesResult.IsSuccess)
            {
                var availableTables = availableTablesResult.Result;

                var tableBindableModels = new ObservableCollection<TableBindableModel>(availableTables.Select(x => x.ToBindableModel()));

                Tables = new (tableBindableModels);

                SelectedTable = Tables[0];
                NumberOfSeats = 1;
            }
        }

        //for testing
        private Task OnSelectTableCommandAsync(TableBindableModel? table)
        {
            NumberOfSeats = 1;

            return Task.CompletedTask;
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
            if (Tables.Count > 0)
            {
                Tables.Clear();
            }
            else
            {
                await RefreshTables();
            }
        }

        #endregion
    }
}