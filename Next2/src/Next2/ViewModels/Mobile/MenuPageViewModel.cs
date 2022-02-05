using Next2.Enums;
using Next2.Models;
using Next2.Services.Menu;
using Next2.Views.Mobile;
using Prism.Navigation;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Next2.ViewModels.Mobile
{
    public class MenuPageViewModel : BaseViewModel
    {
        private MenuItemBindableModel _oldSelectedMenuItem;

        private IMenuService _menuService;

        public MenuPageViewModel(
            INavigationService navigationService,
            IMenuService menuService)
            : base(navigationService)
        {
            _menuService = menuService;

            InitMenuItems();
            InitCategories();
        }

        #region -- Public properties --

        public ObservableCollection<CategoryModel> Categories { get; set; }

        public MenuItemBindableModel SelectedMenuItem { get; set; }

        public ObservableCollection<MenuItemBindableModel> MenuItems { get; set; }

        #endregion

        #region -- Overrides --

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            SelectedMenuItem = MenuItems.FirstOrDefault();
            _oldSelectedMenuItem = SelectedMenuItem;
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            if (args.PropertyName == nameof(SelectedMenuItem))
            {
                if (_oldSelectedMenuItem.State == EMenuItems.NewOrder)
                {
                    _oldSelectedMenuItem = SelectedMenuItem;

                    switch (SelectedMenuItem.State)
                    {
                        case EMenuItems.HoldItems:
                            _navigationService.NavigateAsync(nameof(HoldItemsPage));
                            break;
                        case EMenuItems.OrderTabs:
                            _navigationService.NavigateAsync(nameof(OrderTabsPage));
                            break;
                        case EMenuItems.Customers:
                            _navigationService.NavigateAsync(nameof(CustomersPage));
                            break;
                    }
                }
            }
        }

        #endregion

        #region -- Private methods --

        private void InitMenuItems()
        {
            MenuItems = new ()
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

            _oldSelectedMenuItem = MenuItems.FirstOrDefault();
            SelectedMenuItem = _oldSelectedMenuItem;
        }

        private void InitCategories()
        {
            if (IsInternetConnected)
            {
                var result = _menuService.GetCategoriesAsync();
                var categories = result?.Result?.Result;

                Categories = new (categories);
            }
        }

        #endregion
    }
}
