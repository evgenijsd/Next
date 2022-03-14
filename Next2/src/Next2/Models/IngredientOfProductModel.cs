using Next2.Interfaces;

namespace Next2.Models
{
    public class IngredientOfProductModel : IBaseModel
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public int IngredientId { get; set; }

        public bool IsIndispensable { get; set; }
    }
}
