using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Next2.Services.Order
{
    public interface IOrderService
    {
        Task<AOResult<int>> GetNewOrderIdAsync();

        Task<AOResult<IEnumerable<TableModel>>> GetAvailableTables();

        Task<AOResult<IEnumerable<OrderModel>>> GetOrdersAsync();

        string ApplyNumberFilter(string text);

        string ApplyNameFilter(string text);
    }
}
