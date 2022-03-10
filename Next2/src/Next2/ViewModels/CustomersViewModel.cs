using AutoMapper;
using Next2.Enums;
using Next2.Models;
using Next2.Services.CustomersService;
using Prism.Navigation;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Contracts;
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
        private readonly ICustomersService _customersService;
        private readonly IPopupNavigation _popupNavigation;
        private ECustomersSorting _sortCriterion;

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
        public ICommand SortCommand => _sortCommand ??= new AsyncCommand<ECustomersSorting>(SortAsync);

        private ICommand _refreshCommand;
        public ICommand RefreshCommand => _refreshCommand ??= new AsyncCommand(RefreshAsync);

        private ICommand _addCustomerCommand;
        public ICommand AddCustomerCommand => _addCustomerCommand ??= new AsyncCommand<CustomerBindableModel>(AddCustomerAsync);

        #endregion

        #region -- Overrides --

        public override async void OnAppearing()
        {
            await RefreshAsync();
        }

        public override async void OnDisappearing()
        {
            Customers = new ();
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
                MapperConfiguration config;
                config = new MapperConfiguration(cfg => cfg.CreateMap<CustomerModel, CustomerBindableModel>());
                var mapper = new Mapper(config);

                var result = customersAoresult.Result.OrderBy(x => x.Name);
                var customers = mapper.Map<IEnumerable<CustomerModel>, ObservableCollection<CustomerBindableModel>>(result);

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
                var param = new DialogParameters();
                param.Add(Constants.DialogParameterKeys.MODEL, selectedCustomer);

                if (App.IsTablet)
                {
                    await _popupNavigation.PushAsync(new Views.Tablet.Dialogs
                        .CustomerInfoDialog(param, async (IDialogParameters obj) => await _popupNavigation.PopAsync()));
                }
                else
                {
                    await _popupNavigation.PushAsync(new Views.Mobile.Dialogs
                        .CustomerInfoDialog(param, async (IDialogParameters obj) => await _popupNavigation.PopAsync()));
                }
            }
        }

        private async Task AddCustomerAsync(CustomerBindableModel customer)
        {
                var param = new DialogParameters();
                if (Device.Idiom == TargetIdiom.Phone)
                {
                    await _popupNavigation.PushAsync(new Views.Mobile.Dialogs
                    .CustomerAddDialog(param, AddCustomerDialogCallBack, _customersService));
                }
                else
                {
                    await _popupNavigation.PushAsync(new Views.Tablet.Dialogs
                    .CustomerAddDialog(param, AddCustomerDialogCallBack, _customersService));
                }
        }

        private async void AddCustomerDialogCallBack(IDialogParameters param)
        {
            await _popupNavigation.PopAsync();

            if (param.TryGetValue("Id", out int id))
            {
                await RefreshAsync();
                int index = Customers.IndexOf(Customers.FirstOrDefault(x => x.Id == id));
                Customers.Move(index, 0);
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
