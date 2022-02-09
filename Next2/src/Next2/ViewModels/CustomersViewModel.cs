using Next2;
using Next2.Enums;
using Next2.Extensions;
using Next2.Models;
using Next2.Services.CustomersService;
using Next2.ViewModels;
using Next2.Views.Mobile.Dialogs;
using Next2.Views.Tablet.Dialogs;
using Prism.Navigation;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Contracts;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Next2.ViewModels
{
    public class CustomersViewModel : BaseViewModel
    {
        private readonly ICustomersService _customersService;
        private readonly IPopupNavigation _popupNavigation;
        private CustomerBindableModel? _oldSelectedItem;
        private ESorting _sortCriterion;

        public CustomersViewModel(
            INavigationService navigationService,
            ICustomersService customersService,
            IPopupNavigation popupNavigation)
            : base(navigationService)
        {
            _customersService = customersService;
            _popupNavigation = popupNavigation;
        }

        #region -- Public Properties --

        public ObservableCollection<CustomerBindableModel> Customers { get; set; }

        public bool IsRefreshing { get; set; }

        public CustomerBindableModel? SelectedCustomer { get; set; }

        private ICommand _showInfoCommand;
        public ICommand ShowInfoCommand => _showInfoCommand ??= new AsyncCommand<CustomerBindableModel>(ShowCustomerInfoAsync);

        private ICommand _sortCommand;
        public ICommand SortCommand => _sortCommand ??= new AsyncCommand<ESorting>(SortAsync);

        private ICommand _refreshCommand;
        public ICommand RefreshCommand => _refreshCommand ??= new AsyncCommand(RefreshAsync);

        #endregion

        #region --Overrides--

        public override async void OnAppearing()
        {
            cts = new CancellationTokenSource();
            token = cts.Token;

            await RefreshAsync();
        }

        public override async void OnDisappearing()
        {
            Customers = new ();
           // Customers?.Clear();
            SelectedCustomer = null;
            cts.Cancel();
            await Task.Delay(100);
        }

        #endregion

        #region --Private Helpers--

        private CancellationTokenSource cts;
        private CancellationToken token;
        private async Task RefreshAsync()
        {
            IsRefreshing = true;

            var castomersAoresult = await _customersService.GetAllCustomersAsync();
            if (castomersAoresult.IsSuccess)
            {
                var list = castomersAoresult.Result;
                var listvm = list.Select(x => x.ToCustomerBindableModel()).OrderBy(x => x.Name);
                var customers = new ObservableCollection<CustomerBindableModel>();
                foreach (var item in listvm)
                {
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }
                    else
                    {
                        item.ShowInfoCommand = new AsyncCommand<CustomerBindableModel>(ShowCustomerInfoAsync);
                        item.SelectItemCommand = new AsyncCommand<CustomerBindableModel>(SelectDeselectItemAsync);
                        customers.Add(item);
                    }
                }

                if (customers.Count > 0)
                {
                    Customers = customers;
                }
            }

            IsRefreshing = false;
        }

        private Task SelectDeselectItemAsync(CustomerBindableModel customer)
        {
            SelectedCustomer = customer;

            if (_oldSelectedItem == null)
            {
                _oldSelectedItem = SelectedCustomer;
            }
            else
            {
                if (SelectedCustomer == _oldSelectedItem)
                {
                    SelectedCustomer = null;
                    _oldSelectedItem = null;
                }
                else
                {
                    _oldSelectedItem = SelectedCustomer;
                }
            }

            return Task.CompletedTask;
        }

        private async Task ShowCustomerInfoAsync(CustomerBindableModel customer)
        {
            if (customer is CustomerBindableModel selectedCustomer)
            {
                var param = new DialogParameters();
                param.Add(Constants.DialogParameterKeys.MODEL, selectedCustomer);

                await _popupNavigation.PushAsync(new Views.Tablet.Dialogs
                    .CustomerInfoDialog(param, async (IDialogParameters obj) => await _popupNavigation.PopAsync()));
            }
        }

        private Task SortAsync(ESorting criterion)
        {
            if (SelectedCustomer == null && criterion == ESorting.ByPoints)
            {
                return Task.CompletedTask;
            }
            else
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
                        ESorting.ByName => x => x.Name,
                        ESorting.ByPoints => x => x.Points,
                        ESorting.ByPhoneNumber => x => x.Phone,
                    };
                    Customers = new (Customers.OrderBy(comparer));
                }

                return Task.CompletedTask;
            }
        }

        #endregion

    }
}
