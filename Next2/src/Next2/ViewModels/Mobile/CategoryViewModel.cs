using Next2.Models;
using Prism.Navigation;
using System.Collections.ObjectModel;

namespace Next2.ViewModels.Mobile
{
    public class CategoryViewModel : BaseViewModel
    {
        public CategoryViewModel(INavigationService navigationService)
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
        }

        #region -- Public properties --

        public ObservableCollection<CategoryBindableModel> Categories { get; set; }

        #endregion
    }
}
