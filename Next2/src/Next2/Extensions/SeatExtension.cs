using Next2.Models.API.DTO;
using Next2.Models.Bindables;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Next2.Extensions
{
    public static class SeatExtension
    {
        public static SeatBindableModel ToSeatBindableModel(this SeatModelDTO seat) =>
            new SeatBindableModel
            {
                SeatNumber = seat.Number,
                SelectedDishes = new ObservableCollection<DishBindableModel>(
                    seat.SelectedDishes.Select(y => new DishBindableModel()
                    {
                        TotalPrice = y.TotalPrice,
                        ImageSource = y.ImageSource,
                        Name = y.Name,
                    })),
            };

        public static IEnumerable<SeatBindableModel> ToSeatsBindableModels(this IEnumerable<SeatModelDTO>? seats)
        {
            return seats.Select(x => new SeatBindableModel()
            {
                IsFirstSeat = x.Id == seats.First().Id,
                Checked = false,
                SeatNumber = x.Number,
                SelectedDishes = new(x.SelectedDishes.Select(y => new DishBindableModel()
                {
                    DiscountPrice = y.DiscountPrice,
                    DishId = y.DishId,
                    IsSplitted = y.IsSplitted,
                    SplitPrice = y.SplitPrice,
                    HoldTime = y.HoldTime,
                    Id = y.Id,
                    ImageSource = y.ImageSource,
                    Name = y.Name,
                    TotalPrice = y.TotalPrice,
                    SelectedDishProportion = y.SelectedDishProportion,
                    SelectedProducts = new(y.SelectedProducts.Select(x => new ProductBindableModel()
                    {
                        Id = x.Id,
                        Price = x.Product.DefaultPrice,
                        Comment = x.Comment,
                        Product = x.Product,
                        AddedIngredients = new(x.AddedIngredients),
                        ExcludedIngredients = new(x.ExcludedIngredients),
                        SelectedIngredients = new(x.SelectedIngredients),
                        SelectedOptions = x.SelectedOptions.FirstOrDefault(),
                    })),
                })),
            }).OrderBy(x => x.SeatNumber);
        }

        public static IEnumerable<SeatModelDTO>? ToSeatsModelsDTO(this IEnumerable<SeatBindableModel> seats)
        {
            return seats.Select(x => x.ToSeatModelDTO());
        }

        public static SeatModelDTO ToSeatModelDTO(this SeatBindableModel seat)
        {
            return new SeatModelDTO()
            {
                Number = seat.SeatNumber,
                SelectedDishes = seat.SelectedDishes?.Select(y => new SelectedDishModelDTO
                {
                    Id = y.Id,
                    DiscountPrice = y.DiscountPrice,
                    DishId = y.DishId,
                    ImageSource = y.ImageSource,
                    Name = y.Name,
                    IsSplitted = y.IsSplitted,
                    SplitPrice = y.SplitPrice,
                    HoldTime = y.HoldTime,
                    TotalPrice = y.TotalPrice,
                    SelectedDishProportion = y.SelectedDishProportion,
                    SelectedProducts = y.SelectedProducts?.Select(x => new SelectedProductModelDTO
                    {
                        Id = x.Id,
                        Comment = x.Comment,
                        Product = new SimpleProductModelDTO
                        {
                            Id = x.Product.Id,
                            DefaultPrice = x.Product.DefaultPrice,
                            ImageSource = x.Product?.ImageSource,
                            Name = x.Product?.Name,
                            Options = x.Product?.Options?.Select(x => new OptionModelDTO
                            {
                                Id = x.Id,
                                Name = x.Name,
                            }),
                            Ingredients = x.Product?.Ingredients.Select(x => new SimpleIngredientModelDTO
                            {
                                Id = x.Id,
                                Name = x.Name,
                                ImageSource = x.ImageSource,
                                IngredientsCategory = x.IngredientsCategory is null
                                    ? new()
                                    : x.IngredientsCategory,
                                Price = x.Price,
                            }),
                        },
                        AddedIngredients = x.AddedIngredients,
                        ExcludedIngredients = x.ExcludedIngredients,
                        SelectedIngredients = x.SelectedIngredients,
                        SelectedOptions = x.SelectedOptions is null
                            ? null
                            : new OptionModelDTO[] { x.SelectedOptions },
                    }),
                }),
            };
        }
    }
}
