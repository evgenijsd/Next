using Next2.Extensions;
using Next2.Models.API.DTO;
using Next2.Models.Bindables;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace Next2.ViewModels.Dialogs
{
    public class AddDishToOrderDialogViewModel : BindableBase
    {
        private bool _canExecute = true;

        public AddDishToOrderDialogViewModel(
            DialogParameters parameters,
            Action<IDialogParameters> requestClose)
        {
            RequestClose = requestClose;

            CloseCommand = new Command(() => RequestClose(new DialogParameters()));

            TapAddCommand = new Command(
                execute: () =>
                {
                    var selectedDishProportion = Dish.DishProportions?.FirstOrDefault(row => row.Id == SelectedProportion?.Id);

                    if (selectedDishProportion is not null)
                    {
                        Dish.SelectedDishProportion = selectedDishProportion.ToDishProportionModelDTO();
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

                    Dish.TotalPrice = SelectedProportion is null
                        ? 0
                        : SelectedProportion.Price;

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

            if (parameters.TryGetValue(Constants.DialogParameterKeys.DISH, out DishModelDTO dish)
                && parameters.TryGetValue(Constants.DialogParameterKeys.DISCOUNT_PRICE, out decimal discountPrice))
            {
                Dish = dish.ToDishBindableModel();

                Proportions = dish.DishProportions.Select(row => new ProportionBindableModel()
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
            else
            {
                Proportions = Enumerable.Empty<ProportionBindableModel>();
            }
        }

        #region -- Public properties --

        public DishBindableModel Dish { get; } = new();

        public IEnumerable<ProportionBindableModel> Proportions { get; }

        public ProportionBindableModel SelectedProportion { get; set; } = new();

        public Action<IDialogParameters> RequestClose;

        public ICommand CloseCommand { get; }

        public ICommand TapAddCommand { get; }

        #endregion
    }
}