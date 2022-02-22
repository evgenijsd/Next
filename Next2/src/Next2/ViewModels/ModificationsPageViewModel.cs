using Next2.Models;
using Prism.Navigation;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels
{
    public class ModificationsPageViewModel : BaseViewModel
    {
        public ModificationsPageViewModel(
            INavigationService navigationService)
            : base(navigationService)
        {
            SubmenuItems = new ()
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

            SetProducts = new ()
            {
                new ProductBindableModel
                {
                    Id = 1,
                    Title = "Cheese Burger",
                    Items = SubmenuItems,
                    TapCommand = TapSubmenuCommand,
                },
                new ProductBindableModel
                {
                    Id = 2,
                    Title = "Fries",
                    Items = SubmenuItems,
                    TapCommand = TapSubmenuCommand,
                },
                new ProductBindableModel
                {
                    Id = 3,
                    Title = "Drink",
                    Items = SubmenuItems,
                    TapCommand = TapSubmenuCommand,
                },
            };
        }

        #region -- Public properties --

        public ObservableCollection<ItemSpoilerModel> SubmenuItems { get; set; }

        public ObservableCollection<ProductBindableModel> SetProducts { get; set; }

        public ProductBindableModel SelectedProduct { get; set; }

        public object SelectedMenuItem { get; set; } = "Proportions";

        private ICommand _tapSubmenuCommand;
        public ICommand TapSubmenuCommand => _tapSubmenuCommand ??= new AsyncCommand<ProductBindableModel>(OnTapSubmenuCommandAsync);

        private ICommand _tapOpenProportionsCommand;
        public ICommand TapOpenProportionsCommand => _tapOpenProportionsCommand ??= new AsyncCommand(OnTapOpenProportionsCommandAsync);

        #endregion

        #region --Private methods--

        private async Task OnTapSubmenuCommandAsync(ProductBindableModel item)
        {
            if (item.SelectedItem is not null)
            {
                SelectedProduct = item;
                SelectedMenuItem = item.SelectedItem.Title;

                var idx = SetProducts.IndexOf(item);

                for (int i = 0; i < SetProducts.Count; i++)
                {
                    if (i != idx)
                    {
                        SetProducts[i].SelectedItem = null;
                    }
                }
            }
        }

        private async Task OnTapOpenProportionsCommandAsync()
        {
            SelectedProduct = null;
            SelectedMenuItem = "Proportions";

            for (int i = 0; i < SetProducts.Count; i++)
            {
                SetProducts[i].SelectedItem = null;
            }
        }

        #endregion
    }
}
