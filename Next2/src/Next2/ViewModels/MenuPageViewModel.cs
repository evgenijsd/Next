using Next2.ENums;
using Next2.Models;
using Next2.ViewModels.Mobile;
using Next2.ViewModels.Tablet;
using Prism.Navigation;
using System.Collections.ObjectModel;

namespace Next2.ViewModels
{
    public class MenuPageViewModel : BaseViewModel
    {
        public MenuPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            NewOrderViewModel = new NewOrderViewModel(navigationService);
            CategoryViewModel = new CategoryViewModel(navigationService);
            HoldItemsViewModel = new HoldItemsViewModel(navigationService);
            OrderTabsViewModel = new OrderTabsViewModel(navigationService);
            ReservationsViewModel = new ReservationsViewModel(navigationService);
            MembershipViewModel = new MembershipViewModel(navigationService);
            CustomersViewModel = new CustomersViewModel(navigationService);
            SettingsViewModel = new SettingsViewModel(navigationService);

            TabsSideMenuMob = new ObservableCollection<MenuItemBindableModel>()
            {
                new MenuItemBindableModel()
                {
                    State = EItemsMenu.NewOrder,
                    Title = "New Order",
                    ImagePath = "ic_plus_30x30.png",
                },
                new MenuItemBindableModel()
                {
                    State = EItemsMenu.HoldItems,
                    Title = "Hold Items",
                    ImagePath = "ic_time_circle_30x30.png",
                },
                new MenuItemBindableModel()
                {
                    State = EItemsMenu.OrderTabs,
                    Title = "Order & Tabs",
                    ImagePath = "ic_folder_30x30.png",
                },
                new MenuItemBindableModel()
                {
                    State = EItemsMenu.Customers,
                    Title = "Customers",
                    ImagePath = "ic_user_30x30.png",
                },
            };

            SelectedItemSideMenuMob = TabsSideMenuMob[0];

            TabsSideMenuTab = new ObservableCollection<MenuItemBindableModel>()
            {
                new MenuItemBindableModel()
                {
                    State = EItemsMenu.NewOrder,
                    Title = "New Order",
                    ImagePath = "ic_plus_30x30.png",
                },
                new MenuItemBindableModel()
                {
                    State = EItemsMenu.HoldItems,
                    Title = "Hold Items",
                    ImagePath = "ic_time_circle_30x30.png",
                },
                new MenuItemBindableModel()
                {
                    State = EItemsMenu.OrderTabs,
                    Title = "Order & Tabs",
                    ImagePath = "ic_folder_30x30.png",
                },
                new MenuItemBindableModel()
                {
                    State = EItemsMenu.Reservations,
                    Title = "Reservations",
                    ImagePath = "ic_bookmark_30x30.png",
                },
                new MenuItemBindableModel()
                {
                    State = EItemsMenu.Membership,
                    Title = "Membership",
                    ImagePath = "ic_work_30x30.png",
                },
                new MenuItemBindableModel()
                {
                    State = EItemsMenu.Customers,
                    Title = "Customers",
                    ImagePath = "ic_user_30x30.png",
                },
                new MenuItemBindableModel()
                {
                    State = EItemsMenu.Settings,
                    Title = "Settings",
                    ImagePath = "ic_setting_30x30.png",
                },
            };

            SelectedItemSideMenuTab = TabsSideMenuTab[0];
        }

        #region -- Public properties --

        public MenuItemBindableModel SelectedItemSideMenuMob { get; set; }

        public ObservableCollection<MenuItemBindableModel> TabsSideMenuMob { get; set; }

        public MenuItemBindableModel SelectedItemSideMenuTab { get; set; }

        public ObservableCollection<MenuItemBindableModel> TabsSideMenuTab { get; set; }

        public NewOrderViewModel NewOrderViewModel { get; set; }

        public CategoryViewModel CategoryViewModel { get; set; }

        public HoldItemsViewModel HoldItemsViewModel { get; set; }

        public OrderTabsViewModel OrderTabsViewModel { get; set; }

        public ReservationsViewModel ReservationsViewModel { get; set; }

        public MembershipViewModel MembershipViewModel { get; set; }

        public CustomersViewModel CustomersViewModel { get; set; }

        public SettingsViewModel SettingsViewModel { get; set; }

        #endregion
    }
}
