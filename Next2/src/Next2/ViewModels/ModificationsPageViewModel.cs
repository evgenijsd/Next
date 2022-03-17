using Next2.Models;
using Next2.Services.Menu;
using Next2.Services.Order;
using Prism.Navigation;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels
{
    public class ModificationsPageViewModel : BaseViewModel
    {
        private readonly IOrderService _orderService;

        private bool _isOrderedByDescending;

        private int _indexOfSeat;

        private int _indexOfSelectedSet;

        private SetBindableModel _selectedSet;

        public ModificationsPageViewModel(
            INavigationService navigationService,
            IOrderService orderService)
            : base(navigationService)
        {
            _orderService = orderService;

            CurrentOrder = new(_orderService.CurrentOrder);

            var seat = CurrentOrder.Seats.FirstOrDefault(row => row.SelectedItem != null);

            _indexOfSeat = CurrentOrder.Seats.IndexOf(seat);
            _selectedSet = CurrentOrder.Seats[_indexOfSeat].SelectedItem;
            _indexOfSelectedSet = seat.Sets.IndexOf(_selectedSet);

            InitSubmenuItems();

            InitProductsSet();

            InitPortionsSet();

            SelectedProduct = new() { SelectedItem = new() { Title = "Proportions" } };
        }

        #region -- Public properties --

        public FullOrderBindableModel CurrentOrder { get; set; }

        public ObservableCollection<ItemSpoilerModel> SubmenuItems { get; set; }

        public ObservableCollection<SpoilerBindableModel> ProductsSet { get; set; }

        public SpoilerBindableModel SelectedProduct { get; set; }

        public ObservableCollection<PortionModel> PortionsSet { get; set; }

        public PortionModel SelectedPortion { get; set; }

        public ObservableCollection<OptionModel> OptionsProduct { get; set; }

        public OptionModel SelectedOption { get; set; }

        public bool IsMenuOpen { get; set; }

        private ICommand _tapSubmenuCommand;
        public ICommand TapSubmenuCommand => _tapSubmenuCommand ??= new AsyncCommand<SpoilerBindableModel>(OnTapSubmenuCommandAsync);

        private ICommand _tapOpenProportionsCommand;
        public ICommand TapOpenProportionsCommand => _tapOpenProportionsCommand ??= new AsyncCommand(OnTapOpenProportionsCommandAsync);

        private ICommand _changingOrderSortCommand;
        public ICommand ChangingOrderSortCommand => _changingOrderSortCommand ??= new AsyncCommand(OnChangingOrderSortCommandAsync);

        private ICommand _openMenuCommand;
        public ICommand OpenMenuCommand => _openMenuCommand ??= new AsyncCommand(OnOpenMenuCommandAsync);

        private ICommand _closeMenuCommand;
        public ICommand CloseMenuCommand => _closeMenuCommand ??= new AsyncCommand(OnCloseMenuCommandAsync);

        private ICommand _saveCommand;
        public ICommand SaveCommand => _saveCommand ??= new AsyncCommand(OnSaveCommandAsync);

        #endregion

        #region --Overrides--

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            switch (args.PropertyName)
            {
                case nameof(SelectedPortion):
                    if (SelectedPortion is not null)
                    {
                        CurrentOrder.Seats[_indexOfSeat].Sets[_indexOfSelectedSet].Portion = SelectedPortion;
                    }

                    break;
                case nameof(SelectedOption):
                    if (SelectedOption is not null)
                    {
                        var products = CurrentOrder.Seats[_indexOfSeat].Sets[_indexOfSelectedSet].Products;
                        var product = products.FirstOrDefault(row => row.Id == SelectedProduct.Id);

                        products[products.IndexOf(product)].SelectedOption = SelectedOption;
                    }

                    break;
                case nameof(SelectedProduct):
                    switch (SelectedProduct?.SelectedItem?.Title)
                    {
                        case "Proportions":
                            SelectedPortion = CurrentOrder.Seats[_indexOfSeat].Sets[_indexOfSelectedSet].Portion;
                            break;
                        case "Options":
                            LoadOptionsProduct();
                            break;
                    }

                    break;
            }
        }

        #endregion

        #region --Private methods--

        private void InitSubmenuItems()
        {
            SubmenuItems = new()
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
        }

        private void InitProductsSet()
        {
            var products = CurrentOrder.Seats[_indexOfSeat].Sets[_indexOfSelectedSet].Products;
            if (products is not null)
            {
                ProductsSet = new(products.Select(row => new SpoilerBindableModel
                {
                    Id = row.Id,
                    Title = row.Title,
                    Items = SubmenuItems,
                    TapCommand = TapSubmenuCommand,
                }));
            }
        }

        private void InitPortionsSet()
        {
            var portions = CurrentOrder.Seats[_indexOfSeat].Sets[_indexOfSelectedSet].Portions;

            if (portions is not null)
            {
                PortionsSet = new(portions);

                SelectedPortion = PortionsSet.FirstOrDefault(row => row.Id == _selectedSet.Portion.Id);
                CurrentOrder.Seats[_indexOfSeat].Sets[_indexOfSelectedSet].Portion = SelectedPortion;
            }
        }

        private void LoadOptionsProduct()
        {
            if (SelectedProduct != null)
            {
                var products = CurrentOrder.Seats[_indexOfSeat].Sets[_indexOfSelectedSet].Products;
                var product = products.FirstOrDefault(row => row.Id == SelectedProduct.Id);
                var idxProduct = products.IndexOf(product);

                var options = products[idxProduct].Options;
                if (options is not null)
                {
                    OptionsProduct = new(options);
                    SelectedOption = products[idxProduct].SelectedOption;
                }
            }
        }

        private async Task OnTapSubmenuCommandAsync(SpoilerBindableModel item)
        {
            if (item.SelectedItem is not null)
            {
                SelectedProduct = item;

                var idx = ProductsSet.IndexOf(item);

                for (int i = 0; i < ProductsSet.Count; i++)
                {
                    if (i != idx)
                    {
                        ProductsSet[i].SelectedItem = null;
                    }
                }

                if (!App.IsTablet)
                {
                    await OnCloseMenuCommandAsync();
                }
            }
        }

        private async Task OnTapOpenProportionsCommandAsync()
        {
            SelectedProduct = new() { SelectedItem = new() { Title = "Proportions" } };

            for (int i = 0; i < ProductsSet.Count; i++)
            {
                ProductsSet[i].SelectedItem = null;
            }

            if (!App.IsTablet)
            {
                await OnCloseMenuCommandAsync();
            }
        }

        private async Task OnChangingOrderSortCommandAsync()
        {
            _isOrderedByDescending = !_isOrderedByDescending;

            if (_isOrderedByDescending)
            {
                PortionsSet = new (PortionsSet.OrderBy(row => row.Title));
            }
            else
            {
                PortionsSet = new (PortionsSet.OrderByDescending(row => row.Title));
            }

            SelectedPortion = CurrentOrder.Seats[_indexOfSeat].Sets[_indexOfSelectedSet].Portion;
        }

        private async Task OnOpenMenuCommandAsync()
        {
            IsMenuOpen = true;
        }

        private async Task OnCloseMenuCommandAsync()
        {
            IsMenuOpen = false;
        }

        private async Task OnSaveCommandAsync()
        {
            _orderService.CurrentOrder = CurrentOrder;

            await _navigationService.GoBackAsync();
        }

        #endregion
    }
}
