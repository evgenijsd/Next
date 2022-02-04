using Next2;
using Next2.Extensions;
using Next2.Models;
using Next2.Services.CustomersService;
using Next2.ViewModels;
using Next2.Views.Mobile.Dialogs;
using Next2.Views.Tablet.Dialogs;
using Prism.Navigation;
using Prism.Services.Dialogs;
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
        private bool _isInitialized;
        public CustomersViewModel(INavigationService navigationService, ICustomersService customersService)
            : base(navigationService)
        {
            _customersService = customersService;
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

        public ICommand TabInfoButtonCommand => new AsyncCommand(ShowCustomerInfoAsync);
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
            var cl = await _customersService.GetAllCustomersAsync();
            if (cl.IsSuccess)
            {
                var list = cl.Result;
                var listvm = list.Select(x => x.ToCustomersViewModel());
                CustomersList = new ObservableCollection<CustomerViewModel>();
                foreach (var item in listvm)
                {
                    item.CheckboxImage = "ic_check_box_unhecked_24x24";
                    item.MobSelectCommand = new Command<object>(ShowInfoAndSelectAsync);
                    item.TabSelectCommand = new Command<object>(SelectDeselectItem);
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

        private void SelectDeselectItem(object obj)
        {
            SelectedItem = obj as CustomerViewModel;

            if (_oldSelectedItem == null)
            {
                _oldSelectedItem = SelectedItem;
                SelectedItem.CheckboxImage = "ic_check_box_checked_primary_24x24";
            }
            else
            {
                if (SelectedItem == _oldSelectedItem)
                {
                    SelectedItem.CheckboxImage = "ic_check_box_unhecked_24x24";
                    SelectedItem = null;
                    _oldSelectedItem = null;
                }
                else
                {
                    var si = CustomersList.Where(x => x.Id == _oldSelectedItem.Id).FirstOrDefault();
                    si.CheckboxImage = "ic_check_box_unhecked_24x24";
                    SelectedItem.CheckboxImage = "ic_check_box_checked_primary_24x24";
                    _oldSelectedItem = SelectedItem;
                }
            }
        }

        private async Task ShowCustomerInfoAsync()
        {
            if (SelectedItem != null)
            {
                var param = new DialogParameters();
                param.Add(Constants.DialogParameterKeys.MODEL, SelectedItem);
                param.Add(Constants.DialogParameterKeys.OK_BUTTON_TEXT, "Select");
                param.Add(Constants.DialogParameterKeys.CANCEL_BUTTON_TEXT, "Cancel");
                await Rg.Plugins.Popup.Services
                    .PopupNavigation.Instance
                    .PushAsync(new CustomerInfoDialogTab(param, CloseDialogCallback));
            }
        }

        private async void ShowInfoAndSelectAsync(object obj)
        {
            SelectedItem = obj as CustomerViewModel;
            var param = new DialogParameters();
            param.Add(Constants.DialogParameterKeys.MODEL, obj as CustomerViewModel);
            param.Add(Constants.DialogParameterKeys.OK_BUTTON_TEXT, "Select");
            param.Add(Constants.DialogParameterKeys.CANCEL_BUTTON_TEXT, "Cancel");
            await Rg.Plugins.Popup.Services
                .PopupNavigation.Instance
                .PushAsync(new CustomerInfoDialogMob(param, CloseDialogCallback));
        }

        private async void CloseDialogCallback(IDialogParameters obj)
        {
            await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAsync();
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
