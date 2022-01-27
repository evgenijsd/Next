using Next2.ENums;
using Next2.Models;
using Prism.Navigation;
using System.Collections.ObjectModel;

namespace Next2.ViewModels
{
    public class MenuPageViewModel : BaseViewModel
    {
        public MenuPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            BindingNewOrderViewModel = new NewOrderViewModel(navigationService);
            BindingHoldItemsViewModel = new HoldItemsViewModel(navigationService);
            BindingOrderTabsViewModel = new OrderTabsViewModel(navigationService);
            BindingReservationsViewModel = new ReservationsViewModel(navigationService);
            BindingMembershipViewModel = new MembershipViewModel(navigationService);
            BindingCustomersViewModel = new CustomersViewModel(navigationService);
            BindingSettingsViewModel = new SettingsViewModel(navigationService);

            TabsSideMenuMob = new ObservableCollection<ItemMenuModel>()
            {
                new ItemMenuModel()
                {
                    State = EItemsMenu.NewOrder,
                    Title = "New Order",
                    ImagePath = "ic_plus_30x30.png",
                },
                new ItemMenuModel()
                {
                    State = EItemsMenu.HoldItems,
                    Title = "Hold Items",
                    ImagePath = "ic_time_circle_30x30.png",
                },
                new ItemMenuModel()
                {
                    State = EItemsMenu.OrderTabs,
                    Title = "Order & Tabs",
                    ImagePath = "ic_folder_30x30.png",
                },
                new ItemMenuModel()
                {
                    State = EItemsMenu.Customers,
                    Title = "Customers",
                    ImagePath = "ic_user_30x30.png",
                },
            };

            SelectedItemSideMenuMob = TabsSideMenuMob[0];

            TabsSideMenuTab = new ObservableCollection<ItemMenuModel>()
            {
                new ItemMenuModel()
                {
                    State = EItemsMenu.NewOrder,
                    Title = "New Order",
                    ImagePath = "ic_plus_30x30.png",
                },
                new ItemMenuModel()
                {
                    State = EItemsMenu.HoldItems,
                    Title = "Hold Items",
                    ImagePath = "ic_time_circle_30x30.png",
                },
                new ItemMenuModel()
                {
                    State = EItemsMenu.OrderTabs,
                    Title = "Order & Tabs",
                    ImagePath = "ic_folder_30x30.png",
                },
                new ItemMenuModel()
                {
                    State = EItemsMenu.Reservations,
                    Title = "Reservations",
                    ImagePath = "ic_bookmark_30x30.png",
                },
                new ItemMenuModel()
                {
                    State = EItemsMenu.Membership,
                    Title = "Membership",
                    ImagePath = "ic_work_30x30.png",
                },
                new ItemMenuModel()
                {
                    State = EItemsMenu.Customers,
                    Title = "Customers",
                    ImagePath = "ic_user_30x30.png",
                },
                new ItemMenuModel()
                {
                    State = EItemsMenu.Settings,
                    Title = "Settings",
                    ImagePath = "ic_setting_30x30.png",
                },
            };

            SelectedItemSideMenuTab = TabsSideMenuTab[0];
        }

        #region -- Public properties --

        public ItemMenuModel? SelectedItemSideMenuMob { get; set; }

        public ObservableCollection<ItemMenuModel>? TabsSideMenuMob { get; set; }

        public ItemMenuModel? SelectedItemSideMenuTab { get; set; }

        public ObservableCollection<ItemMenuModel>? TabsSideMenuTab { get; set; }

        public NewOrderViewModel? BindingNewOrderViewModel { get; set; }

        public HoldItemsViewModel? BindingHoldItemsViewModel { get; set; }

        public OrderTabsViewModel? BindingOrderTabsViewModel { get; set; }

        public ReservationsViewModel? BindingReservationsViewModel { get; set; }

        public MembershipViewModel? BindingMembershipViewModel { get; set; }

        public CustomersViewModel? BindingCustomersViewModel { get; set; }

        public SettingsViewModel? BindingSettingsViewModel { get; set; }

        #endregion
    }
}
