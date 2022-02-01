using Next2.Enums;
using Next2.Models;
using Next2.Views.Mobile;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Next2.ViewModels.Mobile
{
    public class MenuPageViewModel : BaseViewModel
    {
        private MenuItemBindableModel _oldSelectedMenuItem;

        public MenuPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Categories = new ObservableCollection<CategoryBindableModel>()
            {
                new CategoryBindableModel()
                {
                    Id = 1,
                    Title = "Baskets & Meals",
                },
                new CategoryBindableModel()
                {
                    Id = 2,
                    Title = "Sauces",
                },
                new CategoryBindableModel()
                {
                    Id = 3,
                    Title = "Steaks & Chops",
                },
                new CategoryBindableModel()
                {
                    Id = 4,
                    Title = "Sides & Snack",
                },
                new CategoryBindableModel()
                {
                    Id = 5,
                    Title = "Starters",
                },
                new CategoryBindableModel()
                {
                    Id = 6,
                    Title = "Dessert",
                },
                new CategoryBindableModel()
                {
                    Id = 7,
                    Title = "Salads",
                },
                new CategoryBindableModel()
                {
                    Id = 8,
                    Title = "Beverages",
                },
                new CategoryBindableModel()
                {
                    Id = 9,
                    Title = "Burgers & Sandwiches",
                },
                new CategoryBindableModel()
                {
                    Id = 10,
                    Title = "Breakfast",
                },
                new CategoryBindableModel()
                {
                    Id = 11,
                    Title = "Soups",
                },
                new CategoryBindableModel()
                {
                    Id = 12,
                    Title = "Baskets & Meals",
                },
                new CategoryBindableModel()
                {
                    Id = 13,
                    Title = "Sauces",
                },
                new CategoryBindableModel()
                {
                    Id = 14,
                    Title = "Steaks & Chops",
                },
                new CategoryBindableModel()
                {
                    Id = 15,
                    Title = "Sides & Snack",
                },
                new CategoryBindableModel()
                {
                    Id = 16,
                    Title = "Starters",
                },
                new CategoryBindableModel()
                {
                    Id = 17,
                    Title = "Dessert",
                },
                new CategoryBindableModel()
                {
                    Id = 18,
                    Title = "Salads",
                },
                new CategoryBindableModel()
                {
                    Id = 19,
                    Title = "Beverages",
                },
                new CategoryBindableModel()
                {
                    Id = 20,
                    Title = "Burgers & Sandwiches",
                },
                new CategoryBindableModel()
                {
                    Id = 21,
                    Title = "Breakfast",
                },
                new CategoryBindableModel()
                {
                    Id = 22,
                    Title = "Soups",
                },
            };

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

            _oldSelectedMenuItem = MenuItems.FirstOrDefault();
            SelectedMenuItem = _oldSelectedMenuItem;
        }

        #region -- Public properties --

        public ObservableCollection<CategoryBindableModel> Categories { get; set; }

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
    }
}
