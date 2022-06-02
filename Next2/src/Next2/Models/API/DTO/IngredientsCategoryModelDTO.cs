using Next2.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Models.API.DTO
{
    public class IngredientsCategoryModelDTO : IBaseApiModel
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public List<Guid>? IngredientsId { get; set; } = new();
    }
}
