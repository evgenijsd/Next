using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using System;
using Next2.Models.API.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Next2.Services.Order
{
    public interface IOrderService
    {
        FullOrderBindableModel CurrentOrder { get; set; }

        SeatBindableModel? CurrentSeat { get; set; }

        Task<AOResult<TaxModel>> GetTaxAsync();

        Task<AOResult<Guid>> CreateNewOrderAndGetIdAsync();

        Task<AOResult<IEnumerable<TableModelDTO>>> GetFreeTablesAsync();

        Task<AOResult<IEnumerable<OrderModel>>> GetOrdersAsync();

        Task<AOResult> DeleteOrderAsync(int orderId);

        Task<AOResult<IEnumerable<SeatModel>>> GetSeatsAsync(int orderdId);

        string ApplyNumberFilter(string text);

        string ApplyNameFilter(string text);

        Task<AOResult> CreateNewCurrentOrderAsync();

        Task<AOResult> AddSetInCurrentOrderAsync(DishBindableModel dish);

        Task<AOResult> AddSeatInCurrentOrderAsync();

        Task<AOResult> DeleteSeatFromCurrentOrder(SeatBindableModel seat);

        Task<AOResult> RedirectSetsFromSeatInCurrentOrder(SeatBindableModel sourceSeat, int destinationSeatNumber);

        Task<AOResult> DeleteSetFromCurrentSeat();

        Task<AOResult> AddSeatAsync(SeatModel seat);

        Task<AOResult> AddOrderAsync(OrderModel order);
    }
}
