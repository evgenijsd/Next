﻿using Next2.Helpers.DTO.Categories.GetAllCategories;
using Next2.Helpers.DTO.Subcategories;
using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using Next2.Models.Api;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Next2.Services.Menu
{
    public interface IMenuService
    {
        Task<AOResult<IEnumerable<CategoryModel>>> GetAllCategoriesAsync();

        Task<AOResult<SubcategoryModelDTO>> GetSubcategoryByIdAsync(Guid categoryId);

        //Task<AOResult<IEnumerable<SetModel>>> GetSetsAsync(int categoryId, int subcategoryId);
        Task<AOResult<IEnumerable<PortionModel>>> GetPortionsSetAsync(int setId);

        Task<AOResult<IEnumerable<IngredientCategoryModel>>> GetIngredientCategoriesAsync();

        Task<AOResult<IEnumerable<IngredientModel>>> GetIngredientsAsync(int categoryId);

        Task<AOResult<IEnumerable<IngredientOfProductModel>>> GetIngredientsOfProductAsync(int productId);

        //Task<AOResult<IEnumerable<OptionModel>>> GetOptionsOfProductAsync(int productId);
        Task<AOResult<IEnumerable<IngredientModel>>> GetIngredientsAsync();
    }
}