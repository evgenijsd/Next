using Next2.Interfaces;
using System;

namespace Next2.Models.API.DTO
{
    public class SimpleIngredientsCategoryModelDTO : IBaseApiModel
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public SimpleIngredientsCategoryModelDTO Clone()
        {
            return new()
            {
                Id = Id,
                Name = Name,
            };
        }
    }
}
