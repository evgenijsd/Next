using Next2;
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

        public ObservableCollection<CustomerBindableModel>? Customers { get; set; }

        public CustomerModel Customer { get; set; }

        public bool IsRefreshing { get; set; }

        public CustomerBindableModel? SelectedItem { get; set; }

        public ICommand ShowInfoCommand => new AsyncCommand<CustomerBindableModel>(ShowCustomerInfoAsync);

        public ICommand SortCommand => new AsyncCommand<string>(SortAsync);

        public ICommand RefreshCommand => new AsyncCommand(RefreshAsync);

        #endregion

        #region --Overrides--

        public override async void OnAppearing()
        {
            base.OnAppearing();
            await RefreshAsync();
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();
            Customers?.Clear();
        }

        #endregion

        #region --Private Helpers--

        private async Task RefreshAsync()
        {
            IsRefreshing = true;

            var castomersAoresult = await _customersService.GetAllCustomersAsync();
            if (castomersAoresult.IsSuccess)
            {
                var list = castomersAoresult.Result;
                var listvm = list.Select(x => x.ToCustomerBindableModel());
                Customers = new ObservableCollection<CustomerBindableModel>();
                foreach (var item in listvm)
                {
                    item.ShowInfoCommand = new AsyncCommand<CustomerBindableModel>(ShowCustomerInfoAsync);
                    item.SelectItemCommand = new AsyncCommand<CustomerBindableModel>(SelectDeselectItemAsync);
                    Customers?.Add(item);
                }

                await SortAsync("Name");
            }

            IsRefreshing = false;
        }

        private CustomerBindableModel? _oldSelectedItem;
        private Task SelectDeselectItemAsync(CustomerBindableModel customer)
        {
            SelectedItem = customer as CustomerBindableModel;

            if (_oldSelectedItem == null)
            {
                _oldSelectedItem = SelectedItem;
            }
            else
            {
                if (SelectedItem == _oldSelectedItem)
                {
                    SelectedItem = null;
                    _oldSelectedItem = null;
                }
                else
                {
                    _oldSelectedItem = SelectedItem;
                }
            }

            return Task.CompletedTask;
        }

        private async Task ShowCustomerInfoAsync(CustomerBindableModel customer)
        {
            if (customer != null)
            {
                SelectedItem = customer as CustomerBindableModel;
            }

            if (SelectedItem != null)
            {
                var param = new DialogParameters();
                param.Add(Constants.DialogParameterKeys.MODEL, SelectedItem);
                param.Add(Constants.DialogParameterKeys.OK_BUTTON_TEXT, "Select");
                param.Add(Constants.DialogParameterKeys.CANCEL_BUTTON_TEXT, "Cancel");

                await _popupNavigation.PushAsync(new Views.Tablet.Dialogs
                    .CustomerInfoDialog(param, async (IDialogParameters obj) => await _popupNavigation.PopAsync()));
            }
        }

        private string _sortCriterion;
        private Task SortAsync(string criterion)
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
                    "Name" => x => x.Name,
                    "Points" => x => x.Points,
                    "Phone" => x => x.Phone,
                };
                Customers = new (Customers.OrderBy(comparer));
            }

            return Task.CompletedTask;
        }

        #endregion

    }
}
