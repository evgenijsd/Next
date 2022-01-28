using Next2;
using Next2.Extensions;
using Next2.Models;
using Next2.Services.CustomersService;
using Next2.ViewModels;
using Next2.Views.Mobile.Dialogs;
using Next2.Views.Tablet.Dialogs;
using Prism.Navigation;
using Prism.Services.Dialogs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace InterTwitter.ViewModels
{
    public class CustomersPageViewModel : BaseViewModel
    {
        private readonly ICustomersService _customersService;
        private int _oldIndex = -1;
        public CustomersPageViewModel(INavigationService navigationService, ICustomersService customersService)
            : base(navigationService)
        {
            _customersService = customersService;
        }

        #region -- Public Properties --

        private ObservableCollection<CustomersViewModel>? _customersList;

        public ObservableCollection<CustomersViewModel>? CustomersList
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

        private CustomersViewModel _selectedItem;

        public CustomersViewModel SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }

        public ICommand TabSelectButtonCommand => new Command(OnTabSelectButtonCommand);
        public ICommand MobSelectButtonCommand => new Command(OnMobSelectButtonCommand);
        public ICommand TabInfoButtonCommand => new AsyncCommand(OnTabInfoButtonCommand);

        #endregion

        #region --Overrides--

        public override async void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
            await InitAsync();
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);
            if (args.PropertyName == nameof(SelectedItem))
            {
                if (_oldIndex == -1)
                {
                    _oldIndex = CustomersList.IndexOf(SelectedItem);
                    SelectedItem.CheckboxImage = "ic_check_box_checked_primary_24x24";
                }
                else
                {
                    if (CustomersList.IndexOf(SelectedItem) == _oldIndex)
                    {
                        SelectedItem.CheckboxImage = "ic_check_box_unhecked_24x24";
                    }

                    CustomersList[_oldIndex].CheckboxImage = "ic_check_box_unhecked_24x24";
                    SelectedItem.CheckboxImage = "ic_check_box_checked_primary_24x24";
                    _oldIndex = CustomersList.IndexOf(SelectedItem);
                }
            }
        }

        #endregion

        #region --Private Helpers--

        private async Task InitAsync()
        {
            var cl = await _customersService.GetAllCustomersAsync();
            if (cl.IsSuccess)
            {
                var list = cl.Result.Concat(cl.Result);
                var listvm = list.Select(x => x.ToCustomersViewModel());
                CustomersList = new ObservableCollection<CustomersViewModel>();
                foreach (var item in listvm)
                {
                    item.CheckboxImage = "ic_check_box_unhecked_24x24";
                    CustomersList?.Add(item);
                }
            }
        }

        private async Task OnTabInfoButtonCommand()
        {
            var param = new DialogParameters();
            param.Add(Constants.DialogParameterKeys.MODEL, SelectedItem);
            param.Add(Constants.DialogParameterKeys.OK_BUTTON_TEXT, "Select");
            param.Add(Constants.DialogParameterKeys.CANCEL_BUTTON_TEXT, "Cancel");
            await Rg.Plugins.Popup.Services
                .PopupNavigation.Instance
                .PushAsync(new CustomerInfoDialogTab(param, CloseDialogCallback));
        }

        private async void OnMobSelectButtonCommand(object obj)
        {
            var param = new DialogParameters();
            param.Add(Constants.DialogParameterKeys.MODEL, SelectedItem);
            param.Add(Constants.DialogParameterKeys.OK_BUTTON_TEXT, "Select");
            param.Add(Constants.DialogParameterKeys.CANCEL_BUTTON_TEXT, "Cancel");
            await Rg.Plugins.Popup.Services
                .PopupNavigation.Instance
                .PushAsync(new CustomerInfoDialogMob(param, CloseDialogCallback));
        }

        private async void OnTabSelectButtonCommand()
        {
        }

        private async void CloseDialogCallback(IDialogParameters obj)
        {
            await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAsync();
        }

        #endregion

    }
}
