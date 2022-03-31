﻿using AutoMapper;
using Next2.ENums;
using Next2.Models;
using Next2.Services.Menu;
using Next2.Services.Order;
using Prism.Navigation;
using System;
using System.Collections.Generic;
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
        private readonly IMenuService _menuService;

        private int _indexOfSeat;
        private int _indexOfSelectedSet;

        private bool _isOrderedByDescendingReplacementProducts = true;
        private bool _isOrderedByDescendingInventory = true;

        private SetBindableModel _selectedSet;
        private SetBindableModel _currentSet;

        public ModificationsPageViewModel(
            INavigationService navigationService,
            IOrderService orderService,
            IMenuService menuService)
            : base(navigationService)
        {
            _orderService = orderService;
            _menuService = menuService;

            CurrentOrder = new(_orderService.CurrentOrder);

            var seat = CurrentOrder.Seats.FirstOrDefault(row => row.SelectedItem != null);

            _indexOfSeat = CurrentOrder.Seats.IndexOf(seat);
            _selectedSet = CurrentOrder.Seats[_indexOfSeat].SelectedItem;
            _indexOfSelectedSet = seat.Sets.IndexOf(_selectedSet);

            _currentSet = CurrentOrder.Seats[_indexOfSeat].Sets[_indexOfSelectedSet];

            InitSubmenuItems();
            InitProductsSet();
            InitPortionsSet();

            SelectedProduct = new() { SelectedItem = new() { State = ESubmenuItemsModifactions.Proportions } };
        }

        #region -- Public properties --

        public FullOrderBindableModel CurrentOrder { get; set; }

        public ObservableCollection<ItemSpoilerModel> SubmenuItems { get; set; }

        public SpoilerBindableModel SelectedProduct { get; set; }

        public ObservableCollection<SpoilerBindableModel> ProductsSet { get; set; }

        public PortionModel? SelectedPortion { get; set; }

        public ObservableCollection<PortionModel> PortionsSet { get; set; }

        public OptionModel? SelectedOption { get; set; }

        public ObservableCollection<OptionModel> OptionsProduct { get; set; }

        public ProductModel? SelectedReplacementProduct { get; set; }

        public ObservableCollection<ProductModel> ReplacementProducts { get; set; }

        public IngredientCategoryModel? SelectedIngredientCategory { get; set; }

        public ObservableCollection<IngredientCategoryModel> IngredientCategories { get; set; }

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
        public ICommand ChangingToggleCommand => _changingToggleCommand ??= new AsyncCommand<IngredientBindableModel>(OnChangingToggleCommandAsync);

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
                        _currentSet.Portion = SelectedPortion;
                    }

                    break;
                case nameof(SelectedReplacementProduct):
                    if (SelectedReplacementProduct is not null)
                    {
                        var products = _currentSet.Products;
                        var product = products.FirstOrDefault(row => row.Id == SelectedProduct.Id);

                        _currentSet.Products[products.IndexOf(product)].SelectedProduct = SelectedReplacementProduct;
                    }

                    break;
                case nameof(SelectedOption):
                    if (SelectedOption is not null)
                    {
                        var products = _currentSet.Products;
                        var product = products.FirstOrDefault(row => row.Id == SelectedProduct.Id);

                        products[products.IndexOf(product)].SelectedOption = SelectedOption;
                    }

                    break;
                case nameof(SelectedIngredientCategory):
                    if (SelectedIngredientCategory is not null)
                    {
                        InitIngredientsAsync(SelectedIngredientCategory.Id).Await();
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
                    State = ESubmenuItemsModifactions.Replace,
                    Title = "Replace",
                    ImagePath = "ic_paper_plus_24x24.png",
                    SelectedImagePath = "ic_paper_plus_primary_24x24.png",
                },
                new ItemSpoilerModel()
                {
                    State = ESubmenuItemsModifactions.Inventory,
                    Title = "Inventory",
                    ImagePath = "ic_paper_plus_24x24.png",
                    SelectedImagePath = "ic_paper_plus_primary_24x24.png",
                },
                new ItemSpoilerModel()
                {
                    State = ESubmenuItemsModifactions.Options,
                    Title = "Options",
                    ImagePath = "ic_paper_plus_24x24.png",
                    SelectedImagePath = "ic_paper_plus_primary_24x24.png",
                },
                new ItemSpoilerModel()
                {
                    State = ESubmenuItemsModifactions.Comment,
                    Title = "Comment",
                    ImagePath = "ic_paper_plus_24x24.png",
                    SelectedImagePath = "ic_paper_plus_primary_24x24.png",
                },
            };
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

            HeightIngredientCategories = (int)(((IsExpandedIngredientCategories ? countRow : 2) * (44 + 8)) - 6);

            return Task.CompletedTask;
        }

        private Task OnChangingToggleCommandAsync(IngredientBindableModel toggleIngredient)
        {
            var product = _currentSet.Products[ProductsSet.IndexOf(SelectedProduct)];

            var ingridient = product.SelectedIngredients.FirstOrDefault(row => row.IngredientId == toggleIngredient.Id);

            if(ingridient is null)
            {
                product.SelectedIngredients.Add(new IngredientOfProductModel()
                {
                    IngredientId = toggleIngredient.Id,
                    ProductId = product.Id,
                });
            }
            else
            {
                product.SelectedIngredients.Remove(ingridient);
            }

            return Task.CompletedTask;
        }

        private void InitProductsSet()
        {
            var products = _currentSet.Products;
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

        private void InitReplacementProductsSet()
        {
            var product = _currentSet.Products[ProductsSet.IndexOf(SelectedProduct)];

            if (product.ReplacementProducts is var replacementProducts)
            {
                if (_isOrderedByDescendingReplacementProducts)
                {
                    ReplacementProducts = new(replacementProducts.OrderBy(row => row.Title));
                }
                else
                {
                    ReplacementProducts = new(replacementProducts.OrderByDescending(row => row.Title));
                }

                SelectedReplacementProduct = ReplacementProducts.FirstOrDefault(row => row.Id == product.SelectedProduct?.Id);
            }
        }

        private async Task InitIngredientCategoriesAsync()
        {
            var ingredientCategories = await _menuService.GetIngredientCategoriesAsync();

            if (ingredientCategories.IsSuccess)
            {
                IngredientCategories = new(ingredientCategories.Result);
                SelectedIngredientCategory = IngredientCategories.FirstOrDefault();

                IsExpandedIngredientCategories = false;
                var countItemsInRow = App.IsTablet ? 6 : 2;
                var countRow = Math.Ceiling((double)IngredientCategories.Count / countItemsInRow);

                HeightIngredientCategories = (int)(((countRow > 2 ? 2 : countRow) * (44 + 8)) - 6);
            }
        }

        private async Task InitIngredientsAsync(int categoryId)
        {
            var ingredients = await _menuService.GetIngredientsAsync(categoryId);

            if (ingredients.IsSuccess)
            {
                var product = _currentSet.Products[ProductsSet.IndexOf(SelectedProduct)];

                Ingredients = new(ingredients.Result.Select(row => new IngredientBindableModel()
                {
                    Id = row.Id,
                    CategoryId = row.CategoryId,
                    IsToggle = product.SelectedIngredients.Any(item => item.IngredientId == row.Id),
                    Title = row.Title,
                    Price = row.Price,
                    ImagePath = row.ImagePath,
                    ChangingToggle = ChangingToggleCommand,
                }));
            }
        }

        private void InitPortionsSet()
        {
            var portions = _currentSet.Portions;

            if (portions is not null)
            {
                PortionsSet = new(portions);

                SelectedPortion = PortionsSet.FirstOrDefault(row => row.Id == _selectedSet.Portion.Id);
                _currentSet.Portion = SelectedPortion;
            }
        }

        private void LoadOptionsProduct()
        {
            if (SelectedProduct != null)
            {
                var products = _currentSet.Products;
                var product = products.FirstOrDefault(row => row.Id == SelectedProduct.Id);
                var indexProduct = products.IndexOf(product);

                var options = products[indexProduct].Options;
                if (options is not null)
                {
                    OptionsProduct = new(options);
                    SelectedOption = products[indexProduct].SelectedOption;
                }
            }
        }

        private async Task OnTapSubmenuCommandAsync(SpoilerBindableModel item)
        {
            if (item.SelectedItem is not null)
            {
                SelectedProduct = item;

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
                    case ESubmenuItemsModifactions.Options:
                        LoadOptionsProduct();
                        break;
                    case ESubmenuItemsModifactions.Replace:
                        InitReplacementProductsSet();
                        break;
                    case ESubmenuItemsModifactions.Inventory:
                        Ingredients = new();
                        SelectedIngredientCategory = null;

                        InitIngredientCategoriesAsync().Await();
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

            for (int i = 0; i < ProductsSet.Count; i++)
            {
                ProductsSet[i].SelectedItem = null;
            }

            SelectedPortion = _currentSet.Portion;

            if (!App.IsTablet)
            {
                await OnCloseMenuCommandAsync();
            }
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
            _orderService.CurrentOrder.UpdateTotalSum();
            _orderService.CurrentSeat = CurrentOrder.Seats.FirstOrDefault(row => row.Id == _orderService?.CurrentSeat?.Id);

            await _navigationService.GoBackAsync();
        }

        #endregion
    }
}
