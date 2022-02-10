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
    public class OrderService : IOrderService
    {
        private IMockService _mockService;

        public OrderService(IMockService mockService)
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

        public async Task<AOResult<IEnumerable<TableModel>>> GetAvailableTables()
        {
            var result = new AOResult<IEnumerable<TableModel>>();

            try
            {
                var allTables = await _mockService.GetAllAsync<TableModel>();

                if (allTables != null)
                {
                    var availableTables = allTables.Where(x => x.NumberOfAvailableSeats > 0);

                    if (availableTables.Count() > 0)
                    {
                        result.SetSuccess(availableTables);
                    }
                }

                if (!result.IsSuccess)
                {
                    result.SetFailure();
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(GetAvailableTables)}: exception", "Some issues", ex);
            }

            return result;
        }

        #endregion
    }
}
