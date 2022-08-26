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

        public DateTime DateTime = new();

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

                    if (dish.SelectedProducts is not null)
                    {
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

                    if (!dish.IsSplitted)
                    {
                        dish.DiscountPrice = discountProductsPrice;
                        dish.TotalPrice = totalProductsPrice;
                    }

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
                    OrderModelDTO newOrder = new()
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

        public async Task<AOResult<IEnumerable<SimpleOrderModelDTO>>> GetOrdersAsync(EOrderStatus? orderStatus = null, Func<SimpleOrderModelDTO, bool>? condition = null)
        {
            var result = new AOResult<IEnumerable<SimpleOrderModelDTO>>();

            try
            {
                var query = $"{Constants.API.HOST_URL}/api/orders";

                var responce = await _restService.RequestAsync<GenericExecutionResult<GetOrdersListQueryResult>>(HttpMethod.Get, query);

                if (responce.Success && responce.Value?.Orders is not null)
                {
                    var allOrders = responce.Value.Orders;

                    var filteredByStatusOrders = orderStatus is null
                        ? allOrders
                        : allOrders.Where(x => x.OrderStatus == orderStatus);

                    var filteredByConditionOrders = condition is null
                        ? filteredByStatusOrders
                        : filteredByStatusOrders.Where(condition);

                    result.SetSuccess(filteredByConditionOrders);
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
                    else if (resultOfGettingOrder.Exception is not null)
                    {
                        throw resultOfGettingOrder.Exception;
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
                    else if (orderCreationResult.Exception is not null)
                    {
                        throw orderCreationResult.Exception;
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

        public Task<AOResult> AddDishInCurrentOrderAsync(DishBindableModel dish)
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

                CurrentOrder.Seats.FirstOrDefault(row => row.SeatNumber == CurrentSeat.SeatNumber).SelectedDishes.Add(dish);

                UpdateTotalSum(CurrentOrder);

                result.SetSuccess();
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(AddDishInCurrentOrderAsync)}: exception", Strings.SomeIssues, ex);
            }

            return Task.FromResult(result);
        }

        public Task<AOResult> AddSeatInCurrentOrderAsync()
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

            return Task.FromResult(result);
        }

        public Task<AOResult> DeleteSeatFromCurrentOrder(SeatBindableModel seat)
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

            return Task.FromResult(result);
        }

        public Task<AOResult> RedirectDishesFromSeatInCurrentOrder(SeatBindableModel sourceSeat, int destinationSeatNumber)
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

            return Task.FromResult(result);
        }

        public Task<AOResult> DeleteDishFromCurrentSeatAsync()
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

            return Task.FromResult(result);
        }

        public async Task<AOResult<Guid>> UpdateOrderAsync(OrderModelDTO order)
        {
            var result = new AOResult<Guid>();

            try
            {
                var updateCommand = order.ToUpdateOrderCommand();

                var query = $"{Constants.API.HOST_URL}/api/orders";

                var response = await _restService.RequestAsync<GenericExecutionResult<Guid>>(HttpMethod.Put, query, updateCommand);

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

        public async Task<AOResult<Guid>> UpdateTableAsync(SimpleTableModelDTO? table, bool isAvailable = false)
        {
            var result = new AOResult<Guid>();

            if (table is not null)
            {
                var updateCommand = new UpdateTableCommand()
                {
                    Id = table.Id,
                    Number = table.Number,
                    SeatNumbers = table.SeatNumbers,
                    IsAvailable = isAvailable,
                };

                try
                {
                    var response = await _restService.RequestAsync<GenericExecutionResult<Guid>>(HttpMethod.Put, $"{Constants.API.HOST_URL}/api/tables", updateCommand);

                    if (response.Success)
                    {
                        result.SetSuccess(response.Value);
                    }
                }
                catch (Exception ex)
                {
                    result.SetError($"{nameof(UpdateTableAsync)}: exception", Strings.SomeIssues, ex);
                }
            }

            return result;
        }

        public void CalculateOrderPrices(OrderModelDTO order)
        {
            var dishes = order.Seats.SelectMany(x => x.SelectedDishes);

            foreach (var dish in dishes)
            {
                dish.DiscountPrice = dish.TotalPrice;
            }

            order.TotalPrice = dishes.Sum(x => x.TotalPrice);
            order.DiscountPrice = dishes.Sum(x => x.DiscountPrice);
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

                var orderDTO = CurrentOrder.ToOrderModelDTO();

                result = await UpdateOrderAsync(orderDTO);
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(UpdateCurrentOrderAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public Task<AOResult<DishBindableModel>> ChangeDishProportionAsync(ProportionBindableModel selectedProportion, DishBindableModel dish, IEnumerable<IngredientModelDTO> ingredients)
        {
            var result = new AOResult<DishBindableModel>();

            try
            {
                dish.SelectedDishProportion = selectedProportion.ToDishProportionModelDTO();

                var selectedProportionPriceRatio = selectedProportion.PriceRatio;

                foreach (var product in dish.SelectedProducts ?? new())
                {
                    product.Price = selectedProportionPriceRatio == 1
                        ? product.Product.DefaultPrice
                        : product.Product.DefaultPrice * (1 + selectedProportionPriceRatio);

                    ChangeIngredientsPriceBaseOnProportion(product, selectedProportionPriceRatio);
                }

                result.SetSuccess(dish);
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(ChangeDishProportionAsync)}: exception", Strings.SomeIssues, ex);
            }

            return Task.FromResult(result);
        }

        public decimal CalculateDishPriceBaseOnProportion(DishBindableModel dish, decimal priceRatio, IEnumerable<IngredientModelDTO> ingredients)
        {
            decimal ingredientsPrice = 0;
            decimal dishPrice = 0;

            try
            {
                foreach (var product in dish.SelectedProducts ?? new())
                {
                    foreach (var addedIngredient in product.AddedIngredients ?? new())
                    {
                        ingredientsPrice += ingredients.FirstOrDefault(row => row.Id == addedIngredient.Id).Price;
                    }

                    foreach (var excludedIngredient in product.ExcludedIngredients ?? new())
                    {
                        ingredientsPrice += ingredients.FirstOrDefault(row => row.Id == excludedIngredient.Id).Price;
                    }
                }

                dishPrice = ingredientsPrice + dish.SelectedProducts.Sum(row => row.Product.DefaultPrice);

                dishPrice = priceRatio == 1
                    ? dishPrice
                    : dishPrice * (1 + priceRatio);
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(CalculateDishPriceBaseOnProportion)}: exception", ex);
            }

            return dishPrice;
        }

        public IEnumerable<SimpleOrderBindableModel> GetSortedOrders(IEnumerable<SimpleOrderBindableModel> orders, EOrdersSortingType sortingType)
        {
            Func<SimpleOrderBindableModel, object> sortingSelector = sortingType switch
            {
                EOrdersSortingType.ByOrderNumber => x => x.Number,
                EOrdersSortingType.ByTableNumber => x => x.TableNumber,
                EOrdersSortingType.ByTotalPrice => x => x.TotalPrice,
                _ => throw new NotImplementedException(),
            };

            return orders.OrderBy(sortingSelector);
        }

        public async Task<AOResult> UpdateOrdersAsync(IEnumerable<Guid> ordersId, string employeeId)
        {
            var resultOfUpdatingOrder = new AOResult();

            bool isSuccess = true;

            try
            {
                var resultOfGettingOrders = await GetOrdersModelDTOAsync(ordersId);

                if (resultOfGettingOrders.IsSuccess)
                {
                    var orders = resultOfGettingOrders.Result;

                    foreach (var order in orders)
                    {
                        order.EmployeeId = employeeId;

                        resultOfUpdatingOrder = await UpdateOrderAsync(order);

                        if (!resultOfUpdatingOrder.IsSuccess)
                        {
                            if (resultOfUpdatingOrder.Exception is not null)
                            {
                                throw resultOfUpdatingOrder.Exception;
                            }
                            else
                            {
                                isSuccess = false;
                                break;
                            }
                        }
                    }

                    if (isSuccess)
                    {
                        resultOfUpdatingOrder.SetSuccess();
                    }
                }
                else if(resultOfGettingOrders.Exception is not null)
                {
                    throw resultOfGettingOrders.Exception;
                }
            }
            catch (Exception ex)
            {
                resultOfUpdatingOrder.SetError($"{nameof(UpdateOrdersAsync)}: exception", Strings.SomeIssues, ex);
            }

            return resultOfUpdatingOrder;
        }

        #endregion

        #region -- Private helpers --

        private Task<AOResult<Guid>> GetIdLastCreatedOrderFromSettingsAsync(string employeeId)
        {
            var result = new AOResult<Guid>();

            try
            {
                if (!string.IsNullOrEmpty(_settingsManager.LastCurrentOrderIds))
                {
                    var lastCurrentOrderIds = JsonConvert.DeserializeObject<Dictionary<string, Guid>>(_settingsManager.LastCurrentOrderIds);

                    if (lastCurrentOrderIds is not null && lastCurrentOrderIds.ContainsKey(employeeId))
                    {
                        result.SetSuccess(lastCurrentOrderIds[employeeId]);
                    }
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(GetIdLastCreatedOrderFromSettingsAsync)}: exception", Strings.SomeIssues, ex);
            }

            return Task.FromResult(result);
        }

        private Task<AOResult> SaveLastOrderIdToSettingsAsync(string employeeId, Guid orderId)
        {
            var result = new AOResult();

            try
            {
                Dictionary<string, Guid>? employeeIdAndOrderIdPairs = new();

                if (!string.IsNullOrEmpty(_settingsManager.LastCurrentOrderIds))
                {
                    employeeIdAndOrderIdPairs = JsonConvert.DeserializeObject<Dictionary<string, Guid>>(_settingsManager.LastCurrentOrderIds);
                }

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

            return Task.FromResult(result);
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
                            var sourceDish = dishes.FirstOrDefault(row => row?.Id == dishId);

                            if (sourceDish is not null)
                            {
                                dish.DishProportions = sourceDish.DishProportions;
                                dish.ReplacementProducts = sourceDish.ReplacementProducts;
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

        private void ChangeIngredientsPriceBaseOnProportion(ProductBindableModel product, decimal priceRatio)
        {
            var allIngredients = product?.Product?.Ingredients;
            var addedIngredients = product?.AddedIngredients;
            var excludedIngredients = product?.ExcludedIngredients;

            if (allIngredients is not null && allIngredients.Any())
            {
                if (addedIngredients is not null && addedIngredients.Any())
                {
                    foreach (var ingredient in addedIngredients)
                    {
                        ingredient.Price = priceRatio == 1
                            ? allIngredients.FirstOrDefault(row => row.Id == ingredient.Id).Price
                            : allIngredients.FirstOrDefault(row => row.Id == ingredient.Id).Price * (1 + priceRatio);
                    }
                }
                else if (excludedIngredients is not null && excludedIngredients.Any())
                {
                    foreach (var ingredient in excludedIngredients)
                    {
                        ingredient.Price = priceRatio == 1
                            ? allIngredients.FirstOrDefault(row => row.Id == ingredient.Id).Price
                            : allIngredients.FirstOrDefault(row => row.Id == ingredient.Id).Price * (1 + priceRatio);
                    }
                }
            }
        }

        private async Task<AOResult<IEnumerable<OrderModelDTO>>> GetOrdersModelDTOAsync(IEnumerable<Guid> ordersId)
        {
            var result = new AOResult<IEnumerable<OrderModelDTO>>();

            bool isSuccess = true;

            try
            {
                var orderIdsNumber = ordersId.Count();

                OrderModelDTO[] orderModelsDTO = new OrderModelDTO[orderIdsNumber];

                List<Guid> orderIds = new(ordersId);

                for (int i = 0; i < orderIdsNumber; i++)
                {
                    var resultOfGettingOrder = await GetOrderByIdAsync(orderIds[i]);

                    if (resultOfGettingOrder.IsSuccess)
                    {
                        orderModelsDTO[i] = resultOfGettingOrder.Result;
                    }
                    else if(resultOfGettingOrder.Exception is not null)
                    {
                        throw resultOfGettingOrder.Exception;
                    }
                    else
                    {
                        isSuccess = false;
                        break;
                    }
                }

                if (isSuccess)
                {
                    result.SetSuccess(orderModelsDTO);
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(GetOrdersModelDTOAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        #endregion
    }
}