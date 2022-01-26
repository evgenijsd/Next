using Next2.Helpers;
using Next2.Models;
using Next2.Resources.Strings;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Next2.Services
{
    public class OrderService : IOrderService
    {
        private readonly IMockService _mockService;

        public OrderService(IMockService mockService)
        {
            _mockService = mockService;
        }

        #region -- Interface implementation --

        public async Task<List<OrderModel>> GetOrdersAsync()
        {
            var result = await _mockService.GetAllAsync<OrderModel>();

            return (List<OrderModel>)result;
        }

        #endregion
    }
}
