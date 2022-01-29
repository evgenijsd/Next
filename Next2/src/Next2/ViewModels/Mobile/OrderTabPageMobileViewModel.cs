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
    public class OrderTabPageMobileViewModel : BaseViewModel
    {
        private readonly IOrderService _orderService;

        public OrderTabPageMobileViewModel(
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

        private ObservableCollection<OrderMobileViewModel> _orders;

        public ObservableCollection<OrderMobileViewModel> Orders
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
            Orders = new ObservableCollection<OrderMobileViewModel>();

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
                        name = r.TableName;
                    }

                    Orders.Add(new OrderMobileViewModel
                    {
                        Name = name,
                        OrderNumber = r.OrderNumber,
                        Total = r.Total,
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

        private async Task OnGoBackCommandAsync()
        {
            await _navigationService.GoBackAsync();
        }

        #endregion
    }
}
