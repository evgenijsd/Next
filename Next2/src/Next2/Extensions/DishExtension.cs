using Next2.Models.API.DTO;
using Next2.Models.Bindables;
using System.Linq;

namespace Next2.Extensions
{
    public static class DishExtension
    {
        public static DishBindableModel ToDishBindableModel(this DishModelDTO dish)
        {
            return new()
            {
                Id = dish.Id,
                DishId = dish.Id,
                Name = dish.Name,
                ImageSource = dish.ImageSource,
                TotalPrice = dish.OriginalPrice,
                DiscountPrice = 0,
                SelectedDishProportion = dish.DishProportions?.Where(x => x.PriceRatio == 1)?.Select(x => x.ToDishProportionModelDTO()).FirstOrDefault(),
                DishProportions = dish.DishProportions,
                ReplacementProducts = dish.ReplacementProducts,
                SelectedProducts = new(dish.ReplacementProducts?.SelectMany(x => x.Products?.Where(product => product.Id == x.ProductId)).Select(row => new ProductBindableModel()
                {
                    Id = row.Id,
                    SelectedOptions = row.Options?.FirstOrDefault(),
                    AddedIngredients = new(row.Ingredients?.Select(row => row.Clone())),
                    Product = row.Clone(),
                })),
            };
        }
    }
}
