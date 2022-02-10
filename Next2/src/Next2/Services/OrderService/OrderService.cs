using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using Next2.Resources.Strings;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<AOResult<IEnumerable<OrderModel>>> GetOrdersAsync()
        {
            var result = new AOResult<IEnumerable<OrderModel>>();

            try
            {
                var orders = await _mockService.GetAllAsync<OrderModel>(); // GetAsync<OrderModel>(x => x.Id != 0);

                if (orders != null)
                {
                    result.SetSuccess(orders);
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

            return result;
        }

        #endregion
    }
}
