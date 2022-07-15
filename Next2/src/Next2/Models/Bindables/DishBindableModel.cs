using Next2.Interfaces;
using Next2.Models.API.DTO;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Next2.Models.Bindables
{
    public class DishBindableModel : BindableBase, IBaseApiModel, ICloneable
    {
        public int SeatNumber { get; set; }

        public bool IsSeatSelected { get; set; }

        public Guid Id { get; set; }

        public Guid DishId { get; set; }

        public string? Name { get; set; }

        public string? ImageSource { get; set; }

        public decimal TotalPrice { get; set; }

        public decimal DiscountPrice { get; set; }

        public bool HasSplittedPrice { get; set; }

        public IEnumerable<SimpleDishProportionModelDTO>? DishProportions { get; set; }

        public DishProportionModelDTO SelectedDishProportion { get; set; } = new();

        public ObservableCollection<SimpleProductModelDTO>? Products { get; set; } = new();

        public ObservableCollection<ProductBindableModel>? SelectedProducts { get; set; }

        public ICommand? SelectDishCommand { get; set; }

        public object Clone()
        {
            return new DishBindableModel()
            {
                Id = Id,
                Name = Name,
                DishId = DishId,
                ImageSource = ImageSource,
                TotalPrice = TotalPrice,
                HasSplittedPrice = HasSplittedPrice,
                DiscountPrice = DiscountPrice,
                DishProportions = DishProportions?.Select(x => new SimpleDishProportionModelDTO
                {
                    Id = x.Id,
                    PriceRatio = x.PriceRatio,
                    ProportionId = x.ProportionId,
                    ProportionName = x?.ProportionName,
                }),
                SelectedDishProportion = new DishProportionModelDTO
                {
                    Id = SelectedDishProportion.Id,
                    PriceRatio = SelectedDishProportion.PriceRatio,
                    Proportion = new ProportionModelDTO
                    {
                        Id = SelectedDishProportion.Proportion.Id,
                        Name = SelectedDishProportion.Proportion?.Name,
                    },
                },
                Products = new(Products.Select(x => new SimpleProductModelDTO
                {
                    Id = x.Id,
                    Name = x.Name,
                    DefaultPrice = x.DefaultPrice,
                    ImageSource = x.ImageSource,
                    Ingredients = x.Ingredients.Select(x => new SimpleIngredientModelDTO
                    {
                        Id = x.Id,
                        ImageSource = x.ImageSource,
                        Name = x.Name,
                        IngredientsCategory = new SimpleIngredientsCategoryModelDTO
                        {
                            Name = x.IngredientsCategory.Name,
                            Id = x.IngredientsCategory.Id,
                        },
                        Price = x.Price,
                    }),
                    Options = x.Options.Select(x => new OptionModelDTO
                    {
                        Id = x.Id,
                        Name = x.Name,
                    }),
                })),
                SelectedProducts = SelectedProducts,
            };
        }
    }
}
