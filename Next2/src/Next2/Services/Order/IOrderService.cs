using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Next2.Services.Order
{
    public interface IOrderService
    {
        FullOrderBindableModel CurrentOrder { get; set; }

        SeatBindableModel? CurrentSeat { get; set; }

        Task<AOResult<int>> GetNewOrderIdAsync();

        Task<AOResult<IEnumerable<TableModel>>> GetFreeTablesAsync();

        Task<AOResult<IEnumerable<OrderModel>>> GetOrdersAsync();

        Task<AOResult<IEnumerable<SeatModel>>> GetSeatsAsync(int orderdId);

        string ApplyNumberFilter(string text);

        string ApplyNameFilter(string text);

        Task<AOResult> CreateNewOrderAsync();

        Task<AOResult> AddSetInCurrentOrderAsync(SetBindableModel set);

        Task<AOResult> AddSeatInCurrentOrderAsync();

        Task<AOResult> DeleteSeatFromCurrentOrder(SeatBindableModel seat);

        Task<AOResult> RedirectSetsFromSeatInCurrentOrder(SeatBindableModel sourceSeat, int destinationSeatNumber);
    }
}
