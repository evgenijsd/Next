using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using Next2.Models.API.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Next2.Services.Menu
{
    public interface IMenuService
    {
        Task<AOResult<IEnumerable<CategoryModel>>> GetAllCategoriesAsync();

        Task<AOResult<IEnumerable<DishModelDTO>>> GetDishesAsync(Guid categoryId, Guid subcategoryId);

        Task<AOResult<IEnumerable<IngredientsCategoryModelDTO>>> GetIngredientCategoriesAsync();

        Task<AOResult<IEnumerable<IngredientModelDTO>>> GetIngredientsAsync();
    }
}