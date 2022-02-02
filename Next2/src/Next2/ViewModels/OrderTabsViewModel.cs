using Next2.Models;
using Next2.Services;
using Prism.Navigation;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels
{
    public class OrderTabsViewModel : BaseViewModel
    {
        private readonly IOrderService _orderService;
        private bool _isDirectionSortNames = true;
        private bool _isDirectionSortOrders = true;
        private List<OrderModel>? _orders_base;
        private List<OrderModel>? _tabs_base;

        public OrderTabsViewModel(INavigationService navigationService, IOrderService orderService)
            : base(navigationService)
        {
            Text = "OrderTabs";
            _orderService = orderService;
        }

        #region -- Public properties --

        public string? Text { get; set; }

        private bool _isSelectedOrders = true;

        public bool IsSelectedOrders
        {
            get => _isSelectedOrders;
            set => SetProperty(ref _isSelectedOrders, value);
        }

        private bool _isSelectedTabs = false;

        public bool IsSelectedTabs
        {
            get => _isSelectedTabs;
            set => SetProperty(ref _isSelectedTabs, value);
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

        #endregion

        #region -- Private helpers --

        private async Task GetVisualCollection()
        {
            SelectedOrder = null;
            Orders = new ObservableCollection<OrderViewModel>();

            if (_orders_base is null)
            {
                _orders_base = await _orderService.GetOrdersAsync();
            }

            if (_tabs_base is null)
            {
                _tabs_base = await _orderService.GetOrdersAsync();
            }

            var result = new List<OrderModel>();

            if (IsSelectedOrders)
            {
                result = _orders_base;
            }
            else
            {
                result = _tabs_base;
            }

            if (result != null)
            {
                foreach (var r in result)
                {
                    string name;

                    if (IsSelectedOrders)
                    {
                        name = r.CustomerName;
                    }
                    else
                    {
                        name = $"Table {r.TableNumber}";
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
        }

        private async Task OnButtonOrdersCommandAsync()
        {
            if (!IsSelectedOrders)
            {
                IsSelectedOrders = !IsSelectedOrders;
                IsSelectedTabs = !IsSelectedTabs;
                await GetVisualCollection();
            }
        }

        private async Task OnButtonTabsCommandAsync()
        {
            if (IsSelectedOrders)
            {
                IsSelectedOrders = !IsSelectedOrders;
                IsSelectedTabs = !IsSelectedTabs;
                await GetVisualCollection();
            }
        }

        private async Task OnGoBackCommandAsync()
        {
            await _navigationService.GoBackAsync();
        }

        private Task OnSortByNameCommandAsync()
        {
            if (!IsSelectedOrders)
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
