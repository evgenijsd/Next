using AutoMapper;
using Next2.Models;
using Next2.Models.API;
using Next2.Models.API.DTO;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Next2.ViewModels
{
    public class AddSetToOrderDialogViewModel : BindableBase
    {
        private bool _canExecute = true;

        public AddSetToOrderDialogViewModel(DialogParameters param, Action<IDialogParameters> requestClose)
        {
            RequestClose = requestClose;
            CloseCommand = new Command(() => RequestClose(null));
            TapAddCommand = new Command(
                execute: () =>
                {
                    var selectedDishProportionModelDTO = Dish?.Dish.DishProportions.FirstOrDefault(row => row.Id == SelectedProportion?.Id);

                    Dish.SelectedDishProportion = new()
                    {
                        Id = selectedDishProportionModelDTO.ProportionId,
                        PriceRatio = selectedDishProportionModelDTO.PriceRatio,
                        Proportion = new ProportionModelDTO()
                        {
                            Id = selectedDishProportionModelDTO.ProportionId,
                            Name = selectedDishProportionModelDTO.ProportionName,
                        },
                    };

                    Dish.SelectedDishProportionPrice = SelectedProportion.Price;

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
                        DiscountPrice = discountPrice,
                        Dish = dish,
                        SelectedProducts = new (dish.Products.Where(row => row.Id == dish.DefaultProductId).Select(row => new ProductBindableModel()
                        {
                            Id = row.Id,
                            SelectedOptions = row.Options.FirstOrDefault(),
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
                    Proportions = dish.DishProportions.Select(row => new Models.API.ProportionModel()
                    {
                        Id = row.Id,
                        Price = row.PriceRatio == 1 ? dish.OriginalPrice : dish.OriginalPrice * (1 + row.PriceRatio),
                        Title = row.ProportionName,
                    });

                    SelectedProportion = Proportions.FirstOrDefault();
                }
            }
        }

        #region --Public Properties--

        public DishBindableModel Dish { get; }

        public IEnumerable<Models.API.ProportionModel> Proportions { get; }

        public Models.API.ProportionModel SelectedProportion { get; set; }

        public Action<IDialogParameters> RequestClose;

        public ICommand CloseCommand { get; }

        public ICommand TapAddCommand { get; }

        #endregion
    }
}