using Next2.Interfaces;
using Next2.Models.API.DTO;
using System;
using System.Collections.ObjectModel;

namespace Next2.Models.Bindables
{
    public class ProductBindableModel : IBaseApiModel
    {
        public bool IsProductReplaced { get; set; }

        public Guid Id { get; set; }

        public string? Comment { get; set; }

        public Guid DishReplacementProductId { get; set; }

        public SimpleProductModelDTO Product { get; set; } = new();

        public decimal Price { get; set; }

        public OptionModelDTO? SelectedOptions { get; set; }

        public ObservableCollection<SimpleIngredientModelDTO>? SelectedIngredients { get; set; }

        public ObservableCollection<SimpleIngredientModelDTO>? AddedIngredients { get; set; }

        public ObservableCollection<SimpleIngredientModelDTO>? ExcludedIngredients { get; set; } = new();
    }
}
