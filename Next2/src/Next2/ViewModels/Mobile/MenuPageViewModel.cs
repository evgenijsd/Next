using Next2.Enums;
using Next2.Models;
using Next2.Services.Menu;
using Next2.Views.Mobile;
using Prism.Navigation;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels.Mobile
{
    public class MenuPageViewModel : BaseViewModel
    {
        private readonly IMenuService _menuService;

        private MenuItemBindableModel _oldSelectedMenuItem;

        public MenuPageViewModel(
            INavigationService navigationService,
            IMenuService menuService)
            : base(navigationService)
        {
            _menuService = menuService;

            InitMenuItems();
            Task.Run(LoadCategoriesAsync);
        }

        #region -- Public properties --

        public ObservableCollection<MenuItemBindableModel> MenuItems { get; set; }

        public MenuItemBindableModel SelectedMenuItem { get; set; }

        public ObservableCollection<CategoryModel> CategoriesItems { get; set; }

        private ICommand _tapCategoryCommand;
        public ICommand TapCategoryCommand => _tapCategoryCommand ??= new AsyncCommand<CategoryModel>(OnTapCategoryCommandAsync);

        private ICommand _goToSettingsCommand;
        public ICommand GoToSettingsCommand => _goToSettingsCommand ??= new AsyncCommand(GoToSettingsCommandAsync);

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

        private async Task LoadCategoriesAsync()
        {
            if (IsInternetConnected)
            {
                var resultCategories = await _menuService.GetCategoriesAsync();

                if (resultCategories.IsSuccess)
                {
                    CategoriesItems = new (resultCategories.Result);
                }
            }
        }

        private async Task OnTapCategoryCommandAsync(CategoryModel category)
        {
            var navigationParams = new NavigationParameters();
            navigationParams.Add(Constants.DialogParameterKeys.CATEGORY, category);

            await _navigationService.NavigateAsync(nameof(ChooseSetPage), navigationParams);
        }

        private async Task GoToSettingsCommandAsync()
        {
            await _navigationService.NavigateAsync($"{nameof(SettingsPage)}");
        }
        #endregion
    }
}