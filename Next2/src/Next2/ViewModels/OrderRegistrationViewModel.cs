﻿using AutoMapper;
using Next2.Enums;
using Next2.Models;
using Next2.Services.Order;
using Next2.Views.Mobile;
using Next2.Views.Tablet;
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

            Task.Run(RefreshOrderIdAsync);
            Task.Run(RefreshTablesAsync);

            List<EOrderType> enums = new (Enum.GetValues(typeof(EOrderType)).Cast<EOrderType>());

            OrderTypes = new (enums.Select(x => new OrderTypeBindableModel
            {
                OrderType = x,
                Text = LocalizationResourceManager.Current[x.ToString()],
            }));
        }

        #region -- Public properties --

        public int NewOrderId { get; set; }

        public ObservableCollection<OrderTypeBindableModel> OrderTypes { get; set; } = new ();

        // value for testing, replace it later
        public ObservableCollection<string> Sets { get; set; } = new () { string.Empty };

        public OrderTypeBindableModel SelectedOrderType { get; set; }

        public ObservableCollection<TableBindableModel> Tables { get; set; } = new ();

        public TableBindableModel SelectedTable { get; set; } = new ();

        public int NumberOfSeats { get; set; } = 0;

        public bool IsOrderWithTax { get; set; } = true;

        // value for testing, delete it later
        public float Tax { get; set; } = 3;

        // value for testing, delete it later
        public float SubTotal { get; set; } = 24;

        // value for testing, delete it later
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

        private ICommand _payCommand;
        public ICommand PayCommand => _payCommand ??= new AsyncCommand(OnPayCommandAsync);

        #endregion

        #region -- Overrides --

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

        private async Task RefreshOrderIdAsync()
        {
            var orderResult = await _orderService.GetNewOrderIdAsync();

            if (orderResult.IsSuccess)
            {
                NewOrderId = orderResult.Result;
            }
        }

        private async Task RefreshTablesAsync()
        {
            var availableTablesResult = await _orderService.GetAvailableTables();

            if (availableTablesResult.IsSuccess)
            {
                MapperConfiguration mapperConfig = new (cfg => cfg.CreateMap<TableModel, TableBindableModel>());
                Mapper mapper = new (mapperConfig);
                var tableBindableModels = mapper.Map<IEnumerable<TableModel>, ObservableCollection<TableBindableModel>>(availableTablesResult.Result);

                Tables = new (tableBindableModels);

                SelectedTable = Tables.First();
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

        private async Task OnRemoveTaxFromOrderCommandAsync()
        {
            string page = App.IsTablet ? nameof(NewOrderView) : nameof(OrderRegistrationPage);
            var parameters = new NavigationParameters { { Constants.Navigations.ADMIN, page } };

            if (App.IsTablet)
            {
                await _navigationService.NavigateAsync(nameof(NumericPage), parameters);
            }
            else
            {
                await _navigationService.NavigateAsync(nameof(Views.Mobile.LoginPage), parameters);
            }

            IsOrderWithTax = false;
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