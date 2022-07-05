using Next2.Helpers.ProcessHelpers;
using Next2.Models.API.Commands;
using Next2.Models.API.DTO;
using Next2.Models.Bindables;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Next2.Services.Order
{
    public interface IOrderService
    {
        FullOrderBindableModel CurrentOrder { get; set; }

        SeatBindableModel? CurrentSeat { get; set; }

        Task<AOResult<OrderModelDTO>> CreateNewOrderAsync();

        Task<AOResult<IEnumerable<TableModelDTO>>> GetFreeTablesAsync();

        Task<AOResult<IEnumerable<SimpleOrderModelDTO>>> GetOrdersAsync();

        Task<AOResult<OrderModelDTO>> GetOrderByIdAsync(Guid orderId);

        string ApplyNumberFilter(string text);

        string ApplyNameFilter(string text);

        Task<AOResult> AddDishInCurrentOrderAsync(DishBindableModel dish);

        Task<AOResult<Guid>> UpdateOrderAsync(UpdateOrderCommand order);

        Task<AOResult> AddSeatInCurrentOrderAsync();

        Task<AOResult> DeleteSeatFromCurrentOrder(SeatBindableModel seat);

        Task<AOResult> RedirectSetsFromSeatInCurrentOrder(SeatBindableModel sourceSeat, int destinationSeatNumber);

        Task<AOResult> DeleteDishFromCurrentSeatAsync();

        Task<AOResult> OpenLastOrCreateNewOrderAsync();

        Task<AOResult> SetEmptyCurrentOrderAsync();

        Task<AOResult<Guid>> UpdateTableAsync(UpdateTableCommand command);

        Task<AOResult> SetCurrentOrderAsync(Guid orderId);

        Task<AOResult<Guid>> UpdateCurrentOrderAsync();

        void UpdateTotalSum(FullOrderBindableModel currentOrder);
    }
}
