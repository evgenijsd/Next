using Acr.UserDialogs;
using Next2.Enums;
using Next2.Extensions;
using Next2.Models.API.DTO;
using Next2.Models.Bindables;
using Next2.Services.Authentication;
using Next2.Services.Notifications;
using Next2.Services.Order;
using Prism.Navigation;
using Prism.Services.Dialogs;
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

        private bool _isOneTime = true;
        private int _selectedSeatNumber = 0;

        public SplitOrderViewModel(
            INavigationService navigationService,
            IAuthenticationService authenticationService,
            INotificationsService notificationsService,
            IOrderService orderService)
            : base(navigationService, authenticationService, notificationsService)
        {
            _orderService = orderService;
        }

        #region -- Public properties --

        public OrderModelDTO Order { get; set; } = new();

        public ObservableCollection<SeatBindableModel> Seats { get; set; } = new();

        public IEnumerable<SeatModelDTO>? OriginalSeats { get; set; }

        public DishBindableModel SelectedDish { get; set; } = new();

        private ICommand? _goBackCommand;
        public ICommand GoBackCommand => _goBackCommand ??= new AsyncCommand(OnGoBackCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _splitByCommand;
        public ICommand SplitByCommand => _splitByCommand ??= new AsyncCommand<ESplitOrderConditions>(OnSplitByCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _selectDishCommand;
        public ICommand SelectDishCommand => _selectDishCommand ??= new AsyncCommand<object?>(OnSelectDishCommandAsync);

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

        private async void SplitOrderDialogCallBack(IDialogParameters parameters)
        {
            if (parameters.TryGetValue(Constants.DialogParameterKeys.SEATS, out List<SeatBindableModel> seats))
            {
                await SplitSelectedDish(seats);
                await RemoveNullPriceDishes();
                await RefreshDisplay();
                await PopupNavigation.PopAsync();

                SelectFirstDish();

                await UpdateOrderAsync();
            }
            else if (parameters.TryGetValue(Constants.DialogParameterKeys.SPLIT_GROUPS, out List<int[]> splittedBySeatsGroups))
            {
                await SplitOrderBySeatsAsync(splittedBySeatsGroups);
            }
            else
            {
                await _notificationsService.CloseAllPopupAsync();
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

                _orderService.CalculateOrderPrices(order);

                var updateResult = await _orderService.UpdateOrderAsync(order);
                isOrderUpdated = updateResult.IsSuccess;

                if (!isOrderUpdated)
                {
                    await EmergencyRestoreSeatsAsync(updateResult.Exception?.Message);
                }
            }
            else
            {
                await EmergencyRestoreSeatsAsync(createOrderResult.Exception?.Message);
            }

            return isOrderUpdated;
        }

        private async Task<bool> UpdateSplittedOrderAsync(IEnumerable<SeatModelDTO> seats)
        {
            Order.Seats = seats;

            _orderService.CalculateOrderPrices(Order);

            var updateResult = await _orderService.UpdateOrderAsync(Order);

            if (!updateResult.IsSuccess)
            {
                await EmergencyRestoreSeatsAsync(updateResult.Exception?.Message);
            }

            return updateResult.IsSuccess;
        }

        private async Task OnSplitByCommandAsync(ESplitOrderConditions condition)
        {
            if (Seats.Count > 1)
            {
                if (SelectedDish.IsSplitted && condition is not ESplitOrderConditions.BySeats)
                {
                    await _notificationsService.ShowInfoDialogAsync(
                        LocalizationResourceManager.Current["Warning"],
                        LocalizationResourceManager.Current["ThisDishAlreadySplitted"],
                        LocalizationResourceManager.Current["Ok"]);
                }
                else
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

                    await PopupNavigation.PushAsync(popupPage);
                }
            }
            else
            {
                await _notificationsService.ShowInfoDialogAsync(
                    LocalizationResourceManager.Current["Warning"],
                    LocalizationResourceManager.Current["CannotSplitWithoutSeats"],
                    LocalizationResourceManager.Current["Ok"]);
            }
        }

        private async Task UpdateOrderAsync()
        {
            Order.Seats = Seats.ToSeatsModelsDTO();

            var orderUpdateResult = await _orderService.UpdateOrderAsync(Order);

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
                await EmergencyRestoreSeatsAsync(orderUpdateResult.Exception?.Message);
            }
        }

        private async Task EmergencyRestoreSeatsAsync(string? exeptionMessage)
        {
            await _notificationsService.CloseAllPopupAsync();

            await ResponseToUnsuccessfulRequestAsync(exeptionMessage);

            RestoreSeats();
        }

        private async Task SplitOrderBySeatsAsync(IList<int[]> splittedBySeatsGroups)
        {
            int successfullCounter = 0;

            await _notificationsService.CloseAllPopupAsync();

            foreach (var group in splittedBySeatsGroups)
            {
                var seats = Seats.Where(s => group.Any(x => x == s.SeatNumber));
                var outSeats = seats.ToSeatsModelsDTO();

                bool isSuccessfull = false;

                if (outSeats is not null)
                {
                    if (group.Contains(_selectedSeatNumber))
                    {
                        isSuccessfull = await UpdateSplittedOrderAsync(outSeats);
                    }
                    else
                    {
                        isSuccessfull = await CreateNewSplittedOrderAsync(outSeats);
                    }
                }

                if (isSuccessfull)
                {
                    successfullCounter++;
                }
                else
                {
                    break;
                }
            }

            if (successfullCounter == splittedBySeatsGroups.Count)
            {
                OriginalSeats = Order.Seats;
                var seatsNumbers = Order.Seats.Select(x => x.Number);
                Seats = new(Seats.Where(x => seatsNumbers.Contains(x.SeatNumber)));

                var toastConfig = new ToastConfig(LocalizationResourceManager.Current["OrderSplitted"])
                {
                    Duration = TimeSpan.FromSeconds(Constants.Limits.TOAST_DURATION),
                    Position = ToastPosition.Bottom,
                };

                UserDialogs.Instance.Toast(toastConfig);
            }
        }

        private async Task CopyCurrentOrderTo(OrderModelDTO order)
        {
            if (Order.Table is not null)
            {
                var updateTableResult = await _orderService.UpdateTableAsync(Order.Table, true);

                if (updateTableResult.IsSuccess)
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
                else
                {
                    await ResponseToUnsuccessfulRequestAsync(updateTableResult.Exception?.Message);
                }
            }
            else
            {
                await ResponseToUnsuccessfulRequestAsync(string.Empty);
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
                        dish.TotalPrice = incomingSeat.SelectedItem is null
                            ? 0
                            : incomingSeat.SelectedItem.TotalPrice;
                        dish.DiscountPrice = dish.TotalPrice;
                        dish.SplitPrice = dish.TotalPrice;
                        dish.IsSplitted = true;
                        seat.SelectedDishes.Add(dish);
                    }
                }
            }

            SelectedDish.IsSplitted = true;
            SelectedDish.DiscountPrice = SelectedDish.TotalPrice;
            SelectedDish.SplitPrice = SelectedDish.TotalPrice;

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

        private Task OnSelectDishCommandAsync(object? sender)
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
