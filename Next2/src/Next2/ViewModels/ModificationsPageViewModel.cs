using AutoMapper;
using Next2.Enums;
using Next2.Extensions;
using Next2.Helpers;
using Next2.Models.API.DTO;
using Next2.Models.Bindables;
using Next2.Services.Authentication;
using Next2.Services.Bonuses;
using Next2.Services.Menu;
using Next2.Services.Notifications;
using Next2.Services.Order;
using Next2.Views;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Next2.ViewModels
{
    public class ModificationsPageViewModel : BaseViewModel
    {
        private readonly IOrderService _orderService;
        private readonly IBonusesService _bonusesService;
        private readonly IMenuService _menuService;
        private readonly IMapper _mapper;

        private readonly ICommand _tapSubmenuCommand;

        private bool _isOrderedByAscendingReplacementProducts = true;
        private bool _isOrderedByDescendingInventory = true;

        private DishBindableModel _currentDish;
        private SeatBindableModel _currentSeat;
        private ProductBindableModel _currentProduct;

        private FullOrderBindableModel _tempCurrentOrder = new();
        private SeatBindableModel _tempCurrentSeat = new();

        private IEnumerable<IngredientModelDTO>? _allIngredients;
        private IEnumerable<IngredientsCategoryModelDTO>? _allIngredientCategories;

        public ModificationsPageViewModel(
            INavigationService navigationService,
            IAuthenticationService authenticationService,
            INotificationsService notificationsService,
            IOrderService orderService,
            IMenuService menuService,
            IMapper mapper,
            IBonusesService bonusService)
            : base(navigationService, authenticationService, notificationsService)
        {
            _orderService = orderService;
            _menuService = menuService;
            _bonusesService = bonusService;
            _mapper = mapper;

            _tapSubmenuCommand ??= new AsyncCommand<SpoilerBindableModel>(OnTapSubmenuCommandAsync);

            CurrentOrder = _mapper.Map<FullOrderBindableModel>(_orderService.CurrentOrder);

            _currentSeat = CurrentOrder.Seats.FirstOrDefault(row => row.SelectedItem != null);

            var dishId = _currentSeat.SelectedItem.Id;

            _currentDish = _currentSeat.SelectedDishes.FirstOrDefault(row => row.Id == dishId);

            InitializeSidebarProducts();

            SelectedSidebarProduct = new() { SelectedItem = new() { State = ESubmenuItemsModifactions.Proportions } };
        }

        #region -- Public properties --

        public FullOrderBindableModel CurrentOrder { get; set; }

        public SpoilerBindableModel SelectedSidebarProduct { get; set; }

        public SpoilerBindableModel SelectedProductDish { get; set; } = new();

        public ObservableCollection<SpoilerBindableModel> SidebarProducts { get; set; } = new();

        public ProportionBindableModel? SelectedProportion { get; set; }

        public ObservableCollection<ProportionBindableModel> PortionsDish { get; set; } = new();

        public OptionBindableModel? SelectedOption { get; set; }

        public ObservableCollection<OptionBindableModel> OptionsProduct { get; set; } = new();

        public SimpleProductBindableModel? SelectedReplacementProduct { get; set; }

        public ObservableCollection<SimpleProductBindableModel> ReplacementProducts { get; set; } = new();

        public IngredientsCategoryModelDTO? SelectedIngredientCategory { get; set; }

        public ObservableCollection<IngredientsCategoryModelDTO> IngredientCategories { get; set; } = new();

        public ObservableCollection<IngredientBindableModel> Ingredients { get; set; } = new();

        public bool IsMenuOpen { get; set; }

        public bool IsExpandedIngredientCategories { get; set; }

        public int HeightIngredientCategories { get; set; }

        private ICommand? _tapOpenProportionsCommand;
        public ICommand TapOpenProportionsCommand => _tapOpenProportionsCommand ??= new AsyncCommand(OnTapOpenProportionsCommandAsync);

        private ICommand? _openMenuCommand;
        public ICommand OpenMenuCommand => _openMenuCommand ??= new AsyncCommand(OnOpenMenuCommandAsync);

        private ICommand? _closeMenuCommand;
        public ICommand CloseMenuCommand => _closeMenuCommand ??= new AsyncCommand(OnCloseMenuCommandAsync);

        private ICommand? _saveCommand;
        public ICommand SaveCommand => _saveCommand ??= new AsyncCommand(OnSaveCommandAsync);

        private ICommand? _changingOrderSortReplacementProductsCommand;
        public ICommand ChangingOrderSortReplacementProductsCommand => _changingOrderSortReplacementProductsCommand ??= new AsyncCommand(OnChangingOrderSortReplacementProductsCommandAsync);

        private ICommand? _changingOrderSortInventoryCommand;
        public ICommand ChangingOrderSortInventoryCommand => _changingOrderSortInventoryCommand ??= new AsyncCommand(OnChangingOrderSortInventoryCommandAsync);

        private ICommand? _expandIngredientCategoriesCommand;
        public ICommand ExpandIngredientCategoriesCommand => _expandIngredientCategoriesCommand ??= new AsyncCommand(OnExpandIngredientCategoriesCommandAsync);

        private ICommand? _changingToggleCommand;
        public ICommand ChangingToggleCommand => _changingToggleCommand ??= new Command<IngredientBindableModel>(OnChangingToggleCommand);

        #endregion

        #region -- Overrides --

        public override async Task InitializeAsync(INavigationParameters parameters)
        {
            await base.InitializeAsync(parameters);

            await InitIngredientsAsync();

            InitProportionDish();
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if (parameters.TryGetValue(Constants.Navigations.INPUT_VALUE, out string text))
            {
                var products = _currentDish.SelectedProducts;

                _currentProduct = products.FirstOrDefault(row => row.DishReplacementProductId == SelectedSidebarProduct.DishReplacementProductId);

                _currentProduct.Comment = text;

                var indexProduct = products.IndexOf(_currentProduct);

                SidebarProducts[indexProduct].Items[3].CanShowDot = !string.IsNullOrWhiteSpace(text);
                SidebarProducts[indexProduct].SelectedItem = SidebarProducts[indexProduct].Items.FirstOrDefault();
            }
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            switch (args.PropertyName)
            {
                case nameof(SelectedProportion):
                    ChangeDishProportionAsync().Await();

                    break;
                case nameof(SelectedReplacementProduct):
                    SelectReplacementProduct();

                    break;
                case nameof(SelectedOption):
                    if (SelectedOption is not null)
                    {
                        _currentProduct.SelectedOptions = SelectedOption.ToOptionModelDTO();
                    }

                    break;
                case nameof(SelectedIngredientCategory):
                    if (SelectedIngredientCategory is not null)
                    {
                        InitIngredientsByCategoryAsync(SelectedIngredientCategory.Id).Await();
                    }

                    break;
            }
        }

        #endregion

        #region -- Private helpers --

        private decimal СalculatePriceOfProportion(decimal price)
        {
            var priceRatio = _currentDish.SelectedDishProportion.PriceRatio;

            return priceRatio == 1
                ? price
                : price * (1 + priceRatio);
        }

        private Task OnChangingOrderSortReplacementProductsCommandAsync()
        {
            _isOrderedByAscendingReplacementProducts = !_isOrderedByAscendingReplacementProducts;

            InitReplacementProductsDish();

            return Task.CompletedTask;
        }

        private Task OnChangingOrderSortInventoryCommandAsync()
        {
            _isOrderedByDescendingInventory = !_isOrderedByDescendingInventory;

            if (_isOrderedByDescendingInventory)
            {
                Ingredients = new(Ingredients.OrderBy(row => row.Name));
            }
            else
            {
                Ingredients = new(Ingredients.OrderByDescending(row => row.Name));
            }

            return Task.CompletedTask;
        }

        private Task OnExpandIngredientCategoriesCommandAsync()
        {
            IsExpandedIngredientCategories = !IsExpandedIngredientCategories;

            return Task.CompletedTask;
        }

        private void OnChangingToggleCommand(IngredientBindableModel toggleIngredient)
        {
            var ingredient = _currentProduct.AddedIngredients.FirstOrDefault(row => row.Id == toggleIngredient.Id);

            if (_currentProduct is not null && SelectedIngredientCategory is not null)
            {
                if (ingredient is null)
                {
                    _currentProduct.AddedIngredients?.Add(toggleIngredient.ToSimpleIngredientModelDTO());

                    if (_currentProduct.Product.Ingredients.Any(row => row.Id == toggleIngredient.Id))
                    {
                        _currentProduct.ExcludedIngredients?.Remove(_currentProduct.ExcludedIngredients.FirstOrDefault(row => row.Id == toggleIngredient.Id));
                    }
                }
                else
                {
                    if (_currentProduct.Product.Ingredients.Any(row => row.Id == ingredient.Id))
                    {
                        _currentProduct.ExcludedIngredients?.Add(ingredient);
                    }

                    _currentProduct.AddedIngredients?.Remove(ingredient);
                }

                InitProportionDish();
            }
        }

        private void InitializeSidebarProducts()
        {
            var products = _currentDish.SelectedProducts;

            if (products is not null)
            {
                SidebarProducts = new(products.Select(row =>
                {
                    var result = new SpoilerBindableModel
                    {
                        Id = row.Product.Id,
                        DishReplacementProductId = row.DishReplacementProductId,
                        Title = row.Product.Name ?? string.Empty,
                        Items = new()
                        {
                            new SpoilerItem()
                            {
                                State = ESubmenuItemsModifactions.Replace,
                                Title = "Replace",
                                ImagePath = "ic_paper_fail_24x24.png",
                                SelectedImagePath = "ic_paper_fail_primary_24x24.png",
                            },
                            new SpoilerItem()
                            {
                                State = ESubmenuItemsModifactions.Inventory,
                                Title = "Inventory",
                                ImagePath = "ic_paper_24x24.png",
                                SelectedImagePath = "ic_paper_primary_24x24.png",
                            },
                            new SpoilerItem()
                            {
                                State = ESubmenuItemsModifactions.Options,
                                Title = "Options",
                                ImagePath = "ic_paper_plus_24x24.png",
                                SelectedImagePath = "ic_paper_plus_primary_24x24.png",
                            },
                            new SpoilerItem()
                            {
                                State = ESubmenuItemsModifactions.Comment,
                                Title = "Comment",
                                ImagePath = "ic_chat_white_24x24.png",
                                SelectedImagePath = "ic_chat_primary.png",
                                CanShowDot = !string.IsNullOrWhiteSpace(row.Comment),
                            },
                        },
                        TapCommand = _tapSubmenuCommand,
                    };

                    result.PropertyChanged += SelectedOptionPropertyChanged;

                    return result;
                }));
            }
        }

        private void SelectedOptionPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is SpoilerBindableModel spoiler
                && e.PropertyName is nameof(spoiler.SelectedItem)
                && SelectedIngredientCategory is not null
                && spoiler.SelectedItem is not null
                && spoiler.SelectedItem.State != ESubmenuItemsModifactions.Inventory)
            {
                SelectedIngredientCategory = null;
            }
        }

        private void InitReplacementProductsDish()
        {
            var replacementProduct = _currentDish.ReplacementProducts?.FirstOrDefault(row => row.Id == SelectedSidebarProduct.DishReplacementProductId);

            if (replacementProduct is not null && replacementProduct.Products is not null)
            {
                var products = _isOrderedByAscendingReplacementProducts
                    ? replacementProduct.Products.OrderBy(row => row.Name)
                    : replacementProduct.Products.OrderByDescending(row => row.Name);

                ReplacementProducts = new(products.Select(row => row.ToSimpleProductBindableModel()));

                foreach (var product in ReplacementProducts)
                {
                    var defaultProductPrice = products.FirstOrDefault(x => x.Id == product.Id).DefaultPrice;

                    product.DefaultPrice = СalculatePriceOfProportion(defaultProductPrice);
                }

                SelectedReplacementProduct = ReplacementProducts.FirstOrDefault(x => x.Id == SelectedSidebarProduct.Id);
            }
        }

        private async Task InitIngredientCategoriesAsync()
        {
            if (_allIngredientCategories is null)
            {
                if (IsInternetConnected)
                {
                    var ingredientCategories = await _menuService.GetIngredientCategoriesAsync();

                    if (ingredientCategories.IsSuccess)
                    {
                        _allIngredientCategories = ingredientCategories.Result;
                    }
                    else
                    {
                        await ResponseToUnsuccessfulRequestAsync(ingredientCategories.Exception?.Message);
                    }
                }
                else
                {
                    await _notificationsService.ShowNoInternetConnectionDialogAsync();
                }
            }

            IngredientCategories = new(_allIngredientCategories);

            SelectedIngredientCategory = IngredientCategories.FirstOrDefault();

            IsExpandedIngredientCategories = App.IsTablet;
        }

        private Task InitIngredientsByCategoryAsync(Guid categoryId)
        {
            if (_allIngredients is not null)
            {
                Ingredients = new(_allIngredients.Where(row => row.IngredientsCategoryId == categoryId).Select(row => row.ToIngredientBindableModel()));

                var selectedDishProportion = _currentDish.SelectedDishProportion;
                var isSelectedDishProportion = selectedDishProportion is not null;

                foreach (var ingredient in Ingredients)
                {
                    ingredient.IsToggled = _currentProduct is not null && _currentProduct.AddedIngredients.Any(item => item.Id == ingredient.Id);

                    if (isSelectedDishProportion)
                    {
                        ingredient.Price = selectedDishProportion.PriceRatio == 1
                            ? ingredient.Price
                            : ingredient.Price * (1 + selectedDishProportion.PriceRatio);
                    }

                    ingredient.ChangingToggle = ChangingToggleCommand;
                }
            }

            return Task.CompletedTask;
        }

        private async Task InitIngredientsAsync()
        {
            if (_allIngredients is null)
            {
                if (IsInternetConnected)
                {
                    var ingredientsResult = await _menuService.GetIngredientsAsync();

                    if (ingredientsResult.IsSuccess)
                    {
                        _allIngredients = ingredientsResult.Result;
                    }
                    else
                    {
                        await ResponseToUnsuccessfulRequestAsync(ingredientsResult.Exception?.Message);
                    }
                }
                else
                {
                    await _notificationsService.ShowNoInternetConnectionDialogAsync();
                }
            }
        }

        private void InitProportionDish()
        {
            var portions = _currentDish.DishProportions;
            var selectedDishProportionId = _currentDish.SelectedDishProportion?.Id;

            if (portions is not null && _allIngredients is not null)
            {
                var bindableProportions = new List<ProportionBindableModel>(portions.Select(row => row.ToProportionBindableModel()));

                foreach(var proportion in bindableProportions)
                {
                    proportion.Price = _orderService.CalculateDishPriceBaseOnProportion(_currentDish, proportion.PriceRatio, _allIngredients);
                }

                PortionsDish = new(bindableProportions.OrderBy(row => row.Price));

                SelectedProportion = PortionsDish.FirstOrDefault(row => row.Id == selectedDishProportionId);
            }
        }

        private void LoadOptionsProduct()
        {
            if (SelectedSidebarProduct != null)
            {
                var options = _currentProduct.Product.Options.Select(row => row.ToOptionBindableModel());

                if (options is not null)
                {
                    OptionsProduct = new(options);

                    SelectedOption = _currentProduct.SelectedOptions is null
                        ? options.FirstOrDefault()
                        : _currentProduct.SelectedOptions.ToOptionBindableModel();
                }
            }
        }

        private async Task OnTapSubmenuCommandAsync(SpoilerBindableModel? item)
        {
            if (IsInternetConnected)
            {
                if (item?.SelectedItem is not null)
                {
                    SelectedSidebarProduct = item;

                    _currentProduct = _currentDish.SelectedProducts.FirstOrDefault(row => row.DishReplacementProductId == SelectedSidebarProduct.DishReplacementProductId);

                    _isOrderedByAscendingReplacementProducts = true;
                    _isOrderedByDescendingInventory = true;

                    var index = SidebarProducts.IndexOf(item);

                    for (int i = 0; i < SidebarProducts.Count; i++)
                    {
                        if (i != index)
                        {
                            SidebarProducts[i].SelectedItem = null;
                        }
                    }

                    switch (item.SelectedItem.State)
                    {
                        case ESubmenuItemsModifactions.Options:
                            LoadOptionsProduct();
                            break;
                        case ESubmenuItemsModifactions.Replace:
                            InitReplacementProductsDish();
                            break;
                        case ESubmenuItemsModifactions.Inventory:
                            Ingredients = new();
                            SelectedIngredientCategory = null;

                            InitIngredientCategoriesAsync().Await();
                            break;
                        case ESubmenuItemsModifactions.Comment:

                            var navigationParameters = new NavigationParameters()
                            {
                                { Constants.Navigations.INPUT_VALUE, _currentProduct.Comment },
                                { Constants.Navigations.PLACEHOLDER, LocalizationResourceManager.Current["CommentForOrder"] },
                            };

                            await _navigationService.NavigateAsync(nameof(InputTextPage), navigationParameters);
                            break;
                    }

                    if (!App.IsTablet)
                    {
                        await OnCloseMenuCommandAsync();
                    }
                }
            }
            else
            {
                await _notificationsService.ShowNoInternetConnectionDialogAsync();
            }
        }

        private async Task OnTapOpenProportionsCommandAsync()
        {
            SelectedSidebarProduct = new() { SelectedItem = new() { State = ESubmenuItemsModifactions.Proportions } };

            for (int i = 0; i < SidebarProducts.Count; i++)
            {
                SidebarProducts[i].SelectedItem = null;
            }

            SelectedProportion = PortionsDish.FirstOrDefault(row => row.Id == _currentDish.SelectedDishProportion?.Id);

            if (!App.IsTablet)
            {
                await OnCloseMenuCommandAsync();
            }
        }

        private Task OnOpenMenuCommandAsync()
        {
            IsMenuOpen = true;

            return Task.CompletedTask;
        }

        private Task OnCloseMenuCommandAsync()
        {
            IsMenuOpen = false;

            return Task.CompletedTask;
        }

        private async Task OnSaveCommandAsync()
        {
            if (IsInternetConnected)
            {
                _tempCurrentOrder = _mapper.Map<FullOrderBindableModel>(_orderService.CurrentOrder);
                _tempCurrentSeat = _mapper.Map<SeatBindableModel>(_orderService.CurrentSeat);

                _currentSeat.SelectedItem = _currentDish;
                _orderService.CurrentOrder = CurrentOrder;

                var seatNumber = _orderService.CurrentSeat?.SeatNumber;

                _orderService.CurrentSeat = CurrentOrder.Seats.FirstOrDefault(row => row.SeatNumber == seatNumber);
                _orderService.UpdateTotalSum(_orderService.CurrentOrder);

                var parameters = new NavigationParameters();

                if (App.IsTablet)
                {
                    parameters.Add(Constants.Navigations.REFRESH_ORDER, true);
                }
                else
                {
                    parameters.Add(Constants.Navigations.DISH_MODIFIED, true);
                }

                var resultOfUpdatingOrder = await _orderService.UpdateCurrentOrderAsync();

                if (!resultOfUpdatingOrder.IsSuccess)
                {
                    _orderService.CurrentOrder = _tempCurrentOrder;
                    _orderService.CurrentSeat = _tempCurrentSeat;

                    var selectedSeat = _orderService.CurrentOrder.Seats.FirstOrDefault(row => row.Checked == true);

                    var dishesInSelectedSeat = selectedSeat.SelectedDishes;

                    var selectedDishId = selectedSeat.SelectedItem?.DishId;
                    var selectedDishTotalPrice = selectedSeat.SelectedItem?.TotalPrice;
                    var selectedDishProportionId = selectedSeat.SelectedItem?.SelectedDishProportion?.Id;

                    var selectedDishInSelectedSeat = dishesInSelectedSeat.FirstOrDefault(row =>
                        row.DishId == selectedDishId
                        && row.TotalPrice == selectedDishTotalPrice
                        && row.SelectedDishProportion?.Id == selectedDishProportionId);

                    _orderService.CurrentOrder.Seats.FirstOrDefault(row => row.SeatNumber == selectedSeat.SeatNumber).SelectedItem = selectedDishInSelectedSeat;

                    await ResponseToUnsuccessfulRequestAsync(resultOfUpdatingOrder.Exception?.Message);
                }
                else
                {
                    await _navigationService.GoBackAsync(parameters);
                }
            }
            else
            {
                await _notificationsService.ShowNoInternetConnectionDialogAsync();
            }
        }

        private void SelectReplacementProduct()
        {
            if (SelectedReplacementProduct is not null)
            {
                int selectedProductIndex = 0;

                foreach (var product in _currentDish.SelectedProducts)
                {
                    if (product.DishReplacementProductId == SelectedSidebarProduct.DishReplacementProductId)
                    {
                        break;
                    }

                    selectedProductIndex++;
                }

                var selectedProductCurrent = _currentDish.SelectedProducts[selectedProductIndex];

                if (selectedProductCurrent.Id != SelectedReplacementProduct.Id)
                {
                    var newSelectedProduct = _currentDish.ReplacementProducts
                        ?.FirstOrDefault(row => row.Id == SelectedSidebarProduct.DishReplacementProductId)
                        ?.Products.FirstOrDefault(product => product.Id == SelectedReplacementProduct?.Id)?.Clone();

                    selectedProductCurrent = SelectedReplacementProduct.ToProductBindableModel();

                    selectedProductCurrent.DishReplacementProductId = SelectedSidebarProduct.DishReplacementProductId;
                    selectedProductCurrent.Product = newSelectedProduct;

                    SidebarProducts[SidebarProducts.IndexOf(SelectedSidebarProduct)].Title = SelectedReplacementProduct.Name ?? string.Empty;
                    SelectedSidebarProduct.Id = SelectedReplacementProduct.Id;

                    foreach (var addedIngredient in selectedProductCurrent.AddedIngredients ?? new())
                    {
                        var price = _allIngredients.FirstOrDefault(row => row.Id == addedIngredient.Id).Price;
                        addedIngredient.Price = СalculatePriceOfProportion(price);
                    }

                    foreach (var excludedIngredient in selectedProductCurrent.ExcludedIngredients ?? new())
                    {
                        var price = _allIngredients.FirstOrDefault(row => row.Id == excludedIngredient.Id).Price;
                        excludedIngredient.Price = СalculatePriceOfProportion(price);
                    }

                    _currentDish.SelectedProducts[selectedProductIndex] = selectedProductCurrent;
                }
            }
        }

        private async Task ChangeDishProportionAsync()
        {
            if (SelectedProportion is not null && _allIngredients is not null && SelectedProportion.PriceRatio != _currentDish.SelectedDishProportion?.PriceRatio)
            {
                var resultOfChangingProportionDish = await _orderService.ChangeDishProportionAsync(SelectedProportion, _currentDish, _allIngredients);

                if (resultOfChangingProportionDish.IsSuccess)
                {
                    _currentDish = resultOfChangingProportionDish.Result;
                }
                else
                {
                    await _notificationsService.ShowSomethingWentWrongDialogAsync();
                }
            }
        }

        #endregion
    }
}
