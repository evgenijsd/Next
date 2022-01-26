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

        public Task<AOResult<List<OrderModel>>>? GetOrdersAsync()
        {
            return AOResult.ExecuteTaskAsync<List<OrderModel>>(async _ =>
            {
                var result = new List<OrderModel>();
                var orders = await _mockService.GetAllAsync<OrderModel>();
                result.AddRange(orders);

                return result;
            });

            /*)
            var result = new AOResult<List<OrderModel>>();

            try
            {
                var orders = await _mockService.GetAllAsync<OrderModel>();

                if (orders != null)
                {
                    result.SetSuccess((List<OrderModel>)orders);
                }
                else
                {
                    result.SetFailure(Strings.NotFoundOrders);
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(GetOrdersAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;*/
        }

        #endregion
    }
}
