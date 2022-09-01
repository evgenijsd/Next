using Next2.Models.API.DTO;
using Next2.Models.Bindables;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Next2.Extensions
{
    public static class DishExtension
    {
        public static DishBindableModel ToDishBindableModel(this DishModelDTO dish)
        {
            var selectedProducts = new ObservableCollection<ProductBindableModel>();

            foreach (var replacementProduct in dish.ReplacementProducts)
            {
                var selectedProduct = replacementProduct.Products.FirstOrDefault(row => row.Id == replacementProduct.ProductId);

                if (selectedProduct is not null)
                {
                    var bindableSelectedProduct = selectedProduct.ToProductBindableModel();

                    bindableSelectedProduct.DishReplacementProductId = replacementProduct.Id;

                    selectedProducts.Add(bindableSelectedProduct);
                }
            }

            return new()
            {
                Id = dish.Id,
                DishId = dish.Id,
                Name = new(dish.Name),
                ImageSource = new(dish.ImageSource),
                TotalPrice = dish.OriginalPrice,
                DiscountPrice = 0,
                SelectedDishProportion = dish.DishProportions?.FirstOrDefault(x => x.PriceRatio == 1)?.ToDishProportionModelDTO(),
                DishProportions = dish.DishProportions?.Select(row => (SimpleDishProportionModelDTO)row.Clone()),
                ReplacementProducts = dish.ReplacementProducts?.Select(row => (DishReplacementProductModelDTO)row.Clone()),
                SelectedProducts = selectedProducts,
            };
        }

        public static IncomingSelectedDishModel ToIncomingSelectedDishModel(this SelectedDishModelDTO dish)
        {
            return new()
            {
                DishId = dish.DishId,
                SelectedDishProportionId = dish.SelectedDishProportion?.Id ?? Guid.Empty,
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
                Name = new(dish.Name),
                ImageSource = new(dish.ImageSource),
                TotalPrice = dish.TotalPrice,
                DiscountPrice = dish.DiscountPrice,
                SplitPrice = dish.SplitPrice,
                IsSplitted = dish.IsSplitted,
                HoldTime = dish.HoldTime,
                SelectedDishProportion = (DishProportionModelDTO)dish.SelectedDishProportion?.Clone(),
                SelectedProducts = new(dish.SelectedProducts?.Select(x => x.ToProductBindableModel())),
            };
        }

        public static IncomingSelectedDishModel ToIncomingSelectedDishModel(this DishBindableModel dish)
        {
            return new()
            {
                DishId = dish.DishId,
                SelectedDishProportionId = dish.SelectedDishProportion?.Id ?? Guid.Empty,
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
                Name = new(dish.Name),
                ImageSource = new(dish.ImageSource),
                TotalPrice = dish.TotalPrice,
                DiscountPrice = dish.DiscountPrice,
                SplitPrice = dish.SplitPrice,
                IsSplitted = dish.IsSplitted,
                HoldTime = dish.HoldTime,
                SelectedDishProportion = (DishProportionModelDTO)dish.SelectedDishProportion?.Clone(),
                SelectedProducts = dish.SelectedProducts?.Select(x => x.ToSelectedProductModelDTO()),
            };
        }
    }
}
