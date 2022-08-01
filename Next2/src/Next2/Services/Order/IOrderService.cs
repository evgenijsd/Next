using Next2.Enums;
using Next2.Helpers.ProcessHelpers;
using Next2.Models.API;
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

        Task<AOResult<IEnumerable<SimpleOrderModelDTO>>> GetOrdersAsync(EOrderStatus? orderStatus = null, Func<SimpleOrderModelDTO, bool>? condition = null);

        Task<AOResult<OrderModelDTO>> GetOrderByIdAsync(Guid orderId);

        Task<AOResult> AddDishInCurrentOrderAsync(DishBindableModel dish);

        Task<AOResult<Guid>> UpdateOrderAsync(OrderModelDTO order);

        Task<AOResult> AddSeatInCurrentOrderAsync();

        Task<AOResult> DeleteSeatFromCurrentOrder(SeatBindableModel seat);

        Task<AOResult> RedirectDishesFromSeatInCurrentOrder(SeatBindableModel sourceSeat, int destinationSeatNumber);

        Task<AOResult> DeleteDishFromCurrentSeatAsync();

        Task<AOResult> OpenLastOrCreateNewOrderAsync();

        Task<AOResult> SetEmptyCurrentOrderAsync();

        Task<AOResult<Guid>> UpdateTableAsync(SimpleTableModelDTO table, bool isAvailable = false);

        Task<AOResult> SetCurrentOrderAsync(Guid orderId);

        Task<AOResult<Guid>> UpdateCurrentOrderAsync();

        Task<AOResult<DishBindableModel>> ChangeDishProportionAsync(ProportionModel selectedProportion, DishBindableModel dish, IEnumerable<IngredientModelDTO> ingredients);

        IEnumerable<SimpleOrderBindableModel> GetSortedOrders(IEnumerable<SimpleOrderBindableModel> orders, EOrdersSortingType sortingType);

        decimal CalculateDishPriceBaseOnProportion(DishBindableModel dish, decimal priceRatio, IEnumerable<IngredientModelDTO> ingredients);

        void UpdateTotalSum(FullOrderBindableModel currentOrder);

        void CalculateOrderPrices(OrderModelDTO order);
    }
}
