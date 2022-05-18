using AutoMapper;
using Next2.Enums;
using Next2.Helpers.DTO;
using Next2.Helpers.Events;
using Next2.Helpers.ProcessHelpers;
using Next2.Models.Bindables;
using Next2.Resources.Strings;
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

        private IEnumerable<OrderModelDTO>? _orders;
        private IEnumerable<OrderModelDTO>? _tabs;
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

        public bool IsTabsSelected { get; set; } = true;

        public SimpleOrderBindableModel? SelectedOrder { get; set; }

        public ObservableCollection<SimpleOrderBindableModel> Orders { get; set; } = new ();

        private ICommand _selectOrdersCommand;
        public ICommand SelectOrdersCommand => _selectOrdersCommand ??= new AsyncCommand(OnSelectOrdersCommandAsync);

        private ICommand _selectTabsCommand;
        public ICommand SelectTabsCommand => _selectTabsCommand ??= new AsyncCommand(OnSelectTabsCommandAsync);

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

            await LoadDataAsync();
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();

            SearchQuery = string.Empty;
            IsSearchActive = false;
            IsNothingFound = false;
            SelectedOrder = null;
            _lastSavedOrderId = 0;
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            if (args.PropertyName is nameof(Orders))
            {
                IsNothingFound = !Orders.Any();
            }
        }

        #endregion

        #region -- Private helpers --

        private Task OnRefreshOrdersCommandAsync()
        {
            return LoadDataAsync();
        }

        public async Task LoadDataAsync()
        {
            if (IsInternetConnected)
            {
                IsOrdersRefreshing = true;

                OrderSortingType = EOrdersSortingType.ByCustomerName;

                AOResult<IEnumerable<OrderModelDTO>> gettingOrdersResult = new AOResult<IEnumerable<OrderModelDTO>>();

                if (IsTabsSelected)
                {
                    gettingOrdersResult = await _orderService.GetOrdersAsync();
                }
                else
                {
                    gettingOrdersResult.SetSuccess(new List<OrderModelDTO>());
                    await Task.Delay(2000);
                }

                if (gettingOrdersResult.IsSuccess)
                {
                    var pendingOrders = gettingOrdersResult.Result.Where(x => x.OrderStatus == EOrderStatus.Pending);

                    _orders = new List<OrderModelDTO>(pendingOrders)
                        .Where(x => !x.IsTab)
                        .OrderBy(x => x.Table?.Number);

                    _tabs = new List<OrderModelDTO>(pendingOrders)
                        .Where(x => x.IsTab);

                    SetVisualCollection();
                }
                else
                {
                    _orders = _tabs = null;
                    Orders = new ();

                    await ShowInfoDialog("SomethingWentWrong", "Error");
                }

                IsOrdersRefreshing = false;
            }
            else
            {
                _orders = _tabs = null;
                Orders = new();

                await ShowInfoDialog("NoInternetConnection", "Error");
            }
        }

        private void SetVisualCollection()
        {
            SelectedOrder = null;
            MapperConfiguration config = null;

            Orders = new ObservableCollection<SimpleOrderBindableModel>();

            IEnumerable<OrderModelDTO>? result = null;

            if (IsTabsSelected)
            {
                config = new MapperConfiguration(cfg => cfg.CreateMap<OrderModelDTO, SimpleOrderBindableModel>()
                    .ForMember(x => x.TableNumberOrName, s => s.MapFrom(x => x.Table == null
                        ? "Not defined"
                        : $"Table {x.Table.Number}")));

                result = _orders;
            }
            else
            {
                config = new MapperConfiguration(cfg => cfg.CreateMap<OrderModelDTO, SimpleOrderBindableModel>()
                    .ForMember(x => x.TableNumberOrName, s => s.MapFrom(x => x.Customer.FullName)));

                result = _tabs;
            }

            if (result != null)
            {
                var mapper = new Mapper(config);

                Orders = mapper.Map<IEnumerable<OrderModelDTO>, ObservableCollection<SimpleOrderBindableModel>>(result);

                for (int i = 0; i < Orders.Count; i++)
                {
                    Orders[i].TableNumberOrName = string.IsNullOrWhiteSpace(Orders[i].TableNumberOrName)
                        ? CreateRandomCustomerName()
                        : Orders[i].TableNumberOrName;
                }

                if (!string.IsNullOrEmpty(SearchQuery))
                {
                    Orders = new(Orders.Where(x =>
                        x.Number.ToString().Contains(SearchQuery) ||
                        x.TableNumberOrName.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase)));
                }
            }

            //SelectedOrder = _lastSavedOrderId == 0
            //    ? null
            //    : Orders.FirstOrDefault(x => x.Id == _lastSavedOrderId);
        }

        private Task OnSelectOrdersCommandAsync()
        {
            if (!IsTabsSelected)
            {
                IsTabsSelected = !IsTabsSelected;
                OrderSortingType = EOrdersSortingType.ByCustomerName;

                SearchQuery = string.Empty;

                SetVisualCollection();
            }

            return Task.CompletedTask;
        }

        private Task OnSelectTabsCommandAsync()
        {
            if (IsTabsSelected)
            {
                IsTabsSelected = !IsTabsSelected;
                OrderSortingType = EOrdersSortingType.ByCustomerName;

                SearchQuery = string.Empty;

                SetVisualCollection();
            }

            return Task.CompletedTask;
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
                    ? LocalizationResourceManager.Current["TableNumberOrOrder"]
                    : LocalizationResourceManager.Current["NameOrOrder"];

                var parameters = new NavigationParameters()
                {
                    { Constants.Navigations.SEARCH, SearchQuery },
                    { Constants.Navigations.FUNC, searchValidator },
                    { Constants.Navigations.PLACEHOLDER, placeholder },
                };

                ClearSearch();
                IsSearchActive = true;

                await _navigationService.NavigateAsync(nameof(SearchPage), parameters);
            }
        }

        private void SearchEventCommand(string searchLine)
        {
            SearchQuery = searchLine;

            Orders = new (Orders.Where(
                x => x.Number.ToString().Contains(SearchQuery) ||
                x.TableNumberOrName.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase)));

            SelectedOrder = null;

            _eventAggregator.GetEvent<EventSearch>().Unsubscribe(SearchEventCommand);
        }

        private async Task OnClearSearchResultCommandAsync()
        {
            if (SearchQuery != string.Empty)
            {
                ClearSearch();
            }
            else
            {
                await OnSearchCommandAsync();
            }
        }

        private void ClearSearch()
        {
            OrderSortingType = EOrdersSortingType.ByCustomerName;

            SelectedOrder = null;
            SearchQuery = string.Empty;

            SetVisualCollection();
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
