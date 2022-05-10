using System.Collections.Generic;

namespace Next2.Helpers.DTO
{
    public class SimpleProductModelDTO
    {
        public string Id { get; set; } = string.Empty;

        public string? Name { get; set; }

        public double DefaultPriceNumber { get; set; }

        public string? ImageSource { get; set; }

        public List<OptionModelDTO>? Options { get; set; }

        public List<SimpleIngredientModelDTO>? Ingredients { get; set; }
    }
}
