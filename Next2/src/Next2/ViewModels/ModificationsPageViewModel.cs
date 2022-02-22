using Next2.Models;
using Prism.Navigation;
using System.Collections.ObjectModel;

namespace Next2.ViewModels
{
    public class ModificationsPageViewModel : BaseViewModel
    {
        public ModificationsPageViewModel(
            INavigationService navigationService)
            : base(navigationService)
        {
            Items = new ()
            {
                new ItemSpoilerModel()
                {
                    Title = "Replace",
                    ImagePath = "ic_paper_plus_24x24.png",
                    SelectedImagePath = "ic_paper_plus_primary_24x24.png",
                },
                new ItemSpoilerModel()
                {
                    Title = "Inventory",
                    ImagePath = "ic_paper_plus_24x24.png",
                    SelectedImagePath = "ic_paper_plus_primary_24x24.png",
                },
                new ItemSpoilerModel()
                {
                    Title = "Options",
                    ImagePath = "ic_paper_plus_24x24.png",
                    SelectedImagePath = "ic_paper_plus_primary_24x24.png",
                },
                new ItemSpoilerModel()
                {
                    Title = "Comment",
                    ImagePath = "ic_paper_plus_24x24.png",
                    SelectedImagePath = "ic_paper_plus_primary_24x24.png",
                },
            };

            Products = new ()
            {
                new ProductBindableModel
                {
                    Id = 1,
                    Title = "Cheese Burger",
                    Items = Items,
                },
                new ProductBindableModel
                {
                    Id = 1,
                    Title = "Fries",
                    Items = Items,
                },
                new ProductBindableModel
                {
                    Id = 1,
                    Title = "Drink",
                    Items = Items,
                },
            };
        }

        #region -- Public properties --

        public ObservableCollection<ItemSpoilerModel> Items { get; set; }

        public ObservableCollection<ProductBindableModel> Products { get; set; }

        public ProductBindableModel SelectedMenuItem
        {
            get;
            set;
        }

        public ItemSpoilerModel SelectedSubmenuItem
        {
            get;
            set;
        }

        #endregion
    }
}
