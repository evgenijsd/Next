using AutoMapper;
using Next2.Enums;
using Next2.Helpers;
using Next2.Models.API;
using Next2.Models.API.DTO;
using Next2.Models.Bindables;
using Next2.Resources.Strings;
using Next2.Services.Bonuses;
using Next2.Services.Menu;
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
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Next2.ViewModels
{
    public class ModificationsPageViewModel : BaseViewModel
    {
        private readonly IOrderService _orderService;
        private readonly IBonusesService _bonusService;
        private readonly IMenuService _menuService;
        private readonly IMapper _mapper;

        private int _indexOfSeat;
        private int _indexOfSelectedDish;

        private bool _isOrderedByDescendingReplacementProducts = true;
        private bool _isOrderedByDescendingInventory = true;

        private DishBindableModel _currentDish;

        private IEnumerable<IngredientModelDTO>? _allIngredients;
        private IEnumerable<IngredientsCategoryModelDTO> _allIngredientCategories;

        public ModificationsPageViewModel(
            INavigationService navigationService,
            IOrderService orderService,
            IMenuService menuService,
            IMapper mapper,
            IBonusesService bonusService)
            : base(navigationService)
        {
            _orderService = orderService;
            _menuService = menuService;
            _bonusService = bonusService;
            _mapper = mapper;

            CurrentOrder = _mapper.Map<FullOrderBindableModel>(_orderService.CurrentOrder);

            var seat = _orderService.CurrentOrder.Seats.FirstOrDefault(row => row.SelectedItem != null);
            _indexOfSeat = _orderService.CurrentOrder.Seats.IndexOf(seat);
            _indexOfSelectedDish = seat.SelectedDishes.IndexOf(seat.SelectedItem ?? new());

            _currentDish = CurrentOrder.Seats[_indexOfSeat].SelectedItem ?? new();

            InitProductsDish();
            SelectedProduct = new() { SelectedItem = new() { State = ESubmenuItemsModifactions.Proportions } };
        }

        #region -- Public properties --

        public FullOrderBindableModel CurrentOrder { get; set; }

        public SpoilerBindableModel SelectedProduct { get; set; }

        public SpoilerBindableModel SelectedProductDish { get; set; }

        public ObservableCollection<SpoilerBindableModel> ProductsDish { get; set; }

        public ProportionModel? SelectedProportion { get; set; }

        public ObservableCollection<ProportionModel> PortionsDish { get; set; }

        public OptionModelDTO? SelectedOption { get; set; }

        public ObservableCollection<OptionModelDTO> OptionsProduct { get; set; }

        public SimpleProductModelDTO? SelectedReplacementProduct { get; set; }

        public ObservableCollection<SimpleProductModelDTO> ReplacementProducts { get; set; }

        public IngredientsCategoryModelDTO? SelectedIngredientCategory { get; set; }

        public ObservableCollection<IngredientsCategoryModelDTO> IngredientCategories { get; set; }

        public ObservableCollection<IngredientBindableModel> Ingredients { get; set; }

        public bool IsMenuOpen { get; set; }

        public bool IsExpandedIngredientCategories { get; set; }

        public int HeightIngredientCategories { get; set; }

        private ICommand _tapSubmenuCommand;
        public ICommand TapSubmenuCommand => _tapSubmenuCommand ??= new AsyncCommand<SpoilerBindableModel>(OnTapSubmenuCommandAsync);

        private ICommand _tapOpenProportionsCommand;
        public ICommand TapOpenProportionsCommand => _tapOpenProportionsCommand ??= new AsyncCommand(OnTapOpenProportionsCommandAsync);

        private ICommand _openMenuCommand;
        public ICommand OpenMenuCommand => _openMenuCommand ??= new AsyncCommand(OnOpenMenuCommandAsync);

        private ICommand _closeMenuCommand;
        public ICommand CloseMenuCommand => _closeMenuCommand ??= new AsyncCommand(OnCloseMenuCommandAsync);

        private ICommand _saveCommand;
        public ICommand SaveCommand => _saveCommand ??= new AsyncCommand(OnSaveCommandAsync);

        private ICommand _changingOrderSortReplacementProductsCommand;
        public ICommand ChangingOrderSortReplacementProductsCommand => _changingOrderSortReplacementProductsCommand ??= new AsyncCommand(OnChangingOrderSortReplacementProductsCommandAsync);

        private ICommand _changingOrderSortInventoryCommand;
        public ICommand ChangingOrderSortInventoryCommand => _changingOrderSortInventoryCommand ??= new AsyncCommand(OnChangingOrderSortInventoryCommandAsync);

        private ICommand _expandIngredientCategoriesCommand;
        public ICommand ExpandIngredientCategoriesCommand => _expandIngredientCategoriesCommand ??= new AsyncCommand(OnExpandIngredientCategoriesCommandAsync);

        private ICommand _changingToggleCommand;
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
                var products = _currentDish.SelectedProducts ?? new();
                var product = products.FirstOrDefault(row => row.Id == SelectedProduct.Id);
                var indexProduct = products.IndexOf(product);

                ProductsDish[indexProduct].Items[3].CanShowDot = !string.IsNullOrWhiteSpace(text);

                products[indexProduct].Comment = text;
                ProductsDish[indexProduct].SelectedItem = ProductsDish[indexProduct].Items.FirstOrDefault();
            }
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            switch (args.PropertyName)
            {
                case nameof(SelectedProportion):
                    if (SelectedProportion is not null && SelectedProportion.PriceRatio != _currentDish.SelectedDishProportion.PriceRatio)
                    {
                        _currentDish.SelectedDishProportion = new DishProportionModelDTO()
                        {
                            Id = SelectedProportion.Id,
                            PriceRatio = SelectedProportion.PriceRatio,
                            Proportion = new ProportionModelDTO()
                            {
                                Id = SelectedProportion.ProportionId,
                                Name = SelectedProportion.ProportionName,
                            },
                        };

                        foreach (var product in _currentDish.SelectedProducts ?? new())
                        {
                            product.Price = SelectedProportion.PriceRatio == 1
                                ? product.Product.DefaultPrice
                                : product.Product.DefaultPrice * (1 + SelectedProportion.PriceRatio);

                            if (product.AddedIngredients is not null)
                            {
                                foreach (var addedIngredient in product.AddedIngredients)
                                {
                                    addedIngredient.Price = SelectedProportion.PriceRatio == 1
                                        ? _allIngredients.FirstOrDefault(row => row.Id == addedIngredient.Id).Price
                                        : _allIngredients.FirstOrDefault(row => row.Id == addedIngredient.Id).Price * (1 + SelectedProportion.PriceRatio);
                                }
                            }

                            if (product.ExcludedIngredients is not null)
                            {
                                foreach (var excludedIngredient in product.ExcludedIngredients)
                                {
                                    excludedIngredient.Price = SelectedProportion.PriceRatio == 1
                                        ? _allIngredients.FirstOrDefault(row => row.Id == excludedIngredient.Id).Price
                                        : _allIngredients.FirstOrDefault(row => row.Id == excludedIngredient.Id).Price * (1 + SelectedProportion.PriceRatio);
                                }
                            }
                        }
                    }

                    break;

                case nameof(SelectedReplacementProduct):
                    if (SelectedReplacementProduct is not null)
                    {
                        int selectedProductIndex = 0;

                        foreach (var product in _currentDish.SelectedProducts)
                        {
                            if (product.Id == SelectedProduct.Id)
                            {
                                break;
                            }

                            selectedProductIndex++;
                        }

                        if (_currentDish.SelectedProducts[selectedProductIndex].Id != SelectedReplacementProduct.Id)
                        {
                            var selectedProduct = _currentDish.Products.FirstOrDefault(x => x.Id == SelectedReplacementProduct.Id);

                            _currentDish.SelectedProducts[selectedProductIndex] = new ()
                            {
                                Id = SelectedReplacementProduct.Id,
                                SelectedOptions = SelectedReplacementProduct.Options.FirstOrDefault(),
                                AddedIngredients = new(SelectedReplacementProduct.Ingredients),
                                Price = SelectedReplacementProduct.DefaultPrice,
                                Product = new()
                                {
                                    Id = selectedProduct.Id,
                                    DefaultPrice = selectedProduct.DefaultPrice,
                                    ImageSource = selectedProduct.ImageSource,
                                    Ingredients = selectedProduct.Ingredients,
                                    Name = selectedProduct.Name,
                                    Options = selectedProduct.Options,
                                },
                            };

                            ProductsDish[ProductsDish.IndexOf(SelectedProduct)].Title = SelectedReplacementProduct.Name ?? string.Empty;
                            SelectedProduct.Id = SelectedReplacementProduct.Id;

                            foreach (var ingredient in _currentDish.SelectedProducts[selectedProductIndex].AddedIngredients)
                            {
                                ingredient.Price = СalculateProductPriceOfProportion(ingredient.Price);
                            }
                        }
                    }

                    break;
                case nameof(SelectedOption):
                    if (SelectedOption is not null)
                    {
                        var products = _currentDish.SelectedProducts;
                        var product = products.FirstOrDefault(row => row.Id == SelectedProduct.Id);

                        product.SelectedOptions = SelectedOption;
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

        #region --Private methods--

        private decimal СalculateProductPriceOfProportion(decimal price)
        {
            var priceRatio = _currentDish.SelectedDishProportion.PriceRatio;

            return priceRatio == 1
                ? price
                : price * (1 + priceRatio);
        }

        private Task OnChangingOrderSortReplacementProductsCommandAsync()
        {
            _isOrderedByDescendingReplacementProducts = !_isOrderedByDescendingReplacementProducts;

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

            var countItemsInRow = App.IsTablet ? 6 : 2;
            var countRow = Math.Ceiling((double)IngredientCategories.Count / countItemsInRow);

            HeightIngredientCategories = (int)(((IsExpandedIngredientCategories ? countRow : 2) * 52) - 6);

            return Task.CompletedTask;
        }

        private void OnChangingToggleCommand(IngredientBindableModel toggleIngredient)
        {
            toggleIngredient.IsToggled = !toggleIngredient.IsToggled;

            var product = _currentDish?.SelectedProducts?.FirstOrDefault();

            var ingredient = product?.AddedIngredients.FirstOrDefault(row => row.Id == toggleIngredient.Id);

            if (product is not null)
            {
                if (ingredient is null)
                {
                    product.AddedIngredients?.Add(new SimpleIngredientModelDTO()
                    {
                        Id = toggleIngredient.Id,
                        ImageSource = toggleIngredient.ImageSource,
                        Name = toggleIngredient.Name,
                        IngredientsCategory = new SimpleIngredientsCategoryModelDTO()
                        {
                            Id = SelectedIngredientCategory.Id,
                            Name = SelectedIngredientCategory.Name,
                        },
                        Price = toggleIngredient.Price,
                    });

                    if (product.Product.Ingredients.Any(row => row.Id == toggleIngredient.Id))
                    {
                        product.ExcludedIngredients?.Remove(product.ExcludedIngredients.FirstOrDefault(row => row.Id == toggleIngredient.Id));
                    }
                }
                else
                {
                    if (product.Product.Ingredients.Any(row => row.Id == ingredient.Id))
                    {
                        product.ExcludedIngredients?.Add(ingredient);
                    }

                    product?.AddedIngredients?.Remove(ingredient);
                }

                InitProportionDish();
            }
        }

        private void InitProductsDish()
        {
            var products = _currentDish.SelectedProducts;

            if (products is not null)
            {
                ProductsDish = new(products.Select(row =>
                {
                    var result = new SpoilerBindableModel
                    {
                        Id = row.Id,
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
                        TapCommand = TapSubmenuCommand,
                    };

                    return result;
                }));
            }
        }

        private void InitReplacementProductsDish()
        {
            var products = _currentDish.Products;

            if (products is not null)
            {
                if (_isOrderedByDescendingReplacementProducts)
                {
                    ReplacementProducts = _mapper.Map<ObservableCollection<SimpleProductModelDTO>>(products.OrderBy(row => row.Name));
                }
                else
                {
                    ReplacementProducts = _mapper.Map<ObservableCollection<SimpleProductModelDTO>>(products.OrderByDescending(row => row.Name));
                }

                foreach (var product in ReplacementProducts)
                {
                    product.DefaultPrice = СalculateProductPriceOfProportion(products.FirstOrDefault(x => x.Id == product.Id).DefaultPrice);
                }

                SelectedReplacementProduct = ReplacementProducts.FirstOrDefault(x => x.Id == SelectedProduct.Id);
            }
        }

        private async Task InitIngredientCategoriesAsync()
        {
            if (_allIngredientCategories is null)
            {
                var ingredientCategories = await _menuService.GetIngredientCategoriesAsync();

                if (ingredientCategories.IsSuccess)
                {
                    _allIngredientCategories = ingredientCategories.Result;
                }
            }

            IngredientCategories = new(_allIngredientCategories);

            SelectedIngredientCategory = IngredientCategories.FirstOrDefault();

            IsExpandedIngredientCategories = false;

            var countItemsInRow = App.IsTablet ? 6 : 2;
            var countRow = Math.Ceiling((double)IngredientCategories.Count / countItemsInRow);

            HeightIngredientCategories = (int)(((countRow > 2 ? 2 : countRow) * (44 + 8)) - 6);
        }

        private Task InitIngredientsByCategoryAsync(Guid categoryId)
        {
            if (_allIngredients is not null)
            {
                var product = _currentDish?.SelectedProducts?.FirstOrDefault();

                Ingredients = new(_allIngredients.Where(row => row.IngredientsCategoryId == categoryId).Select(row => new IngredientBindableModel()
                {
                    Id = row.Id,
                    CategoryId = row.IngredientsCategoryId,
                    IsToggled = product is not null ? product.AddedIngredients.Any(item => item.Id == row.Id) : false,
                    Name = row.Name,
                    Price = _currentDish?.SelectedDishProportion.PriceRatio == 1
                                ? row.Price
                                : row.Price * (1 + _currentDish.SelectedDishProportion.PriceRatio),
                    ImageSource = row.ImageSource,
                    ChangingToggle = ChangingToggleCommand,
                }));
            }

            return Task.CompletedTask;
        }

        private async Task InitIngredientsAsync()
        {
            if (_allIngredients is null)
            {
                var ingredientsResult = await _menuService.GetIngredientsAsync();

                if (ingredientsResult.IsSuccess)
                {
                    _allIngredients = ingredientsResult.Result;
                }
            }
        }

        private void InitProportionDish()
        {
            var portions = _currentDish.DishProportions;
            var selectedDishProportionId = _currentDish.SelectedDishProportion.Id;

            if (portions is not null)
            {
                PortionsDish = new(_currentDish.DishProportions.Select(row => new ProportionModel()
                {
                    Id = row.Id,
                    ProportionId = row.ProportionId,
                    PriceRatio = row.PriceRatio,
                    Price = CalculateDishPriceBaseOnProportion(_currentDish, row.PriceRatio),
                    ProportionName = row.ProportionName,
                }).OrderBy(row => row.Price));

                SelectedProportion = PortionsDish.FirstOrDefault(row => row.Id == selectedDishProportionId);
            }
        }

        private void LoadOptionsProduct()
        {
            if (SelectedProduct != null)
            {
                var products = _currentDish.SelectedProducts;
                var product = products.FirstOrDefault(row => row.Id == SelectedProduct.Id);

                var options = product.Product.Options;

                if (options is not null)
                {
                    OptionsProduct = new(options);
                    SelectedOption = product.SelectedOptions is null
                        ? options.FirstOrDefault()
                        : product.SelectedOptions;
                }
            }
        }

        private async Task OnTapSubmenuCommandAsync(SpoilerBindableModel? item)
        {
            if (item?.SelectedItem is not null)
            {
                SelectedProduct = item;
                _isOrderedByDescendingReplacementProducts = true;
                _isOrderedByDescendingInventory = true;

                var index = ProductsDish.IndexOf(item);

                for (int i = 0; i < ProductsDish.Count; i++)
                {
                    if (i != index)
                    {
                        ProductsDish[i].SelectedItem = null;
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
                        var products = _currentDish.SelectedProducts.ToList();
                        var product = products.FirstOrDefault(row => row.Id == SelectedProduct.Id);
                        var indexProduct = products.IndexOf(product);

                        var navigationParameters = new NavigationParameters()
                        {
                            { Constants.Navigations.INPUT_VALUE, _currentDish.SelectedProducts?[indexProduct].Comment },
                            { Constants.Navigations.PLACEHOLDER, Strings.CommentForOrder },
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

        private async Task OnTapOpenProportionsCommandAsync()
        {
            SelectedProduct = new() { SelectedItem = new() { State = ESubmenuItemsModifactions.Proportions } };

            for (int i = 0; i < ProductsDish.Count; i++)
            {
                ProductsDish[i].SelectedItem = null;
            }

            SelectedProportion = PortionsDish.FirstOrDefault(row => row.Id == _currentDish.SelectedDishProportion.Id);

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
            CurrentOrder.Seats[_indexOfSeat].SelectedDishes[_indexOfSelectedDish] = _currentDish;
            _orderService.CurrentOrder = CurrentOrder;
            _orderService.CurrentOrder.UpdateTotalSum();
            _orderService.CurrentOrder = await _bonusService.СalculationBonusAsync(CurrentOrder);
            _orderService.CurrentSeat = CurrentOrder.Seats[_indexOfSeat];

            var parameters = new NavigationParameters();

            if (App.IsTablet)
            {
                parameters.Add(Constants.Navigations.REFRESH_ORDER, true);
                parameters.Add(Constants.Navigations.SET_MODIFIED, true);
            }

            await _navigationService.GoBackAsync(parameters);
        }

        private decimal CalculateDishPriceBaseOnProportion(DishBindableModel dish, decimal priceRatio)
        {
            decimal ingredientsPrice = 0;
            decimal dishPrice = 0;

            foreach (var product in dish.SelectedProducts ?? new())
            {
                foreach (var addedIngredient in product.AddedIngredients ?? new())
                {
                    ingredientsPrice += _allIngredients.FirstOrDefault(row => row.Id == addedIngredient.Id).Price;
                }

                foreach (var excludedIngredient in product.ExcludedIngredients ?? new())
                {
                    ingredientsPrice += _allIngredients.FirstOrDefault(row => row.Id == excludedIngredient.Id).Price;
                }
            }

            dishPrice = ingredientsPrice + dish.SelectedProducts.Sum(row => row.Product.DefaultPrice);

            return dishPrice = priceRatio == 1
                ? dishPrice
                : dishPrice * (1 + priceRatio);
        }

        #endregion
    }
}
