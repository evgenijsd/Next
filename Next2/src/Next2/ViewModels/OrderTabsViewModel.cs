using AutoMapper;
using Next2.Enums;
using Next2.Models;
using Next2.Services;
using Next2.Views.Mobile;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;
using static Next2.Constants;

namespace Next2.ViewModels
{
    public class OrderTabsViewModel : BaseViewModel
    {
        private readonly IOrderService _orderService;

        private IEnumerable<OrderModel>? _ordersBase;
        private IEnumerable<OrderModel>? _tabsBase;
        private double _summRowHight;

        public OrderTabsViewModel(
            INavigationService navigationService,
            IOrderService orderService)
            : base(navigationService)
        {
            _orderService = orderService;
        }

        #region -- Public properties --

        public double HeightPage { get; set; }

        public bool IsOrdersRefreshing { get; set; }

        public EOrderTabSorting OrderTabSorting { get; set; }

        private GridLength _heightCollectionGrid;
        public GridLength HeightCollectionGrid
        {
            get => _heightCollectionGrid;
            set => SetProperty(ref _heightCollectionGrid, value);
        }

        private bool _isOrderTabsSelected = true;
        public bool IsOrderTabsSelected
        {
            get => _isOrderTabsSelected;
            set => SetProperty(ref _isOrderTabsSelected, value);
        }

        private OrderBindableModel? _selectedOrder = null;
        public OrderBindableModel? SelectedOrder
        {
            get => _selectedOrder;
            set => SetProperty(ref _selectedOrder, value);
        }

        private ObservableCollection<OrderBindableModel>? _orders;
        public ObservableCollection<OrderBindableModel>? Orders
        {
            get => _orders;
            set => SetProperty(ref _orders, value);
        }

        private ICommand _ButtonOrdersCommand;
        public ICommand ButtonOrdersCommand => _ButtonOrdersCommand ??= new AsyncCommand(OnButtonOrdersCommandAsync);

        private ICommand _ButtonTabsCommand;
        public ICommand ButtonTabsCommand => _ButtonTabsCommand ??= new AsyncCommand(OnButtonTabsCommandAsync);

        private ICommand _GoBackCommand;
        public ICommand GoBackCommand => _GoBackCommand ??= new AsyncCommand(OnGoBackCommandAsync);

        private ICommand _ButtonSearchCommand;
        public ICommand ButtonSearchCommand => _ButtonSearchCommand ??= new AsyncCommand(OnButtonSearchCommandAsync);

        private ICommand _refreshOrdersCommand;
        public ICommand RefreshOrdersCommand => _refreshOrdersCommand ??= new AsyncCommand(OnRefreshOrdersCommandAsync);

        private ICommand _orderTabSortingChangeCommand;
        public ICommand OrderTabSortingChangeCommand => _orderTabSortingChangeCommand ??= new AsyncCommand<EOrderTabSorting>(OnOrderTabSortingChangeCommandAsync);

        #endregion

        #region -- Overrides --

        public override async void OnNavigatedTo(INavigationParameters parametrs)
        {
            _summRowHight = LayoutOrderTabs.SUMM_ROW_HEIGHT_MOBILE;

            var heightCollectionScreen = HeightPage - _summRowHight;
            HeightCollectionGrid = new GridLength(heightCollectionScreen);

            await LoadData();

            var heightCollection = (Orders.Count + 1) * LayoutOrderTabs.ROW_HEIGHT;
            if (heightCollectionScreen > heightCollection)
            {
                heightCollectionScreen = heightCollection;
            }

            HeightCollectionGrid = new GridLength(heightCollectionScreen);
        }

        public override async void OnAppearing()
        {
            _summRowHight = LayoutOrderTabs.SUMM_ROW_HEIGHT_TABLET;

            var heightCollectionScreen = HeightPage - _summRowHight;
            HeightCollectionGrid = new GridLength(heightCollectionScreen);

            await LoadData();

            var heightCollection = (Orders.Count + 1) * LayoutOrderTabs.ROW_HEIGHT;
            if (heightCollectionScreen > heightCollection)
            {
                heightCollectionScreen = heightCollection;
            }

            HeightCollectionGrid = new GridLength(heightCollectionScreen);
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            if (args.PropertyName == nameof(SelectedOrder))
            {
                var heightCollection = (Orders.Count + 1) * LayoutOrderTabs.ROW_HEIGHT;
                var heightCollectionScreen = HeightPage - _summRowHight;

                if (SelectedOrder != null)
                {
                    heightCollectionScreen = HeightPage - _summRowHight - LayoutOrderTabs.BUTTONS_HEIGHT;
                }

                if (heightCollectionScreen > heightCollection)
                {
                    heightCollectionScreen = heightCollection;
                }

                HeightCollectionGrid = new GridLength(heightCollectionScreen);
            }
        }

        #endregion

        #region -- Private helpers --

        private async Task OnRefreshOrdersCommandAsync()
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            IsOrdersRefreshing = true;
            OrderTabSorting = EOrderTabSorting.ByCustomerName;

            var resultOrders = await _orderService.GetOrdersAsync();
            if (resultOrders.IsSuccess)
            {
                _ordersBase = new List<OrderModel>(resultOrders.Result.OrderBy(x => x.TableNumber));

                var resultTabs = await _orderService.GetOrdersAsync();
                if (resultTabs.IsSuccess)
                {
                    _tabsBase = new List<OrderModel>(resultTabs.Result.OrderBy(x => x.CustomerName));
                }
            }

            IsOrdersRefreshing = false;

            await SetVisualCollection();
        }

        private Task SetVisualCollection()
        {
            SelectedOrder = null;
            MapperConfiguration config;

            Orders = new ObservableCollection<OrderBindableModel>();

            IEnumerable<OrderModel>? result;

            if (IsOrderTabsSelected)
            {
                config = new MapperConfiguration(cfg => cfg.CreateMap<OrderModel, OrderBindableModel>()
                            .ForMember(x => x.Name, s => s.MapFrom(x => $"Table {x.TableNumber}")));
                result = _ordersBase;
            }
            else
            {
                config = new MapperConfiguration(cfg => cfg.CreateMap<OrderModel, OrderBindableModel>()
                            .ForMember(x => x.Name, s => s.MapFrom(x => x.CustomerName)));
                result = _tabsBase;
            }

            if (result != null)
            {
                var mapper = new Mapper(config);

                Orders = mapper.Map<IEnumerable<OrderModel>, ObservableCollection<OrderBindableModel>>(result);
            }

            return Task.CompletedTask;
        }

        private async Task OnButtonOrdersCommandAsync()
        {
            if (!IsOrderTabsSelected)
            {
                IsOrderTabsSelected = !IsOrderTabsSelected;
                OrderTabSorting = EOrderTabSorting.ByCustomerName;
                await SetVisualCollection();
            }
        }

        private async Task OnButtonTabsCommandAsync()
        {
            if (IsOrderTabsSelected)
            {
                IsOrderTabsSelected = !IsOrderTabsSelected;
                OrderTabSorting = EOrderTabSorting.ByCustomerName;
                await SetVisualCollection();
            }
        }

        private async Task OnGoBackCommandAsync()
        {
            await _navigationService.GoBackAsync();
        }

        private async Task OnButtonSearchCommandAsync()
        {
            await _navigationService.NavigateAsync(nameof(SearchPage));
        }

        private IEnumerable<OrderBindableModel> GetSortedMembers(IEnumerable<OrderBindableModel> orders)
        {
            EOrderTabSorting orderTabSorting = OrderTabSorting == EOrderTabSorting.ByCustomerName && IsOrderTabsSelected ? EOrderTabSorting.ByTableNumber : OrderTabSorting;

            Func<OrderBindableModel, object> comparer = orderTabSorting switch
            {
                EOrderTabSorting.ByOrderNumber => x => x.OrderNumber,
                EOrderTabSorting.ByTableNumber => x => x.TableNumber,
                _ => x => x.Name,
            };

            return orders.OrderBy(comparer);
        }

        private Task OnOrderTabSortingChangeCommandAsync(EOrderTabSorting orderTabSorting)
        {
            if (OrderTabSorting == orderTabSorting)
            {
                Orders = new (Orders.Reverse());
            }
            else
            {
                OrderTabSorting = orderTabSorting;

                var sortedOrders = GetSortedMembers(Orders);

                Orders = new (sortedOrders);
            }

            return Task.CompletedTask;
        }

        #endregion
    }
}
