using AutoMapper;
using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using Next2.Resources.Strings;
using Next2.Services.Bonuses;
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
        private readonly IBonusesService _bonusService;
        private readonly IMapper _mapper;

        public OrderService(
            IMockService mockService,
            IBonusesService bonusesService,
            IMapper mapper)
        {
            _mockService = mockService;
            _bonusService = bonusesService;
            _mapper = mapper;

            CurrentOrder.Seats = new ();

            Task.Run(CreateNewOrderAsync);
        }

        #region -- Public properties --

        public FullOrderBindableModel CurrentOrder { get; set; } = new ();

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

        public async Task<AOResult<int>> GetNewOrderIdAsync()
        {
            var result = new AOResult<int>();

            try
            {
                int newOrderId = _mockService.MaxIdentifier<OrderModel>() + 1;

                result.SetSuccess(newOrderId);
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(GetNewOrderIdAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public async Task<AOResult<IEnumerable<TableModel>>> GetFreeTablesAsync()
        {
            var result = new AOResult<IEnumerable<TableModel>>();

            try
            {
                var allTables = await _mockService.GetAllAsync<TableModel>();

                if (allTables is not null)
                {
                    var allOrders = await _mockService.GetAllAsync<OrderModel>();

                    if (allOrders is not null)
                    {
                        var freeTables = allTables.Where(table => allOrders
                            .All(order => order.TableNumber != table.TableNumber || order.OrderStatus is Constants.OrderStatus.CANCELLED or Constants.OrderStatus.PAYED));

                        if (freeTables is not null)
                        {
                            result.SetSuccess(freeTables);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(GetFreeTablesAsync)}: exception", Strings.SomeIssues, ex);
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

        public async Task<AOResult> DeleteOrderAsync(int orderId)
        {
            var result = new AOResult();

            try
            {
                var removalOrder = await _mockService.FindAsync<OrderModel>(x => x.Id == orderId);

                if (removalOrder is not null)
                {
                    await _mockService.RemoveAsync(removalOrder);

                    result.SetSuccess();
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(DeleteOrderAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public async Task<AOResult<IEnumerable<SeatModel>>> GetSeatsAsync(int orderId)
        {
            var result = new AOResult<IEnumerable<SeatModel>>();

            try
            {
                var seats = await _mockService.GetAsync<SeatModel>(x => x.OrderId == orderId);

                if (seats is not null)
                {
                    result.SetSuccess(seats);
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(GetSeatsAsync)}: exception", Strings.SomeIssues, ex);
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
                var availableTables = await GetFreeTablesAsync();

                if (orderId.IsSuccess && availableTables.IsSuccess)
                {
                    var tableBindableModels = _mapper.Map<ObservableCollection<TableBindableModel>>(availableTables.Result);

                    CurrentOrder = new();
                    CurrentOrder.Seats = new();

                    var tax = await GetTaxAsync();

                    if (tax.IsSuccess)
                    {
                        CurrentOrder.Tax = tax.Result;
                    }

                    CurrentOrder.Id = orderId.Result;
                    CurrentOrder.OrderNumber = orderId.Result;
                    CurrentOrder.OrderStatus = Constants.OrderStatus.IN_PROGRESS;
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
            bool success = true;

            try
            {
                if (CurrentSeat is null)
                {
                    var seat = new SeatBindableModel
                    {
                        Id = 1,
                        SeatNumber = 1,
                        Sets = new(),
                        Checked = true,
                        IsFirstSeat = true,
                    };

                    CurrentOrder.Seats.Add(seat);

                    CurrentSeat = seat;
                }

                var resultProducts = await _mockService.GetAsync<ProductModel>(row => row.SetId == set.Id);

                if (resultProducts is not null)
                {
                    set.Products = new();

                    foreach (var product in resultProducts)
                    {
                        var newProduct = new ProductBindableModel()
                        {
                            Id = product.Id,
                            ReplacementProducts = new(),
                            SelectedIngredients = new(),
                            Title = product.Title,
                            ImagePath = product.ImagePath,
                            ProductPrice = product.ProductPrice,
                            IngredientsPrice = product.IngredientsPrice,
                            TotalPrice = product.ProductPrice,
                            Comment = product.Comment,
                        };

                        var resultOptionsProduct = await _mockService.GetAsync<OptionModel>(row => row.ProductId == product.Id);

                        if (resultOptionsProduct is not null)
                        {
                            newProduct.SelectedOption = resultOptionsProduct.FirstOrDefault(row => row.Id == product.DefaultOptionId);
                            newProduct.Options = new(resultOptionsProduct);
                        }
                        else
                        {
                            newProduct.SelectedOption = new();
                            newProduct.Options = new();
                        }

                        var resultReplacementProducts = await _mockService.GetAsync<ReplacementProductModel>(row => row.ReplacementProductId == product.Id);

                        foreach (var replacementProduct in resultReplacementProducts)
                        {
                            var itemProduct = await _mockService.GetAsync<ProductModel>(row => row.Id == replacementProduct.ProductId);
                            newProduct.ReplacementProducts.Add(itemProduct.FirstOrDefault());
                        }

                        newProduct.SelectedProduct = newProduct.ReplacementProducts.FirstOrDefault(row => row.Id == product.DefaultProductId);

                        if (newProduct.SelectedProduct is null)
                        {
                            newProduct.SelectedProduct = product;
                        }

                        var selectedIngredients = await _mockService.GetAsync<IngredientOfProductModel>(row => row.ProductId == newProduct.SelectedProduct.Id);

                        if (selectedIngredients is not null)
                        {
                            newProduct.SelectedIngredients = new(selectedIngredients);
                            newProduct.DefaultSelectedIngredients = new(selectedIngredients);
                        }

                        set.Products.Add(newProduct);

                        set.IngredientsPrice += newProduct.IngredientsPrice;
                        set.ProductsPrice += newProduct.SelectedProduct.ProductPrice;
                    }

                    set.TotalPrice = set.IngredientsPrice + set.Portion.Price;

                    CurrentOrder.Total += set.TotalPrice;
                }
                else
                {
                    success = false;
                }

                if (!success)
                {
                    result.SetFailure();
                }

                CurrentOrder.Seats[CurrentOrder.Seats.IndexOf(CurrentSeat)].Sets.Add(set);
                CurrentOrder.SubTotal += set.Portion.Price;

                CurrentOrder.PriceTax = CurrentOrder.SubTotal * CurrentOrder.Tax.Value;
                CurrentOrder.Total = CurrentOrder.SubTotal + CurrentOrder.PriceTax;

                if (CurrentOrder.BonusType != Enums.EBonusType.None)
                {
                    CurrentOrder = await _bonusService.СalculationBonusAsync(CurrentOrder);
                }

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
                foreach (var item in CurrentOrder.Seats)
                {
                    item.Checked = false;
                }

                var seat = new SeatBindableModel
                {
                    Id = CurrentOrder.Seats.Count + 1,
                    SeatNumber = CurrentOrder.Seats.Count + 1,
                    Sets = new (),
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
                        CurrentOrder.Seats[i].Id--;
                        CurrentOrder.Seats[i].SeatNumber--;
                    }

                    CurrentSeat = CurrentOrder.Seats.FirstOrDefault();

                    CurrentOrder.SubTotal -= seat.Sets.Sum(row => row.TotalPrice);
                    CurrentOrder.PriceTax = CurrentOrder.SubTotal * CurrentOrder.Tax.Value;
                    CurrentOrder.Total = CurrentOrder.SubTotal + CurrentOrder.PriceTax;

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
                    foreach (var item in sourceSeat.Sets)
                    {
                        seats[destinationSeatIndex].Sets.Add(item);
                    }

                    int sourceSeatIndex = seats.IndexOf(sourceSeat);

                    seats[sourceSeatIndex].Sets.Clear();

                    result.SetSuccess();
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(RedirectSetsFromSeatInCurrentOrder)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public async Task<AOResult> DeleteSetFromCurrentSeat()
        {
            var result = new AOResult();

            try
            {
                SetBindableModel? setTobeRemoved = CurrentOrder.Seats.FirstOrDefault(x => x.SelectedItem is not null)?.SelectedItem;
                if (setTobeRemoved is not null)
                {
                    CurrentOrder.Seats.FirstOrDefault(x => x.SelectedItem is not null).Sets.Remove(setTobeRemoved);
                    CurrentOrder.SubTotal -= setTobeRemoved.TotalPrice;
                    CurrentOrder.PriceTax = CurrentOrder.SubTotal * CurrentOrder.Tax.Value;
                    CurrentOrder.Total = CurrentOrder.SubTotal + CurrentOrder.PriceTax;
                }

                result.SetSuccess();
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(DeleteSetFromCurrentSeat)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public async Task<AOResult> AddSeatAsync(SeatModel seat)
        {
            var result = new AOResult();

            try
            {
                if (seat is not null)
                {
                    var seatId = await _mockService.AddAsync(seat);
                    if (seatId >= 0)
                    {
                        result.SetSuccess();
                    }
                    else
                    {
                        result.SetFailure();
                    }
                }
                else
                {
                    result.SetFailure();
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(AddSeatAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public async Task<AOResult> AddOrderAsync(OrderModel order)
        {
            var result = new AOResult();

            try
            {
                if (order is not null)
                {
                    var orderId = await _mockService.AddAsync(order);

                    if (orderId >= 0)
                    {
                        result.SetSuccess();
                    }
                    else
                    {
                        result.SetFailure();
                    }
                }
                else
                {
                    result.SetFailure();
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(AddOrderAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        #endregion
    }
}