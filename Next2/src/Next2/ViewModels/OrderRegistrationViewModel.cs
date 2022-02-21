﻿using Next2.Enums;
using Next2.Extensions;
using Next2.Models;
using Next2.Services.Order;
using Prism.Navigation;
using System;
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
        private readonly IOrderService _orderService;

        public OrderRegistrationViewModel(
            INavigationService navigationService,
            IOrderService orderService)
            : base(navigationService)
        {
            _orderService = orderService;

            var r = Enum.GetValues(typeof(EOrderType)).Cast<EOrderType>().ToList();

            OrderTypes = new (r.Select(x => new OrderTypeBindableModel
            {
                OrderType = x,
                OrderTypeValue = LocalizationResourceManager.Current[x.ToString()],
            }));

            Task.Run(RefreshOrderId);
            Task.Run(RefreshTables);
        }

        #region -- Public properties --

        public int NewOrderId { get; set; }

        public ObservableCollection<OrderTypeBindableModel> OrderTypes { get; set; } = new ();

        public ObservableCollection<string> Sets { get; set; } = new ("0123456789abcdef".Select(x => x.ToString()));

        public OrderTypeBindableModel SelectedOrderType { get; set; }

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
            RefreshTables(),
            RefreshOrderId());
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            if (args.PropertyName == nameof(SelectedTable))
            {
                NumberOfSeats = 1;
            }
        }

        #endregion

        #region -- Private helpers --

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
            if (Sets.Count > 0)
            {
                Sets.Clear();
            }
            else
            {
                Sets = new ("0123456789abcdef".Select(x => x.ToString()));
            }
        }

        #endregion
    }
}