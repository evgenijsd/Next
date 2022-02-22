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
                    TapCommand = TapCommand,
                },
                new ProductBindableModel
                {
                    Id = 1,
                    Title = "Fries",
                    Items = Items,
                    TapCommand = TapCommand,
                },
                new ProductBindableModel
                {
                    Id = 1,
                    Title = "Drink",
                    Items = Items,
                    TapCommand = TapCommand,
                },
            };
        }

        #region -- Public properties --

        public ObservableCollection<ItemSpoilerModel> Items { get; set; }

        public ObservableCollection<ProductBindableModel> Products { get; set; }

        public ProductBindableModel SelectedMenuItem { get; set; } = new ()
        {
            Title = "Proportions",
        };

        private ICommand _tapCommand;
        public ICommand TapCommand => _tapCommand ??= new AsyncCommand<ProductBindableModel>(OnTapCommandAsync);

        private ICommand _tapProportionsCommand;
        public ICommand TapProportionsCommand => _tapProportionsCommand ??= new AsyncCommand(OnTapProportionsCommandAsync);

        #endregion

        #region --Private methods--

        private async Task OnTapCommandAsync(ProductBindableModel item)
        {
            if (item.SelectedItem is not null)
            {
                SelectedMenuItem = null;

                var idx = Products.IndexOf(item);

                for (int i = 0; i < Products.Count; i++)
                {
                    if (i != idx)
                    {
                        Products[i].SelectedItem = null;
                    }
                }
            }
        }

        private async Task OnTapProportionsCommandAsync()
        {
            SelectedMenuItem = new ()
            {
                Title = "Proportions",
            };

            for (int i = 0; i < Products.Count; i++)
            {
                Products[i].SelectedItem = null;
            }
        }

        #endregion
    }
}
