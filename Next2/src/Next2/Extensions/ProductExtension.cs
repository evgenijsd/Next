using Next2.Models.API.DTO;
using Next2.Models.Bindables;
using System;
using System.Linq;

namespace Next2.Extensions
{
    public static class ProductExtension
    {
        public static SimpleProductBindableModel ToSimpleProductBindableModel(this SimpleProductModelDTO product)
        {
            return new()
            {
                Id = product.Id,
                Name = product.Name,
                DefaultPrice = product.DefaultPrice,
                ImageSource = product.ImageSource,
                Options = product.Options?.Select(row => row.Clone()),
                Ingredients = product.Ingredients?.Select(row => row.Clone()),
            };
        }

        public static SimpleProductModelDTO ToSimpleProductModelDTO(this SimpleProductBindableModel product)
        {
            return new()
            {
                Id = product.Id,
                Name = product.Name,
                DefaultPrice = product.DefaultPrice,
                ImageSource = product.ImageSource,
                Options = product.Options?.Select(row => row.Clone()),
                Ingredients = product.Ingredients?.Select(row => row.Clone()),
            };
        }

        public static ProductBindableModel ToProductBindableModel(this SimpleProductBindableModel product)
        {
            var ingredients = product.Ingredients?.Select(row => row.Clone());

            return new()
            {
                Id = product.Id,
                Product = product.ToSimpleProductModelDTO(),
                Price = product.DefaultPrice,
                SelectedOptions = product.Options?.FirstOrDefault()?.Clone(),
                SelectedIngredients = new(ingredients),
                AddedIngredients = new(ingredients),
            };
        }

        public static ProductBindableModel ToProductBindableModel(this SimpleProductModelDTO product)
        {
            var ingredients = product.Ingredients?.Select(row => row.Clone());

            return new()
            {
                Id = product.Id,
                Product = product.Clone(),
                Price = product.DefaultPrice,
                SelectedOptions = product.Options?.FirstOrDefault()?.Clone(),
                SelectedIngredients = new(ingredients),
                AddedIngredients = new(ingredients),
            };
        }

        public static IncomingSelectedProductModel ToIncomingSelectedProductModel(this SelectedProductModelDTO product)
        {
            return new()
            {
                ProductId = product.Product.Id,
                Comment = new(product.Comment),
                SelectedOptionsId = product.SelectedOptions?.Select(x => x.Id),
                SelectedIngredientsId = product.SelectedIngredients?.Select(x => x.Id),
                AddedIngredientsId = product.AddedIngredients?.Select(x => x.Id),
                ExcludedIngredientsId = product.ExcludedIngredients?.Select(x => x.Id),
            };
        }

        public static ProductBindableModel ToProductBindableModel(this SelectedProductModelDTO product)
        {
            return new()
            {
                Id = product.Id,
                Comment = new(product.Comment),
                Product = product.Product?.Clone(),
                Price = product.Product is not null
                    ? product.Product.DefaultPrice
                    : 0,
                SelectedOptions = product.SelectedOptions?.FirstOrDefault()?.Clone(),
                SelectedIngredients = new(product.SelectedIngredients?.Select(row => row.Clone())),
                AddedIngredients = new(product.AddedIngredients?.Select(row => row.Clone())),
                ExcludedIngredients = new(product.ExcludedIngredients?.Select(row => row.Clone())),
            };
        }

        public static SelectedProductModelDTO ToSelectedProductModelDTO(this ProductBindableModel product)
        {
            return new()
            {
                Id = product.Id,
                Comment = new(product.Comment),
                Product = product.Product?.Clone(),
                SelectedOptions = product.SelectedOptions == null
                    ? null
                    : new OptionModelDTO[] { product.SelectedOptions },
                SelectedIngredients = product.SelectedOptions == null
                    ? null
                    : product.SelectedIngredients?.Select(row => row.Clone()),
                AddedIngredients = product.AddedIngredients?.Select(row => row.Clone()),
                ExcludedIngredients = product.ExcludedIngredients?.Select(row => row.Clone()),
            };
        }

        public static IncomingSelectedProductModel ToIncomingSelectedProductModel(this ProductBindableModel product)
        {
            return new()
            {
                ProductId = product.Id,
                Comment = new(product.Comment),
                SelectedOptionsId = product.SelectedOptions is not null
                            ? new Guid[1] { product.SelectedOptions.Id }
                            : null,
                SelectedIngredientsId = product.SelectedIngredients?.Select(x => x.Id),
                AddedIngredientsId = product.AddedIngredients?.Select(x => x.Id),
                ExcludedIngredientsId = product.ExcludedIngredients?.Select(x => x.Id),
            };
        }
    }
}
