using Next2.Interfaces;
using Next2.Models.API.DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Next2.Models
{
    public class ProductBindableModel : IBaseApiModel
    {
        public ProductBindableModel()
        {
        }

        public ProductBindableModel(ProductBindableModel product)
        {
            Id = product.Id;

            Comment = product.Comment;

            Product = new SimpleProductModelDTO()
            {
                Id = product.Product.Id,
                DefaultPrice = product.Product.DefaultPrice,
                ImageSource = product.Product.ImageSource,
                Ingredients = product.Product.Ingredients,
                Name = product.Product.Name,
                Options = product.Product.Options,
            };

            SelectedOptions = product.SelectedOptions;

            SelectedIngredients = product.SelectedIngredients;

            AddedIngredients = product.AddedIngredients;

            ExcludedIngredients = product.ExcludedIngredients;
        }

        public Guid Id { get; set; }

        public string? Comment { get; set; }

        public SimpleProductModelDTO Product { get; set; } = new();

        public decimal ProductPriceBaseOnProportion { get; set; }

        public OptionModelDTO? SelectedOptions { get; set; }

        public bool IsProductReplaced { get; set; }

        public ObservableCollection<SimpleIngredientModelDTO>? SelectedIngredients { get; set; }

        public ObservableCollection<SimpleIngredientModelDTO>? AddedIngredients { get; set; }

        public ObservableCollection<SimpleIngredientModelDTO>? ExcludedIngredients { get; set; } = new();
    }
}
