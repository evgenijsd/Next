using System.Collections.ObjectModel;
using Next2.ENums;
using Next2.Models;
using Prism.Navigation;

namespace Next2.ViewModels
{
    public class NewOrderViewModel : BaseViewModel
    {
        public NewOrderViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Text = "NewOrder";

            Categories = new ObservableCollection<ItemMenuModel>()
            {
                new ItemMenuModel()
                {
                    State = EItemsMenu.NewOrder,
                    Title = "Baskets & Meals",
                },
                new ItemMenuModel()
                {
                    State = EItemsMenu.HoldItems,
                    Title = "Sauces",
                },
                new ItemMenuModel()
                {
                    State = EItemsMenu.OrderTabs,
                    Title = "Steaks & Chops",
                },
                new ItemMenuModel()
                {
                    State = EItemsMenu.Reservations,
                    Title = "Sides & Snack",
                },
                new ItemMenuModel()
                {
                    State = EItemsMenu.Membership,
                    Title = "Starters",
                },
                new ItemMenuModel()
                {
                    State = EItemsMenu.NewOrder,
                    Title = "Dessert",
                },
                new ItemMenuModel()
                {
                    State = EItemsMenu.HoldItems,
                    Title = "Salads",
                },
                new ItemMenuModel()
                {
                    State = EItemsMenu.OrderTabs,
                    Title = "Beverages",
                },
                new ItemMenuModel()
                {
                    State = EItemsMenu.Reservations,
                    Title = "Burgers & Sandwiches",
                },
                new ItemMenuModel()
                {
                    State = EItemsMenu.Membership,
                    Title = "Breakfast",
                },
                new ItemMenuModel()
                {
                    State = EItemsMenu.NewOrder,
                    Title = "Soups",
                },
                new ItemMenuModel()
                {
                    State = EItemsMenu.NewOrder,
                    Title = "Baskets & Meals",
                },
                new ItemMenuModel()
                {
                    State = EItemsMenu.HoldItems,
                    Title = "Sauces",
                },
                new ItemMenuModel()
                {
                    State = EItemsMenu.OrderTabs,
                    Title = "Steaks & Chops",
                },
                new ItemMenuModel()
                {
                    State = EItemsMenu.Reservations,
                    Title = "Sides & Snack",
                },
                new ItemMenuModel()
                {
                    State = EItemsMenu.Membership,
                    Title = "Starters",
                },
                new ItemMenuModel()
                {
                    State = EItemsMenu.NewOrder,
                    Title = "Dessert",
                },
                new ItemMenuModel()
                {
                    State = EItemsMenu.HoldItems,
                    Title = "Salads",
                },
                new ItemMenuModel()
                {
                    State = EItemsMenu.OrderTabs,
                    Title = "Beverages",
                },
                new ItemMenuModel()
                {
                    State = EItemsMenu.Reservations,
                    Title = "Burgers & Sandwiches",
                },
                new ItemMenuModel()
                {
                    State = EItemsMenu.Membership,
                    Title = "Breakfast",
                },
                new ItemMenuModel()
                {
                    State = EItemsMenu.NewOrder,
                    Title = "Soups",
                },
            };
        }

        #region -- Public properties --

        public string? Text { get; set; }

        public ObservableCollection<ItemMenuModel>? Categories { get; set; }

        #endregion
    }
}
