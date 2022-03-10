using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using Next2.Resources.Strings;
using Next2.Services.Mock;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<AOResult<IEnumerable<CategoryModel>>> GetCategoriesAsync()
        {
            var result = new AOResult<IEnumerable<CategoryModel>>();

            try
            {
                var categories = await _mockService.GetAllAsync<CategoryModel>();

                if (categories is not null)
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
                result.SetError($"{nameof(GetCategoriesAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public async Task<AOResult<IEnumerable<SubcategoryModel>>> GetSubcategoriesAsync(int categoryId)
        {
            var result = new AOResult<IEnumerable<SubcategoryModel>>();

            try
            {
                var subcategories = await _mockService.GetAsync<SubcategoryModel>(row => row.CategoryId == categoryId);

                if (subcategories is not null)
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
                result.SetError($"{nameof(GetSubcategoriesAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public async Task<AOResult<IEnumerable<SetModel>>> GetSetsAsync(int categoryId, int subcategoryId)
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

                if (sets is not null)
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
                result.SetError($"{nameof(GetSetsAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public async Task<AOResult<IEnumerable<PortionModel>>> GetPortionsSetAsync(int setId)
        {
            var result = new AOResult<IEnumerable<PortionModel>>();

            try
            {
                var portions = await _mockService.GetAsync<PortionModel>(row => row.SetId == setId);

                if (portions is not null)
                {
                    result.SetSuccess(portions);
                }
                else
                {
                    result.SetFailure();
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(GetPortionsSetAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        #endregion
    }
}
