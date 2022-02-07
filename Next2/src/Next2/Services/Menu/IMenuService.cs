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
        Task<AOResult<IEnumerable<CategoryModel>>> GetCategoriesAsync();

        Task<AOResult<IEnumerable<SubcategoryModel>>> GetSubcategoriesAsync(int categoryId);

        Task<AOResult<IEnumerable<SetModel>>> GetSetsAsync(int categoryId, int subcategoryId);
    }
}