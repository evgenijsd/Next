using Next2.Models;
using Next2.Services;
using Next2.Views.Mobile;
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
        private bool _isDirectionSortNames = false;
        private bool _isDirectionSortOrders = true;
        private List<OrderModel>? _ordersBase;
        private List<OrderModel>? _tabsBase;

        public OrderTabsViewModel(
            INavigationService navigationService,
            IOrderService orderService)
            : base(navigationService)
        {
            Text = "OrderTabs";
            _orderService = orderService;
        }

        #region -- Public properties --

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

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.TryGetValue(Constants.Navigation.ORDERS, out bool isOrders))
            {
                var parametersSelected = new NavigationParameters { { Constants.Navigation.SELECTED, isOrders } };
                await _navigationService.GoBackAsync(parametersSelected);
            }
            else
            {
                if (parameters.TryGetValue(Constants.Navigation.TABS, out string isTabs))
                {
                    var parameterSelected = new NavigationParameters { { Constants.Navigation.SELECTED, isTabs } };
                    await _navigationService.GoBackAsync(parameterSelected);
                }
                else
                {
                    if (parameters.TryGetValue(Constants.Navigation.SELECTED, out bool parametersSelected))
                    {
                        IsOrdersRefreshing = false;
                        IsSelectedOrders = parametersSelected;
                        await GetVisualCollection();
                    }
                    else
                    {
                        await LoadData();
                    }
                }
            }
        }

        public override async void OnAppearing()
        {
            base.OnAppearing();

            await LoadData();
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
                _ordersBase = new List<OrderModel>(resultOrders.Result.OrderBy(x => x.TableNumber));

                var resultTabs = await _orderService.GetOrdersAsync();
                if (resultTabs.IsSuccess)
                {
                    _tabsBase = new List<OrderModel>(resultTabs.Result.OrderBy(x => x.CustomerName));
                    IsOrdersRefreshing = false;
                }
            }

            await GetVisualCollection();
        }

        private Task GetVisualCollection()
        {
            SelectedOrder = null;
            _isDirectionSortNames = false;
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
                var parameters = new NavigationParameters { { Constants.Navigation.ORDERS, !IsSelectedOrders } };
                await _navigationService.NavigateAsync(nameof(OrderTabsPage), parameters);
            }
        }

        private async Task OnButtonTabsCommandAsync()
        {
            if (IsSelectedOrders)
            {
                var parameters = new NavigationParameters { { Constants.Navigation.TABS, !IsSelectedOrders } };
                await _navigationService.NavigateAsync(nameof(OrderTabsPage), parameters);
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
