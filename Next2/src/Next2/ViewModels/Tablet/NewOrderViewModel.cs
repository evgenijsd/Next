using Next2.Enums;
using Next2.Extensions;
using Next2.Interfaces;
using Next2.Models;
using Next2.Services.Menu;
using Next2.Services.MockService;
using Prism.Navigation;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Next2.ViewModels.Tablet
{
    public class NewOrderViewModel : BaseViewModel, IPageActionsHandler
    {
        private IMenuService _menuService;

        public NewOrderViewModel(
            INavigationService navigationService,
            IMenuService menuService)
            : base(navigationService)
        {
            _menuService = menuService;

            if (IsInternetConnected)
            {
                var result = _menuService.GetCategories();
                if (result.IsCompleted)
                {
                    if (result.Result.IsSuccess)
                    {
                        CategoriesItems = new ObservableCollection<CategoryBindableModel>(result.Result.Result.Select(row => row.ToBindableModel()));
                        SelectedCategoryItem = CategoriesItems.FirstOrDefault();
                    }
                }
            }

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

        private ObservableCollection<CategoryBindableModel> _categoriesItems;
        public ObservableCollection<CategoryBindableModel> CategoriesItems
        {
            get => _categoriesItems;
            set => SetProperty(ref _categoriesItems, value);
        }

        private CategoryBindableModel _selectedCategoryItem;
        public CategoryBindableModel SelectedCategoryItem
        {
            get => _selectedCategoryItem;
            set => SetProperty(ref _selectedCategoryItem, value);
        }

        private MenuItemBindableModel _selectedMenuItem;
        public MenuItemBindableModel SelectedMenuItem
        {
            get => _selectedMenuItem;
            set
            {
                SetProperty(ref _selectedMenuItem, value);
            }
        }

        public ObservableCollection<MenuItemBindableModel> MenuItems { get; set; }

        #endregion

        #region -- IPageActionsHandler implementation --

        public void OnAppearing()
        {
        }

        public void OnDisappearing()
        {
        }

        #endregion
    }
}