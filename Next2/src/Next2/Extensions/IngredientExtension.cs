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
                Name = new(ingredient.Name),
            };
        }

        public static SimpleIngredientModelDTO ToSimpleIngredientModelDTO(this IngredientBindableModel ingredient)
        {
            return new()
            {
                Id = ingredient.Id,
                Name = new(ingredient.Name),
                Price = ingredient.Price,
                ImageSource = new(ingredient.ImageSource),
                IngredientsCategory = ingredient.ToSimpleIngredientsCategoryModelDTO(),
            };
        }

        public static IngredientBindableModel ToIngredientBindableModel(this IngredientModelDTO ingredient)
        {
            return new()
            {
                Id = ingredient.Id,
                CategoryId = ingredient.IngredientsCategoryId,
                Name = new(ingredient.Name),
                Price = ingredient.Price,
                ImageSource = new(ingredient.ImageSource),
            };
        }
    }
}
