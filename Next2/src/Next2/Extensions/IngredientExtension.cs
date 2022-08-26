using Next2.Models.API.DTO;
using Next2.Models.Bindables;

namespace Next2.Extensions
{
    public static class IngredientExtension
    {
        public static SimpleIngredientsCategoryModelDTO ToSimpleIngredientsCategoryModelDTO(this IngredientBindableModel ingredient)
        {
            return new()
            {
                Id = ingredient.Id,
                Name = ingredient.Name,
            };
        }

        public static SimpleIngredientModelDTO ToSimpleIngredientModelDTO(this IngredientBindableModel ingredient)
        {
            return new()
            {
                Id = ingredient.Id,
                Name = ingredient.Name,
                Price = ingredient.Price,
                ImageSource = ingredient.ImageSource,
                IngredientsCategory = ingredient.ToSimpleIngredientsCategoryModelDTO(),
            };
        }
    }
}
