﻿using AutoMapper;
using Next2.Enums;
using Next2.Helpers;
using Next2.Models;
using Next2.Services.CustomersService;
using Next2.Services.Order;
using Next2.Views.Mobile;
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
using Xamarin.Forms;

namespace Next2.ViewModels
{
    public class CustomersViewModel : BaseViewModel
    {
        private readonly IMapper _mapper;
        private readonly ICustomersService _customersService;
        private readonly IOrderService _orderService;
        private readonly IPopupNavigation _popupNavigation;
        private ECustomersSorting _sortCriterion;

        public CustomersViewModel(
            IMapper mapper,
            INavigationService navigationService,
            ICustomersService customersService,
            IOrderService orderService,
            IPopupNavigation popupNavigation)
            : base(navigationService)
        {
            _mapper = mapper;
            _customersService = customersService;
            _orderService = orderService;
            _popupNavigation = popupNavigation;
        }

        #region -- Public Properties --

        public ObservableCollection<CustomerBindableModel> Customers { get; set; }

        public bool IsRefreshing { get; set; }

        public CustomerBindableModel? SelectedCustomer { get; set; }

        private ICommand _showInfoCommand;
        public ICommand ShowInfoCommand => _showInfoCommand ??= new AsyncCommand<CustomerBindableModel>(ShowCustomerInfoAsync);

        private ICommand _sortCommand;
        public ICommand SortCommand => _sortCommand ??= new AsyncCommand<ECustomersSorting>(SortAsync);

        private ICommand _refreshCommand;
        public ICommand RefreshCommand => _refreshCommand ??= new AsyncCommand(RefreshAsync);

        private ICommand _addCustomerCommand;
        public ICommand AddCustomerCommand => _addCustomerCommand ??= new AsyncCommand<CustomerBindableModel>(AddCustomerAsync);

        private ICommand _addCustomerToOrderCommand;
        public ICommand AddCustomerToOrderCommand => _addCustomerToOrderCommand ??= new AsyncCommand(OnAddCustomerToOrderCommandAsync);

        #endregion

        #region -- Overrides --

        public override async void OnAppearing()
        {
            await RefreshAsync();
        }

        public override async void OnDisappearing()
        {
            Customers = new();
            SelectedCustomer = null;
        }

        #endregion

        #region -- Private Helpers --

        private async Task RefreshAsync()
        {
            IsRefreshing = true;

            var customersAoresult = await _customersService.GetAllCustomersAsync();

            if (customersAoresult.IsSuccess)
            {
                var result = customersAoresult.Result.OrderBy(x => x.Name);
                var customers = _mapper.Map<IEnumerable<CustomerModel>, ObservableCollection<CustomerBindableModel>>(result);

                foreach (var item in customers)
                {
                    item.ShowInfoCommand = new AsyncCommand<CustomerBindableModel>(ShowCustomerInfoAsync);
                    item.SelectItemCommand = new Command<CustomerBindableModel>(SelectDeselectItem);
                }

                if (customers.Any())
                {
                    Customers = customers;
                }
            }

            IsRefreshing = false;
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
                _orderService.CurrentOrder.Customer = _mapper.Map<CustomerBindableModel, CustomerModel>(SelectedCustomer);

                if (App.IsTablet)
                {
                    MessagingCenter.Send<PageSwitchingMessage>(new (EMenuItems.NewOrder), Constants.Navigations.SWITCH_PAGE);
                }
                else
                {
                    string path = $"/{nameof(NavigationPage)}/{nameof(MenuPage)}/{nameof(OrderRegistrationPage)}";
                    await _navigationService.NavigateAsync(path);
                }
            }
        }

        private async Task AddCustomerAsync(CustomerBindableModel customer)
        {
            if (customer is CustomerBindableModel selectedCustomer)
            {
                var param = new DialogParameters();

                await _popupNavigation.PushAsync(new Views.Tablet.Dialogs
                    .CustomerAddDialog(param, async (IDialogParameters obj) => await _popupNavigation.PopAsync()));
            }
        }

        private Task SortAsync(ECustomersSorting criterion)
        {
            if (_sortCriterion == criterion)
            {
                Customers = new (Customers.Reverse());
            }
            else
            {
                _sortCriterion = criterion;

                Func<CustomerBindableModel, object> comparer = criterion switch
                {
                    ECustomersSorting.ByName => x => x.Name,
                    ECustomersSorting.ByPoints => x => x.Points,
                    ECustomersSorting.ByPhoneNumber => x => x.Phone,
                    _ => throw new NotImplementedException(),
                };

                Customers = new (Customers.OrderBy(comparer));
            }

            return Task.CompletedTask;
        }

        #endregion

    }
}
