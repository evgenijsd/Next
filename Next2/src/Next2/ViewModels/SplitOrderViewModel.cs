using Acr.UserDialogs;
using Next2.Enums;
using Next2.Extensions;
using Next2.Models.API.Commands;
using Next2.Models.API.DTO;
using Next2.Models.Bindables;
using Next2.Services.Order;
using Prism.Navigation;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Contracts;
using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels
{
    public class SplitOrderViewModel : BaseViewModel
    {
        private readonly IOrderService _orderService;
        private readonly IPopupNavigation _popupNavigation;

        private bool _isOneTime = true;
        private int _selectedSeatNumber = 0;

        public SplitOrderViewModel(
            INavigationService navigationService,
            IPopupNavigation popupNavigation,
            IOrderService orderService)
            : base(navigationService)
        {
            _orderService = orderService;
            _popupNavigation = popupNavigation;
        }

        #region -- Public properties --

        public OrderModelDTO Order { get; set; }

        public DishBindableModel SelectedDish { get; set; }

        public ObservableCollection<SeatBindableModel> Seats { get; set; } = new();

        public IEnumerable<SeatModelDTO>? OriginalSeats { get; set; }

        private ICommand _goBackCommand;
        public ICommand GoBackCommand => _goBackCommand ??= new AsyncCommand(OnGoBackCommandAsync, allowsMultipleExecutions: false);

        private ICommand _splitByCommand;
        public ICommand SplitByCommand => _splitByCommand ??= new AsyncCommand<ESplitOrderConditions>(OnSplitByCommandAsync, allowsMultipleExecutions: false);

        private ICommand _selectDishCommand;
        public ICommand SelectDishCommand => _selectDishCommand ??= new AsyncCommand<object?>(OnDishSelectionCommandAsync);

        #endregion

        #region -- Overrides --

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if (parameters.TryGetValue(Constants.Navigations.ORDER, out OrderModelDTO order))
            {
                Order = order;

                OriginalSeats = Order.Seats;

                InitSeats();
            }
        }

        #endregion

        #region -- Private helpers --

        private void RestoreSeats()
        {
            if (OriginalSeats is not null)
            {
                var seats = OriginalSeats.ToSeatsBindableModels();

                InitSeats(seats);
            }
        }

        private async Task<bool> LoadOrderAndInitAsync()
        {
            var orderId = Order.Id;
            var resultOfGettingOrder = await _orderService.GetOrderByIdAsync(orderId);

            var isloadedSuccess = resultOfGettingOrder.IsSuccess;

            if (isloadedSuccess)
            {
                Order = resultOfGettingOrder.Result;

                OriginalSeats = Order.Seats;

                InitSeats();
            }

            return isloadedSuccess;
        }

        private void InitSeats(IEnumerable<SeatBindableModel>? seats = null)
        {
            if (seats is null)
            {
                seats = Order.Seats.ToSeatsBindableModels();
            }

            Seats = new(seats);

            foreach (var seat in Seats)
            {
                seat.DishSelectionCommand = SelectDishCommand;
            }

            SelectFirstDish();
        }

        private async Task OnSplitByCommandAsync(ESplitOrderConditions condition)
        {
            if (Seats.Count > 1)
            {
                if (condition == ESplitOrderConditions.BySeats || !SelectedDish.HasSplittedPrice)
                {
                    var param = new DialogParameters
                    {
                        { Constants.DialogParameterKeys.CONDITION, condition },
                        { Constants.DialogParameterKeys.SEATS, Seats },
                        { Constants.DialogParameterKeys.DISH, SelectedDish },
                    };

                    PopupPage popupPage = App.IsTablet
                        ? new Views.Tablet.Dialogs.SplitOrderDialog(param, SplitOrderDialogCallBack)
                        : new Views.Mobile.Dialogs.SplitOrderDialog(param, SplitOrderDialogCallBack);

                    await _popupNavigation.PushAsync(popupPage);
                }
            }
        }

        private async void SplitOrderDialogCallBack(IDialogParameters parameters)
        {
            if (parameters.TryGetValue(Constants.DialogParameterKeys.SEATS, out List<SeatBindableModel> seats))
            {
                await SplitSelectedDish(seats);
                await RemoveNullPriceDishes();
                await RefreshDisplay();
                await _popupNavigation.PopAsync();

                SelectFirstDish();

                await UpdateOrderAsync();
            }
            else if (parameters.TryGetValue(Constants.DialogParameterKeys.SPLIT_GROUPS, out List<int[]> groupList))
            {
                await SplitOrderBySeats(groupList);
            }
            else if (_popupNavigation.PopupStack.Count > 0)
            {
                await _popupNavigation.PopAllAsync();
            }
        }

        private async Task UpdateOrderAsync()
        {
            Order.Seats = Seats.ToSeatsModelsDTO();
            var updateOrderCommand = Order.ToUpdateOrderCommand();

            var orderUpdateResult = await _orderService.UpdateOrderAsync(updateOrderCommand);

            if (orderUpdateResult.IsSuccess)
            {
                OriginalSeats = Order.Seats;

                var toastConfig = new ToastConfig(LocalizationResourceManager.Current["OrderUpdated"])
                {
                    Duration = TimeSpan.FromSeconds(Constants.Limits.TOAST_DURATION),
                    Position = ToastPosition.Bottom,
                };

                UserDialogs.Instance.Toast(toastConfig);
            }
            else
            {
                await EmergencyReloadOrderAsync(orderUpdateResult.Exception.Message);
            }
        }

        private async Task<bool> UpdateSplittedOrderAsync(IEnumerable<SeatModelDTO> seats)
        {
            Order.Seats = seats;

            CalculateOrderPrices(Order);

            var updateResult = await _orderService.UpdateOrderAsync(Order.ToUpdateOrderCommand());

            if (!updateResult.IsSuccess)
            {
                await EmergencyReloadOrderAsync(updateResult.Exception.Message);
            }

            return updateResult.IsSuccess;
        }

        private async Task EmergencyReloadOrderAsync(string exeptionMessage)
        {
            if (_popupNavigation.PopupStack.Count > 0)
            {
                await _popupNavigation.PopAllAsync();
            }

            await ResponseToBadRequestAsync(exeptionMessage);

            RestoreSeats();

            await LoadOrderAndInitAsync();
        }

        private async Task SplitOrderBySeats(IList<int[]> groupList)
        {
            int successfulUpdatesCounter = 0;

            if (_popupNavigation.PopupStack.Count > 0)
            {
                await _popupNavigation.PopAsync();
            }

            foreach (var group in groupList)
            {
                var seats = Seats.Where(s => group.Any(x => x == s.SeatNumber));
                var outSeats = seats.ToSeatsModelsDTO();

                if (group.Contains(_selectedSeatNumber))
                {
                    var isOrderUpdated = await UpdateSplittedOrderAsync(outSeats);

                    if (isOrderUpdated)
                    {
                        successfulUpdatesCounter++;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    var createResult = await CreateNewSplittedOrderAsync(outSeats);

                    if (createResult)
                    {
                        successfulUpdatesCounter++;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (successfulUpdatesCounter == groupList.Count)
            {
                OriginalSeats = Order.Seats;
                Seats = new(Order.Seats.ToSeatsBindableModels());

                var toastConfig = new ToastConfig(LocalizationResourceManager.Current["OrderSplitted"])
                {
                    Duration = TimeSpan.FromSeconds(Constants.Limits.TOAST_DURATION),
                    Position = ToastPosition.Bottom,
                };

                UserDialogs.Instance.Toast(toastConfig);
            }
        }

        private async Task<bool> CreateNewSplittedOrderAsync(IEnumerable<SeatModelDTO> seats)
        {
            var createOrderResult = await _orderService.CreateNewOrderAsync();
            bool isOrderUpdated = false;

            if (createOrderResult.IsSuccess)
            {
                var order = createOrderResult.Result;

                await CopyCurrentOrderTo(order);

                order.Seats = seats;
                order.Open = DateTime.Now;

                CalculateOrderPrices(order);

                var updateResult = await _orderService.UpdateOrderAsync(order.ToUpdateOrderCommand());
                isOrderUpdated = updateResult.IsSuccess;

                if (!isOrderUpdated)
                {
                    await EmergencyReloadOrderAsync(updateResult.Exception.Message);
                }
            }
            else
            {
                await EmergencyReloadOrderAsync(createOrderResult.Exception.Message);
            }

            return isOrderUpdated;
        }

        private void CalculateOrderPrices(OrderModelDTO order)
        {
            var dishes = order.Seats.SelectMany(x => x.SelectedDishes);

            foreach (var dish in dishes)
            {
                dish.DiscountPrice = dish.TotalPrice;
            }

            order.TotalPrice = dishes.Sum(x => x.TotalPrice);
            order.DiscountPrice = dishes.Sum(x => x.DiscountPrice);
        }

        private async Task CopyCurrentOrderTo(OrderModelDTO order)
        {
            var isTableUpdated = await UpdateTableAsync();

            if (isTableUpdated)
            {
                order.IsCashPayment = Order.IsCashPayment;
                order.IsTab = Order.IsTab;
                order.Open = Order.Open;
                order.OrderStatus = Order.OrderStatus;
                order.TaxCoefficient = Order.TaxCoefficient;
                order.OrderType = Order.OrderType;
                order.Coupon = Order.Coupon;
                order.Discount = Order.Discount;
                order.TotalPrice = Order.TotalPrice;
                order.Table = new SimpleTableModelDTO
                {
                    Id = Order.Table.Id,
                    Number = Order.Table.Number,
                    SeatNumbers = Order.Table.SeatNumbers,
                };
            }
        }

        private async Task<bool> UpdateTableAsync()
        {
            UpdateTableCommand command = new()
            {
                Id = Order.Table.Id,
                Number = Order.Table.Number,
                SeatNumbers = Order.Table.SeatNumbers,
                IsAvailable = true,
            };

            var createTableResult = await _orderService.UpdateTableAsync(command);

            return createTableResult.IsSuccess;
        }

        private void SelectFirstDish()
        {
            foreach (var seat in Seats)
            {
                seat.IsFirstSeat = false;
                seat.Checked = false;
                seat.SelectedItem = null;
            }

            if (App.IsTablet)
            {
                var firstSeat = Seats.FirstOrDefault(x => x.SelectedDishes.Count > 0);
                _selectedSeatNumber = firstSeat.SeatNumber;
                firstSeat.SelectedItem = firstSeat.SelectedDishes.FirstOrDefault();
                SelectedDish = firstSeat.SelectedItem;
                firstSeat.Checked = true;
                firstSeat.IsFirstSeat = true;
            }
            else
            {
                Seats.First().IsFirstSeat = true;
                _selectedSeatNumber = Seats.First().SeatNumber;
            }
        }

        private Task SplitSelectedDish(List<SeatBindableModel> seats)
        {
            foreach (var seat in Seats)
            {
                var incomingSeat = seats.FirstOrDefault(x => x.SeatNumber == seat.SeatNumber);

                if (incomingSeat is not null)
                {
                    if (SelectedDish.Clone() is DishBindableModel dish)
                    {
                        dish.TotalPrice = incomingSeat.SelectedItem.TotalPrice;
                        dish.HasSplittedPrice = true;
                        seat.SelectedDishes.Add(dish);
                    }
                }
            }

            SelectedDish.HasSplittedPrice = true;

            return Task.CompletedTask;
        }

        private Task RemoveNullPriceDishes()
        {
            foreach (var seat in Seats)
            {
                seat.SelectedDishes = new(seat.SelectedDishes.Where(x => x.TotalPrice > 0));
            }

            return Task.CompletedTask;
        }

        private Task OnDishSelectionCommandAsync(object? sender)
        {
            if (_isOneTime)
            {
                _isOneTime = false;

                if (sender is SeatBindableModel seat)
                {
                    foreach (var item in Seats.Where(x => x.SeatNumber != seat.SeatNumber))
                    {
                        item.SelectedItem = null;
                        item.Checked = false;
                    }

                    SelectedDish = seat.SelectedItem;
                    _selectedSeatNumber = seat.SeatNumber;
                    seat.Checked = true;
                    _isOneTime = true;
                }
            }

            return Task.CompletedTask;
        }

        private Task RefreshDisplay()
        {
            var imageSource = SelectedDish.ImageSource;
            SelectedDish.ImageSource = null;
            SelectedDish.ImageSource = imageSource;

            Seats = new(Seats);

            return Task.CompletedTask;
        }

        private Task OnGoBackCommandAsync()
        {
            return _navigationService.GoBackAsync();
        }

        #endregion
    }
}
