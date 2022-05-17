using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using Next2.Models.Api;
using Next2.Models.Api.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Next2.Services.Menu
{
    public interface IMenuService
    {
        Task<AOResult<IEnumerable<CategoryModel>>> GetAllCategoriesAsync();

        Task<AOResult<IEnumerable<DishModelDTO>>> GetDishesAsync(Guid categoryId, Guid subcategoryId);

        Task<AOResult<IEnumerable<PortionModel>>> GetPortionsSetAsync(int setId);

        Task<AOResult<IEnumerable<IngredientCategoryModel>>> GetIngredientCategoriesAsync();

        Task<AOResult<IEnumerable<IngredientModel>>> GetIngredientsAsync(int categoryId);

        Task<AOResult<IEnumerable<IngredientOfProductModel>>> GetIngredientsOfProductAsync(int productId);

        //Task<AOResult<IEnumerable<OptionModel>>> GetOptionsOfProductAsync(int productId);
        Task<AOResult<IEnumerable<IngredientModel>>> GetIngredientsAsync();
    }
}