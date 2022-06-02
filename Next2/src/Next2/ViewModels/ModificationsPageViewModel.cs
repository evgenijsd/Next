using AutoMapper;
using Next2.Enums;
using Next2.Helpers;
using Next2.Models;
using Next2.Models.API.DTO;
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

        private DishBindableModel _selectedDish;
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

            var seat = CurrentOrder.Seats.FirstOrDefault(row => row.SelectedItem != null);

            _indexOfSeat = CurrentOrder.Seats.IndexOf(seat);
            _selectedDish = CurrentOrder.Seats[_indexOfSeat].SelectedItem;
            _indexOfSelectedDish = seat.SelectedDishes.IndexOf(_selectedDish);

            _currentDish = CurrentOrder.Seats[_indexOfSeat].SelectedDishes[_indexOfSelectedDish];
            InitProductsSet();
            //InitPortionsSet();
            SelectedProduct = new() { SelectedItem = new() { State = ESubmenuItemsModifactions.Proportions } };
        }

        #region -- Public properties --

        public FullOrderBindableModel CurrentOrder { get; set; }

        public SpoilerBindableModel SelectedProduct { get; set; }

        public ObservableCollection<SpoilerBindableModel> ProductsSet { get; set; }

        public PortionModel? SelectedPortion { get; set; }

        public ObservableCollection<PortionModel> PortionsSet { get; set; }

        public OptionModel? SelectedOption { get; set; }

        public ObservableCollection<OptionModel> OptionsProduct { get; set; }

        private ProductModel _selectedReplacementProduct;
        public ProductModel? SelectedReplacementProduct
        {
            get => _selectedReplacementProduct;
            set => SetProperty(ref _selectedReplacementProduct, value);
        }

        public ObservableCollection<ProductModel> ReplacementProducts { get; set; }

        public IngredientsCategoryModelDTO? SelectedIngredientCategory { get; set; }

        public ObservableCollection<IngredientsCategoryModelDTO> IngredientCategories { get; set; }

        public ObservableCollection<IngredientBindableModel> Ingredients { get; set; }

        public bool IsMenuOpen { get; set; }

        public bool IsExpandedIngredientCategories { get; set; }

        public int HeightIngredientCategories { get; set; }

        private ICommand _tapSubmenuCommand;
        public ICommand TapSubmenuCommand => _tapSubmenuCommand ??= new AsyncCommand<SpoilerBindableModel>(OnTapSubmenuCommandAsync);

        //private ICommand _tapOpenProportionsCommand;
        //public ICommand TapOpenProportionsCommand => _tapOpenProportionsCommand ??= new AsyncCommand(OnTapOpenProportionsCommandAsync);
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

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            //if (parameters.TryGetValue(Constants.Navigations.INPUT_VALUE, out string text))
            //{
            //    var products = _currentSet.Products;
            //    var product = products.FirstOrDefault(row => row.Id == SelectedProduct.Id);
            //    var indexProduct = products.IndexOf(product);

            //    ProductsSet[indexProduct].Items[3].CanShowDot = !string.IsNullOrWhiteSpace(text);

            //    _currentSet.Products[indexProduct].Comment = text;

            //    ProductsSet[indexProduct].SelectedItem = ProductsSet[indexProduct].Items.FirstOrDefault();
            //}
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            switch (args.PropertyName)
            {
                //case nameof(SelectedPortion):
                //    if (SelectedPortion is not null)
                //    {
                //        _currentSet.Portion = SelectedPortion;
                //    }

                //    break;
                //case nameof(SelectedReplacementProduct):
                //    if (SelectedReplacementProduct is not null)
                //    {
                //        var products = _currentSet.Products;
                //        var product = products.FirstOrDefault(row => row.Id == SelectedProduct.Id);

                //        product.SelectedProduct = SelectedReplacementProduct;
                //        ProductsSet[ProductsSet.IndexOf(SelectedProduct)].Title = SelectedReplacementProduct.Title;

                //        _currentSet.Price = 0;

                //        foreach (var item in _currentSet.Products)
                //        {
                //            _currentSet.Price += item.SelectedProduct.ProductPrice + item.IngredientsPrice;
                //        }

                //        ResetSelectedIngredientsAsync(product);

                //        ResetSelectedOptionsAsync(product);
                //    }

                //    break;
                //case nameof(SelectedOption):
                //    if (SelectedOption is not null)
                //    {
                //        var products = _currentSet.Products;
                //        var product = products.FirstOrDefault(row => row.Id == SelectedProduct.Id);

                //        products[products.IndexOf(product)].SelectedOption = SelectedOption;
                //    }

                //    break;
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

        //private async Task ResetSelectedIngredientsAsync(ProductBindableModel product)
        //{
        //    var ingridients = await _menuService.GetIngredientsOfProductAsync(product.SelectedProduct.Id);

        //    if (ingridients.IsSuccess)
        //    {
        //        product.SelectedIngredients = new(ingridients.Result);
        //        product.DefaultSelectedIngredients = new(ingridients.Result);
        //    }
        //}
        private async Task ResetSelectedOptionsAsync(ProductBindableModel product)
        {
            //var options = await _menuService.GetOptionsOfProductAsync(product.SelectedProduct.Id);

            //if (options.IsSuccess)
            //{
            //    if (options.Result is not null)
            //    {
            //        product.SelectedOption = options.Result.FirstOrDefault(row => row.Id == product.SelectedProduct.DefaultOptionId);
            //        product.Options = new(options.Result);
            //    }
            //    else
            //    {
            //        product.SelectedOption = new();
            //        product.Options = new();
            //    }
            //}
        }

        private Task OnChangingOrderSortReplacementProductsCommandAsync()
        {
            _isOrderedByDescendingReplacementProducts = !_isOrderedByDescendingReplacementProducts;

            InitReplacementProductsSet();

            return Task.CompletedTask;
        }

        private Task OnChangingOrderSortInventoryCommandAsync()
        {
            _isOrderedByDescendingInventory = !_isOrderedByDescendingInventory;

            if (_isOrderedByDescendingInventory)
            {
                Ingredients = new(Ingredients.OrderBy(row => row.Title));
            }
            else
            {
                Ingredients = new(Ingredients.OrderByDescending(row => row.Title));
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

            var product = _currentDish?.SelectedProducts?[ProductsSet.IndexOf(SelectedProduct)];

            var ingredient = product?.AddedIngredients.FirstOrDefault(row => row.Id == toggleIngredient.Id);

            if (product is not null)
            {
                if (ingredient is null)
                {
                    product.AddedIngredients?.Add(new SimpleIngredientModelDTO()
                    {
                        Id = toggleIngredient.Id,
                        ImageSource = toggleIngredient.ImagePath,
                        Name = toggleIngredient.Title,
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
            }
        }

        private void InitProductsSet()
        {
            var products = _currentDish.SelectedProducts;

            if (products is not null)
            {
                ProductsSet = new(products.Select(row =>
                {
                    var result = new SpoilerBindableModel
                    {
                        Id = row.Id,
                        Title = row.Product.Name,
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

        private void InitReplacementProductsSet()
        {
            //var product = _currentSet.Products[ProductsSet.IndexOf(SelectedProduct)];

            //if (product.ReplacementProducts is var replacementProducts)
            //{
            //    if (_isOrderedByDescendingReplacementProducts)
            //    {
            //        ReplacementProducts = new(replacementProducts.OrderBy(row => row.Title));
            //    }
            //    else
            //    {
            //        ReplacementProducts = new(replacementProducts.OrderByDescending(row => row.Title));
            //    }

            //    _selectedReplacementProduct = ReplacementProducts.FirstOrDefault(row => row.Id == product.SelectedProduct?.Id);
            //}
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

            IsExpandedIngredientCategories = false;

            var countItemsInRow = App.IsTablet ? 6 : 2;
            var countRow = Math.Ceiling((double)IngredientCategories.Count / countItemsInRow);

            HeightIngredientCategories = (int)(((countRow > 2 ? 2 : countRow) * (44 + 8)) - 6);
        }

        private async Task InitIngredientsByCategoryAsync(Guid categoryId)
        {
            if (_allIngredients is not null)
            {
                var product = _currentDish?.SelectedProducts?[ProductsSet.IndexOf(SelectedProduct)];

                Ingredients = new(_allIngredients.Where(row => row.IngredientsCategoryId == categoryId).Select(row => new IngredientBindableModel()
                {
                    Id = row.Id,
                    CategoryId = row.IngredientsCategoryId,
                    IsToggled = product is not null ? product.AddedIngredients.Any(item => item.Id == row.Id) : false,
                    Title = row.Name,
                    Price = _currentDish.SelectedDishProportion.PriceRatio == 1 ? row.Price : row.Price * (1 + _currentDish.SelectedDishProportion.PriceRatio),
                    ImagePath = row.ImageSource,
                    ChangingToggle = ChangingToggleCommand,
                }));
            }
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

            SelectedIngredientCategory = IngredientCategories.FirstOrDefault();
        }

        private void InitPortionsSet()
        {
            //var portions = _currentDish.Portions;

            //if (portions is not null)
            //{
            //    PortionsSet = new(portions);

            //    SelectedPortion = PortionsSet.FirstOrDefault(row => row.Id == _selectedDish.Portion.Id);
            //    _currentDish.Portion = SelectedPortion;
            //}
        }

        private void LoadOptionsProduct()
        {
            //if (SelectedProduct != null)
            //{
            //    var products = _currentSet.Products;
            //    var product = products.FirstOrDefault(row => row.Id == SelectedProduct.Id);
            //    var indexProduct = products.IndexOf(product);

            //    var options = products[indexProduct].Options;

            //    if (options is not null)
            //    {
            //        OptionsProduct = new(options);
            //        SelectedOption = products[indexProduct].SelectedOption;
            //    }
            //}
        }

        private async Task OnTapSubmenuCommandAsync(SpoilerBindableModel item)
        {
            if (item.SelectedItem is not null)
            {
                SelectedProduct = item;
                _isOrderedByDescendingReplacementProducts = true;
                _isOrderedByDescendingInventory = true;

                var index = ProductsSet.IndexOf(item);

                for (int i = 0; i < ProductsSet.Count; i++)
                {
                    if (i != index)
                    {
                        ProductsSet[i].SelectedItem = null;
                    }
                }

                switch (item.SelectedItem.State)
                {
                    //case ESubmenuItemsModifactions.Replace:
                    //    InitReplacementProductsSet();
                    //    break;
                    case ESubmenuItemsModifactions.Inventory:
                        Ingredients = new();
                        SelectedIngredientCategory = null;

                        InitIngredientCategoriesAsync().Await();
                        InitIngredientsAsync().Await();
                        break;
                        //case ESubmenuItemsModifactions.Comment:
                        //    var products = _currentSet.Products;
                        //    var product = products.FirstOrDefault(row => row.Id == SelectedProduct.Id);
                        //    var indexProduct = products.IndexOf(product);

                        //    var navigationParameters = new NavigationParameters()
                        //    {
                        //        { Constants.Navigations.INPUT_VALUE, _currentSet.Products[indexProduct].Comment },
                        //        { Constants.Navigations.PLACEHOLDER, Strings.CommentForOrder },
                        //    };

                        //    await _navigationService.NavigateAsync(nameof(InputTextPage), navigationParameters);
                        //    break;
                }

                if (!App.IsTablet)
                {
                    await OnCloseMenuCommandAsync();
                }
            }
        }

        //private async Task OnTapOpenProportionsCommandAsync()
        //{
        //    SelectedProduct = new() { SelectedItem = new() { State = ESubmenuItemsModifactions.Proportions } };

        //    for (int i = 0; i < ProductsSet.Count; i++)
        //    {
        //        ProductsSet[i].SelectedItem = null;
        //    }

        //    SelectedPortion = _currentDish.Portion;

        //    if (!App.IsTablet)
        //    {
        //        await OnCloseMenuCommandAsync();
        //    }
        //}
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
            _orderService.CurrentOrder = CurrentOrder;
            _orderService.CurrentOrder.UpdateTotalSum();
            CurrentOrder = await _bonusService.СalculationBonusAsync(CurrentOrder);
            _orderService.CurrentSeat = CurrentOrder.Seats.FirstOrDefault(row => row.SeatNumber == _orderService?.CurrentSeat?.SeatNumber);

            if (App.IsTablet)
            {
                var parameters = new NavigationParameters
                {
                    { Constants.Navigations.REFRESH_ORDER, true },
                    { Constants.Navigations.SET_MODIFIED, true },
                };

                await _navigationService.GoBackAsync(parameters);
            }
            else
            {
                await _navigationService.GoBackAsync();
            }
        }

        #endregion
    }
}
