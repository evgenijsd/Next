using Next2.Models;
using Next2.Services;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels
{
    public class OrderTabPageTabletViewModel : BaseViewModel
    {
        private readonly IOrderService _orderService;

        public OrderTabPageTabletViewModel(
            INavigationService navigationService,
            IOrderService orderService)
            : base(navigationService)
        {
            _orderService = orderService;
        }

        #region -- Public properties --

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

        private ICommand _SortByNameCommand;

        public ICommand SortByNameCommand => _SortByNameCommand ??= new AsyncCommand(OnSortByNameCommandAsync);

        private ICommand _SortByOrderCommand;

        public ICommand SortByOrderCommand => _SortByOrderCommand ??= new AsyncCommand(OnSortByOrderCommandAsync);

        #endregion

        #region -- Overrides --

        public override async void OnNavigatedTo(INavigationParameters parametrs)
        {
            await GetOrders();
        }

        #endregion

        #region -- Private helpers --

        private async Task GetOrders()
        {
            SelectedOrder = null;
            Orders = new ObservableCollection<OrderViewModel>();

            var result = await _orderService.GetOrdersAsync();
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
            if (IsSelectedTabs)
            {
                IsSelectedOrders = !IsSelectedOrders;
                IsSelectedTabs = !IsSelectedTabs;
                await GetOrders();
            }
        }

        private async Task OnButtonTabsCommandAsync()
        {
            if (IsSelectedOrders)
            {
                IsSelectedOrders = !IsSelectedOrders;
                IsSelectedTabs = !IsSelectedTabs;
                await GetOrders();
            }
        }

        private Task OnSortByNameCommandAsync()
        {
            if (IsSelectedTabs)
            {
                Orders = new ObservableCollection<OrderViewModel>(Orders.OrderBy(x => x.TableNumber));
            }
            else
            {
                Orders = new ObservableCollection<OrderViewModel>(Orders.OrderBy(x => x.Name));
            }

            return Task.CompletedTask;
        }

        private Task OnSortByOrderCommandAsync()
        {
            Orders = new ObservableCollection<OrderViewModel>(Orders.OrderBy(x => x.OrderNumber));

            return Task.CompletedTask;
        }

        #endregion
    }
}
