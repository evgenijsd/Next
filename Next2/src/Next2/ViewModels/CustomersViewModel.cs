using AutoMapper;
using Next2.Enums;
using Next2.Helpers;
using Next2.Models;
using Next2.Services.Customers;
using Next2.Services.Order;
using Next2.Views.Mobile;
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
    public class CustomersViewModel : BaseViewModel
    {
        private readonly IMapper _mapper;
        private readonly ICustomersService _customersService;
        private readonly IOrderService _orderService;
        private ECustomersSorting _sortCriterion;

        private List<CustomerBindableModel> _allCustomers = new();

        public CustomersViewModel(
            IMapper mapper,
            INavigationService navigationService,
            ICustomersService customersService,
            IOrderService orderService)
            : base(navigationService)
        {
            _mapper = mapper;
            _customersService = customersService;
            _orderService = orderService;
        }

        #region -- Public properties --

        public int IndexLastVisibleElement { get; set; }

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
            if (App.IsTablet)
            {
                SearchText = string.Empty;
                SelectedCustomer = null;
                AnyCustomersLoaded = false;
            }
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            switch (args.PropertyName)
            {
                case nameof(DisplayedCustomers):
                    AnyCustomersLoaded = _allCustomers.Any();
                    break;
            }
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if (parameters.TryGetValue(Constants.Navigations.SEARCH_CUSTOMER, out string searchCustomer))
            {
                SetSearchQuery(searchCustomer);
            }
        }

        #endregion

        #region -- Public helpers --

        public void SetSearchQuery(string searchCustomer)
        {
            SearchText = searchCustomer;

            DisplayedCustomers = SearchCustomers(SearchText);
        }

        #endregion

        #region -- Private helpers --

        private async Task RefreshAsync()
        {
            if (IsInternetConnected)
            {
                IsRefreshing = true;

                var resultOfGettingCustomers = await _customersService.GetCustomersAsync();

                if (resultOfGettingCustomers.IsSuccess)
                {
                    var customers = resultOfGettingCustomers.Result.ToList();

                    foreach (var item in customers)
                    {
                        item.ShowInfoCommand = ShowInfoCommand;
                        item.SelectItemCommand = new Command<CustomerBindableModel>(SelectDeselectItem);
                    }

                    if (customers.Any())
                    {
                        _allCustomers = customers;
                        DisplayedCustomers = SearchCustomers(SearchText);

                        SelectCurrentCustomer();
                    }
                }
                else
                {
                    await ResponseToBadRequestAsync(resultOfGettingCustomers.Exception?.Message);
                }

                IsRefreshing = false;
            }
            else
            {
                await ShowInfoDialogAsync(
                    LocalizationResourceManager.Current["Error"],
                    LocalizationResourceManager.Current["NoInternetConnection"],
                    LocalizationResourceManager.Current["Ok"]);
            }
        }

        private void SelectCurrentCustomer()
        {
            var currentCustomer = _orderService.CurrentOrder.Customer;

            if (currentCustomer is not null)
            {
                var customerId = currentCustomer.Id;

                SelectedCustomer = DisplayedCustomers.FirstOrDefault(x => x.Id == customerId);
            }
        }

        private void SelectDeselectItem(CustomerBindableModel customer)
        {
            SelectedCustomer = SelectedCustomer == customer
                ? null
                : customer;
        }

        private async Task ShowCustomerInfoAsync(CustomerBindableModel customer)
        {
            if (customer is CustomerBindableModel selectedCustomer)
            {
                SelectedCustomer = customer;

                var param = new DialogParameters { { Constants.DialogParameterKeys.MODEL, selectedCustomer } };

                PopupPage customerInfoDialog = App.IsTablet
                    ? new Views.Tablet.Dialogs.CustomerInfoDialog(param, CloseCustomerInfoDialogCallback)
                    : new Views.Mobile.Dialogs.CustomerInfoDialog(param, CloseCustomerInfoDialogCallback);

                await PopupNavigation.PushAsync(customerInfoDialog);
            }
        }

        private async void CloseCustomerInfoDialogCallback(IDialogParameters parameters)
        {
            await CloseAllPopupAsync();

            if (parameters.TryGetValue(Constants.DialogParameterKeys.ACCEPT, out bool isCustomerSelected) && isCustomerSelected)
            {
                await OnAddCustomerToOrderCommandAsync();
            }
        }

        private async Task OnAddCustomerToOrderCommandAsync()
        {
            if (SelectedCustomer is not null)
            {
                if (IsInternetConnected)
                {
                    _orderService.CurrentOrder.Customer = SelectedCustomer;

                    var resultOfUpdatingCurrentOrder = await _orderService.UpdateCurrentOrderAsync();

                    if (resultOfUpdatingCurrentOrder.IsSuccess)
                    {
                        if (App.IsTablet)
                        {
                            MessagingCenter.Send<MenuPageSwitchingMessage>(new(EMenuItems.NewOrder), Constants.Navigations.SWITCH_PAGE);
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
                    else
                    {
                        _orderService.CurrentOrder.Customer = new();

                        await ResponseToBadRequestAsync(resultOfUpdatingCurrentOrder.Exception?.Message);
                    }
                }
                else
                {
                    await ShowInfoDialogAsync(
                        LocalizationResourceManager.Current["Error"],
                        LocalizationResourceManager.Current["NoInternetConnection"],
                        LocalizationResourceManager.Current["Ok"]);
                }
            }
        }

        private Task OnAddNewCustomerCommandAsync(CustomerBindableModel customer)
        {
            var param = new DialogParameters();

            PopupPage popupPage = App.IsTablet
                ? new Views.Tablet.Dialogs.CustomerAddDialog(param, AddCustomerDialogCallBack, _customersService)
                : new Views.Mobile.Dialogs.CustomerAddDialog(param, AddCustomerDialogCallBack, _customersService);

            return PopupNavigation.PushAsync(popupPage);
        }

        private async void AddCustomerDialogCallBack(IDialogParameters param)
        {
            await CloseAllPopupAsync();

            if (param.TryGetValue(Constants.DialogParameterKeys.CUSTOMER_ID, out Guid customerId))
            {
                await RefreshAsync();

                int index = DisplayedCustomers.IndexOf(DisplayedCustomers.FirstOrDefault(x => x.Id == customerId));

                DisplayedCustomers.Move(index, 0);
            }
            else
            {
                if (param.TryGetValue(Constants.DialogParameterKeys.ACCEPT, out bool isCustomerCreationConfirmationRequestAccepted))
                {
                    if (isCustomerCreationConfirmationRequestAccepted && !param.ContainsKey(Constants.DialogParameterKeys.CUSTOMER_ID))
                    {
                        await ShowInfoDialogAsync(
                            LocalizationResourceManager.Current["Error"],
                            LocalizationResourceManager.Current["SomethingWentWrong"],
                            LocalizationResourceManager.Current["Ok"]);
                    }
                }
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
                Func<string, string> searchValidator = Filters.StripInvalidNameCharacters;

                var parameters = new NavigationParameters()
                {
                    { Constants.Navigations.SEARCH_CUSTOMER, SearchText },
                    { Constants.Navigations.FUNC, searchValidator },
                    { Constants.Navigations.PLACEHOLDER, LocalizationResourceManager.Current["NameOrPhone"] },
                };

                await _navigationService.NavigateAsync(nameof(SearchPage), parameters);
            }
        }

        private ObservableCollection<CustomerBindableModel> SearchCustomers(string searchLine)
        {
            bool containsName(CustomerBindableModel x) => x.FullName.Contains(searchLine, StringComparison.OrdinalIgnoreCase);
            bool containsPhone(CustomerBindableModel x) => x.Phone.Contains(searchLine);

            return new ObservableCollection<CustomerBindableModel>(_allCustomers.Where(x => containsName(x) || containsPhone(x)));
        }

        private Task OnClearSearchCommandAsync()
        {
            SetSearchQuery(string.Empty);

            return Task.CompletedTask;
        }

        #endregion
    }
}
