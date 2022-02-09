using AutoMapper;
using Next2.Enums;
using Next2.Models;
using Next2.Services;
using Prism.Navigation;
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

        private bool _isDirectionSortNames = false;
        private bool _isDirectionSortOrders = true;
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

        public EOrderTabSorting OrderSorting { get; set; }

        private GridLength _heightCollectionGrid;
        public GridLength HeightCollectionGrid
        {
            get => _heightCollectionGrid;
            set => SetProperty(ref _heightCollectionGrid, value);
        }

        public string? Text { get; set; }

        public bool IsOrdersRefreshing { get; set; }

        private bool _isOrderTabsSelected = true;
        public bool IsOrderTabsSelected
        {
            get => _isOrderTabsSelected;
            set => SetProperty(ref _isOrderTabsSelected, value);
        }

        private OrderViewModel? _selectedOrder = null;
        public OrderViewModel? SelectedOrder
        {
            get => _selectedOrder;
            set => SetProperty(ref _selectedOrder, value);
        }

        private ObservableCollection<OrderViewModel>? _orders;
        public ObservableCollection<OrderViewModel>? Orders
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

        private ICommand _SortByNameCommand;
        public ICommand SortByNameCommand => _SortByNameCommand ??= new AsyncCommand(OnSortByNameCommandAsync);

        private ICommand _SortByOrderCommand;
        public ICommand SortByOrderCommand => _SortByOrderCommand ??= new AsyncCommand(OnSortByOrderCommandAsync);

        private ICommand _refreshOrdersCommand;
        public ICommand RefreshOrdersCommand => _refreshOrdersCommand ??= new AsyncCommand(OnRefreshOrdersCommandAsync);

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

        /*private IEnumerable<OrderViewModel> GetSortedMembers(IEnumerable<OrderViewModel> orderTabs)
        {
            Func<OrderViewModel, object> comparer = OrderSorting switch
            {
                EOrderTabSorting.ByTableNumber => x =>
                //EMemberSorting.ByMembershipEndTime => x => x.MembershipEndTime,
                _ => x => x.CustomerName,
            };

            return orderTabs.OrderBy(comparer);
        }*/

        private async Task OnRefreshOrdersCommandAsync()
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            IsOrdersRefreshing = true;

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
            _isDirectionSortNames = false;
            _isDirectionSortOrders = true;
            MapperConfiguration config;

            Orders = new ObservableCollection<OrderViewModel>();

            IEnumerable<OrderModel>? result;

            if (IsOrderTabsSelected)
            {
                config = new MapperConfiguration(cfg => cfg.CreateMap<OrderModel, OrderViewModel>()
                            .ForMember(x => x.Name, s => s.MapFrom(x => $"Table {x.TableNumber}")));
                result = _ordersBase;
            }
            else
            {
                config = new MapperConfiguration(cfg => cfg.CreateMap<OrderModel, OrderViewModel>()
                            .ForMember(x => x.Name, s => s.MapFrom(x => x.CustomerName)));
                result = _tabsBase;
            }

            if (result != null)
            {
                var mapper = new Mapper(config);

                Orders = mapper.Map<IEnumerable<OrderModel>, ObservableCollection<OrderViewModel>>(result);
            }

            return Task.CompletedTask;
        }

        private async Task OnButtonOrdersCommandAsync()
        {
            if (!IsOrderTabsSelected)
            {
                IsOrderTabsSelected = !IsOrderTabsSelected;
                await SetVisualCollection();
            }
        }

        private async Task OnButtonTabsCommandAsync()
        {
            if (IsOrderTabsSelected)
            {
                IsOrderTabsSelected = !IsOrderTabsSelected;
                await SetVisualCollection();
            }
        }

        private async Task OnGoBackCommandAsync()
        {
            await _navigationService.GoBackAsync();
        }

        private Task OnSortByNameCommandAsync()
        {
            if (IsOrderTabsSelected)
            {
                if (_isDirectionSortNames)
                {
                    Orders = new ObservableCollection<OrderViewModel>(Orders.OrderBy(x => x.TableNumber));
                }
                else
                {
                    Orders = new ObservableCollection<OrderViewModel>(Orders.OrderByDescending(x => x.TableNumber));
                }
            }
            else
            {
                if (_isDirectionSortNames)
                {
                    Orders = new ObservableCollection<OrderViewModel>(Orders.OrderBy(x => x.Name));
                }
                else
                {
                    Orders = new ObservableCollection<OrderViewModel>(Orders.OrderByDescending(x => x.Name));
                }
            }

            _isDirectionSortNames = !_isDirectionSortNames;
            _isDirectionSortOrders = true;

            return Task.CompletedTask;
        }

        private Task OnSortByOrderCommandAsync()
        {
            if (_isDirectionSortOrders)
            {
                Orders = new ObservableCollection<OrderViewModel>(Orders.OrderBy(x => x.OrderNumber));
            }
            else
            {
                Orders = new ObservableCollection<OrderViewModel>(Orders.OrderByDescending(x => x.OrderNumber));
            }

            _isDirectionSortOrders = !_isDirectionSortOrders;
            _isDirectionSortNames = true;

            return Task.CompletedTask;
        }

        #endregion
    }
}
