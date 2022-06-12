using Next2.Interfaces;
using System;

namespace Next2.Models.API.DTO
{
    public class SimpleIngredientModelDTO : IBaseApiModel
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public decimal Price { get; set; }

        public string? ImageSource { get; set; }

        public SimpleIngredientsCategoryModelDTO IngredientsCategory { get; set; } = new();
    }
}
