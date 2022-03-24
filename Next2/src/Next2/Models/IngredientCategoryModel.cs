using Next2.Interfaces;

namespace Next2.Models
{
    public class IngredientCategoryModel : IBaseModel
    {
        public int Id { get; set; }

        public string Title { get; set; }
    }
}
