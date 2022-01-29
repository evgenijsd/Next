using Next2.Models;
using Next2.Services;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

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

        private ObservableCollection<OrderMobileViewModel> _orders;

        public ObservableCollection<OrderMobileViewModel> Orders
        {
            get => _orders;
            set => SetProperty(ref _orders, value);
        }

        #endregion

        #region -- Overrides --

        public override async void OnNavigatedTo(INavigationParameters parametrs)
        {
            Orders = new ObservableCollection<OrderMobileViewModel>();

            List<OrderModel> result = await _orderService.GetOrdersAsync();
            if (result != null)
            {
                foreach (var r in result)
                {
                    string name;

                    name = r.CustomerName;

                    Orders.Add(new OrderMobileViewModel
                    {
                        Name = name,
                        OrderNumber = r.OrderNumber,
                        Total = r.Total,
                    });
                }
            }
        }

        #endregion
    }
}
