using Next2.Models;
using Next2.Services.Menu;
using Next2.Services.Order;
using Prism.Navigation;
using System;
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
        private readonly IMenuService _menuService;

        private readonly IOrderService _orderService;

        private bool _order;

        private int _idxSeat;

        private int _idxSet;

        private SetBindableModel _set;

        public ModificationsPageViewModel(
            INavigationService navigationService,
            IMenuService menuService,
            IOrderService orderService)
            : base(navigationService)
        {
            _menuService = menuService;
            _orderService = orderService;

            var seat = _orderService.CurrentOrder.Seats.FirstOrDefault(row => row.SelectedItem != null);

            _idxSeat = _orderService.CurrentOrder.Seats.IndexOf(seat);
            _set = _orderService.CurrentOrder.Seats[_idxSeat].SelectedItem;
            _idxSet = seat.Sets.IndexOf(_set);

            InitSubmenuItems();

            InitProductsSetAsync();

            InitPortionsSetAsync();
        }

        #region -- Public properties --

        public ObservableCollection<ItemSpoilerModel> SubmenuItems { get; set; }

        public ObservableCollection<ProductBindableModel> SetProducts { get; set; }

        public ObservableCollection<PortionModel> SetPortions { get; set; }

        public ProductBindableModel SelectedProduct { get; set; }

        public PortionModel CurrentSelectedPortion { get; set; }

        public PortionModel SelectedPortion { get; set; }

        public object SelectedMenuItem { get; set; } = "Proportions";

        public bool IsShowMenu { get; set; }

        private ICommand _tapSubmenuCommand;
        public ICommand TapSubmenuCommand => _tapSubmenuCommand ??= new AsyncCommand<ProductBindableModel>(OnTapSubmenuCommandAsync);

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

            if (args.PropertyName == nameof(SelectedMenuItem))
            {
                switch (SelectedMenuItem)
                {
                    case "Proportions":
                        SelectedPortion = CurrentSelectedPortion;
                        break;
                }
            }
            else if (args.PropertyName == nameof(SelectedPortion))
            {
                if (SelectedPortion is not null)
                {
                    CurrentSelectedPortion = SelectedPortion;
                }
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

        private async Task InitProductsSetAsync()
        {
            var products = await _menuService.GetProductsSetAsync(_set.Id);

            if (products.IsSuccess)
            {
                SetProducts = new(products.Result.Select(row => new ProductBindableModel
                {
                    Id = row.Id,
                    Title = row.Title,
                    Items = SubmenuItems,
                    TapCommand = TapSubmenuCommand,
                }));
            }
        }

        private async Task InitPortionsSetAsync()
        {
            var portions = await _menuService.GetPortionsSetAsync(_set.Id);

            if (portions.IsSuccess)
            {
                SetPortions = new(portions.Result);

                SelectedPortion = SetPortions.FirstOrDefault(row => row.Id == _set.Portion.Id);
                CurrentSelectedPortion = SelectedPortion;
            }
        }

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

                if (!App.IsTablet)
                {
                    await OnCloseMenuCommandAsync();
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

            if (!App.IsTablet)
            {
                await OnCloseMenuCommandAsync();
            }
        }

        private async Task OnChangingOrderSortCommandAsync()
        {
            _order = !_order;

            if (_order)
            {
                SetPortions = new (SetPortions.OrderBy(row => row.Title));
            }
            else
            {
                SetPortions = new (SetPortions.OrderByDescending(row => row.Title));
            }

            SelectedPortion = SetPortions[SetPortions.IndexOf(CurrentSelectedPortion)];
        }

        private async Task OnOpenMenuCommandAsync()
        {
            IsShowMenu = true;
        }

        private async Task OnCloseMenuCommandAsync()
        {
            IsShowMenu = false;
        }

        private async Task OnSaveCommandAsync()
        {
            _orderService.CurrentOrder.Seats[_idxSeat].Sets[_idxSet].Portion = SelectedPortion;

            await _navigationService.GoBackAsync();
        }

        #endregion
    }
}
