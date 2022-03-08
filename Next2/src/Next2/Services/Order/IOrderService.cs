﻿using Next2.Helpers.ProcessHelpers;
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

        Task<AOResult<IEnumerable<TableModel>>> GetAvailableTablesAsync();

        Task<AOResult<IEnumerable<OrderModel>>> GetOrdersAsync();

        string ApplyNumberFilter(string text);

        string ApplyNameFilter(string text);

        Task<AOResult> CreateNewOrderAsync();

        Task<AOResult> AddSetInCurrentOrderAsync(SetBindableModel set);

        Task<AOResult> AddSeatInCurrentOrderAsync();

        Task<AOResult> DeleteSeatFromCurrentOrder(int seatNumber);

        Task<AOResult> RedirectSetsFromCurrentOrder(int sourceSeatNumber, int destinationSeatNumber);
    }
}
