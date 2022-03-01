using AutoMapper;
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

        private int _indexCurrentSeat;

        public OrderService(IMockService mockService)
        {
            _mockService = mockService;
        }

        #region -- Public properties --

        public FullOrderModel CurrentOrder { get; set; }

        #endregion

        #region -- IOrderService implementation --

        public async Task<AOResult<int>> GetNewOrderIdAsync()
        {
            var result = new AOResult<int>();

            try
            {
                var orders = await _mockService.GetAllAsync<OrderModel>();

                if (orders != null)
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

        public async Task<AOResult<IEnumerable<OrderModel>>> GetOrdersAsync()
        {
            var result = new AOResult<IEnumerable<OrderModel>>();

            try
            {
                var orders = await _mockService.GetAsync<OrderModel>(x => x.Id != 0);

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
                var availableTables = await GetAvailableTables();

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
                CurrentOrder.Seats[_indexCurrentSeat].Sets.Add(set);
                CurrentOrder.SubTotal += set.Price;
                CurrentOrder.Total += set.Price;

                result.SetSuccess();
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(AddSetInCurrentOrderAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        #endregion
    }
}
