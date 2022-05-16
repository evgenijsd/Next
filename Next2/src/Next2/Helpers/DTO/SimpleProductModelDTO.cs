using System;
using System.Collections.Generic;

namespace Next2.Helpers.DTO
{
    public class SimpleProductModelDTO
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public double DefaultPriceNumber { get; set; }

        public string? ImageSource { get; set; }

        public List<OptionModelDTO>? Options { get; set; }

        public List<SimpleIngredientModelDTO>? Ingredients { get; set; }
    }
}
