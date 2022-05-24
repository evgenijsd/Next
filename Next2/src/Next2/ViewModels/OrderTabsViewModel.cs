using AutoMapper;
using Next2.Enums;
using Next2.Helpers.Events;
using Next2.Models;
using Next2.Resources.Strings;
using Next2.Services.Order;
using Next2.Views.Mobile;
using Prism.Events;
using Prism.Navigation;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Contracts;
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
        private readonly double _summRowHeight = App.IsTablet ? Constants.LayoutOrderTabs.SUMM_ROW_HEIGHT_TABLET : Constants.LayoutOrderTabs.SUMM_ROW_HEIGHT_MOBILE;
        private readonly double _offsetHeight = App.IsTablet ? Constants.LayoutOrderTabs.OFFSET_TABLET : Constants.LayoutOrderTabs.OFFSET_MOBILE;
        private readonly IOrderService _orderService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IPopupNavigation _popupNavigation;

        private IEnumerable<OrderModel>? _ordersBase;
        private IEnumerable<OrderModel>? _tabsBase;
        private Guid _lastSavedOrderId;
        public double _heightPage;

        public OrderTabsViewModel(
            INavigationService navigationService,
            IOrderService orderService,
            IEventAggregator eventAggregator,
            IPopupNavigation popupNavigation)
            : base(navigationService)
        {
            _orderService = orderService;
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<OrderSelectedEvent>().Subscribe(SetLastSavedOrderId);
            _eventAggregator.GetEvent<OrderMovedEvent>().Subscribe(SetOrderStatus);

            _popupNavigation = popupNavigation;
        }

        #region -- Public properties --

        public double HeightPage { get; set; }

        public bool IsOrdersRefreshing { get; set; }

        public EOrderTabSorting CurrentOrderTabSorting { get; set; }

        public GridLength HeightCollectionGrid { get; set; }

        public string SearchText { get; set; } = string.Empty;

        public string SearchPlaceholder { get; set; }

        public bool IsNotingFound { get; set; } = false;

        public bool IsSearching { get; set; } = false;

        public bool IsOrderTabsSelected { get; set; } = true;

        public OrderBindableModel? SelectedOrder { get; set; }

        public ObservableCollection<OrderBindableModel> Orders { get; set; } = new ();

        private ICommand _SelectOrdersCommand;
        public ICommand SelectOrdersCommand => _SelectOrdersCommand ??= new AsyncCommand(OnSelectOrdersCommandAsync);

        private ICommand _SelectTabsCommand;
        public ICommand SelectTabsCommand => _SelectTabsCommand ??= new AsyncCommand(OnSelectTabsCommandAsync);

        private ICommand _SearchCommand;
        public ICommand SearchCommand => _SearchCommand ??= new AsyncCommand(OnSearchCommandAsync, allowsMultipleExecutions: false);

        private ICommand _ClearSearchCommand;
        public ICommand ClearSearchCommand => _ClearSearchCommand ??= new AsyncCommand(OnClearSearchCommandAsync);

        private ICommand _refreshOrdersCommand;
        public ICommand RefreshOrdersCommand => _refreshOrdersCommand ??= new AsyncCommand(OnRefreshOrdersCommandAsync);

        private ICommand _orderTabSortingChangeCommand;
        public ICommand OrderTabSortingChangeCommand => _orderTabSortingChangeCommand ??= new AsyncCommand<EOrderTabSorting>(OnOrderTabSortingChangeCommandAsync);

        private ICommand _tapSelectCommand;
        public ICommand TapSelectCommand => _tapSelectCommand ??= new AsyncCommand<OrderBindableModel?>(OnTapSelectCommandAsync);

        private ICommand _removeOrderCommand;
        public ICommand RemoveOrderCommand => _removeOrderCommand ??= new AsyncCommand(OnRemoveOrderCommandAsync, allowsMultipleExecutions: false);

        private ICommand _printCommand;
        public ICommand PrintCommand => _printCommand ??= new AsyncCommand(OnPrintCommandAsync, allowsMultipleExecutions: false);

        private ICommand _goBackCommand;
        public ICommand GoBackCommand => _goBackCommand ??= new AsyncCommand(OnGoBackCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Overrides --

        public override async void OnAppearing()
        {
            base.OnAppearing();

            _heightPage = HeightPage;
            HeightCollectionGrid = new GridLength(_heightPage - _summRowHeight);

            await LoadDataAsync();
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();

            SearchText = string.Empty;
            IsSearching = false;
            IsNotingFound = false;
            SelectedOrder = null;
            _lastSavedOrderId = Guid.Empty;
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            if (args.PropertyName is nameof(SelectedOrder))
            {
                SetHeightCollection();
            }
            else if (args.PropertyName is nameof(Orders))
            {
                IsNotingFound = !Orders.Any();

                if (IsNotingFound)
                {
                    HeightCollectionGrid = new GridLength(_heightPage - _summRowHeight);
                }
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
            IsOrdersRefreshing = true;

            CurrentOrderTabSorting = EOrderTabSorting.ByCustomerName;

            var resultGettingOrders = await _orderService.GetOrdersAsync();

            if (resultGettingOrders.IsSuccess)
            {
                _ordersBase = new List<OrderModel>(resultGettingOrders.Result.Where(x => !x.IsTab).OrderBy(x => x.TableNumber));

                _tabsBase = new List<OrderModel>(resultGettingOrders.Result.Where(x => x.IsTab).OrderBy(x => x.Customer?.FullName));
            }

            IsOrdersRefreshing = false;

            SetVisualCollection();
        }

        private void SetVisualCollection()
        {
            SelectedOrder = null;
            MapperConfiguration config;

            Orders = new ObservableCollection<OrderBindableModel>();

            IEnumerable<OrderModel>? result;

            if (IsOrderTabsSelected)
            {
                config = new MapperConfiguration(cfg => cfg.CreateMap<OrderModel, OrderBindableModel>()
                            .ForMember(x => x.Name, s => s.MapFrom(x => $"Table {x.TableNumber}"))
                            .ForMember(x => x.OrderNumberText, s => s.MapFrom(x => $"{x.OrderNumber}")));
                result = _ordersBase;
            }
            else
            {
                config = new MapperConfiguration(cfg => cfg.CreateMap<OrderModel, OrderBindableModel>()
                            .ForMember(x => x.Name, s => s.MapFrom(x => x.Customer.FullName))
                            .ForMember(x => x.OrderNumberText, s => s.MapFrom(x => $"{x.OrderNumber}")));
                result = _tabsBase;
            }

            if (result != null)
            {
                var mapper = new Mapper(config);

                Orders = mapper.Map<IEnumerable<OrderModel>, ObservableCollection<OrderBindableModel>>(result);

                for (int i = 0; i < Orders.Count; i++)
                {
                    Orders[i].Name = string.IsNullOrWhiteSpace(Orders[i].Name) ? CreateRandomCustomerName() : Orders[i].Name;
                }

                if (!string.IsNullOrEmpty(SearchText))
                {
                    Orders = new(Orders.Where(x => x.OrderNumberText.ToLower().Contains(SearchText.ToLower()) || x.Name.ToLower().Contains(SearchText.ToLower())));
                }

                SetHeightCollection();
            }

            //SelectedOrder = _lastSavedOrderId != null ? Orders.FirstOrDefault(x => x.Id == _lastSavedOrderId) : null;
        }

        private void SetHeightCollection()
        {
            var heightCollectionScreen = _heightPage - _summRowHeight;

            if (SelectedOrder != null && !App.IsTablet)
            {
                heightCollectionScreen -= Constants.LayoutOrderTabs.BUTTONS_HEIGHT;
            }

            HeightCollectionGrid = new GridLength(heightCollectionScreen);

            if (Orders.Any())
            {
                var heightCollection = (Orders.Count * Constants.LayoutOrderTabs.ROW_HEIGHT) + _offsetHeight;

                if (heightCollectionScreen > heightCollection)
                {
                    heightCollectionScreen = heightCollection;
                }

                HeightCollectionGrid = new GridLength(heightCollectionScreen);
            }
            else
            {
                HeightCollectionGrid = new GridLength(_heightPage - _summRowHeight);
            }
        }

        private Task OnSelectOrdersCommandAsync()
        {
            if (!IsOrderTabsSelected)
            {
                IsOrderTabsSelected = !IsOrderTabsSelected;
                CurrentOrderTabSorting = EOrderTabSorting.ByCustomerName;

                SearchText = string.Empty;

                SetVisualCollection();
            }

            return Task.CompletedTask;
        }

        private Task OnSelectTabsCommandAsync()
        {
            if (IsOrderTabsSelected)
            {
                IsOrderTabsSelected = !IsOrderTabsSelected;
                CurrentOrderTabSorting = EOrderTabSorting.ByCustomerName;

                SearchText = string.Empty;

                SetVisualCollection();
            }

            return Task.CompletedTask;
        }

        private async Task OnSearchCommandAsync()
        {
            if (Orders.Any() || !string.IsNullOrEmpty(SearchText))
            {
                _eventAggregator.GetEvent<EventSearch>().Subscribe(SearchEventCommand);

                Func<string, string> searchValidator = IsOrderTabsSelected ? _orderService.ApplyNumberFilter : _orderService.ApplyNameFilter;
                var placeholder = IsOrderTabsSelected ? Strings.TableNumberOrOrder : Strings.NameOrOrder;

                var parameters = new NavigationParameters()
                {
                    { Constants.Navigations.SEARCH, SearchText },
                    { Constants.Navigations.FUNC, searchValidator },
                    { Constants.Navigations.PLACEHOLDER, placeholder },
                };

                ClearSearch();
                IsSearching = true;

                await _navigationService.NavigateAsync(nameof(SearchPage), parameters);
            }
        }

        private void SearchEventCommand(string searchLine)
        {
            SearchText = searchLine;

            Orders = new (Orders.Where(x => x.OrderNumberText.ToLower().Contains(SearchText.ToLower()) || x.Name.ToLower().Contains(SearchText.ToLower())));
            SelectedOrder = null;

            _eventAggregator.GetEvent<EventSearch>().Unsubscribe(SearchEventCommand);

            SetHeightCollection();
        }

        private async Task OnClearSearchCommandAsync()
        {
            if (SearchText != string.Empty)
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
            CurrentOrderTabSorting = EOrderTabSorting.ByCustomerName;

            SelectedOrder = null;
            SearchText = string.Empty;

            SetVisualCollection();
        }

        private IEnumerable<OrderBindableModel> GetSortedOrders(IEnumerable<OrderBindableModel> orders)
        {
            EOrderTabSorting orderTabSorting = CurrentOrderTabSorting == EOrderTabSorting.ByCustomerName && IsOrderTabsSelected ? EOrderTabSorting.ByTableNumber : CurrentOrderTabSorting;

            Func<OrderBindableModel, object> comparer = orderTabSorting switch
            {
                EOrderTabSorting.ByOrderNumber => x => x.OrderNumber,
                EOrderTabSorting.ByTableNumber => x => x.TableNumber,
                EOrderTabSorting.ByCustomerName => x => x.Name,
                _ => throw new NotImplementedException(),
            };

            return orders.OrderBy(comparer);
        }

        private Task OnOrderTabSortingChangeCommandAsync(EOrderTabSorting newOrderTabSorting)
        {
            if (CurrentOrderTabSorting == newOrderTabSorting)
            {
                Orders = new (Orders.Reverse());
            }
            else
            {
                CurrentOrderTabSorting = newOrderTabSorting;

                Orders = new (GetSortedOrders(Orders));
            }

            return Task.CompletedTask;
        }

        private Task OnTapSelectCommandAsync(OrderBindableModel? order)
        {
            SelectedOrder = order == SelectedOrder ? null : order;

            return Task.CompletedTask;
        }

        private async Task OnRemoveOrderCommandAsync()
        {
            if (SelectedOrder is not null)
            {
                var seatsResult = await _orderService.GetSeatsAsync(SelectedOrder.Id);

                if (seatsResult.IsSuccess)
                {
                    var seats = seatsResult.Result;

                    var param = new DialogParameters
                    {
                        { Constants.DialogParameterKeys.ORDER_NUMBER, SelectedOrder.OrderNumber },
                        { Constants.DialogParameterKeys.SEATS,  seats },
                        { Constants.DialogParameterKeys.TITLE, LocalizationResourceManager.Current["Remove"] },
                        { Constants.DialogParameterKeys.CANCEL_BUTTON_TEXT, LocalizationResourceManager.Current["Cancel"] },
                        { Constants.DialogParameterKeys.OK_BUTTON_TEXT, LocalizationResourceManager.Current["Remove"] },
                        { Constants.DialogParameterKeys.OK_BUTTON_BACKGROUND, Application.Current.Resources["IndicationColor_i3"] },
                        { Constants.DialogParameterKeys.OK_BUTTON_TEXT_COLOR, Application.Current.Resources["TextAndBackgroundColor_i1"] },
                    };

                    PopupPage deleteSeatDialog = App.IsTablet
                        ? new Views.Tablet.Dialogs.OrderDetailDialog(param, CloseDeleteOrderDialogCallbackAsync)
                        : new Views.Mobile.Dialogs.OrderDetailDialog(param, CloseDeleteOrderDialogCallbackAsync);

                    await _popupNavigation.PushAsync(deleteSeatDialog);
                }
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

                    await _popupNavigation.PushAsync(confirmDialog);
                }
            }
            else
            {
                await _popupNavigation.PopAsync();
            }
        }

        private async void CloseConfirmDialogCallback(IDialogParameters parameters)
        {
            if (parameters is not null && parameters.TryGetValue(Constants.DialogParameterKeys.ACCEPT, out bool isOrderRemovingAccepted))
            {
                if (isOrderRemovingAccepted && SelectedOrder is not null)
                {
                    int removalOrderId = SelectedOrder.Id;
                    var result = await _orderService.DeleteOrderAsync(removalOrderId);

                    if (result.IsSuccess)
                    {
                        var removalBindableOrder = Orders.FirstOrDefault(x => x.Id == SelectedOrder.Id);

                        await LoadDataAsync();
                        SelectedOrder = null;
                    }

                    await _popupNavigation.PopAsync();
                }
            }

            await _popupNavigation.PopAsync();
        }

        private async Task OnPrintCommandAsync()
        {
            if (SelectedOrder is not null)
            {
                var seatsResult = await _orderService.GetSeatsAsync(SelectedOrder.Id);

                if (seatsResult.IsSuccess)
                {
                    var seats = seatsResult.Result;

                    var param = new DialogParameters
                    {
                        { Constants.DialogParameterKeys.ORDER_NUMBER, SelectedOrder.OrderNumber },
                        { Constants.DialogParameterKeys.SEATS,  seats },
                        { Constants.DialogParameterKeys.TITLE, LocalizationResourceManager.Current["Print"] },
                        { Constants.DialogParameterKeys.CANCEL_BUTTON_TEXT, LocalizationResourceManager.Current["Cancel"] },
                        { Constants.DialogParameterKeys.OK_BUTTON_TEXT, LocalizationResourceManager.Current["Print"] },
                        { Constants.DialogParameterKeys.OK_BUTTON_BACKGROUND, Application.Current.Resources["IndicationColor_i1"] },
                        { Constants.DialogParameterKeys.OK_BUTTON_TEXT_COLOR, Application.Current.Resources["TextAndBackgroundColor_i6"] },
                    };

                    PopupPage deleteSeatDialog = App.IsTablet
                        ? new Views.Tablet.Dialogs.OrderDetailDialog(param, ClosePrintOrderDialogCallbackAsync)
                        : new Views.Mobile.Dialogs.OrderDetailDialog(param, ClosePrintOrderDialogCallbackAsync);

                    await _popupNavigation.PushAsync(deleteSeatDialog);
                }
            }
        }

        private async void ClosePrintOrderDialogCallbackAsync(IDialogParameters parameters)
        {
            if (parameters is not null && parameters.TryGetValue(Constants.DialogParameterKeys.ACCEPT, out bool isOrderPrintingAccepted))
            {
                if (isOrderPrintingAccepted && SelectedOrder is not null)
                {
                    await _popupNavigation.PopAsync();
                }
            }
            else
            {
                await _popupNavigation.PopAsync();
            }
        }

        private void SetLastSavedOrderId(Guid orderId)
        {
            _lastSavedOrderId = orderId;
        }

        private void SetOrderStatus(Enum orderStatus)
        {
            IsOrderTabsSelected = orderStatus is EOrderStatus.Pending;
        }

        private Task OnGoBackCommandAsync()
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
