using Next2.Enums;
using Next2.Models;
using Next2.Services;
using Next2.Services.Membership;
using Next2.ViewModels.Mobile;
using Next2.ViewModels.Tablet;
using Prism.Navigation;
using System.Collections.ObjectModel;
using System.Linq;

namespace Next2.ViewModels
{
    public class MenuPageViewModel : BaseViewModel
    {
        public MenuPageViewModel(
            INavigationService navigationService,
            IMembershipService membershipService,
            IOrderService orderService)
            : base(navigationService)
        {
            NewOrderViewModel = new NewOrderViewModel(navigationService);
            HoldItemsViewModel = new HoldItemsViewModel(navigationService);
            OrderTabsViewModel = new OrderTabsViewModel(navigationService, orderService);
            ReservationsViewModel = new ReservationsViewModel(navigationService);
            MembershipViewModel = new MembershipViewModel(navigationService, membershipService);
            CustomersViewModel = new CustomersViewModel(navigationService);
            SettingsViewModel = new SettingsViewModel(navigationService);

            if (App.IsTablet)
            {
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
            }
            else
            {
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
                        State = EMenuItems.Customers,
                        Title = "Customers",
                        ImagePath = "ic_user_30x30.png",
                    },
                };
            }

            SelectedMenuItem = MenuItems.FirstOrDefault();
        }

        #region -- Public properties --

        public MenuItemBindableModel SelectedMenuItem { get; set; }

        public ObservableCollection<MenuItemBindableModel> MenuItems { get; set; }

        public NewOrderViewModel NewOrderViewModel { get; set; }

        public HoldItemsViewModel HoldItemsViewModel { get; set; }

        public OrderTabsViewModel OrderTabsViewModel { get; set; }

        public ReservationsViewModel ReservationsViewModel { get; set; }

        public MembershipViewModel MembershipViewModel { get; set; }

        public CustomersViewModel CustomersViewModel { get; set; }

        public SettingsViewModel SettingsViewModel { get; set; }

        #endregion
    }
}
