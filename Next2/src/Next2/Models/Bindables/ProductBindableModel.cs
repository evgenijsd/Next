using Next2.Interfaces;
using Next2.Models.API.DTO;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Next2.Models.Bindables
{
    public class ProductBindableModel : IBaseApiModel, ICloneable
    {
        public Guid Id { get; set; }

        public bool IsProductReplaced { get; set; }

        public string? Comment { get; set; }

        public Guid DishReplacementProductId { get; set; }

        public SimpleProductModelDTO Product { get; set; } = new();

        public decimal Price { get; set; }

        public OptionModelDTO? SelectedOptions { get; set; }

        public ObservableCollection<SimpleIngredientModelDTO>? SelectedIngredients { get; set; }

        public ObservableCollection<SimpleIngredientModelDTO>? AddedIngredients { get; set; }

        public ObservableCollection<SimpleIngredientModelDTO>? ExcludedIngredients { get; set; } = new();

        public object Clone()
        {
            return new ProductBindableModel()
            {
                Id = Id,
                IsProductReplaced = IsProductReplaced,
                Comment = new(Comment),
                DishReplacementProductId = DishReplacementProductId,
                Product = (SimpleProductModelDTO)Product.Clone(),
                Price = Price,
                SelectedOptions = (OptionModelDTO)SelectedOptions.Clone(),
                SelectedIngredients = new(SelectedIngredients.Select(row => (SimpleIngredientModelDTO)row.Clone())),
                AddedIngredients = new(AddedIngredients.Select(row => (SimpleIngredientModelDTO)row.Clone())),
                ExcludedIngredients = new(ExcludedIngredients.Select(row => (SimpleIngredientModelDTO)row.Clone())),
            };
        }
    }
}
