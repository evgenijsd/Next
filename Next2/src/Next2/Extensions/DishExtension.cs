using Next2.Models.API.DTO;
using Next2.Models.Bindables;
using System;
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
                SelectedDishProportion = dish.DishProportions?.Where(x => x.PriceRatio == 1)?.Select(x => x.ToDishProportionModelDTO())?.FirstOrDefault(),
                DishProportions = dish.DishProportions?.Select(row => row.Clone()),
                ReplacementProducts = dish.ReplacementProducts?.Select(row => row.Clone()),
                SelectedProducts = new(dish.ReplacementProducts?.SelectMany(x => x.Products?.Where(product => product.Id == x.ProductId))?.Select(row => row.ToProductBindableModel())),
            };
        }

        public static IncomingSelectedDishModel ToIncomingSelectedDishModel(this SelectedDishModelDTO dish)
        {
            return new()
            {
                DishId = dish.DishId,
                SelectedDishProportionId = dish.SelectedDishProportion is null
                    ? Guid.Empty
                    : dish.SelectedDishProportion.Id,
                TotalPrice = dish.TotalPrice,
                DiscountPrice = dish.DiscountPrice,
                SplitPrice = dish.SplitPrice,
                HoldTime = dish.HoldTime,
                SelectedProducts = dish.SelectedProducts?.Select(x => x.ToIncomingSelectedProductModel()),
            };
        }

        public static DishBindableModel ToDishBindableModel(this SelectedDishModelDTO dish)
        {
            return new()
            {
                Id = dish.Id,
                DishId = dish.DishId,
                Name = dish.Name,
                ImageSource = dish.ImageSource,
                TotalPrice = dish.TotalPrice,
                DiscountPrice = dish.DiscountPrice,
                SplitPrice = dish.SplitPrice,
                IsSplitted = dish.IsSplitted,
                HoldTime = dish.HoldTime,
                SelectedDishProportion = dish.SelectedDishProportion?.Clone(),
                SelectedProducts = new(dish.SelectedProducts?.Select(x => x.ToProductBindableModel())),
            };
        }

        public static IncomingSelectedDishModel ToIncomingSelectedDishModel(this DishBindableModel dish)
        {
            return new()
            {
                DishId = dish.DishId,
                SelectedDishProportionId = dish.SelectedDishProportion is null
                    ? Guid.Empty
                    : dish.SelectedDishProportion.Id,
                TotalPrice = dish.TotalPrice,
                DiscountPrice = dish.DiscountPrice,
                SplitPrice = dish.SplitPrice,
                HoldTime = dish.HoldTime,
                SelectedProducts = dish.SelectedProducts?.Select(x => x.ToIncomingSelectedProductModel()),
            };
        }

        public static SelectedDishModelDTO ToSelectedDishModelDTO(this DishBindableModel dish)
        {
            return new()
            {
                Id = dish.Id,
                DishId = dish.DishId,
                Name = dish.Name,
                ImageSource = dish.ImageSource,
                TotalPrice = dish.TotalPrice,
                DiscountPrice = dish.DiscountPrice,
                SplitPrice = dish.SplitPrice,
                IsSplitted = dish.IsSplitted,
                HoldTime = dish.HoldTime,
                SelectedDishProportion = dish.SelectedDishProportion?.Clone(),
                SelectedProducts = dish.SelectedProducts?.Select(x => x.ToSelectedProductModelDTO()),
            };
        }
    }
}
