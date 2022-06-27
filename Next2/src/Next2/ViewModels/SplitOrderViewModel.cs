using AutoMapper;
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
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels
{
    public class SplitOrderViewModel : BaseViewModel
    {
        private readonly IOrderService _orderService;
        private readonly IPopupNavigation _popupNavigation;

        private bool isOneTime = true;
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

        private ICommand _goBackCommand;
        public ICommand GoBackCommand => _goBackCommand ??= new AsyncCommand(OnGoBackCommandAsync, allowsMultipleExecutions: false);

        private ICommand _splitByCommand;
        public ICommand SplitByCommand => _splitByCommand ??= new AsyncCommand<ESplitOrderConditions>(OnSplitByCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Overrides --

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if (parameters.TryGetValue(Constants.Navigations.ORDER_ID, out Guid id))
            {
                var response = await _orderService.GetOrderByIdAsync(id);

                if (response.IsSuccess)
                {
                    Order = response.Result;

                    var seats = Order.Seats.ToSeatsBindableModels();

                    Seats = new(seats);

                    foreach (var seat in Seats)
                    {
                        seat.SetSelectionCommand = new AsyncCommand<object?>(OnDishSelectionCommand);
                    }

                    if (App.IsTablet)
                    {
                        SelectFirstDish();
                    }
                }
            }
        }

        #endregion

        #region -- Private Helpers --

        private async Task OnSplitByCommandAsync(ESplitOrderConditions condition)
        {
            var param = new DialogParameters
            {
                { Constants.DialogParameterKeys.DESCRIPTION, condition },
                { Constants.DialogParameterKeys.SEATS, Seats },
                { Constants.DialogParameterKeys.DISH, SelectedDish },
            };

            PopupPage popupPage = App.IsTablet
                ? new Views.Tablet.Dialogs.SplitOrderDialog(param, SplitOrderDialogCallBack)
                : new Views.Mobile.Dialogs.SplitOrderDialog(param, SplitOrderDialogCallBack);

            await _popupNavigation.PushAsync(popupPage);
        }

        private async void SplitOrderDialogCallBack(IDialogParameters dialogResult)
        {
            if (dialogResult.TryGetValue(Constants.DialogParameterKeys.SEATS, out List<SeatBindableModel> seats))
            {
                foreach (var seat in Seats)
                {
                    var incSeat = seats.FirstOrDefault(x => x.SeatNumber == seat.SeatNumber);

                    if (incSeat is not null)
                    {
                        var dish = SelectedDish.Clone() as DishBindableModel;
                        dish.TotalPrice = incSeat.SelectedItem.TotalPrice;
                        seat.SelectedDishes.Add(dish);
                    }
                }

                if (SelectedDish.TotalPrice == 0)
                {
                    var seat = Seats.FirstOrDefault(x => x.SelectedDishes.Any(x => x.TotalPrice == 0));

                    seat.SelectedDishes.Remove(SelectedDish);

                    if (seat.SelectedDishes.Count == 0)
                    {
                        Seats.Remove(seat);
                    }
                }

                await RefreshDisplay();
            }

            if (dialogResult.TryGetValue(Constants.DialogParameterKeys.SPLIT_GROUPS, out List<int[]> groupList))
            {
                await SplitOrderBySeats(groupList);
            }

            await _popupNavigation.PopAsync();
        }

        private async Task SplitOrderBySeats(IList<int[]> groupList)
        {
            foreach (var group in groupList)
            {
                var seats = Seats.Where(s => group.Any(x => x == s.SeatNumber));
                var outSeats = seats.ToSeatsModelsDTO();

                if (group.Contains(_selectedSeatNumber))
                {
                    Order.Seats = outSeats;
                    CalculateOrderPrices(Order);
                    var updateResult = await _orderService.UpdateOrderAsync(Order.ToUpdateOrderCommand());

                    if (!updateResult.IsSuccess)
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

                    var orderIdResult = await _orderService.CreateNewOrderAndGetIdAsync();

                    if (orderIdResult.IsSuccess)
                    {
                        var incOrderResult = await _orderService.GetOrderByIdAsync(orderIdResult.Result);

                        if (incOrderResult.IsSuccess)
                        {
                            var order = incOrderResult.Result;
                            await CopyCurrentOrderDataTo(order);
                            order.Seats = outSeats;
                            CalculateOrderPrices(order);
                            var updateResult = await _orderService.UpdateOrderAsync(order.ToUpdateOrderCommand());
                            if (!updateResult.IsSuccess)
                            {
                                break;
                            }
                        }
                    }
                }
            }

            await OnGoBackCommandAsync();
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

        private async Task CopyCurrentOrderDataTo(OrderModelDTO order)
        {
            if (await UpdateTableAsync())
            {
                order.IsCashPayment = Order.IsCashPayment;
                order.IsTab = Order.IsTab;
                order.Open = Order.Open;
                order.OrderStatus = Order.OrderStatus;
                order.TaxCoefficient = Order.TaxCoefficient;
                order.OrderType = Order.OrderType;
                order.Open = DateTime.Now;
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
            SelectedDish = Seats.FirstOrDefault().SelectedDishes.FirstOrDefault();
            Seats.FirstOrDefault().SelectedItem = Seats.FirstOrDefault().SelectedDishes.FirstOrDefault();
            Seats.FirstOrDefault().Checked = true;
        }

        private Task OnDishSelectionCommand(object? sender)
        {
            if (isOneTime)
            {
                isOneTime = false;
                var seat = sender as SeatBindableModel;

                foreach (var item in Seats.Where(x => x.SeatNumber != seat.SeatNumber))
                {
                    item.SelectedItem = null;
                    item.Checked = false;
                }

                SelectedDish = seat.SelectedItem;
                _selectedSeatNumber = seat.SeatNumber;
                seat.Checked = true;
                isOneTime = true;
            }

            return Task.CompletedTask;
        }

        private Task RefreshDisplay()
        {
            var imageSource = SelectedDish.ImageSource;
            SelectedDish.ImageSource = null;
            SelectedDish.ImageSource = imageSource;

            var seats = new ObservableCollection<SeatBindableModel>(Seats);
            Seats = new();

            foreach (var seat in seats)
            {
                Seats.Add(seat);
            }

            return Task.CompletedTask;
        }

        private async Task OnGoBackCommandAsync()
        {
            await _navigationService.GoBackAsync();
        }

        #endregion

    }
}
