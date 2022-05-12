using Next2.Helpers.DTO;
using Next2.Helpers.DTO.Categories;
using Next2.Helpers.DTO.Subcategories;
using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using Next2.Resources.Strings;
using Next2.Services.Mock;
using Next2.Services.Rest;
using Next2.Services.SettingsService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Next2.Services.Menu
{
    public class MenuService : IMenuService
    {
        private readonly IRestService _restService;
        private readonly ISettingsManager _settingsManager;
        private IMockService _mockService;

        public MenuService(
            IMockService mockService,
            IRestService restService,
            ISettingsManager settingsManager)
        {
            _mockService = mockService;
            _restService = restService;
            _settingsManager = settingsManager;
        }

        #region -- IMenuService implementation --

        public async Task<AOResult<IEnumerable<CategoryModelDTO>>> GetAllCategoriesAsync()
        {
            var result = new AOResult<IEnumerable<CategoryModelDTO>>();

            try
            {
                var categories = await _restService.RequestAsync<GenericExecutionResult<GetCategoriesListQueryResult>>(HttpMethod.Get, $"{Constants.API.HOST_URL}/api/categories");

                if (categories.Success && categories.Value.Categories is not null)
                {
                    result.SetSuccess(categories.Value.Categories);
                }
                else
                {
                    result.SetFailure();
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(GetAllCategoriesAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public async Task<AOResult<IEnumerable<SubcategoryModelDTO>>> GetAllSubcategoriesAsync()
        {
            var result = new AOResult<IEnumerable<SubcategoryModelDTO>>();

            try
            {
                var subcategories = await _restService.RequestAsync<GenericExecutionResult<GetSubcategoriesListQueryResult>>(HttpMethod.Get, $"{Constants.API.HOST_URL}/api/subcategories");

                if (subcategories.Success && subcategories is not null)
                {
                    result.SetSuccess(subcategories.Value.Subcategories);
                }
                else
                {
                    result.SetFailure();
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(GetAllSubcategoriesAsync)}: exception", Strings.SomeIssues, ex);
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

        public async Task<AOResult<IEnumerable<IngredientCategoryModel>>> GetIngredientCategoriesAsync()
        {
            var result = new AOResult<IEnumerable<IngredientCategoryModel>>();

            try
            {
                var resultData = await _mockService.GetAllAsync<IngredientCategoryModel>();

                if (resultData is not null)
                {
                    result.SetSuccess(resultData);
                }
                else
                {
                    result.SetFailure();
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(GetIngredientCategoriesAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public async Task<AOResult<IEnumerable<IngredientModel>>> GetIngredientsAsync(int categoryId)
        {
            var result = new AOResult<IEnumerable<IngredientModel>>();

            try
            {
                var resultData = await _mockService.GetAsync<IngredientModel>(row => row.CategoryId == categoryId);

                if (resultData is not null)
                {
                    result.SetSuccess(resultData);
                }
                else
                {
                    result.SetFailure();
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(GetIngredientsAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public async Task<AOResult<IEnumerable<IngredientOfProductModel>>> GetIngredientsOfProductAsync(int productId)
        {
            var result = new AOResult<IEnumerable<IngredientOfProductModel>>();

            try
            {
                var resultData = await _mockService.GetAsync<IngredientOfProductModel>(row => row.ProductId == productId);

                if (resultData is not null)
                {
                    result.SetSuccess(resultData);
                }
                else
                {
                    result.SetFailure();
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(GetIngredientsOfProductAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public async Task<AOResult<IEnumerable<OptionModel>>> GetOptionsOfProductAsync(int productId)
        {
            var result = new AOResult<IEnumerable<OptionModel>>();

            try
            {
                var resultData = await _mockService.GetAsync<OptionModel>(row => row.ProductId == productId);

                if (resultData is not null)
                {
                    result.SetSuccess(resultData);
                }
                else
                {
                    result.SetFailure();
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(GetOptionsOfProductAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public async Task<AOResult<IEnumerable<IngredientModel>>> GetIngredientsAsync()
        {
            var result = new AOResult<IEnumerable<IngredientModel>>();

            try
            {
                var resultData = await _mockService.GetAllAsync<IngredientModel>();

                if (resultData is not null)
                {
                    result.SetSuccess(resultData);
                }
                else
                {
                    result.SetFailure();
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(GetIngredientsAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        #endregion
    }
}
