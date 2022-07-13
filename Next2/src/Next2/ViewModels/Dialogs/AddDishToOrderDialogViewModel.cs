using Next2.Models.API;
using Next2.Models.API.DTO;
using Next2.Models.Bindables;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace Next2.ViewModels
{
    public class AddDishToOrderDialogViewModel : BindableBase
    {
        private bool _canExecute = true;

        public AddDishToOrderDialogViewModel(DialogParameters param, Action<IDialogParameters> requestClose)
        {
            RequestClose = requestClose;
            CloseCommand = new Command(() => RequestClose(null));
            TapAddCommand = new Command(
                execute: () =>
                {
                    var selectedDishProportion = Dish?.DishProportions.FirstOrDefault(row => row.Id == SelectedProportion?.Id);

                    if (selectedDishProportion is not null)
                    {
                        Dish.SelectedDishProportion = new()
                        {
                            Id = selectedDishProportion.Id,
                            PriceRatio = selectedDishProportion.PriceRatio,
                            Proportion = new ProportionModelDTO()
                            {
                                Id = selectedDishProportion.Id,
                                Name = selectedDishProportion.ProportionName,
                            },
                        };
                    }

                    if (Dish.SelectedProducts is not null)
                    {
                        foreach (var product in Dish.SelectedProducts)
                        {
                            product.Price = Dish.SelectedDishProportion.PriceRatio == 1
                                ? product.Product.DefaultPrice
                                : product.Product.DefaultPrice * (1 + Dish.SelectedDishProportion.PriceRatio);

                            if (product.AddedIngredients is not null)
                            {
                                foreach (var adedIngredient in product.AddedIngredients)
                                {
                                    adedIngredient.Price = Dish.SelectedDishProportion.PriceRatio == 1
                                        ? adedIngredient.Price
                                        : adedIngredient.Price * (1 + Dish.SelectedDishProportion.PriceRatio);
                                }
                            }
                        }
                    }

                    Dish.TotalPrice = SelectedProportion.Price;

                    RequestClose(new DialogParameters() { { Constants.DialogParameterKeys.DISH, Dish } });
                },
                canExecute: () =>
                {
                    bool result = false;

                    if (_canExecute)
                    {
                        _canExecute = false;
                        result = true;
                    }

                    return result;
                });

            if (param.ContainsKey(Constants.DialogParameterKeys.DISH))
            {
                if (param.TryGetValue(Constants.DialogParameterKeys.DISH, out DishModelDTO dish)
                    && param.TryGetValue(Constants.DialogParameterKeys.DISCOUNT_PRICE, out decimal discountPrice))
                {
                    Dish = new DishBindableModel()
                    {
                        Id = dish.Id,
                        DishId = dish.Id,
                        Name = dish.Name,
                        ImageSource = dish.ImageSource,
                        TotalPrice = dish.OriginalPrice,
                        DiscountPrice = discountPrice,
                        DishProportions = dish.DishProportions,
                        Products = new (dish.Products),
                        SelectedProducts = new (dish.Products.Where(row => row.Id == dish.DefaultProductId).Select(row => new ProductBindableModel()
                        {
                            Id = row.Id,
                            SelectedOptions = row.Options.FirstOrDefault(),
                            AddedIngredients = new(row.Ingredients.Select(row => new SimpleIngredientModelDTO()
                            {
                                Id = row.Id,
                                ImageSource = row.ImageSource,
                                IngredientsCategory = row.IngredientsCategory,
                                Name = row.Name,
                                Price = row.Price,
                            })),
                            Product = new()
                            {
                                Id = row.Id,
                                DefaultPrice = row.DefaultPrice,
                                ImageSource = row.ImageSource,
                                Ingredients = row.Ingredients,
                                Name = row.Name,
                                Options = row.Options,
                            },
                        })),
                    };
                    Proportions = dish.DishProportions.Select(row => new ProportionModel()
                    {
                        Id = row.Id,
                        ProportionId = row.ProportionId,
                        Price = row.PriceRatio == 1
                            ? dish.OriginalPrice
                            : dish.OriginalPrice * (1 + row.PriceRatio),
                        ProportionName = row.ProportionName,
                    });

                    Proportions = Proportions.OrderBy(x => x.Price);

                    SelectedProportion = Proportions.FirstOrDefault(x => x.Price == dish.OriginalPrice);
                }
            }
        }

        #region -- Public properties --

        public DishBindableModel Dish { get; }

        public IEnumerable<ProportionModel> Proportions { get; }

        public ProportionModel SelectedProportion { get; set; }

        public Action<IDialogParameters> RequestClose;

        public ICommand CloseCommand { get; }

        public ICommand TapAddCommand { get; }

        #endregion
    }
}