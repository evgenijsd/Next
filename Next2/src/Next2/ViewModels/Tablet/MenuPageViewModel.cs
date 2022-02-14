using Next2.Enums;
using Next2.Models;
using Next2.Services.Authentication;
using Next2.Views.Tablet.Dialogs;
using Prism.Navigation;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels.Tablet
{
    public class MenuPageViewModel : BaseViewModel
    {
        private IAuthenticationService _authenticationService;
        public MenuPageViewModel(
            INavigationService navigationService,
            IAuthenticationService authenticationService,
            NewOrderViewModel newOrderViewModel,
            HoldItemsViewModel holdItemsViewModel,
            OrderTabsViewModel orderTabsViewModel,
            ReservationsViewModel reservationsViewModel,
            MembershipViewModel membershipViewModel,
            CustomersViewModel customersViewModel,
            SettingsViewModel settingsViewModel)
            : base(navigationService)
        {
            NewOrderViewModel = newOrderViewModel;
            HoldItemsViewModel = holdItemsViewModel;
            OrderTabsViewModel = orderTabsViewModel;
            ReservationsViewModel = reservationsViewModel;
            MembershipViewModel = membershipViewModel;
            CustomersViewModel = customersViewModel;
            SettingsViewModel = settingsViewModel;
            _authenticationService = authenticationService;

            InitMenuItems();
        }

        #region -- Public properties --

        private ICommand _logOutCommand;
        public ICommand LogOutCommand => _logOutCommand ??= new AsyncCommand(OnLogOutCommandAsync);

        private MenuItemBindableModel _selectedMenuItem;
        public MenuItemBindableModel SelectedMenuItem
        {
            get => _selectedMenuItem;
            set
            {
                _selectedMenuItem?.ViewModel?.OnDisappearing();
                SetProperty(ref _selectedMenuItem, value);

                _selectedMenuItem?.ViewModel?.OnAppearing();
            }
        }

        public ObservableCollection<MenuItemBindableModel> MenuItems { get; set; }

        public NewOrderViewModel NewOrderViewModel { get; set; }

        public HoldItemsViewModel HoldItemsViewModel { get; set; }

        public OrderTabsViewModel OrderTabsViewModel { get; set; }

        public ReservationsViewModel ReservationsViewModel { get; set; }

        public MembershipViewModel MembershipViewModel { get; set; }

        public CustomersViewModel CustomersViewModel { get; set; }

        public SettingsViewModel SettingsViewModel { get; set; }

        #endregion

        #region -- Private methods --

        private void InitMenuItems()
        {
            MenuItems = new ObservableCollection<MenuItemBindableModel>()
            {
                new MenuItemBindableModel()
                {
                    State = EMenuItems.NewOrder,
                    Title = "New Order",
                    ImagePath = "ic_plus_30x30.png",
                    ViewModel = NewOrderViewModel,
                },
                new MenuItemBindableModel()
                {
                    State = EMenuItems.HoldItems,
                    Title = "Hold Items",
                    ImagePath = "ic_time_circle_30x30.png",
                    ViewModel = HoldItemsViewModel,
                },
                new MenuItemBindableModel()
                {
                    State = EMenuItems.OrderTabs,
                    Title = "Order & Tabs",
                    ImagePath = "ic_folder_30x30.png",
                    ViewModel = OrderTabsViewModel,
                },
                new MenuItemBindableModel()
                {
                    State = EMenuItems.Reservations,
                    Title = "Reservations",
                    ImagePath = "ic_bookmark_30x30.png",
                    ViewModel = ReservationsViewModel,
                },
                new MenuItemBindableModel()
                {
                    State = EMenuItems.Membership,
                    Title = "Membership",
                    ImagePath = "ic_work_30x30.png",
                    ViewModel = MembershipViewModel,
                },
                new MenuItemBindableModel()
                {
                    State = EMenuItems.Customers,
                    Title = "Customers",
                    ImagePath = "ic_user_30x30.png",
                    ViewModel = CustomersViewModel,
                },
                new MenuItemBindableModel()
                {
                    State = EMenuItems.Settings,
                    Title = "Settings",
                    ImagePath = "ic_setting_30x30.png",
                    ViewModel = SettingsViewModel,
                },
            };

            SelectedMenuItem = MenuItems.FirstOrDefault();
        }

        private async Task OnLogOutCommandAsync()
        {
            var param = new DialogParameters();

            string partOfMessage2 = LocalizationResourceManager.Current["WannaLogOut"];

            string okButton = LocalizationResourceManager.Current["LogOut(ApperCase)"];

            string cancelButton = LocalizationResourceManager.Current["Cancel"];

            param.Add(Constants.DialogParameterKeys.TITLE, $"{partOfMessage2}");

            param.Add(Constants.DialogParameterKeys.OK_BUTTON_TEXT, $"{okButton}");

            param.Add(Constants.DialogParameterKeys.CANCEL_BUTTON_TEXT, $"{cancelButton}");

            await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(new LogOutAlertView(param, CloseDialogCallback));
        }

        private async void CloseDialogCallback(IDialogParameters dialogResult)
        {
            bool result = (bool)dialogResult?[Constants.DialogParameterKeys.ACCEPT];

            if (result)
            {
                _authenticationService.LogOut();

                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAsync();

                var navigationParameters = new NavigationParameters
                {
                    { "IsLastUserLoggedOut", result },
                };

                await _navigationService.GoBackToRootAsync(navigationParameters);
            }
            else
            {
                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAsync();
            }
        }
        #endregion
    }
}
