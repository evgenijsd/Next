﻿using AutoMapper;
using Next2.Enums;
using Next2.Interfaces;
using Next2.Models.API.DTO;
using Next2.Models.Bindables;
using Next2.Services.Notifications;
using Next2.Services.Order;
using Next2.Views.Mobile.Dialogs;
using Prism.Navigation;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Contracts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Next2.ViewModels
{
    public class PrintReceiptViewModel : BaseViewModel
    {
        private readonly IOrderService _orderService;
        private readonly INotificationsService _notificationsService;
        private readonly IMapper _mapper;

        private IEnumerable<SimpleOrderBindableModel> _allClosedOrders = Enumerable.Empty<SimpleOrderBindableModel>();

        public PrintReceiptViewModel(
            INavigationService navigationService,
            IMapper mapper,
            INotificationsService notificationsService,
            IOrderService orderService)
            : base(navigationService)
        {
            _orderService = orderService;
            _notificationsService = notificationsService;
            _mapper = mapper;
        }

        #region -- Public properties --

        public int IndexLastVisibleElement { get; set; }

        public ObservableCollection<SimpleOrderBindableModel> Orders { get; set; } = new();

        public SimpleOrderBindableModel? SelectedOrder { get; set; }

        public DateTime SelectedDate { get; set; } = DateTime.Now;

        public bool IsNothingFound => !Orders.Any();

        public bool IsOrdersRefreshing { get; set; }

        public bool IsPreloadStateActive => !IsInternetConnected || (IsOrdersRefreshing && !Orders.Any());

        public EOrdersSortingType OrderSortingType { get; set; }

        private ICommand? _refreshOrdersCommand;
        public ICommand RefreshOrdersCommand => _refreshOrdersCommand ??= new AsyncCommand(OnRefreshOrdersCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _selectOrderCommand;
        public ICommand SelectOrderCommand => _selectOrderCommand ??= new Command<SimpleOrderBindableModel?>(OnSelectOrderCommand);

        private ICommand? _changeSortOrderCommand;
        public ICommand ChangeSortOrderCommand => _changeSortOrderCommand ??= new Command<EOrdersSortingType>(OnChangeSortOrderCommand);

        private ICommand? _selectingDateCommand;
        public ICommand SelectingDateCommand => _selectingDateCommand ??= new AsyncCommand(OnSelectingDateCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Overrides --

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            if (args.PropertyName == nameof(SelectedDate))
            {
                FilterOrdersByDateAsync(SelectedDate);

                if (!Orders.Contains(SelectedOrder))
                {
                    SelectedOrder = null;
                }
            }
        }

        public override void OnAppearing()
        {
            base.OnAppearing();

            IsOrdersRefreshing = true;
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        #endregion

        #region -- Private helpers --

        private Task OnSelectingDateCommandAsync()
        {
            var param = new DialogParameters()
            {
                { Constants.DialogParameterKeys.SELECTED_DATE, SelectedDate },
            };

            var popupPage = new SelectDateDialog(param, CloseDialogCallBack);

            return PopupNavigation.PushAsync(popupPage);
        }

        private async void CloseDialogCallBack(IDialogParameters parameters)
        {
            if (parameters.TryGetValue(Constants.DialogParameterKeys.SELECTED_DATE, out DateTime selectedDate))
            {
                SelectedDate = selectedDate;
            }

            await PopupNavigation.PopAllAsync();
        }

        private async Task OnRefreshOrdersCommandAsync()
        {
            bool isOrdersLoaded = false;
            SelectedOrder = null;

            if (IsInternetConnected)
            {
                var gettingOrdersResult = await _orderService.GetOrdersAsync(EOrderStatus.Closed);

                if (gettingOrdersResult.IsSuccess)
                {
                    var closedOrders = gettingOrdersResult.Result;

                    Orders = _mapper.Map<IEnumerable<SimpleOrderModelDTO>, ObservableCollection<SimpleOrderBindableModel>>(closedOrders);

                    _allClosedOrders = Orders.Select(x => x);

                    await AddDummyClosingDateToOrders();
                    await FilterOrdersByDateAsync(SelectedDate);

                    isOrdersLoaded = true;
                }
                else
                {
                    await _notificationsService.ResponseToBadRequestAsync(gettingOrdersResult.Exception?.Message);
                }
            }
            else
            {
                await _notificationsService.ShowInfoDialogAsync(
                    LocalizationResourceManager.Current["Error"],
                    LocalizationResourceManager.Current["NoInternetConnection"],
                    LocalizationResourceManager.Current["Ok"]);
            }

            if (!isOrdersLoaded)
            {
                Orders = new();
            }

            IsOrdersRefreshing = false;
        }

        private Task AddDummyClosingDateToOrders()
        {
            var ordersCount = _allClosedOrders.Count();
            var ordersArray = _allClosedOrders.ToArray();

            var currentDate = DateTime.Now;
            var rand = new Random();
            var previousDay = currentDate;
            var theDayBeforeYesterday = currentDate;

            previousDay = previousDay.AddDays(-1);
            theDayBeforeYesterday = theDayBeforeYesterday.AddDays(-2);

            for (int i = 0; i < ordersCount; i++)
            {
                var randomHours = rand.Next(0, 23);
                var randomMinutes = rand.Next(0, 59);

                if (i < ordersCount / 3)
                {
                    ordersArray[i].Close = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, randomHours, randomMinutes, 0);
                }
                else if (i < ordersCount / 2)
                {
                    ordersArray[i].Close = new DateTime(currentDate.Year, previousDay.Month, previousDay.Day, randomHours, randomMinutes, 0);
                }
                else
                {
                    ordersArray[i].Close = new DateTime(currentDate.Year, theDayBeforeYesterday.Month, theDayBeforeYesterday.Day, randomHours, randomMinutes, 0);
                }
            }

            return Task.CompletedTask;
        }

        private Task FilterOrdersByDateAsync(DateTime date)
        {
            Orders = new(_allClosedOrders.Where(x => x.Close.ToShortDateString() == date.ToShortDateString()));
            return Task.CompletedTask;
        }

        private void OnSelectOrderCommand(SimpleOrderBindableModel? order)
        {
            SelectedOrder = order == SelectedOrder
                ? null
                : order;
        }

        private void OnChangeSortOrderCommand(EOrdersSortingType orderSortingType)
        {
            if (OrderSortingType == orderSortingType)
            {
                Orders = new(Orders.Reverse());
            }
            else
            {
                OrderSortingType = orderSortingType;
                var sortedOrders = _orderService.GetSortedOrders(Orders, orderSortingType);

                Orders = new(sortedOrders);
            }
        }

        #endregion
    }
}