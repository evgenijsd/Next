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
        Task<AOResult<IEnumerable<CategoryModel>>> GetCategories();

        Task<AOResult<IEnumerable<SubcategoryModel>>> GetSubcategories(int categoryId);

        Task<AOResult<IEnumerable<SetModel>>> GetSets(int categoryId, int subcategoryId = 0);
    }
}