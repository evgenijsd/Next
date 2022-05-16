using System;

namespace Next2.Helpers.DTO
{
    public class SimpleIngredientModelDTO
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public double Price { get; set; }

        public string? ImageSource { get; set; }

        public SimpleIngredientsCategoryModelDTO IngredientCategory = new ();
    }
}
