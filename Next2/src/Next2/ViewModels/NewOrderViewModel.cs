using Next2.Enums;
using Next2.Extensions;
using Next2.Models;
using Next2.Services.Order;
using Prism.Navigation;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels
{
    public class NewOrderViewModel : BaseViewModel
    {
        private readonly IOrderService _newOrderService;

        public NewOrderViewModel(
            INavigationService navigationService,
            IOrderService newOrderService)
            : base(navigationService)
        {
            _newOrderService = newOrderService;
        }

        #region -- Public properties --

        public OrderBindableModel NewOrder { get; set; }

        public EOrderType OrderType { get; set; }

        public ObservableCollection<TableBindableModel> Tables { get; set; } = new ();

        public TableBindableModel SelectedTable { get; set; } = new ();

        public int NumberOfSeats { get; set; }

        public bool IsAnyDishSelected { get; set; }

        public bool IsOrderWithTax { get; set; }

        public float Tax { get; set; }

        public float SubTotal { get; set; }

        public float Total { get; set; }

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

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            NewOrder = new ();
            SelectedTable = new ();
            OrderType = EOrderType.DineIn;
            NumberOfSeats = 1;
            IsOrderWithTax = true;
            IsAnyDishSelected = false;

            await InitData();
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);

            Tables.Clear();
        }

        public override async void OnAppearing()
        {
            base.OnAppearing();

            NewOrder = new ();
            SelectedTable = new ();
            OrderType = EOrderType.DineIn;
            NumberOfSeats = 1;
            IsOrderWithTax = true;
            IsAnyDishSelected = false;

            await InitData();
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

        private async Task InitData()
        {
            var orderResult = await _newOrderService.GetNewOrderIdAsync();

            if (orderResult.IsSuccess)
            {
                NewOrder.Id = orderResult.Result;
            }

            var tablesResult = await _newOrderService.GetTables();

            if (tablesResult.IsSuccess)
            {
                var allTables = tablesResult.Result;

                var tableBindableModels = new ObservableCollection<TableBindableModel>(allTables.Select(x => x.ToBindableModel()));

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

        private Task OnPayCommandAsync()
        {
            return Task.CompletedTask;
        }

        #endregion
    }
}