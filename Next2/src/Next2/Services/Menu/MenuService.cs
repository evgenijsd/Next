using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using Next2.Models.API;
using Next2.Models.API.DTO;
using Next2.Models.API.Results;
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

        public async Task<AOResult<IEnumerable<CategoryModel>>> GetAllCategoriesAsync()
        {
            var result = new AOResult<IEnumerable<CategoryModel>>();

            try
            {
                var categories = await _restService.RequestAsync<Models.API.GenericExecutionResult<GetCategoriesListQueryResult>>(HttpMethod.Get, $"{Constants.API.HOST_URL}/api/categories");

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

        public async Task<AOResult<IEnumerable<DishModelDTO>>> GetDishesAsync(Guid categoryId, Guid subcategoryId)
        {
            var result = new AOResult<IEnumerable<DishModelDTO>>();

            try
            {
                var query = $"{Constants.API.HOST_URL}/api/dishes/{categoryId}.{subcategoryId}";

                var resultGettingDishes = await _restService.RequestAsync<Models.API.GenericExecutionResult<GetDishesListQueryResult>>(HttpMethod.Get, query);

                if (resultGettingDishes.Success)
                {
                    result.SetSuccess(resultGettingDishes.Value.Dishes);
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(GetDishesAsync)}: exception", Strings.SomeIssues, ex);
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

        public async Task<AOResult<IEnumerable<IngredientsCategoryModelDTO>>> GetIngredientCategoriesAsync()
        {
            var result = new AOResult<IEnumerable<IngredientsCategoryModelDTO>>();

            try
            {
                var query = $"{Constants.API.HOST_URL}/api/ingredients-categories";
                var resultData = await _restService.RequestAsync<GenericExecutionResult<GetIngredientsCategoriesListQueryResult>>(HttpMethod.Get, query);

                if (resultData.Success && resultData?.Value?.IngredientsCategories is not null)
                {
                    result.SetSuccess(resultData.Value.IngredientsCategories);
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(GetIngredientCategoriesAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public async Task<AOResult<IEnumerable<IngredientModelDTO>>> GetIngredientsAsync(Guid categoryId)
        {
            var result = new AOResult<IEnumerable<IngredientModelDTO>>();

            try
            {
                var query = $"{Constants.API.HOST_URL}/api/ingredients";
                var resultData = await _restService.RequestAsync<GenericExecutionResult<GetIngredientsListQueryResult>>(HttpMethod.Get, query);

                if (resultData.Success && resultData?.Value?.Ingredients is not null)
                {
                    result.SetSuccess(resultData.Value.Ingredients.Where(row => row.IngredientsCategoryId == categoryId));
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

        public async Task<AOResult<IEnumerable<IngredientModelDTO>>> GetIngredientsAsync()
        {
            var result = new AOResult<IEnumerable<IngredientModelDTO>>();

            try
            {
                var query = $"{Constants.API.HOST_URL}/api/ingredients";
                var resultData = await _restService.RequestAsync<GenericExecutionResult<GetIngredientsListQueryResult>>(HttpMethod.Get, query);

                if (resultData.Success && resultData?.Value?.Ingredients is not null)
                {
                    result.SetSuccess(resultData.Value.Ingredients);
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
