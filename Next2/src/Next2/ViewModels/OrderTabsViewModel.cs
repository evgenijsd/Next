using AutoMapper;
using Next2.Enums;
using Next2.Helpers;
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
using MobileViews = Next2.Views.Mobile;
using TabletViews = Next2.Views.Tablet;

namespace Next2.ViewModels
{
    public class OrderTabsViewModel : BaseViewModel
    {
        private readonly IOrderService _orderService;

        private IEnumerable<OrderModel>? _ordersBase;
        private IEnumerable<OrderModel>? _tabsBase;
        private double _summRowHight;
        private double _offcetHeight;
        private string _placeholder;

        public OrderTabsViewModel(
            INavigationService navigationService,
            IOrderService orderService)
            : base(navigationService)
        {
            _orderService = orderService;
            _placeholder = _searchLine;
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

        private string _searchLine = Resources.Strings.Strings.SearchTableNumber;
        public string SearchLine
        {
            get => _searchLine;
            set => SetProperty(ref _searchLine, value);
        }

        private bool _isSearch = false;
        public bool IsSearch
        {
            get => _isSearch;
            set => SetProperty(ref _isSearch, value);
        }

        private bool _isNotFound = false;
        public bool IsNotFound
        {
            get => _isNotFound;
            set => SetProperty(ref _isNotFound, value);
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

        private ICommand _ClearSearchCommand;
        public ICommand ClearSearchCommand => _ClearSearchCommand ??= new AsyncCommand(OnClearSearchCommandAsync);

        private ICommand _refreshOrdersCommand;
        public ICommand RefreshOrdersCommand => _refreshOrdersCommand ??= new AsyncCommand(OnRefreshOrdersCommandAsync);

        private ICommand _orderTabSortingChangeCommand;
        public ICommand OrderTabSortingChangeCommand => _orderTabSortingChangeCommand ??= new AsyncCommand<EOrderTabSorting>(OnOrderTabSortingChangeCommandAsync);

        private ICommand _tapSelectCommand;
        public ICommand TapSelectCommand => _tapSelectCommand ??= new AsyncCommand<object>(OnTapSelectCommandAsync);

        #endregion

        #region -- Overrides --

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            if (!parameters.TryGetValue(Constants.Navigations.SEARCH, out string searchLine))
            {
                _summRowHight = LayoutOrderTabs.SUMM_ROW_HEIGHT_MOBILE;
                _offcetHeight = LayoutOrderTabs.OFFCET_MOBILE;
                HeightCollectionGrid = new GridLength(HeightPage - _summRowHight);

                await LoadData();
            }
        }

        public override async void OnAppearing()
        {
            _summRowHight = LayoutOrderTabs.SUMM_ROW_HEIGHT_TABLET;
            _offcetHeight = LayoutOrderTabs.OFFCET_TABLET;
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

            if (args.PropertyName == nameof(Orders))
            {
                if (Orders?.Count == 0)
                {
                    HeightCollectionGrid = new GridLength(HeightPage - _summRowHight);
                    IsNotFound = true;
                }
                else
                {
                    IsNotFound = false;
                }
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

        private Task SetHeightCollection()
        {
            var heightCollectionScreen = HeightPage - _summRowHight;
            if (SelectedOrder != null && Xamarin.Forms.Device.Idiom == TargetIdiom.Phone)
            {
                heightCollectionScreen -= LayoutOrderTabs.BUTTONS_HEIGHT;
            }

            HeightCollectionGrid = new GridLength(heightCollectionScreen);

            if (Orders?.Count != 0)
            {
                var heightCollection = (Orders.Count * LayoutOrderTabs.ROW_HEIGHT) + _offcetHeight;
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

            return Task.CompletedTask;
        }

        private async Task OnButtonOrdersCommandAsync()
        {
            if (!IsOrderTabsSelected)
            {
                IsOrderTabsSelected = !IsOrderTabsSelected;
                OrderTabSorting = EOrderTabSorting.ByCustomerName;
                SearchLine = Resources.Strings.Strings.SearchTableNumber;
                _placeholder = SearchLine;
                IsSearch = false;
                await SetVisualCollection();
            }
        }

        private async Task OnButtonTabsCommandAsync()
        {
            if (IsOrderTabsSelected)
            {
                IsOrderTabsSelected = !IsOrderTabsSelected;
                OrderTabSorting = EOrderTabSorting.ByCustomerName;
                SearchLine = Resources.Strings.Strings.SearchName;
                _placeholder = SearchLine;
                IsSearch = false;
                await SetVisualCollection();
            }
        }

        private async Task OnGoBackCommandAsync()
        {
            await _navigationService.GoBackAsync();
        }

        private async Task OnButtonSearchCommandAsync()
        {
            MessagingCenter.Subscribe<MessageEvent>(this, MessageEvent.SearchMessage, (me) => SearchCommandAsync(me));

            await ClearSearchAsync();

            string searchPage = nameof(TabletViews.SearchPage);
            if (Xamarin.Forms.Device.Idiom == TargetIdiom.Phone)
            {
                searchPage = nameof(MobileViews.SearchPage);
            }

            var parametrs = new NavigationParameters { { Constants.Navigations.SEARCH, IsOrderTabsSelected } };
            await _navigationService.NavigateAsync(searchPage, parametrs);
        }

        private Task SearchCommandAsync(MessageEvent me)
        {
            SearchLine = me.SearchLine;
            IsSearch = true;
            Orders = new (Orders.Where(x => x.OrderNumberText.Contains(SearchLine) || x.Name.Contains(SearchLine)));
            SelectedOrder = null;

            MessagingCenter.Unsubscribe<MessageEvent>(this, MessageEvent.SearchMessage);

            SetHeightCollection();

            return Task.CompletedTask;
        }

        private async Task OnClearSearchCommandAsync()
        {
            if (IsSearch)
            {
                await ClearSearchAsync();
            }
            else
            {
                await OnButtonSearchCommandAsync();
            }
        }

        private async Task ClearSearchAsync()
        {
            OrderTabSorting = EOrderTabSorting.ByCustomerName;
            SearchLine = _placeholder;
            SelectedOrder = null;
            IsSearch = false;
            await SetVisualCollection();
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

        private Task OnTapSelectCommandAsync(object args)
        {
            OrderBindableModel? order = args as OrderBindableModel;
            if (order == SelectedOrder)
            {
                SelectedOrder = null;
            }
            else
            {
                SelectedOrder = order;
            }

            return Task.CompletedTask;
        }

        #endregion
    }
}
