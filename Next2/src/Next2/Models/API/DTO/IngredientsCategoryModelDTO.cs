using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Models.API.DTO
{
    public class IngredientsCategoryModelDTO
    {
        public Guid id { get; set; }

        public string? Name { get; set; }

        public List<Guid>? IngredientsId { get; set; } = new();
    }
}
