using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using Next2.Services.Mock;
using Next2.Services.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Next2.Services
{
    public class NewOrderService : IOrderService
    {
        private IMockService _mockService;

        public NewOrderService(IMockService mockService)
        {
            _mockService = mockService;
        }

        #region -- INewOrderService implementation

        public async Task<AOResult<int>> GetNewOrderIdAsync()
        {
            var result = new AOResult<int>();

            try
            {
                var orders = await _mockService.GetAllAsync<OrderModel>();

                if (orders != null)
                {
                    int newOrderId = orders.LastOrDefault().Id + 1;

                    result.SetSuccess(newOrderId);
                }
                else
                {
                    result.SetFailure();
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(GetNewOrderIdAsync)}: exception", "Some issues", ex);
            }

            return result;
        }

        public async Task<AOResult<IEnumerable<TableModel>>> GetTables()
        {
            var result = new AOResult<IEnumerable<TableModel>>();

            try
            {
                var tables = await _mockService.GetAllAsync<TableModel>();

                if (tables != null)
                {
                    result.SetSuccess(tables);
                }
                else
                {
                    result.SetFailure();
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(GetTables)}: exception", "Some issues", ex);
            }

            return result;
        }

        #endregion
    }
}
