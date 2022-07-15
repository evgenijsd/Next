using Newtonsoft.Json;
using Next2.Enums;
using Next2.Extensions;
using Next2.Helpers.ProcessHelpers;
using Next2.Models.API.Commands;
using Next2.Models.API.DTO;
using Next2.Models.API.Results;
using Next2.Models.Bindables;
using Next2.Resources.Strings;
using Next2.Services.Authentication;
using Next2.Services.Bonuses;
using Next2.Services.Customers;
using Next2.Services.Menu;
using Next2.Services.Rest;
using Next2.Services.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Next2.Services.Order
{
    public class OrderService : IOrderService
    {
        private readonly ISettingsManager _settingsManager;
        private readonly IRestService _restService;
        private readonly IBonusesService _bonusesService;
        private readonly IAuthenticationService _authenticationService;
        private readonly ICustomersService _customersService;

        public OrderService(
            IRestService restService,
            IBonusesService bonusesService,
            ISettingsManager settingsManager,
            IAuthenticationService authenticationService,
            ICustomersService customersService)
        {
            _settingsManager = settingsManager;
            _restService = restService;
            _bonusesService = bonusesService;
            _restService = restService;
            _authenticationService = authenticationService;
            _customersService = customersService;
        }

        #region -- Public properties --

        public FullOrderBindableModel CurrentOrder { get; set; } = new();

        public SeatBindableModel? CurrentSeat { get; set; }

        #endregion

        #region -- IOrderService implementation --

        public void UpdateTotalSum(FullOrderBindableModel currentOrder)
        {
            currentOrder.SubTotalPrice = 0;
            currentOrder.DiscountPrice = 0;
            var discountPercentage = currentOrder.Coupon?.DiscountPercentage ?? 0 + currentOrder.Discount?.DiscountPercentage ?? 0;
            var couponDishes = currentOrder.Coupon?.Dishes?.ToList() ?? new();

            foreach (var seat in currentOrder.Seats)
            {
                foreach (var dish in seat.SelectedDishes)
                {
                    decimal totalProductsPrice = 0;

                    foreach (var product in dish.SelectedProducts)
                    {
                        var ingredientsPrice = product.AddedIngredients is not null
                            ? product.AddedIngredients.Sum(row => row.Price)
                            : 0;

                        ingredientsPrice += product.ExcludedIngredients is not null
                            ? product.ExcludedIngredients.Sum(row => row.Price)
                            : 0;

                        var productPrice = product.Price;
                        totalProductsPrice += ingredientsPrice + productPrice;
                    }

                    var discountProductsPrice = totalProductsPrice;

                    if (currentOrder.Coupon is not null)
                    {
                        var couponDish = couponDishes.FirstOrDefault(x => x.Id == dish.DishId);

                        if (couponDish is not null)
                        {
                            discountProductsPrice = totalProductsPrice - (discountPercentage * totalProductsPrice / 100);
                            couponDishes.Remove(couponDish);
                        }
                    }
                    else
                    {
                        discountProductsPrice = totalProductsPrice - (discountPercentage * totalProductsPrice / 100);
                    }

                    dish.DiscountPrice = discountProductsPrice;
                    dish.TotalPrice = totalProductsPrice;

                    currentOrder.SubTotalPrice += dish.TotalPrice;
                    currentOrder.DiscountPrice += dish.DiscountPrice;
                }
            }

            if (currentOrder.DiscountPrice == currentOrder.SubTotalPrice)
            {
                currentOrder.Discount = null;
                currentOrder.Coupon = null;
            }

            currentOrder.PriceTax = (decimal)currentOrder.DiscountPrice * currentOrder.TaxCoefficient;
            currentOrder.TotalPrice = (decimal)currentOrder.DiscountPrice + currentOrder.PriceTax;
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

                var order = creationResult.Value;

                if (order is not null)
                {
                    var newOrder = new OrderModelDTO()
                    {
                        Id = order.Id,
                        Number = order.OrderNumber,
                        Open = order.Open,
                        TaxCoefficient = order.TaxCoefficient,
                        OrderType = EOrderType.DineIn.ToString(),
                        OrderStatus = EOrderStatus.Pending,
                        EmployeeId = employeeId,
                        Table = null,
                        Coupon = null,
                        Discount = null,
                        Customer = null,
                        SubTotalPrice = 0m,
                        TotalPrice = 0m,
                    };

                    result.SetSuccess(newOrder);
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

                if (freeTables.Success && freeTables.Value?.Tables is not null)
                {
                    result.SetSuccess(freeTables.Value.Tables);
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
                var query = $"{Constants.API.HOST_URL}/api/orders";

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
                var query = $"{Constants.API.HOST_URL}/api/orders/{orderId}";

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

        public async Task<AOResult> SetEmptyCurrentOrderAsync()
        {
            var result = new AOResult();

            try
            {
                var isCurrentOrderSet = false;
                var employeeId = _authenticationService.AuthorizedUserId.ToString();

                var resultOfGettingLastOrderId = await GetIdLastCreatedOrderFromSettingsAsync(employeeId);

                if (resultOfGettingLastOrderId.IsSuccess && resultOfGettingLastOrderId.Result != Guid.Empty)
                {
                    var resultOfGettingOrder = await GetOrderByIdAsync(resultOfGettingLastOrderId.Result);

                    if (resultOfGettingOrder.IsSuccess)
                    {
                        var seats = resultOfGettingOrder.Result.Seats;

                        if (!IsOrderWithDishes(resultOfGettingOrder.Result) && IsOpenOrder(resultOfGettingOrder.Result))
                        {
                            var resultOfSettingCurrentOrder = await SetCurrentOrderAsync(resultOfGettingOrder.Result);

                            isCurrentOrderSet = resultOfSettingCurrentOrder.IsSuccess;
                        }
                    }
                }

                if (!isCurrentOrderSet)
                {
                    var orderCreationResult = await CreateNewOrderAsync();

                    if (orderCreationResult.IsSuccess)
                    {
                        await SaveLastOrderIdToSettingsAsync(employeeId, orderCreationResult.Result.Id);

                        var resultOfSettingCurrentOrder = await SetCurrentOrderAsync(orderCreationResult.Result);

                        isCurrentOrderSet = resultOfSettingCurrentOrder.IsSuccess;
                    }
                }

                if (isCurrentOrderSet)
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
                var resultOfGettingLastOrderId = await GetIdLastCreatedOrderFromSettingsAsync(_authenticationService.AuthorizedUserId.ToString());

                if (resultOfGettingLastOrderId.IsSuccess)
                {
                    result = await SetCurrentOrderAsync(resultOfGettingLastOrderId.Result);
                }

                if (!result.IsSuccess)
                {
                    result = await SetEmptyCurrentOrderAsync();
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(OpenLastOrCreateNewOrderAsync)}: exception", Strings.SomeIssues, ex);
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

                    CurrentSeat = CurrentOrder.Seats.FirstOrDefault(row => row.Checked);
                }

                if (CurrentSeat is null)
                {
                    CurrentSeat = CurrentOrder.Seats.FirstOrDefault();
                }

                CurrentOrder.Seats[CurrentOrder.Seats.IndexOf(CurrentSeat)].SelectedDishes.Add(dish);

                UpdateTotalSum(CurrentOrder);

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
                    IsFirstSeat = !CurrentOrder.Seats.Any(),
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
                        UpdateTotalSum(CurrentOrder);
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

        public async Task<AOResult> RedirectDishesFromSeatInCurrentOrder(SeatBindableModel sourceSeat, int destinationSeatNumber)
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
                result.SetError($"{nameof(RedirectDishesFromSeatInCurrentOrder)}: exception", Strings.SomeIssues, ex);
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
                    UpdateTotalSum(CurrentOrder);
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
                var query = $"{Constants.API.HOST_URL}/api/orders";

                var response = await _restService.RequestAsync<GenericExecutionResult<Guid>>(HttpMethod.Put, query, order);

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

        public async Task<AOResult<Guid>> UpdateTableAsync(UpdateTableCommand command)
        {
            var result = new AOResult<Guid>();

            try
            {
                var response = await _restService.RequestAsync<GenericExecutionResult<Guid>>(HttpMethod.Put, $"{Constants.API.HOST_URL}/api/tables", command);

                if (response.Success)
                {
                    result.SetSuccess(response.Value);
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(UpdateTableAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public async Task<AOResult> SetCurrentOrderAsync(Guid orderId)
        {
            var result = new AOResult();

            try
            {
                var orderResult = await GetOrderByIdAsync(orderId);

                if (orderResult.IsSuccess && IsOpenOrder(orderResult.Result))
                {
                    result = await SetCurrentOrderAsync(orderResult.Result);
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(SetCurrentOrderAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public async Task<AOResult<Guid>> UpdateCurrentOrderAsync()
        {
            var result = new AOResult<Guid>();

            try
            {
                UpdateTotalSum(CurrentOrder);
                var orderForUpdate = CurrentOrder.ToUpdateOrderCommand();

                result = await UpdateOrderAsync(orderForUpdate);
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(UpdateCurrentOrderAsync)}: exception", Strings.SomeIssues, ex);
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

        private async Task<AOResult> AddAdditionalDishesInformationToOrderAsync(FullOrderBindableModel currentOrder)
        {
            var result = new AOResult();
            var isSuccessUpdateDishes = true;

            if (currentOrder.Seats.Any(row => row.SelectedDishes.Any()))
            {
                var menuService = App.Resolve<IMenuService>();

                List<Task<AOResult<DishModelDTO>>> tasks = new();
                var dishesIds = currentOrder.Seats.SelectMany(row => row.SelectedDishes).Select(row => row.DishId).Distinct();

                foreach (var dishId in dishesIds)
                {
                    tasks.Add(menuService.GetDishByIdAsync(dishId));
                }

                var dishesResult = await Task.WhenAll(tasks);

                isSuccessUpdateDishes = !dishesResult.Any(row => row.IsSuccess == false);

                if (isSuccessUpdateDishes)
                {
                    var dishes = dishesResult.Where(row => row.IsSuccess).Select(row => row?.Result);

                    foreach (var seat in currentOrder.Seats)
                    {
                        foreach (var dish in seat.SelectedDishes)
                        {
                            var dishId = dish.DishId;
                            var sourceDish = dishes.FirstOrDefault(row => row.Id == dishId);

                            if (sourceDish is not null)
                            {
                                dish.DishProportions = sourceDish.DishProportions;
                                dish.Products = new(sourceDish.Products);
                            }
                        }
                    }
                }
            }

            currentOrder.Seats = new(currentOrder.Seats?.OrderBy(row => row.SeatNumber));

            if (isSuccessUpdateDishes)
            {
                result.SetSuccess();
            }

            return result;
        }

        private async Task<AOResult> SetCurrentOrderAsync(OrderModelDTO order)
        {
            var result = new AOResult();

            var currentOrder = order.ToFullOrderBindableModel();

            if (currentOrder is not null && IsOpenOrder(order))
            {
                var resultOfUpdatingDishes = await AddAdditionalDishesInformationToOrderAsync(currentOrder);

                if (resultOfUpdatingDishes.IsSuccess)
                {
                    var isSuccessAddCustomer = true;

                    if (currentOrder.Customer?.Id is not null)
                    {
                        var resultOfAddingCustomerToOrder = await AddCustomerToOrderAsync(currentOrder, currentOrder.Customer.Id);

                        isSuccessAddCustomer = resultOfAddingCustomerToOrder.IsSuccess;
                    }

                    if (isSuccessAddCustomer)
                    {
                        currentOrder.Seats = new(currentOrder.Seats?.OrderBy(row => row.SeatNumber));

                        CurrentOrder = currentOrder;
                        CurrentSeat = CurrentOrder.Seats.FirstOrDefault(x => x.Checked);

                        result.SetSuccess();
                    }
                }
            }

            return result;
        }

        private async Task<AOResult> AddCustomerToOrderAsync(FullOrderBindableModel currentOrder, Guid customerId)
        {
            var result = new AOResult();

            var resultOfGettingCustomer = await _customersService.GetCustomerByIdAsync(customerId);

            if (resultOfGettingCustomer.IsSuccess && resultOfGettingCustomer.Result is not null)
            {
                currentOrder.Customer = resultOfGettingCustomer.Result;

                result.SetSuccess();
            }

            return result;
        }

        private bool IsOpenOrder(OrderModelDTO order)
        {
            return order.OrderStatus is not EOrderStatus.Deleted or EOrderStatus.Closed or EOrderStatus.Canceled;
        }

        private bool IsOrderWithDishes(OrderModelDTO order)
        {
            var result = false;

            if (order.Seats?.Count() > 0)
            {
                result = order.Seats.Any(seat => seat.SelectedDishes.Count() > 0);
            }

            return result;
        }

        #endregion
    }
}