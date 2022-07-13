﻿using Acr.UserDialogs;
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

        #region -- Public Properties --

        public OrderModelDTO Order { get; set; }

        public DishBindableModel SelectedDish { get; set; }

        public ObservableCollection<SeatBindableModel> Seats { get; set; } = new();

        public IEnumerable<SeatModelDTO> OldSeats { get; set; }

        private ICommand _goBackCommand;
        public ICommand GoBackCommand => _goBackCommand ??= new AsyncCommand(OnGoBackCommandAsync, allowsMultipleExecutions: false);

        private ICommand _splitByCommand;
        public ICommand SplitByCommand => _splitByCommand ??= new AsyncCommand<ESplitOrderConditions>(OnSplitByCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Overrides --

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if (parameters.TryGetValue(Constants.Navigations.ORDER_ID, out Guid orderId))
            {
                if (OldSeats is not null)
                {
                    var seats = OldSeats.ToSeatsBindableModels();
                    Seats = new(seats);

                    foreach (var seat in Seats)
                    {
                        seat.DishSelectionCommand = new AsyncCommand<object?>(OnDishSelectionCommand);
                    }

                    SelectFirstDish();
                }

                var resultOfGettingOrder = await _orderService.GetOrderByIdAsync(orderId);

                if (resultOfGettingOrder.IsSuccess)
                {
                    if (resultOfGettingOrder.Result?.Seats?.Count() > 0)
                    {
                        Order = resultOfGettingOrder.Result;

                        OldSeats = Order.Seats;

                        var seats = Order.Seats.ToSeatsBindableModels();

                        Seats = new(seats);

                        foreach (var seat in Seats)
                        {
                            seat.DishSelectionCommand = new AsyncCommand<object?>(OnDishSelectionCommand);
                        }

                        SelectFirstDish();
                    }
                    else
                    {
                        OnGoBackCommandAsync();
                    }
                }
            }
        }

        #endregion

        #region -- Private Helpers --

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
            else
            {
                if (_popupNavigation.PopupStack.Count > 0)
                {
                    await _popupNavigation.PopAllAsync();
                }
            }
        }

        private async Task UpdateOrderAsync()
        {
            Order.Seats = Seats.ToSeatsModelsDTO();
            var updateOrderCommand = Order.ToUpdateOrderCommand();

            var res = await _orderService.UpdateOrderAsync(updateOrderCommand);

            if (res.IsSuccess)
            {
                OldSeats = Order.Seats;

                var toastConfig = new ToastConfig(LocalizationResourceManager.Current["OrderUpdated"])
                {
                    Duration = TimeSpan.FromSeconds(Constants.Limits.TOAST_DURATION),
                    Position = ToastPosition.Bottom,
                };

                UserDialogs.Instance.Toast(toastConfig);
            }
            else
            {
                await EmergencyReloadOrderAsync();
            }
        }

        private async Task EmergencyReloadOrderAsync()
        {
            if (_popupNavigation.PopupStack.Count > 0)
            {
                await _popupNavigation.PopAllAsync();
            }

            await ShowInfoDialogAsync(
                LocalizationResourceManager.Current["Error"],
                LocalizationResourceManager.Current["NoInternetConnection"],
                LocalizationResourceManager.Current["Ok"]);

            var param = new NavigationParameters
                {
                    { Constants.Navigations.ORDER_ID, Order.Id },
                };

            OnNavigatedTo(param);
        }

        private async Task SplitOrderBySeats(IList<int[]> groupList)
        {
            int goodUpdateCounter = 0;

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
                    Order.Seats = outSeats;

                    CalculateOrderPrices(Order);

                    var updateResult = await _orderService.UpdateOrderAsync(Order.ToUpdateOrderCommand());

                    if (updateResult.IsSuccess)
                    {
                        OldSeats = Order.Seats;
                        Seats = new(Order.Seats.ToSeatsBindableModels());
                        goodUpdateCounter++;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    Enum.TryParse(Order.OrderType, out EOrderType orderType);

                    var createOrderCommand = new CreateOrderCommand
                    {
                        OrderType = orderType,
                        EmployeeId = Order?.EmployeeId,
                        IsTab = Order.IsTab,
                        TableId = Order?.Table?.Id
                    };

                    var orderIdResult = await _orderService.CreateNewOrderAsync();

                    if (orderIdResult.IsSuccess)
                    {
                        var order = orderIdResult.Result;

                        await CopyCurrentOrderTo(order);

                        order.Seats = outSeats;
                        order.Open = DateTime.Now;

                        CalculateOrderPrices(order);

                        var updateResult = await _orderService.UpdateOrderAsync(order.ToUpdateOrderCommand());

                        if (updateResult.IsSuccess)
                        {
                            goodUpdateCounter++;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            if (goodUpdateCounter == groupList.Count)
            {
                var toastConfig = new ToastConfig(LocalizationResourceManager.Current["OrderSplitted"])
                {
                    Duration = TimeSpan.FromSeconds(Constants.Limits.TOAST_DURATION),
                    Position = ToastPosition.Bottom,
                };

                UserDialogs.Instance.Toast(toastConfig);
            }
            else
            {
                await EmergencyReloadOrderAsync();
            }
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

        private Task OnDishSelectionCommand(object? sender)
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
