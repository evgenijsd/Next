using Next2.Interfaces;
using System;

namespace Next2.Models.API.DTO
{
    public class SimpleIngredientModelDTO : IBaseApiModel, ICloneable
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public decimal Price { get; set; }

        public string? ImageSource { get; set; }

        public SimpleIngredientsCategoryModelDTO IngredientsCategory { get; set; } = new();

        public object Clone()
        {
            return new SimpleIngredientModelDTO()
            {
                Id = Id,
                Name = Name,
                Price = Price,
                ImageSource = ImageSource,
                IngredientsCategory = (SimpleIngredientsCategoryModelDTO)IngredientsCategory?.Clone(),
            };
        }
    }
}
