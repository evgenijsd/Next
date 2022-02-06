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

namespace Next2.ViewModels
{
    public class OrderTabsViewModel : BaseViewModel
    {
        private readonly IOrderService _orderService;
        private bool _isDirectionSortNames = true;
        private bool _isDirectionSortOrders = true;
        private List<OrderModel>? _ordersBase;
        private List<OrderModel>? _tabsBase;
        private double _maxHeightButton = 0;
        private double _maxHeightCollection = 0;

        public OrderTabsViewModel(
            INavigationService navigationService,
            IOrderService orderService)
            : base(navigationService)
        {
            Text = "OrderTabs";
            _orderService = orderService;
        }

        #region -- Public properties --

        public double HeightStackLayoutGrid { get; }
        public double HeightStackLayoutButton { get; }

        private GridLength _heightCollectionGrid;
        public GridLength HeightCollectionGrid
        {
            get => _heightCollectionGrid;
            set => SetProperty(ref _heightCollectionGrid, value);
        }

        public string? Text { get; set; }

        public bool IsOrdersRefreshing { get; set; }

        private bool _isSelectedOrders = true;

        public bool IsSelectedOrders
        {
            get => _isSelectedOrders;
            set => SetProperty(ref _isSelectedOrders, value);
        }

        private OrderViewModel? _selectedOrder = null;

        public OrderViewModel? SelectedOrder
        {
            get => _selectedOrder;
            set => SetProperty(ref _selectedOrder, value);
        }

        private ObservableCollection<OrderViewModel> _orders;

        public ObservableCollection<OrderViewModel> Orders
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
            HeightCollectionGrid = new GridLength(500);

            await LoadData();
        }

        public override async void OnAppearing()
        {
            await LoadData();
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            if (args.PropertyName == nameof(HeightStackLayoutGrid))
            {
                if (HeightStackLayoutGrid > _maxHeightCollection)
                {
                    _maxHeightCollection = HeightStackLayoutGrid;
                }
            }

            if (args.PropertyName == nameof(HeightStackLayoutButton))
            {
                if (HeightStackLayoutButton > _maxHeightButton)
                {
                    _maxHeightButton = HeightStackLayoutButton;
                }
            }

            if (args.PropertyName == nameof(SelectedOrder))
            {
                if (SelectedOrder != null)
                {
                    HeightCollectionGrid = new GridLength(300);
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

            var resultOrders = await _orderService.GetOrdersAsync();
            if (resultOrders.IsSuccess)
            {
                _ordersBase = resultOrders.Result;

                var resultTabs = await _orderService.GetOrdersAsync();
                if (resultTabs.IsSuccess)
                {
                    _tabsBase = resultTabs.Result;
                    IsOrdersRefreshing = false;
                }
            }

            await GetVisualCollection();
        }

        private Task GetVisualCollection()
        {
            SelectedOrder = null;
            _isDirectionSortNames = true;
            _isDirectionSortOrders = true;
            Orders = new ObservableCollection<OrderViewModel>();

            List<OrderModel>? result = new List<OrderModel>();

            if (IsSelectedOrders)
            {
                result = _ordersBase;
            }
            else
            {
                result = _tabsBase;
            }

            if (result != null)
            {
                foreach (var r in result)
                {
                    string? name;

                    if (IsSelectedOrders)
                    {
                        name = $"Table {r.TableNumber}";
                    }
                    else
                    {
                        name = r.CustomerName;
                    }

                    Orders.Add(new OrderViewModel
                    {
                        Name = name,
                        OrderNumber = r.OrderNumber,
                        Total = r.Total,
                        TableNumber = r.TableNumber,
                        OrderStatus = r.OrderStatus,
                        OrderType = r.OrderType,
                    });
                }
            }

            return Task.CompletedTask;
        }

        private async Task OnButtonOrdersCommandAsync()
        {
            if (!IsSelectedOrders)
            {
                HeightCollectionGrid = new GridLength(500);
                IsSelectedOrders = !IsSelectedOrders;
                await GetVisualCollection();
            }
        }

        private async Task OnButtonTabsCommandAsync()
        {
            if (IsSelectedOrders)
            {
                HeightCollectionGrid = new GridLength(500);
                IsSelectedOrders = !IsSelectedOrders;
                await GetVisualCollection();
            }
        }

        private async Task OnGoBackCommandAsync()
        {
            await _navigationService.GoBackAsync();
        }

        private Task OnSortByNameCommandAsync()
        {
            if (IsSelectedOrders)
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
