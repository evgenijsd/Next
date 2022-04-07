using Next2.Interfaces;

namespace Next2.Models
{
    public class IngredientOfProductModel : IBaseModel
    {
        public IngredientOfProductModel()
        {
        }

        public IngredientOfProductModel(IngredientOfProductModel ingredient)
        {
            Id = ingredient.Id;
            ProductId = ingredient.ProductId;
            IngredientId = ingredient.IngredientId;
            Price = ingredient.Price;
            Title = ingredient.Title;
        }

        public int Id { get; set; }

        public int ProductId { get; set; }

        public int IngredientId { get; set; }

        public float? Price { get; set; }

        public string Title { get; set; }
    }
}
