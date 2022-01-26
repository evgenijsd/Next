using Next2.Models;
using Next2.Services;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Next2.ViewModels.Mobile
{
    public class OrderPageViewModel : BaseViewModel
    {
        private readonly IOrderService _orderService;

        public OrderPageViewModel(
            INavigationService navigationService,
            IOrderService orderService)
            : base(navigationService)
        {
            _orderService = orderService;
        }

        #region -- Public properties --

        private ObservableCollection<OrderModel> _orders;

        public ObservableCollection<OrderModel> Orders
        {
            get => _orders;
            set => SetProperty(ref _orders, value);
        }

        #endregion

        #region -- Overrides --

        public override async void OnNavigatedTo(INavigationParameters parametrs)
        {
            var result = await _orderService.GetOrdersAsync();
            if (result.IsSuccess)
            {
                Orders = new ObservableCollection<OrderModel>(result.Result);
            }
        }

        #endregion
    }
}
