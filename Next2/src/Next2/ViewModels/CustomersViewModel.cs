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
        private bool _isInitialized;
        public CustomersViewModel(INavigationService navigationService, ICustomersService customersService, IPopupNavigation popupNavigation)
            : base(navigationService)
        {
            _customersService = customersService;
            _popupNavigation = popupNavigation;
        }

        #region -- Public Properties --

        private ObservableCollection<CustomerViewModel>? _customersList;

        public ObservableCollection<CustomerViewModel>? CustomersList
        {
            get => _customersList;
            set => SetProperty(ref _customersList, value);
        }

        private CustomerModel _customer;

        public CustomerModel Customer
        {
            get => _customer;
            set => SetProperty(ref _customer, value);
        }

        private bool _isRefreshing;

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        private CustomerViewModel _oldSelectedItem;
        private CustomerViewModel _selectedItem;

        public CustomerViewModel SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }

        public ICommand ShowInfoCommand => new AsyncCommand<CustomerViewModel>(ShowCustomerInfoAsync);
        public ICommand SortCommand => new AsyncCommand<string>(SortAsync);
        public ICommand RefreshCommand => new AsyncCommand(RefreshAsync);

        #endregion

        #region --Overrides--

        public override async void OnAppearing()
        {
            base.OnAppearing();
            if (!_isInitialized)
            {
                await InitAsync();
                _isInitialized = true;
            }
        }

        #endregion

        #region --Private Helpers--

        private async Task InitAsync()
        {
            var castomersAoresult = await _customersService.GetAllCustomersAsync();
            if (castomersAoresult.IsSuccess)
            {
                var list = castomersAoresult.Result;
                var listvm = list.Select(x => x.ToCustomersViewModel());
                CustomersList = new ObservableCollection<CustomerViewModel>();
                foreach (var item in listvm)
                {
                    item.ShowInfoCommand = new AsyncCommand<CustomerViewModel>(ShowCustomerInfoAsync);
                    item.SelectItemCommand = new AsyncCommand<CustomerViewModel>(SelectDeselectItemAsync);
                    CustomersList?.Add(item);
                }
            }
        }

        private async Task RefreshAsync()
        {
            IsRefreshing = true;
            await InitAsync();
            IsRefreshing = false;
        }

        private async Task SelectDeselectItemAsync(CustomerViewModel customer)
        {
            SelectedItem = customer as CustomerViewModel;

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
        }

        private async Task ShowCustomerInfoAsync(CustomerViewModel customer)
        {
            if (customer != null)
            {
                SelectedItem = customer as CustomerViewModel;
            }

            var param = new DialogParameters();
            param.Add(Constants.DialogParameterKeys.MODEL, SelectedItem);
            param.Add(Constants.DialogParameterKeys.OK_BUTTON_TEXT, "Select");
            param.Add(Constants.DialogParameterKeys.CANCEL_BUTTON_TEXT, "Cancel");

            await _popupNavigation.PushAsync(new Views.Tablet.Dialogs
                .CustomerInfoDialog(param, async (IDialogParameters obj) => await _popupNavigation.PopAsync()));
        }

        private bool _isSortedAscending;
        private Task SortAsync(string param)
        {
            switch (param)
            {
                case "Name":
                    {
                        if (_isSortedAscending)
                        {
                            CustomersList = new ObservableCollection<CustomerViewModel>(CustomersList
                                .OrderByDescending(x => x.Name));
                            _isSortedAscending = false;
                        }
                        else
                        {
                            CustomersList = new ObservableCollection<CustomerViewModel>(CustomersList
                                .OrderBy(x => x.Name));
                            _isSortedAscending = true;
                        }

                        break;
                    }

                case "Points":
                    {
                        if (_isSortedAscending)
                        {
                            CustomersList = new ObservableCollection<CustomerViewModel>(CustomersList
                                .OrderByDescending(x => x.Points));
                            _isSortedAscending = false;
                        }
                        else
                        {
                            CustomersList = new ObservableCollection<CustomerViewModel>(CustomersList
                                .OrderBy(x => x.Points));
                            _isSortedAscending = true;
                        }

                        break;
                    }

                case "Phone":
                    {
                        if (_isSortedAscending)
                        {
                            CustomersList = new ObservableCollection<CustomerViewModel>(CustomersList
                                .OrderByDescending(x => x.Phone));
                            _isSortedAscending = false;
                        }
                        else
                        {
                            CustomersList = new ObservableCollection<CustomerViewModel>(CustomersList
                                .OrderBy(x => x.Phone));
                            _isSortedAscending = true;
                        }

                        break;
                    }
            }

            return Task.CompletedTask;
        }

        #endregion

    }
}
