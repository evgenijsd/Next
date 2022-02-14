using AutoMapper;
using Next2.Enums;
using Next2.Models;
using Next2.Services;
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
        private double _offsetHeight;

        public OrderTabsViewModel(
            INavigationService navigationService,
            IOrderService orderService)
            : base(navigationService)
        {
            _orderService = orderService;
            IsOrderTabsSelected = true;
        }

        #region -- Public properties --

        public double HeightPage { get; set; }

        public bool IsOrdersRefreshing { get; set; }

        public EOrderTabSorting CurrentOrderTabSorting { get; set; }

        public GridLength HeightCollectionGrid { get; set; }

        public bool IsOrderTabsSelected { get; set; }

        public OrderBindableModel? SelectedOrder { get; set; }

        public ObservableCollection<OrderBindableModel>? Orders { get; set; }

        private ICommand _ButtonOrderTabsCommand;
        public ICommand ButtonOrderTabsCommand => _ButtonOrderTabsCommand ??= new AsyncCommand<string>(OnButtonOrderTabsCommandAsync);

        private ICommand _GoBackCommand;
        public ICommand GoBackCommand => _GoBackCommand ??= new AsyncCommand(OnGoBackCommandAsync);

        private ICommand _refreshOrdersCommand;
        public ICommand RefreshOrdersCommand => _refreshOrdersCommand ??= new AsyncCommand(OnRefreshOrdersCommandAsync);

        private ICommand _orderTabSortingChangeCommand;
        public ICommand OrderTabSortingChangeCommand => _orderTabSortingChangeCommand ??= new AsyncCommand<EOrderTabSorting>(OnOrderTabSortingChangeCommandAsync);

        #endregion

        #region -- Overrides --

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            _summRowHight = LayoutOrderTabs.SUMM_ROW_HEIGHT_MOBILE;
            _offsetHeight = LayoutOrderTabs.OFFSET_MOBILE;
            HeightCollectionGrid = new GridLength(HeightPage - _summRowHight);

            await LoadData();
        }

        public override async void OnAppearing()
        {
            _summRowHight = LayoutOrderTabs.SUMM_ROW_HEIGHT_TABLET;
            _offsetHeight = LayoutOrderTabs.OFFSET_TABLET;
            HeightCollectionGrid = new GridLength(HeightPage - _summRowHight);

            await LoadData();
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            if (args.PropertyName == nameof(SelectedOrder))
            {
                SetHeightCollection();
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
            CurrentOrderTabSorting = EOrderTabSorting.ByCustomerName;

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
                            .ForMember(x => x.Name, s => s.MapFrom(x => $"Table {x.TableNumber}"))
                            .ForMember(x => x.OrderNumberText, s => s.MapFrom(x => $"{x.OrderNumber}")));
                result = _ordersBase;
            }
            else
            {
                config = new MapperConfiguration(cfg => cfg.CreateMap<OrderModel, OrderBindableModel>()
                            .ForMember(x => x.Name, s => s.MapFrom(x => x.CustomerName))
                            .ForMember(x => x.OrderNumberText, s => s.MapFrom(x => $"{x.OrderNumber}")));
                result = _tabsBase;
            }

            if (result != null)
            {
                var mapper = new Mapper(config);

                Orders = mapper.Map<IEnumerable<OrderModel>, ObservableCollection<OrderBindableModel>>(result);

                SetHeightCollection();
            }

            return Task.CompletedTask;
        }

        private void SetHeightCollection()
        {
            var heightCollectionScreen = HeightPage - _summRowHight;
            if (SelectedOrder != null && Xamarin.Forms.Device.Idiom == TargetIdiom.Phone)
            {
                heightCollectionScreen -= LayoutOrderTabs.BUTTONS_HEIGHT;
            }

            HeightCollectionGrid = new GridLength(heightCollectionScreen);

            if (Orders?.Count != 0)
            {
                var heightCollection = (Orders.Count * LayoutOrderTabs.ROW_HEIGHT) + _offsetHeight;
                if (heightCollectionScreen > heightCollection)
                {
                    heightCollectionScreen = heightCollection;
                }

                HeightCollectionGrid = new GridLength(heightCollectionScreen);
            }
            else
            {
                HeightCollectionGrid = new GridLength(HeightPage - _summRowHight);
            }
        }

        private async Task OnButtonOrderTabsCommandAsync(string orderTabsSelected)
        {
            if (orderTabsSelected == "Tab" && IsOrderTabsSelected)
            {
                IsOrderTabsSelected = !IsOrderTabsSelected;
                CurrentOrderTabSorting = EOrderTabSorting.ByCustomerName;
                await SetVisualCollection();
            }

            if (orderTabsSelected == "Order" && !IsOrderTabsSelected)
            {
                IsOrderTabsSelected = !IsOrderTabsSelected;
                CurrentOrderTabSorting = EOrderTabSorting.ByCustomerName;
                await SetVisualCollection();
            }
        }

        private async Task OnGoBackCommandAsync()
        {
            await _navigationService.GoBackAsync();
        }

        private IEnumerable<OrderBindableModel> GetSortedMembers(IEnumerable<OrderBindableModel> orders)
        {
            EOrderTabSorting orderTabSorting = CurrentOrderTabSorting == EOrderTabSorting.ByCustomerName && IsOrderTabsSelected ? EOrderTabSorting.ByTableNumber : CurrentOrderTabSorting;

            Func<OrderBindableModel, object> comparer = orderTabSorting switch
            {
                EOrderTabSorting.ByOrderNumber => x => x.OrderNumber,
                EOrderTabSorting.ByTableNumber => x => x.TableNumber,
                _ => x => x.Name,
            };

            return orders.OrderBy(comparer);
        }

        private Task OnOrderTabSortingChangeCommandAsync(EOrderTabSorting newOrderTabSorting)
        {
            if (CurrentOrderTabSorting == newOrderTabSorting)
            {
                Orders = new (Orders.Reverse());
            }
            else
            {
                CurrentOrderTabSorting = newOrderTabSorting;

                var sortedOrders = GetSortedMembers(Orders);

                Orders = new (sortedOrders);
            }

            return Task.CompletedTask;
        }

        #endregion
    }
}
