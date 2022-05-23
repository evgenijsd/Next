﻿using AutoMapper;
using Next2.Enums;
using Next2.Helpers.DTO;
using Next2.Helpers.Events;
using Next2.Models.Bindables;
using Next2.Services.Order;
using Next2.Views.Mobile;
using Prism.Events;
using Prism.Navigation;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Next2.ViewModels
{
    public class OrderTabsViewModel : BaseViewModel
    {
        private readonly IOrderService _orderService;
        private readonly IEventAggregator _eventAggregator;

        private int _lastSavedOrderId = -1;

        public OrderTabsViewModel(
            INavigationService navigationService,
            IOrderService orderService,
            IEventAggregator eventAggregator)
            : base(navigationService)
        {
            _orderService = orderService;

            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<OrderSelectedEvent>().Subscribe(SetLastSavedOrderId);
            _eventAggregator.GetEvent<OrderMovedEvent>().Subscribe(SetOrderType);
        }

        #region -- Public properties --

        public bool IsOrdersRefreshing { get; set; }

        public EOrdersSortingType OrderSortingType { get; set; }

        public string SearchQuery { get; set; } = string.Empty;

        public bool IsNothingFound { get; set; } = false;

        public bool IsSearchActive { get; set; } = false;

        public bool IsTabsSelected { get; set; }

        public bool IsOrdersInitializing => !IsInternetConnected || (IsOrdersRefreshing && !Orders.Any());

        public SimpleOrderBindableModel? SelectedOrder { get; set; }

        public ObservableCollection<SimpleOrderBindableModel> Orders { get; set; } = new ();

        private ICommand _switchToOrdersCommand;
        public ICommand SwitchTotOrdersCommand => _switchToOrdersCommand ??= new AsyncCommand(OnSwitchTotOrdersCommandAsync, allowsMultipleExecutions: false);

        private ICommand _switchToTabsComamnd;
        public ICommand SwitchToTabsComamnd => _switchToTabsComamnd ??= new AsyncCommand(OnSwitchToTabsComamndAsync, allowsMultipleExecutions: false);

        private ICommand _searchCommand;
        public ICommand SearchCommand => _searchCommand ??= new AsyncCommand(OnSearchCommandAsync, allowsMultipleExecutions: false);

        private ICommand _clearSearchCommand;
        public ICommand ClearSearchResultCommand => _clearSearchCommand ??= new AsyncCommand(OnClearSearchResultCommandAsync);

        private ICommand _refreshOrdersCommand;
        public ICommand RefreshOrdersCommand => _refreshOrdersCommand ??= new AsyncCommand(OnRefreshOrdersCommandAsync);

        private ICommand _orderTabSortingChangeCommand;
        public ICommand ChangeOrderSortingCommand => _orderTabSortingChangeCommand ??= new AsyncCommand<EOrdersSortingType>(OnChangeOrderSortingCommandAsync);

        private ICommand _selectOrderCommand;
        public ICommand SelectOrderCommand => _selectOrderCommand ??= new AsyncCommand<SimpleOrderBindableModel?>(OnSelectOrderCommandAsync);

        private ICommand _removeOrderCommand;
        public ICommand RemoveOrderCommand => _removeOrderCommand ??= new AsyncCommand(OnRemoveOrderCommandAsync, allowsMultipleExecutions: false);

        private ICommand _printCommand;
        public ICommand PrintCommand => _printCommand ??= new AsyncCommand(OnPrintCommandAsync, allowsMultipleExecutions: false);

        private ICommand _goBackCommand;
        public ICommand GoBackCommand => _goBackCommand ??= new AsyncCommand(OnGoBackCommand, allowsMultipleExecutions: false);

        #endregion

        #region -- Overrides --

        public override async void OnAppearing()
        {
            base.OnAppearing();

            IsSearchActive = IsNothingFound = false;

            IsOrdersRefreshing = true;

            //await LoadOrdersAsync(IsTabsSelected);
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();

            _lastSavedOrderId = 0;
            SearchQuery = string.Empty;
            IsSearchActive = IsNothingFound = IsOrdersRefreshing = false;
            SelectedOrder = null;
            Orders = new();
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            if (args.PropertyName is nameof(Orders) or nameof(IsSearchActive))
            {
                IsNothingFound = IsSearchActive && !Orders.Any();
            }
        }

        #endregion

        #region -- Private helpers --

        private Task OnRefreshOrdersCommandAsync()
        {
            return LoadOrdersAsync();
        }

        public async Task LoadOrdersAsync()
        {
            bool isOrderGettingSuccessed = false;

            OrderSortingType = EOrdersSortingType.ByCustomerName;
            SearchQuery = string.Empty;
            SelectedOrder = null;

            if (IsInternetConnected)
            {
                OrderSortingType = EOrdersSortingType.ByCustomerName;

                var gettingOrdersResult = await _orderService.GetOrdersAsync();

                if (gettingOrdersResult.IsSuccess)
                {
                    isOrderGettingSuccessed = true;

                    var pendingOrders = gettingOrdersResult.Result
                        .Where(x => x.OrderStatus == EOrderStatus.Pending);

                    var displayedOrders = IsTabsSelected
                        ? pendingOrders.Where(x => x.IsTab)
                        : pendingOrders.Where(x => !x.IsTab).OrderBy(x => x.TableNumber);

                    var mapper = new Mapper(GetOrderConfig(IsTabsSelected));

                    Orders = mapper.Map<IEnumerable<SimpleOrderModelDTO>, ObservableCollection<SimpleOrderBindableModel>>(displayedOrders);

                    if (!string.IsNullOrEmpty(SearchQuery))
                    {
                        Orders = new(Orders.Where(x =>
                            x.Number.ToString().Contains(SearchQuery) ||
                            x.TableNumberOrName.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase)));
                    }

                    //SelectedOrder = _lastSavedOrderId == 0
                    //    ? null
                    //    : Orders.FirstOrDefault(x => x.Id == _lastSavedOrderId);
                }
                else
                {
                    await ShowInfoDialog("SomethingWentWrong", "Error");
                }
            }
            else
            {
                await ShowInfoDialog("NoInternetConnection", "Error");
            }

            if (!isOrderGettingSuccessed)
            {
                Orders = new();
            }

            IsOrdersRefreshing = false;
        }

        private MapperConfiguration GetOrderConfig(bool isTabsLoading)
        {
            MapperConfiguration config = null;

            if (isTabsLoading)
            {
                config = new MapperConfiguration(cfg => cfg.CreateMap<SimpleOrderModelDTO, SimpleOrderBindableModel>()
                    .ForMember(x => x.TableNumberOrName, s => s.MapFrom(x => x.Customer != null
                        ? x.Customer.FullName
                        : CreateRandomCustomerName())));
            }
            else
            {
                config = new MapperConfiguration(cfg => cfg.CreateMap<SimpleOrderModelDTO, SimpleOrderBindableModel>()
                    .ForMember<string>(x => x.TableNumberOrName, s => s.MapFrom(x => GetTableName(x.TableNumber))));
            }

            return config;
        }

        private string GetTableName(int? tableNumber)
        {
            return tableNumber == null
                ? "Not defined"
                : $"Table {tableNumber}";
        }

        private async Task OnSwitchTotOrdersCommandAsync()
        {
            if (IsTabsSelected)
            {
                Orders = new();
                IsTabsSelected = false;
                IsOrdersRefreshing = true;
            }
        }

        private async Task OnSwitchToTabsComamndAsync()
        {
            if (!IsTabsSelected)
            {
                Orders = new();
                IsTabsSelected = true;
                IsOrdersRefreshing = true;
            }
        }

        private async Task OnSearchCommandAsync()
        {
            if (Orders.Any() || !string.IsNullOrEmpty(SearchQuery))
            {
                _eventAggregator.GetEvent<EventSearch>().Subscribe(SearchEventCommand);

                Func<string, string> searchValidator = IsTabsSelected
                    ? _orderService.ApplyNumberFilter
                    : _orderService.ApplyNameFilter;

                var placeholder = IsTabsSelected
                    ? LocalizationResourceManager.Current["NameOrOrder"]
                    : LocalizationResourceManager.Current["TableNumberOrOrder"];

                var parameters = new NavigationParameters()
                {
                    { Constants.Navigations.SEARCH, SearchQuery },
                    { Constants.Navigations.FUNC, searchValidator },
                    { Constants.Navigations.PLACEHOLDER, placeholder },
                };

                await ClearSearchAsync();

                IsSearchActive = true;

                await _navigationService.NavigateAsync(nameof(SearchPage), parameters);
            }
        }

        private void SearchEventCommand(string searchLine)
        {
            SearchQuery = searchLine;

            Orders = new (Orders.Where(
                x => x.Number.ToString().Contains(SearchQuery) ||
                (!string.IsNullOrEmpty(x.TableNumberOrName) && x.TableNumberOrName.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase))));

            SelectedOrder = null;

            _eventAggregator.GetEvent<EventSearch>().Unsubscribe(SearchEventCommand);
        }

        private async Task OnClearSearchResultCommandAsync()
        {
            if (SearchQuery != string.Empty)
            {
                await ClearSearchAsync();
            }
            else
            {
                await OnSearchCommandAsync();
            }
        }

        private Task ClearSearchAsync()
        {
            OrderSortingType = EOrdersSortingType.ByCustomerName;

            SelectedOrder = null;
            SearchQuery = string.Empty;

            IsOrdersRefreshing = true;

            return Task.CompletedTask;
        }

        private IEnumerable<SimpleOrderBindableModel> GetSortedOrders(IEnumerable<SimpleOrderBindableModel> orders)
        {
            EOrdersSortingType orderTabSorting = OrderSortingType == EOrdersSortingType.ByCustomerName && IsTabsSelected
                ? EOrdersSortingType.ByTableNumber
                : OrderSortingType;

            Func<SimpleOrderBindableModel, object> comparer = orderTabSorting switch
            {
                EOrdersSortingType.ByOrderNumber => x => x.Number,
                EOrdersSortingType.ByTableNumber => x => x.TableNumber,
                EOrdersSortingType.ByCustomerName => x => x.TableNumberOrName,
                _ => throw new NotImplementedException(),
            };

            return orders.OrderBy(comparer);
        }

        private Task OnChangeOrderSortingCommandAsync(EOrdersSortingType orderSortingType)
        {
            if (OrderSortingType == orderSortingType)
            {
                Orders = new (Orders.Reverse());
            }
            else
            {
                OrderSortingType = orderSortingType;

                Orders = new (GetSortedOrders(Orders));
            }

            return Task.CompletedTask;
        }

        private Task OnSelectOrderCommandAsync(SimpleOrderBindableModel? order)
        {
            SelectedOrder = order == SelectedOrder ? null : order;

            return Task.CompletedTask;
        }

        private async Task OnRemoveOrderCommandAsync()
        {
            if (SelectedOrder is not null)
            {
                //var seatsResult = await _orderService.GetSeatsAsync(SelectedOrder.Id);

                //if (seatsResult.IsSuccess)
                //{
                //    var seats = seatsResult.Result;

                //    var param = new DialogParameters
                //    {
                //        { Constants.DialogParameterKeys.ORDER_NUMBER, SelectedOrder.Number },
                //        { Constants.DialogParameterKeys.SEATS,  seats },
                //        { Constants.DialogParameterKeys.TITLE, LocalizationResourceManager.Current["Remove"] },
                //        { Constants.DialogParameterKeys.CANCEL_BUTTON_TEXT, LocalizationResourceManager.Current["Cancel"] },
                //        { Constants.DialogParameterKeys.OK_BUTTON_TEXT, LocalizationResourceManager.Current["Remove"] },
                //        { Constants.DialogParameterKeys.OK_BUTTON_BACKGROUND, Application.Current.Resources["IndicationColor_i3"] },
                //        { Constants.DialogParameterKeys.OK_BUTTON_TEXT_COLOR, Application.Current.Resources["TextAndBackgroundColor_i1"] },
                //    };

                //    PopupPage deleteSeatDialog = App.IsTablet
                //        ? new Views.Tablet.Dialogs.OrderDetailDialog(param, CloseDeleteOrderDialogCallbackAsync)
                //        : new Views.Mobile.Dialogs.OrderDetailDialog(param, CloseDeleteOrderDialogCallbackAsync);

                //    await PopupNavigation.PushAsync(deleteSeatDialog);
                //}
            }
        }

        private async void CloseDeleteOrderDialogCallbackAsync(IDialogParameters parameters)
        {
            if (parameters is not null
                && parameters.TryGetValue(Constants.DialogParameterKeys.ACCEPT, out bool isOrderDeletionConfirmationRequestCalled))
            {
                if (isOrderDeletionConfirmationRequestCalled)
                {
                    var confirmDialogParameters = new DialogParameters
                    {
                        { Constants.DialogParameterKeys.CONFIRM_MODE, EConfirmMode.Attention },
                        { Constants.DialogParameterKeys.TITLE, LocalizationResourceManager.Current["AreYouSure"] },
                        { Constants.DialogParameterKeys.DESCRIPTION, LocalizationResourceManager.Current["OrderWillBeRemoved"] },
                        { Constants.DialogParameterKeys.CANCEL_BUTTON_TEXT, LocalizationResourceManager.Current["Cancel"] },
                        { Constants.DialogParameterKeys.OK_BUTTON_TEXT, LocalizationResourceManager.Current["Remove"] },
                    };

                    PopupPage confirmDialog = App.IsTablet
                        ? new Next2.Views.Tablet.Dialogs.ConfirmDialog(confirmDialogParameters, CloseConfirmDialogCallback)
                        : new Next2.Views.Mobile.Dialogs.ConfirmDialog(confirmDialogParameters, CloseConfirmDialogCallback);

                    await PopupNavigation.PushAsync(confirmDialog);
                }
            }
            else
            {
                await PopupNavigation.PopAsync();
            }
        }

        private async void CloseConfirmDialogCallback(IDialogParameters parameters)
        {
            if (parameters is not null && parameters.TryGetValue(Constants.DialogParameterKeys.ACCEPT, out bool isOrderRemovingAccepted))
            {
                //if (isOrderRemovingAccepted && SelectedOrder is not null)
                //{
                //    int removalOrderId = SelectedOrder.Id;
                //    var result = await _orderService.DeleteOrderAsync(removalOrderId);

                //    if (result.IsSuccess)
                //    {
                //        var removalBindableOrder = Orders.FirstOrDefault(x => x.Id == SelectedOrder.Id);

                //        await LoadDataAsync();
                //        SelectedOrder = null;
                //    }

                //    await PopupNavigation.PopAsync();
                //}
            }

            await PopupNavigation.PopAsync();
        }

        private async Task OnPrintCommandAsync()
        {
            if (SelectedOrder is not null)
            {
                //var seatsResult = await _orderService.GetSeatsAsync(SelectedOrder.Id);

                //if (seatsResult.IsSuccess)
                //{
                //    var seats = seatsResult.Result;

                //    var param = new DialogParameters
                //    {
                //        { Constants.DialogParameterKeys.ORDER_NUMBER, SelectedOrder.OrderNumber },
                //        { Constants.DialogParameterKeys.SEATS,  seats },
                //        { Constants.DialogParameterKeys.TITLE, LocalizationResourceManager.Current["Print"] },
                //        { Constants.DialogParameterKeys.CANCEL_BUTTON_TEXT, LocalizationResourceManager.Current["Cancel"] },
                //        { Constants.DialogParameterKeys.OK_BUTTON_TEXT, LocalizationResourceManager.Current["Print"] },
                //        { Constants.DialogParameterKeys.OK_BUTTON_BACKGROUND, Application.Current.Resources["IndicationColor_i1"] },
                //        { Constants.DialogParameterKeys.OK_BUTTON_TEXT_COLOR, Application.Current.Resources["TextAndBackgroundColor_i6"] },
                //    };

                //    PopupPage deleteSeatDialog = App.IsTablet
                //        ? new Views.Tablet.Dialogs.OrderDetailDialog(param, ClosePrintOrderDialogCallbackAsync)
                //        : new Views.Mobile.Dialogs.OrderDetailDialog(param, ClosePrintOrderDialogCallbackAsync);

                //    await PopupNavigation.PushAsync(deleteSeatDialog);
                //}
            }
        }

        private async void ClosePrintOrderDialogCallbackAsync(IDialogParameters parameters)
        {
            if (parameters is not null && parameters.TryGetValue(Constants.DialogParameterKeys.ACCEPT, out bool isOrderPrintingAccepted))
            {
                if (isOrderPrintingAccepted && SelectedOrder is not null)
                {
                    await PopupNavigation.PopAsync();
                }
            }
            else
            {
                await PopupNavigation.PopAsync();
            }
        }

        private void SetLastSavedOrderId(int orderId)
        {
            _lastSavedOrderId = orderId;
        }

        private void SetOrderType(bool isTab) => IsTabsSelected = isTab;

        private Task OnGoBackCommand()
        {
            return _navigationService.NavigateAsync($"/{nameof(NavigationPage)}/{nameof(LoginPage)}/{nameof(MenuPage)}");
        }

        private string CreateRandomCustomerName()
        {
            string[] names = { "Bob", "Tom", "Sam" };

            string[] surnames = { "White", "Black", "Red" };

            Random random = new();

            string name = names[random.Next(3)];

            string surname = surnames[random.Next(3)];

            return name + " " + surname;
        }

        #endregion
    }
}
