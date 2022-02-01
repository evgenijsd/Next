using Next2.ENums;
using Next2.Models;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Next2.ViewModels.Tablet
{
    public class MenuPageViewModel : BaseViewModel
    {
        private MenuItemBindableModel _oldSelectedMenuItem;

        public MenuPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            NewOrderViewModel = new NewOrderViewModel(navigationService);
            HoldItemsViewModel = new HoldItemsViewModel(navigationService);
            OrderTabsViewModel = new OrderTabsViewModel(navigationService);
            ReservationsViewModel = new ReservationsViewModel(navigationService);
            MembershipViewModel = new MembershipViewModel(navigationService);
            CustomersViewModel = new CustomersViewModel(navigationService);
            SettingsViewModel = new SettingsViewModel(navigationService);

            MenuItems = new ObservableCollection<MenuItemBindableModel>()
            {
                new MenuItemBindableModel()
                {
                    State = EMenuItems.NewOrder,
                    Title = "New Order",
                    ImagePath = "ic_plus_30x30.png",
                },
                new MenuItemBindableModel()
                {
                    State = EMenuItems.HoldItems,
                    Title = "Hold Items",
                    ImagePath = "ic_time_circle_30x30.png",
                },
                new MenuItemBindableModel()
                {
                    State = EMenuItems.OrderTabs,
                    Title = "Order & Tabs",
                    ImagePath = "ic_folder_30x30.png",
                },
                new MenuItemBindableModel()
                {
                    State = EMenuItems.Reservations,
                    Title = "Reservations",
                    ImagePath = "ic_bookmark_30x30.png",
                },
                new MenuItemBindableModel()
                {
                    State = EMenuItems.Membership,
                    Title = "Membership",
                    ImagePath = "ic_work_30x30.png",
                },
                new MenuItemBindableModel()
                {
                    State = EMenuItems.Customers,
                    Title = "Customers",
                    ImagePath = "ic_user_30x30.png",
                },
                new MenuItemBindableModel()
                {
                    State = EMenuItems.Settings,
                    Title = "Settings",
                    ImagePath = "ic_setting_30x30.png",
                },
            };

            SelectedMenuItem = MenuItems.FirstOrDefault();
        }

        #region -- Public properties --

        public MenuItemBindableModel _selectedMenuItem;
        public MenuItemBindableModel SelectedMenuItem
        {
            get
            {
                return _selectedMenuItem;
            }
            set
            {
                _oldSelectedMenuItem = _selectedMenuItem;
                SetProperty(ref _selectedMenuItem, value);
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

        #region -- Overrides --

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            if (args.PropertyName == nameof(SelectedMenuItem))
            {
                switch (SelectedMenuItem.State)
                {
                    case EMenuItems.NewOrder:
                        NewOrderViewModel.OnAppearing();
                        break;
                    case EMenuItems.HoldItems:
                        HoldItemsViewModel.OnAppearing();
                        break;
                    case EMenuItems.OrderTabs:
                        OrderTabsViewModel.OnAppearing();
                        break;
                    case EMenuItems.Reservations:
                        ReservationsViewModel.OnAppearing();
                        break;
                    case EMenuItems.Membership:
                        MembershipViewModel.OnAppearing();
                        break;
                    case EMenuItems.Customers:
                        CustomersViewModel.OnAppearing();
                        break;
                    case EMenuItems.Settings:
                        SettingsViewModel.OnAppearing();
                        break;
                }

                if (_oldSelectedMenuItem != null)
                {
                    switch (_oldSelectedMenuItem.State)
                    {
                        case EMenuItems.NewOrder:
                            NewOrderViewModel.OnDisappearing();
                            break;
                        case EMenuItems.HoldItems:
                            HoldItemsViewModel.OnDisappearing();
                            break;
                        case EMenuItems.OrderTabs:
                            OrderTabsViewModel.OnDisappearing();
                            break;
                        case EMenuItems.Reservations:
                            ReservationsViewModel.OnDisappearing();
                            break;
                        case EMenuItems.Membership:
                            MembershipViewModel.OnDisappearing();
                            break;
                        case EMenuItems.Customers:
                            CustomersViewModel.OnDisappearing();
                            break;
                        case EMenuItems.Settings:
                            SettingsViewModel.OnDisappearing();
                            break;
                    }
                }
            }
        }

        #endregion
    }
}
