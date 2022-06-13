using Next2.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Models.API.DTO
{
    public class IngredientModelDTO : IBaseApiModel
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public decimal Price { get; set; }

        public string? ImageSource { get; set; }

        public Guid IngredientsCategoryId { get; set; }
    }
}
