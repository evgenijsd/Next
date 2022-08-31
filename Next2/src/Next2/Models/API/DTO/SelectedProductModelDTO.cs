using Next2.Interfaces;
using System;
using System.Collections.Generic;

namespace Next2.Models.API.DTO
{
    public class SelectedProductModelDTO : IBaseApiModel
    {
        public Guid Id { get; set; }

        public string? Comment { get; set; }

        public Guid DishReplacementProductId { get; set; }

        public SimpleProductModelDTO Product { get; set; } = new();

        public IEnumerable<OptionModelDTO>? SelectedOptions { get; set; }

        public IEnumerable<SimpleIngredientModelDTO>? SelectedIngredients { get; set; }

        public IEnumerable<SimpleIngredientModelDTO>? AddedIngredients { get; set; }

        public IEnumerable<SimpleIngredientModelDTO>? ExcludedIngredients { get; set; }
    }
}
