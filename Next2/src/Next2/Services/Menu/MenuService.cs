using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using Next2.Services.MockService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Next2.Services.Menu
{
    public class MenuService : IMenuService
    {
        private IMockService _mockService;

        public MenuService(IMockService mockService)
        {
            _mockService = mockService;
        }

        #region -- IMenuService implementation --

        public async Task<AOResult<IEnumerable<CategoryModel>>> GetCategories()
        {
            var result = new AOResult<IEnumerable<CategoryModel>>();

            try
            {
                var categories = await _mockService.GetAllAsync<CategoryModel>();

                if (categories != null)
                {
                    result.SetSuccess(categories);
                }
                else
                {
                    result.SetFailure();
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(GetCategories)}: exception", "Some issues", ex);
            }

            return result;
        }

        public async Task<AOResult<IEnumerable<SubcategoryModel>>> GetSubcategories(int categoryId)
        {
            var result = new AOResult<IEnumerable<SubcategoryModel>>();

            try
            {
                var subcategories = await _mockService.GetAsync<SubcategoryModel>(row => row.CategoryId == categoryId);

                if (subcategories != null)
                {
                    result.SetSuccess(subcategories);
                }
                else
                {
                    result.SetFailure();
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(GetSubcategories)}: exception", "Some issues", ex);
            }

            return result;
        }

        public async Task<AOResult<IEnumerable<SetModel>>> GetSets(int categoryId, int subcategoryId = 0)
        {
            var result = new AOResult<IEnumerable<SetModel>>();

            try
            {
                IEnumerable<SetModel> sets;

                if (subcategoryId == 0)
                {
                    var subcategories = await _mockService.GetAsync<SubcategoryModel>(row => row.CategoryId == categoryId);

                    sets = await _mockService.GetAsync<SetModel>(x => subcategories.Any(row => row.Id == x.SubcategoryId));
                }
                else
                {
                    sets = await _mockService.GetAsync<SetModel>(row => row.SubcategoryId == subcategoryId);
                }

                if (sets != null)
                {
                    result.SetSuccess(sets);
                }
                else
                {
                    result.SetFailure();
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(GetSets)}: exception", "Some issues", ex);
            }

            return result;
        }

        #endregion
    }
}
