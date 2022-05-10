namespace Next2.Helpers.DTO
{
    public class SimpleIngredientModelDTO
    {
        public string Id { get; set; } = string.Empty;

        public string? Name { get; set; }

        public double Price { get; set; }

        public string? ImageSource { get; set; }

        public SimpleIngredientsCategoryModelDTO IngredientCategory = new ();
    }
}
