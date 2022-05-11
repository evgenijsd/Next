using Next2.Helpers.DTO.Categories;
using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Next2.Services.Menu
{
    public interface IMenuService
    {
        Task<AOResult<IEnumerable<CategoryModelDTO>>> GetCategoriesAsync();

        //Task<AOResult<IEnumerable<SubcategoryModel>>> GetSubcategoriesAsync(int categoryId);

        //Task<AOResult<IEnumerable<SetModel>>> GetSetsAsync(int categoryId, int subcategoryId);
        Task<AOResult<IEnumerable<PortionModel>>> GetPortionsSetAsync(int setId);

        Task<AOResult<IEnumerable<IngredientCategoryModel>>> GetIngredientCategoriesAsync();

        Task<AOResult<IEnumerable<IngredientModel>>> GetIngredientsAsync(int categoryId);

        Task<AOResult<IEnumerable<IngredientOfProductModel>>> GetIngredientsOfProductAsync(int productId);

        //Task<AOResult<IEnumerable<OptionModel>>> GetOptionsOfProductAsync(int productId);
        Task<AOResult<IEnumerable<IngredientModel>>> GetIngredientsAsync();
    }
}