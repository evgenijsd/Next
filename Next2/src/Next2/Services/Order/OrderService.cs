using AutoMapper;
using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using Next2.Models.API;
using Next2.Models.API.Commands;
using Next2.Models.API.DTO;
using Next2.Models.API.Results;
using Next2.Resources.Strings;
using Next2.Services.Bonuses;
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
using Xamarin.Forms.Internals;

namespace Next2.Services.Order
{
    public class OrderService : IOrderService
    {
        private readonly IMockService _mockService;
        private readonly ISettingsManager _settingsManager;
        private readonly IRestService _restService;
        private readonly IBonusesService _bonusService;
        private readonly IMapper _mapper;

        public OrderService(
            IMockService mockService,
            IRestService restService,
            IBonusesService bonusesService,
            ISettingsManager settingsManager,
            IMapper mapper)
        {
            _mockService = mockService;
            _settingsManager = settingsManager;
            _restService = restService;
            _bonusService = bonusesService;
            _mapper = mapper;

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

        public async Task<AOResult<Guid>> CreateNewOrderAndGetIdAsync()
        {
            var result = new AOResult<Guid>();

            try
            {
                var requestBody = new CreateOrderCommand
                {
                    EmployeeId = _settingsManager.UserId.ToString(),
                };

                var query = $"{Constants.API.HOST_URL}/api/orders";
                var resultCreate = await _restService.RequestAsync<GenericExecutionResult<OrderModelDTO>>(HttpMethod.Post, query, requestBody);

                if (resultCreate?.Value?.Id is not null)
                {
                    result.SetSuccess(resultCreate.Value.Id);
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(CreateNewOrderAndGetIdAsync)}: exception", Strings.SomeIssues, ex);
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

        public async Task<AOResult> CreateNewCurrentOrderAsync()
        {
            var result = new AOResult();

            try
            {
                var orderId = await CreateNewOrderAndGetIdAsync();
                var availableTables = await GetFreeTablesAsync();

                if (orderId.IsSuccess && availableTables.IsSuccess)
                {
                    var tableBindableModels = _mapper.Map<ObservableCollection<TableBindableModel>>(availableTables.Result);

                    var query = $"{Constants.API.HOST_URL}/api/orders/{orderId.Result}";
                    var order = await _restService.RequestAsync<GenericExecutionResult<GetOrderByIdQueryResult>>(HttpMethod.Get, query);

                    CurrentOrder = _mapper.Map<FullOrderBindableModel>(order?.Value?.Order);
                    CurrentOrder.Seats = new();

                    //var tax = await GetTaxAsync();

                    //if (tax.IsSuccess)
                    //{
                    //    CurrentOrder.Tax = tax.Result;
                    //}

                    //CurrentOrder.Id = orderId.Result;
                    //CurrentOrder.OrderNumber = orderId.Result;
                    //CurrentOrder.OrderStatus = Constants.OrderStatus.IN_PROGRESS;
                    //CurrentOrder.OrderType = Enums.EOrderType.DineIn;
                    //CurrentOrder.Table = tableBindableModels.FirstOrDefault();

                    //CurrentSeat = null;
                    result.SetSuccess();
                }
                else
                {
                    result.SetFailure();
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(CreateNewCurrentOrderAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public async Task<AOResult> AddSetInCurrentOrderAsync(DishBindableModel dish)
        {
            var result = new AOResult();

            try
            {
                if (CurrentSeat is null)
                {
                    var seat = new SeatBindableModel
                    {
                        SeatNumber = 1,
                        SelectedDishes = new(),
                        Checked = true,
                        IsFirstSeat = true,
                    };

                    CurrentOrder.Seats.Add(seat);

                    CurrentSeat = seat;
                }

                CurrentOrder.Seats[CurrentOrder.Seats.IndexOf(CurrentSeat)].SelectedDishes.Add(dish);

                CurrentOrder.SubTotalPrice = CurrentOrder.Seats.Sum(row => row.SelectedDishes.Sum(row => row.TotalPrice));

                CurrentOrder.PriceTax = (decimal)(CurrentOrder.SubTotalPrice * CurrentOrder.TaxCoefficient);

                CurrentOrder.TotalPrice = (decimal)(CurrentOrder.SubTotalPrice + CurrentOrder.PriceTax);

                //if (CurrentOrder.BonusType != Enums.EBonusType.None)
                //{
                //    CurrentOrder = await _bonusService.СalculationBonusAsync(CurrentOrder);
                //}
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

        public async Task<AOResult> DeleteSetFromCurrentSeat()
        {
            var result = new AOResult();

            try
            {
                //SetBindableModel? setTobeRemoved = CurrentOrder.Seats.FirstOrDefault(x => x.SelectedItem is not null)?.SelectedItem;
                //if (setTobeRemoved is not null)
                //{
                //   // CurrentOrder.Seats.FirstOrDefault(x => x.SelectedItem is not null).Sets.Remove(setTobeRemoved);
                //   // CurrentOrder.SubTotal -= setTobeRemoved.TotalPrice;
                //   // CurrentOrder.PriceTax = CurrentOrder.SubTotal * CurrentOrder.Tax.Value;
                //   // CurrentOrder.Total = CurrentOrder.SubTotal + CurrentOrder.PriceTax;
                //}
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