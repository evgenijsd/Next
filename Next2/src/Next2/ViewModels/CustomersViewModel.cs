using AutoMapper;
using Next2.Enums;
using Next2.Helpers;
using Next2.Helpers.Events;
using Next2.Models;
using Next2.Services.CustomersService;
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
    public class CustomersViewModel : BaseViewModel
    {
        private readonly IMapper _mapper;
        private readonly IEventAggregator _eventAggregator;
        private readonly ICustomersService _customersService;
        private readonly IOrderService _orderService;
        private readonly IPopupNavigation _popupNavigation;
        private ECustomersSorting _sortCriterion;

        private List<CustomerBindableModel> _allCustomers = new();

        public CustomersViewModel(
            IMapper mapper,
            IEventAggregator eventAggregator,
            INavigationService navigationService,
            ICustomersService customersService,
            IOrderService orderService,
            IPopupNavigation popupNavigation)
            : base(navigationService)
        {
            _mapper = mapper;
            _eventAggregator = eventAggregator;
            _customersService = customersService;
            _orderService = orderService;
            _popupNavigation = popupNavigation;
        }

        #region -- Public Properties --

        public string SearchText { get; set; } = string.Empty;

        public ObservableCollection<CustomerBindableModel> DisplayedCustomers { get; set; } = new();

        public bool AnyCustomersLoaded { get; set; }

        public bool IsRefreshing { get; set; }

        public CustomerBindableModel? SelectedCustomer { get; set; }

        private ICommand _showInfoCommand;
        public ICommand ShowInfoCommand => _showInfoCommand ??= new AsyncCommand<CustomerBindableModel>(ShowCustomerInfoAsync, allowsMultipleExecutions: false);

        private ICommand _sortCommand;
        public ICommand SortCommand => _sortCommand ??= new AsyncCommand<ECustomersSorting>(SortAsync, allowsMultipleExecutions: false);

        private ICommand _refreshCommand;
        public ICommand RefreshCommand => _refreshCommand ??= new AsyncCommand(RefreshAsync, allowsMultipleExecutions: false);

        private ICommand _addNewCustomerCommand;
        public ICommand AddNewCustomerCommand => _addNewCustomerCommand ??= new AsyncCommand<CustomerBindableModel>(OnAddNewCustomerCommandAsync, allowsMultipleExecutions: false);

        private ICommand _addCustomerToOrderCommand;
        public ICommand AddCustomerToOrderCommand => _addCustomerToOrderCommand ??= new AsyncCommand(OnAddCustomerToOrderCommandAsync, allowsMultipleExecutions: false);

        private ICommand _searchCommand;
        public ICommand SearchCommand => _searchCommand ??= new AsyncCommand(OnSearchCommandAsync, allowsMultipleExecutions: false);

        private ICommand _clearSearchCommand;
        public ICommand ClearSearchCommand => _clearSearchCommand ??= new AsyncCommand(OnClearSearchCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Overrides --

        public override async void OnAppearing()
        {
            await RefreshAsync();
        }

        public override void OnDisappearing()
        {
            ClearSearch();

            SelectedCustomer = null;
            AnyCustomersLoaded = false;
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            if (args.PropertyName is nameof(DisplayedCustomers))
            {
                AnyCustomersLoaded = _allCustomers.Any();
            }
        }

        #endregion

        #region -- Private helpers --

        private async Task RefreshAsync()
        {
            IsRefreshing = true;

            var customersAoresult = await _customersService.GetAllCustomersAsync();

            if (customersAoresult.IsSuccess)
            {
                var customers = customersAoresult.Result.ToList();

                foreach (var item in customers)
                {
                    item.ShowInfoCommand = ShowInfoCommand;
                    item.SelectItemCommand = new Command<CustomerBindableModel>(SelectDeselectItem);
                }

                if (customers.Any())
                {
                    _allCustomers = customers;
                    DisplayedCustomers = SearchCustomers(SearchText);
                    await AddGiftCardsToDisplayedCustomersAsync();
                    SelectCurrentCustomer();
                }
            }

            IsRefreshing = false;
        }

        private async Task AddGiftCardsToDisplayedCustomersAsync()
        {
            if (App.IsTablet)
            {
                foreach (var customer in DisplayedCustomers)
                {
                    var result = await _customersService.GetFullGiftCardsDataAsync(customer);

                    if (result.IsSuccess)
                    {
                       customer.GiftCards = result.Result.GiftCards;
                    }
                }
            }
        }

        private void SelectCurrentCustomer()
        {
            var currentCustomer = _orderService.CurrentOrder.Customer;

            if (currentCustomer is not null)
            {
                SelectedCustomer = DisplayedCustomers.FirstOrDefault(x => x.Id == currentCustomer.Id);
            }
        }

        private void SelectDeselectItem(CustomerBindableModel customer)
        {
            SelectedCustomer = SelectedCustomer == customer ? null : customer;
        }

        private async Task ShowCustomerInfoAsync(CustomerBindableModel customer)
        {
            if (customer is CustomerBindableModel selectedCustomer)
            {
                SelectedCustomer = customer;
                var res = await _customersService.GetFullGiftCardsDataAsync(selectedCustomer);
                selectedCustomer = res.IsSuccess
                    ? res.Result
                    : selectedCustomer;

                var param = new DialogParameters { { Constants.DialogParameterKeys.MODEL, selectedCustomer } };

                PopupPage customerInfoDialog = App.IsTablet
                    ? new Views.Tablet.Dialogs.CustomerInfoDialog(param, CloseCustomerInfoDialogCallback)
                    : new Views.Mobile.Dialogs.CustomerInfoDialog(param, CloseCustomerInfoDialogCallback);

                await _popupNavigation.PushAsync(customerInfoDialog);
            }
        }

        private async void CloseCustomerInfoDialogCallback(IDialogParameters parameters)
        {
            await _popupNavigation.PopAsync();

            if (parameters.TryGetValue(Constants.DialogParameterKeys.ACCEPT, out bool isCustomerSelected) && isCustomerSelected)
            {
                await OnAddCustomerToOrderCommandAsync();
            }
        }

        private async Task OnAddCustomerToOrderCommandAsync()
        {
            if (SelectedCustomer is not null)
            {
                _orderService.CurrentOrder.Customer = _mapper.Map<CustomerBindableModel, CustomerBindableModel>(SelectedCustomer);

                if (App.IsTablet)
                {
                    MessagingCenter.Send<MenuPageSwitchingMessage>(new (EMenuItems.NewOrder), Constants.Navigations.SWITCH_PAGE);
                }
                else
                {
                    string path = $"/{nameof(NavigationPage)}/{nameof(MenuPage)}";

                    if (_orderService.CurrentOrder.Seats.Any())
                    {
                        path += $"/{nameof(OrderRegistrationPage)}";
                    }

                    await _navigationService.NavigateAsync(path);
                }
            }
        }

        private Task OnAddNewCustomerCommandAsync(CustomerBindableModel customer)
        {
            var param = new DialogParameters();

            PopupPage popupPage = App.IsTablet
                ? new Views.Tablet.Dialogs.CustomerAddDialog(param, AddCustomerDialogCallBack, _customersService)
                : new Views.Mobile.Dialogs.CustomerAddDialog(param, AddCustomerDialogCallBack, _customersService);

            return _popupNavigation.PushAsync(popupPage);
        }

        private async void AddCustomerDialogCallBack(IDialogParameters param)
        {
            await _popupNavigation.PopAsync();

            if (param.TryGetValue(Constants.DialogParameterKeys.CUSTOMER_ID, out Guid customerId))
            {
                await RefreshAsync();

                int index = DisplayedCustomers.IndexOf(DisplayedCustomers.FirstOrDefault(x => x.Id == customerId));
                DisplayedCustomers.Move(index, 0);
            }
        }

        private Task SortAsync(ECustomersSorting criterion)
        {
            if (_sortCriterion == criterion)
            {
                DisplayedCustomers = new(DisplayedCustomers.Reverse());
            }
            else
            {
                _sortCriterion = criterion;

                Func<CustomerBindableModel, object> comparer = criterion switch
                {
                    ECustomersSorting.ByName => x => x.FullName,
                    ECustomersSorting.ByPoints => x => x.Points,
                    ECustomersSorting.ByPhoneNumber => x => x.Phone,
                    _ => throw new NotImplementedException(),
                };

                DisplayedCustomers = new(DisplayedCustomers.OrderBy(comparer));
            }

            return Task.CompletedTask;
        }

        private async Task OnSearchCommandAsync()
        {
            if (DisplayedCustomers.Any() || !string.IsNullOrEmpty(SearchText))
            {
                _eventAggregator.GetEvent<EventSearch>().Subscribe(OnSearchEvent);

                Func<string, string> searchValidator = _orderService.ApplyNameFilter;

                var parameters = new NavigationParameters()
                {
                    { Constants.Navigations.SEARCH, SearchText },
                    { Constants.Navigations.FUNC, searchValidator },
                    { Constants.Navigations.PLACEHOLDER, LocalizationResourceManager.Current["NameOrPhone"] },
                };

                ClearSearch();

                await _navigationService.NavigateAsync(nameof(SearchPage), parameters);
            }
        }

        private void OnSearchEvent(string searchLine)
        {
            SearchText = searchLine;

            DisplayedCustomers = SearchCustomers(SearchText);

            _eventAggregator.GetEvent<EventSearch>().Unsubscribe(OnSearchEvent);
        }

        private ObservableCollection<CustomerBindableModel> SearchCustomers(string searchLine)
        {
            bool containsName(CustomerBindableModel x) => x.FullName.Contains(searchLine, StringComparison.OrdinalIgnoreCase);
            bool containsPhone(CustomerBindableModel x) => x.Phone.Replace("-", string.Empty).Contains(searchLine);

            return new ObservableCollection<CustomerBindableModel>(_allCustomers.Where(x => containsName(x) || containsPhone(x)));
        }

        private Task OnClearSearchCommandAsync()
        {
            ClearSearch();

            return Task.CompletedTask;
        }

        private void ClearSearch()
        {
            SearchText = string.Empty;

            DisplayedCustomers = SearchCustomers(SearchText);
        }

        #endregion
    }
}
