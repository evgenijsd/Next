﻿using AutoMapper;
using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using Next2.Resources.Strings;
using Next2.Services.Mock;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Next2.Services.Order
{
    public class OrderService : IOrderService
    {
        private readonly IMockService _mockService;

        public OrderService(IMockService mockService)
        {
            _mockService = mockService;

            Task.Run(CreateNewOrderAsync);
        }

        #region -- Public properties --

        public FullOrderBindableModel CurrentOrder { get; set; }

        public SeatBindableModel? CurrentSeat { get; set; }

        #endregion

        #region -- IOrderService implementation --

        public async Task<AOResult<int>> GetNewOrderIdAsync()
        {
            var result = new AOResult<int>();

            try
            {
                var orders = await _mockService.GetAllAsync<OrderModel>();

                if (orders is not null)
                {
                    int newOrderId = orders.Max(row => row.Id) + 1;

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

        public async Task<AOResult<IEnumerable<TableModel>>> GetAvailableTablesAsync()
        {
            var result = new AOResult<IEnumerable<TableModel>>();

            try
            {
                var allTables = await _mockService.GetAllAsync<TableModel>();

                if (allTables is not null)
                {
                    var allOrders = await _mockService.GetAllAsync<OrderModel>();

                    var freeTables = allTables?.Where(table => allOrders.All(order => order.TableNumber != table.TableNumber));

                    if (freeTables is not null)
                    {
                        result.SetSuccess(freeTables);
                    }
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(GetAvailableTablesAsync)}: exception", "Some issues", ex);
            }

            return result;
        }

        public async Task<AOResult<IEnumerable<OrderModel>>> GetOrdersAsync()
        {
            var result = new AOResult<IEnumerable<OrderModel>>();

            try
            {
                var orders = await _mockService.GetAsync<OrderModel>(x => x.Id != 0);

                if (orders is not null)
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

        public string ApplyNumberFilter(string text)
        {
            Regex regexNumber = new(Constants.Validators.NUMBER);

            return regexNumber.Replace(text, string.Empty);
        }

        public string ApplyNameFilter(string text)
        {
            Regex regexName = new(Constants.Validators.NAME);
            Regex regexNumber = new(Constants.Validators.NUMBER);
            Regex regexText = new(Constants.Validators.TEXT);

            var result = regexText.Replace(text, string.Empty);
            result = Regex.IsMatch(result, Constants.Validators.CHECK_NUMBER) ? regexNumber.Replace(result, string.Empty) : regexName.Replace(result, string.Empty);

            return result;
        }

        public async Task<AOResult> CreateNewOrderAsync()
        {
            var result = new AOResult();

            try
            {
                var orderId = await GetNewOrderIdAsync();
                var availableTables = await GetAvailableTablesAsync();

                if (orderId.IsSuccess && availableTables.IsSuccess)
                {
                    MapperConfiguration mapperConfig = new(cfg => cfg.CreateMap<TableModel, TableBindableModel>());
                    Mapper mapper = new(mapperConfig);
                    var tableBindableModels = mapper.Map<IEnumerable<TableModel>, ObservableCollection<TableBindableModel>>(availableTables.Result);

                    CurrentOrder = new();
                    CurrentOrder.Seats = new();

                    CurrentOrder.Id = orderId.Result;
                    CurrentOrder.OrderNumber = orderId.Result;
                    CurrentOrder.OrderStatus = "Open";
                    CurrentOrder.OrderType = Enums.EOrderType.DineIn;
                    CurrentOrder.Table = tableBindableModels.FirstOrDefault();

                    CurrentSeat = null;

                    result.SetSuccess();
                }
                else
                {
                    result.SetFailure();
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(CreateNewOrderAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public async Task<AOResult> AddSetInCurrentOrderAsync(SetBindableModel set)
        {
            var result = new AOResult();

            try
            {
                if (CurrentSeat is null)
                {
                    var seat = new SeatBindableModel();
                    seat.Id = 1;
                    seat.SeatNumber = 1;
                    seat.Sets = new();
                    seat.Checked = true;
                    seat.IsFirstSeat = true;

                    CurrentOrder.Seats.Add(seat);

                    CurrentSeat = seat;
                }

                CurrentOrder.Seats[CurrentOrder.Seats.IndexOf(CurrentSeat)].Sets.Add(set);
                CurrentOrder.SubTotal += set.Portion.Price;
                CurrentOrder.Total += set.Portion.Price;

                result.SetSuccess();
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(AddSetInCurrentOrderAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public async Task<AOResult> AddSeatInCurrentOrderAsync()
        {
            var result = new AOResult();

            try
            {
                int newSeatIndex;
                int newSeatNumber;
                bool isSeatsOrderBroken;

                GetIndexAndNewSeatNumber(out newSeatIndex, out newSeatNumber, out isSeatsOrderBroken);

                var seat = new SeatBindableModel
                {
                    SeatNumber = newSeatNumber,
                    Sets = new (),
                    Checked = true,
                };

                foreach (var item in CurrentOrder.Seats)
                {
                    item.Checked = false;
                }

                if (isSeatsOrderBroken)
                {
                    CurrentOrder.Seats.Insert(newSeatIndex, seat);
                }
                else
                {
                    CurrentOrder.Seats.Add(seat);
                }

                CurrentSeat = CurrentOrder.Seats.LastOrDefault();

                result.SetSuccess();
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(AddSetInCurrentOrderAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public async Task<AOResult> DeleteSeatFromCurrentOrder(int seatNumber)
        {
            var result = new AOResult();

            try
            {
                var seat = CurrentOrder?.Seats?.FirstOrDefault(x => x.SeatNumber == seatNumber);

                if (seat is not null)
                {
                    bool isDeleted = CurrentOrder.Seats.Remove(seat);

                    if (isDeleted)
                    {
                        if (seat.Checked)
                        {
                            CurrentSeat = null;
                        }

                        result.SetSuccess();
                    }
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(DeleteSeatFromCurrentOrder)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public async Task<AOResult> RedirectSetsFromCurrentOrder(int sourceSeatNumber, int destinationSeatNumber)
        {
            var result = new AOResult();

            try
            {
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(RedirectSetsFromCurrentOrder)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        #endregion

        #region Private helpers --

        private void GetIndexAndNewSeatNumber(out int newSeatIndex, out int newSeatNumber, out bool isSeatsOrderBroken)
        {
            var seats = CurrentOrder.Seats;

            newSeatIndex = 0;
            newSeatNumber = seats.Count + 1;
            isSeatsOrderBroken = false;

            if (seats.FirstOrDefault().SeatNumber == 1)
            {
                for (int i = 1; !isSeatsOrderBroken && i < seats.Count; i++)
                {
                    isSeatsOrderBroken = seats[i].SeatNumber != seats[i - 1].SeatNumber + 1;

                    if (isSeatsOrderBroken)
                    {
                        newSeatIndex = i;
                        newSeatNumber = seats[i - 1].SeatNumber + 1;
                    }
                }
            }
        }

        #endregion
    }
}
