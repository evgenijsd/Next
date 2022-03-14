using Next2.Interfaces;

namespace Next2.Models
{
    public class IngredientModel : IBaseModel
    {
        public int Id { get; set; }

        public int CategoryId { get; set; }

        public string Title { get; set; }

        public float Price { get; set; }

        public string ImagePath { get; set; }
    }
}
