using AutoMapper;
using Next2.Enums;
using Next2.Helpers;
using Next2.Models;
using Next2.Services.OrderService;
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

namespace Next2.ViewModels
{
    public class OrderTabsViewModel : BaseViewModel
    {
        private readonly double _summRowHeight = App.IsTablet ? Constants.LayoutOrderTabs.SUMM_ROW_HEIGHT_TABLET : Constants.LayoutOrderTabs.SUMM_ROW_HEIGHT_MOBILE;
        private readonly double _offsetHeight = App.IsTablet ? Constants.LayoutOrderTabs.OFFSET_TABLET : Constants.LayoutOrderTabs.OFFSET_MOBILE;
        private readonly IOrderService _orderService;

        private IEnumerable<OrderModel>? _ordersBase;
        private IEnumerable<OrderModel>? _tabsBase;
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

        public EOrderTabSorting CurrentOrderTabSorting { get; set; }

        public GridLength HeightCollectionGrid { get; set; }

        public string SearchLine { get; set; } = Resources.Strings.Strings.SearchTableNumber;
        public bool IsSearch { get; set; } = false;

        public bool IsNotFound { get; set; } = false;

        public bool IsOrderTabsSelected { get; set; } = true;

        public OrderBindableModel? SelectedOrder { get; set; }

        public ObservableCollection<OrderBindableModel> Orders { get; set; } = new ();

        private ICommand _SelectOrdersCommand;
        public ICommand SelectOrdersCommand => _SelectOrdersCommand ??= new AsyncCommand(OnSelectOrdersCommandAsync);

        private ICommand _SelectTabsCommand;
        public ICommand SelectTabsCommand => _SelectTabsCommand ??= new AsyncCommand(OnSelectTabsCommandAsync);

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
            HeightCollectionGrid = new GridLength(HeightPage - _summRowHeight);

            await LoadData();
        }

        public override async void OnAppearing()
        {
            HeightCollectionGrid = new GridLength(HeightPage - _summRowHeight);

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
                if (Orders.Count == 0)
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

        private Task OnRefreshOrdersCommandAsync()
        {
            return LoadData();
        }

        private async Task LoadData()
        {
            IsOrdersRefreshing = true;

            CurrentOrderTabSorting = EOrderTabSorting.ByCustomerName;

            var resultOrders = await _orderService.GetOrdersAsync();

            if (resultOrders.IsSuccess)
            {
                _ordersBase = new List<OrderModel>(resultOrders.Result.OrderBy(x => x.TableNumber));
            }

            var resultTabs = await _orderService.GetOrdersAsync();

            if (resultTabs.IsSuccess)
            {
                _tabsBase = new List<OrderModel>(resultTabs.Result.OrderBy(x => x.CustomerName));
            }

            IsOrdersRefreshing = false;

            SetVisualCollection();
        }

        private void SetVisualCollection()
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
        }

        private void SetHeightCollection()
        {
            var heightCollectionScreen = HeightPage - _summRowHeight;

            if (SelectedOrder != null && !App.IsTablet)
            {
                heightCollectionScreen -= Constants.LayoutOrderTabs.BUTTONS_HEIGHT;
            }

            HeightCollectionGrid = new GridLength(heightCollectionScreen);

            if (Orders.Count != 0)
            {
                var heightCollection = (Orders.Count * Constants.LayoutOrderTabs.ROW_HEIGHT) + _offsetHeight;

                if (heightCollectionScreen > heightCollection)
                {
                    heightCollectionScreen = heightCollection;
                }

                HeightCollectionGrid = new GridLength(heightCollectionScreen);
            }
            else
            {
                HeightCollectionGrid = new GridLength(HeightPage - _summRowHeight);
            }
        }

        private Task OnSelectOrdersCommandAsync()
        {
            if (!IsOrderTabsSelected)
            {
                IsOrderTabsSelected = !IsOrderTabsSelected;
                CurrentOrderTabSorting = EOrderTabSorting.ByCustomerName;
                
                SearchLine = Resources.Strings.Strings.SearchTableNumber;
                _placeholder = SearchLine;
                IsSearch = false;

                SetVisualCollection();
            }

            return Task.CompletedTask;
        }

        private Task OnSelectTabsCommandAsync()
        {
            if (IsOrderTabsSelected)
            {
                IsOrderTabsSelected = !IsOrderTabsSelected;
                CurrentOrderTabSorting = EOrderTabSorting.ByCustomerName;
                
                SearchLine = Resources.Strings.Strings.SearchName;
                _placeholder = SearchLine;
                IsSearch = false;

                SetVisualCollection();
            }

            return Task.CompletedTask;
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
            EOrderTabSorting orderTabSorting = CurrentOrderTabSorting == EOrderTabSorting.ByCustomerName && IsOrderTabsSelected ? EOrderTabSorting.ByTableNumber : CurrentOrderTabSorting;

            Func<OrderBindableModel, object> comparer = orderTabSorting switch
            {
                EOrderTabSorting.ByOrderNumber => x => x.OrderNumber,
                EOrderTabSorting.ByTableNumber => x => x.TableNumber,
                EOrderTabSorting.ByCustomerName => x => x.Name,
                _ => throw new NotImplementedException(),
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
