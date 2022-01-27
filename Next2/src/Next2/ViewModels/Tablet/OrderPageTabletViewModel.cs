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
    public class OrderPageTabletViewModel : BaseViewModel
    {
        private readonly IOrderService _orderService;

        public OrderPageTabletViewModel(
            INavigationService navigationService,
            IOrderService orderService)
            : base(navigationService)
        {
            _orderService = orderService;
        }

        #region -- Public properties --

        private ObservableCollection<OrderViewModel> _orders;

        public ObservableCollection<OrderViewModel> Orders
        {
            get => _orders;
            set => SetProperty(ref _orders, value);
        }

        #endregion

        #region -- Overrides --

        public override async void OnNavigatedTo(INavigationParameters parametrs)
        {
            Orders = new ObservableCollection<OrderViewModel>();

            var result = await _orderService.GetOrdersAsync();
            if (result != null)
            {
                foreach (var r in result)
                {
                    Orders.Add(new OrderViewModel
                    {
                        CustomerName = r.CustomerName,
                        OrderNumber = r.OrderNumber,
                        TableName = r.TableName,
                        Total = r.Total,
                    });
                }
            }
        }

        #endregion
    }
}
