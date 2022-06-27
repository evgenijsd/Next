﻿using AutoMapper;
using Newtonsoft.Json;
using Next2.Enums;
using Next2.Extensions;
using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using Next2.Models.API.Commands;
using Next2.Models.API.DTO;
using Next2.Models.API.Results;
using Next2.Models.Bindables;
using Next2.Resources.Strings;
using Next2.Services.Authentication;
using Next2.Services.Bonuses;
using Next2.Services.Menu;
using Next2.Services.Mock;
using Next2.Services.Rest;
using Next2.Services.SettingsService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Next2.Services.Order
{
    public class OrderService : IOrderService
    {
        private readonly IMockService _mockService;
        private readonly ISettingsManager _settingsManager;
        private readonly IRestService _restService;
        private readonly IBonusesService _bonusesService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IMapper _mapper;

        public OrderService(
            IMockService mockService,
            IRestService restService,
            IBonusesService bonusesService,
            ISettingsManager settingsManager,
            IAuthenticationService authenticationService,
            IMapper mapper)
        {
            _mockService = mockService;
            _settingsManager = settingsManager;
            _restService = restService;
            _bonusesService = bonusesService;
            _mapper = mapper;
            _restService = restService;
            _authenticationService = authenticationService;

            CurrentOrder.Seats = new ();
        }

        #region -- Public properties --

        public FullOrderBindableModel CurrentOrder { get; set; } = new();

        public SeatBindableModel? CurrentSeat { get; set; }

        #endregion

        #region -- IOrderService implementation --

        public async Task<AOResult<TaxModel>> GetTaxAsync()
        {
            var result = new AOResult<TaxModel>();

            try
            {
                var taxMock = await _mockService.FindAsync<TaxModel>(x => x.Id == 1);
                var tax = new TaxModel() { Id = taxMock.Id, Name = taxMock.Name, Value = taxMock.Value };

                if (tax is not null)
                {
                    result.SetSuccess(tax);
                }
                else
                {
                    result.SetFailure();
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(GetTaxAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public async Task<AOResult<OrderModelDTO>> CreateNewOrderAsync()
        {
            var result = new AOResult<OrderModelDTO>();

            try
            {
                var employeeId = _settingsManager.UserId.ToString();

                var requestBody = new CreateOrderCommand
                {
                    EmployeeId = employeeId,
                };

                var query = $"{Constants.API.HOST_URL}/api/orders";
                var creationResult = await _restService.RequestAsync<GenericExecutionResult<OrderModelResult>>(HttpMethod.Post, query, requestBody);

                var orderResult = creationResult.Value;

                if (orderResult is not null)
                {
                    var order = new OrderModelDTO()
                    {
                        Id = orderResult.Id,
                        Number = orderResult.OrderNumber,
                        Open = orderResult.Open,
                        TaxCoefficient = orderResult.TaxCoefficient,
                        OrderType = EOrderType.DineIn.ToString(),
                        OrderStatus = EOrderStatus.Pending,
                        EmployeeId = employeeId,
                        Table = null,
                        Coupon = null,
                        Discount = null,
                        Customer = null,
                    };

                    result.SetSuccess(order);
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(CreateNewOrderAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public async Task<AOResult<IEnumerable<TableModelDTO>>> GetFreeTablesAsync()
        {
            var result = new AOResult<IEnumerable<TableModelDTO>>();

            try
            {
                var query = $"{Constants.API.HOST_URL}/api/tables/available";
                var freeTables = await _restService.RequestAsync<GenericExecutionResult<GetAvailableTablesListQueryResult>>(HttpMethod.Get, query);

                if (freeTables.Success)
                {
                    if (freeTables?.Value?.Tables is not null)
                    {
                        result.SetSuccess(freeTables.Value.Tables);
                    }
                }
                else
                {
                    result.SetFailure();
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(GetFreeTablesAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public async Task<AOResult<IEnumerable<SimpleOrderModelDTO>>> GetOrdersAsync()
        {
            var result = new AOResult<IEnumerable<SimpleOrderModelDTO>>();

            try
            {
                string query = $"{Constants.API.HOST_URL}/api/orders";
                var responce = await _restService.RequestAsync<GenericExecutionResult<GetOrdersListQueryResult>>(HttpMethod.Get, query);

                if (responce.Success && responce.Value?.Orders is not null)
                {
                    result.SetSuccess(responce.Value.Orders);
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(GetOrdersAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public async Task<AOResult<OrderModelDTO>> GetOrderByIdAsync(Guid orderId)
        {
            var result = new AOResult<OrderModelDTO>();

            try
            {
                string query = $"{Constants.API.HOST_URL}/api/orders/{orderId}";
                var responce = await _restService.RequestAsync<GenericExecutionResult<GetOrderByIdQueryResult>>(HttpMethod.Get, query);

                if (responce.Success && responce.Value?.Order is not null)
                {
                    result.SetSuccess(responce.Value.Order);
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(GetOrderByIdAsync)}: exception", Strings.SomeIssues, ex);
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

        public async Task<AOResult> SetEmptyCurrentOrderAsync()
        {
            var result = new AOResult();

            try
            {
                var setCurrentOrder = false;
                var employeeId = _authenticationService.AuthorizedUserId.ToString();

                var lastOrderId = await GetIdLastCreatedOrderFromSettingsAsync(employeeId);

                if (lastOrderId.IsSuccess && lastOrderId.Result != Guid.Empty)
                {
                    var orderResult = await GetOrderByIdAsync(lastOrderId.Result);

                    if (orderResult.IsSuccess)
                    {
                        var seats = orderResult.Result.Seats;

                        if (seats?.Count() == 0)
                        {
                            await SetCurrentOrderAsync(orderResult.Result);

                            setCurrentOrder = true;
                        }
                    }
                }

                if (!setCurrentOrder)
                {
                    var orderCreationResult = await CreateNewOrderAsync();

                    if (orderCreationResult.IsSuccess)
                    {
                        await SaveLastOrderIdToSettingsAsync(employeeId, orderCreationResult.Result.Id);

                        await SetCurrentOrderAsync(orderCreationResult.Result);

                        setCurrentOrder = true;
                    }
                }

                if (setCurrentOrder)
                {
                    result.SetSuccess();
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(SetEmptyCurrentOrderAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public async Task<AOResult> OpenLastOrCreateNewOrderAsync()
        {
            var result = new AOResult();

            try
            {
                var lastOrderId = await GetIdLastCreatedOrderFromSettingsAsync(_authenticationService.AuthorizedUserId.ToString());

                if (lastOrderId.IsSuccess)
                {
                    await SetCurrentOrderAsync(lastOrderId.Result);
                }
                else
                {
                    await SetEmptyCurrentOrderAsync();
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(SetCurrentOrderAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public async Task<AOResult> AddDishInCurrentOrderAsync(DishBindableModel dish)
        {
            var result = new AOResult();

            try
            {
                if (CurrentSeat is null && !CurrentOrder.Seats.Any())
                {
                    var seat = new SeatBindableModel
                    {
                        SeatNumber = 1,
                        SelectedDishes = new(),
                        Checked = true,
                        IsFirstSeat = true,
                    };

                    CurrentOrder.Seats.Add(seat);

                    CurrentSeat = CurrentOrder.Seats.FirstOrDefault(row => row.SeatNumber == seat.SeatNumber);
                }

                dish.DiscountPrice = dish.SelectedDishProportionPrice;

                if (CurrentSeat is null)
                {
                    CurrentSeat = CurrentOrder.Seats.FirstOrDefault();
                }

                CurrentOrder.Seats[CurrentOrder.Seats.IndexOf(CurrentSeat)].SelectedDishes.Add(dish);

                CurrentOrder.SubTotalPrice = CurrentOrder.Seats.Sum(row => row.SelectedDishes.Sum(row => row.TotalPrice));

                CurrentOrder.PriceTax = (decimal)(CurrentOrder.SubTotalPrice * CurrentOrder.TaxCoefficient);

                CurrentOrder.TotalPrice = (decimal)(CurrentOrder.SubTotalPrice + CurrentOrder.PriceTax);

                if (CurrentOrder.Coupon is not null || CurrentOrder.Discount is not null)
                {
                    _bonusesService.ResetСalculationBonus(CurrentOrder);
                    _bonusesService.СalculationBonus(CurrentOrder);
                }

                result.SetSuccess();
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(AddDishInCurrentOrderAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public async Task<AOResult> AddSeatInCurrentOrderAsync()
        {
            var result = new AOResult();

            try
            {
                foreach (var item in CurrentOrder.Seats)
                {
                    item.Checked = false;
                }

                var seat = new SeatBindableModel
                {
                    SeatNumber = CurrentOrder.Seats.Count + 1,
                    SelectedDishes = new(),
                    Checked = true,
                };

                CurrentOrder.Seats.Add(seat);

                CurrentSeat = CurrentOrder.Seats.LastOrDefault();

                result.SetSuccess();
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(AddSeatInCurrentOrderAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public async Task<AOResult> DeleteSeatFromCurrentOrder(SeatBindableModel seat)
        {
            var result = new AOResult();

            try
            {
                bool isDeleted = CurrentOrder.Seats.Remove(seat);

                if (isDeleted)
                {
                    for (int i = seat.SeatNumber - 1; i < CurrentOrder.Seats.Count; i++)
                    {
                        CurrentOrder.Seats[i].SeatNumber--;
                    }

                    CurrentSeat = CurrentOrder.Seats.FirstOrDefault();

                    if (seat.SelectedDishes.Count != 0)
                    {
                        CurrentOrder.SubTotalPrice -= seat.SelectedDishes.Sum(row => row.TotalPrice);
                        CurrentOrder.PriceTax = (decimal)(CurrentOrder.SubTotalPrice * CurrentOrder.TaxCoefficient);
                        CurrentOrder.TotalPrice = (decimal)(CurrentOrder.SubTotalPrice + CurrentOrder.PriceTax);
                    }

                    result.SetSuccess();
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(DeleteSeatFromCurrentOrder)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public async Task<AOResult> RedirectSetsFromSeatInCurrentOrder(SeatBindableModel sourceSeat, int destinationSeatNumber)
        {
            var result = new AOResult();

            try
            {
                var seats = CurrentOrder.Seats;
                var destinationSeat = seats.FirstOrDefault(x => x.SeatNumber == destinationSeatNumber);
                int destinationSeatIndex = seats.IndexOf(destinationSeat);

                if (destinationSeatIndex != -1 && destinationSeat.SeatNumber != sourceSeat.SeatNumber)
                {
                    foreach (var item in sourceSeat.SelectedDishes)
                    {
                        seats[destinationSeatIndex].SelectedDishes.Add(item);
                    }

                    int sourceSeatIndex = seats.IndexOf(sourceSeat);

                    seats[sourceSeatIndex].SelectedDishes.Clear();

                    result.SetSuccess();
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(RedirectSetsFromSeatInCurrentOrder)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public async Task<AOResult> DeleteDishFromCurrentSeatAsync()
        {
            var result = new AOResult();

            try
            {
                DishBindableModel? dishTobeRemoved = CurrentOrder.Seats.FirstOrDefault(x => x.SelectedItem is not null)?.SelectedItem;

                if (dishTobeRemoved is not null)
                {
                    CurrentOrder.Seats.FirstOrDefault(x => x.SelectedItem is not null).SelectedDishes.Remove(dishTobeRemoved);
                    CurrentOrder.SubTotalPrice -= dishTobeRemoved.TotalPrice;
                    CurrentOrder.PriceTax = (decimal)(CurrentOrder.SubTotalPrice * CurrentOrder.TaxCoefficient);
                    CurrentOrder.TotalPrice = (decimal)(CurrentOrder.SubTotalPrice + CurrentOrder.PriceTax);
                }

                result.SetSuccess();
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(DeleteDishFromCurrentSeatAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public async Task<AOResult<Guid>> UpdateOrderAsync(UpdateOrderCommand order)
        {
            var result = new AOResult<Guid>();

            try
            {
                var response = await _restService.RequestAsync<GenericExecutionResult<Guid>>(HttpMethod.Put, $"{Constants.API.HOST_URL}/api/orders", order);

                if (response.Success)
                {
                    result.SetSuccess(response.Value);
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(UpdateOrderAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        #endregion

        #region -- Private helpers --

        private async Task<AOResult<Guid>> GetIdLastCreatedOrderFromSettingsAsync(string employeeId)
        {
            var result = new AOResult<Guid>();

            try
            {
                if (_settingsManager?.LastCurrentOrderIds != string.Empty)
                {
                    var lastCurrentOrderIds = JsonConvert.DeserializeObject<Dictionary<string, Guid>>(_settingsManager.LastCurrentOrderIds);

                    if (lastCurrentOrderIds.ContainsKey(employeeId))
                    {
                        result.SetSuccess(lastCurrentOrderIds[employeeId]);
                    }
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(GetIdLastCreatedOrderFromSettingsAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        private async Task<AOResult> SaveLastOrderIdToSettingsAsync(string employeeId, Guid orderId)
        {
            var result = new AOResult();

            try
            {
                var employeeIdAndOrderIdPairs = JsonConvert.DeserializeObject<Dictionary<string, Guid>>(_settingsManager.LastCurrentOrderIds);

                employeeIdAndOrderIdPairs ??= new();

                if (employeeIdAndOrderIdPairs.ContainsKey(employeeId))
                {
                    employeeIdAndOrderIdPairs[employeeId] = orderId;
                }
                else
                {
                    employeeIdAndOrderIdPairs.Add(employeeId, orderId);
                }

                _settingsManager.LastCurrentOrderIds = JsonConvert.SerializeObject(employeeIdAndOrderIdPairs);

                result.SetSuccess();
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(SaveLastOrderIdToSettingsAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        private async Task AddAdditionalDishesInformationToOrderAsync(FullOrderBindableModel currentOrder)
        {
            if (currentOrder.Seats.Any(row => row.SelectedDishes.Any()))
            {
                var menuService = App.Resolve<IMenuService>();

                List<Task<AOResult<DishModelDTO>>> tasks = new();
                var dishesId = currentOrder.Seats.SelectMany(row => row.SelectedDishes).Select(row => row.DishId).Distinct();

                foreach (var dishId in dishesId)
                {
                    tasks.Add(menuService.GetDishByIdAsync(dishId));
                }

                var dishesResult = await Task.WhenAll(tasks);

                var dishes = dishesResult.Where(row => row.IsSuccess).Select(row => row?.Result);

                foreach (var seat in currentOrder.Seats)
                {
                    foreach (var dish in seat.SelectedDishes)
                    {
                        var dishId = dish.DishId;
                        var source = dishes.FirstOrDefault(row => row.Id == dishId);

                        if (source is not null)
                        {
                            dish.DishProportions = source.DishProportions;
                            dish.Products = new(source.Products);
                        }
                    }
                }
            }

            currentOrder.Seats = new(currentOrder.Seats?.OrderBy(row => row.SeatNumber));
        }

        private async Task<AOResult> SetCurrentOrderAsync(Guid orderId)
        {
            var result = new AOResult();

            try
            {
                var orderResult = await GetOrderByIdAsync(orderId);

                if (orderResult.IsSuccess)
                {
                    await SetCurrentOrderAsync(orderResult.Result);
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(SetCurrentOrderAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        private async Task SetCurrentOrderAsync(OrderModelDTO order)
        {
            var currentOrder = order.ToFullOrderBindableModel();

            if (currentOrder is not null)
            {
                await AddAdditionalDishesInformationToOrderAsync(currentOrder);

                CurrentOrder = currentOrder;
                CurrentSeat = CurrentOrder.Seats.FirstOrDefault();
            }
        }

        #endregion
    }
}