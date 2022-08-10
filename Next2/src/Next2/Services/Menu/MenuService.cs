using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using Next2.Models.API.DTO;
using Next2.Models.API.Results;
using Next2.Resources.Strings;
using Next2.Services.Order;
using Next2.Services.Rest;
using Next2.Services.Settings;
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
        private readonly IOrderService _orderService;

        public MenuService(
            IRestService restService,
            IOrderService orderService,
            ISettingsManager settingsManager)
        {
            _restService = restService;
            _settingsManager = settingsManager;
            _orderService = orderService;
        }

        #region -- IMenuService implementation --

        public async Task<AOResult<IEnumerable<CategoryModel>>> GetAllCategoriesAsync()
        {
            var result = new AOResult<IEnumerable<CategoryModel>>();

            try
            {
                var query = $"{Constants.API.HOST_URL}/api/categories";

                var resultOfGettingCategories = await _restService.RequestAsync<GenericExecutionResult<GetCategoriesListQueryResult>>(HttpMethod.Get, query);

                if (resultOfGettingCategories.Success && resultOfGettingCategories.Value?.Categories is not null)
                {
                    result.SetSuccess(resultOfGettingCategories.Value.Categories);
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

                var resultOfGettingDishes = await _restService.RequestAsync<GenericExecutionResult<GetDishesListQueryResult>>(HttpMethod.Get, query);

                if (resultOfGettingDishes.Success && resultOfGettingDishes.Value is not null)
                {
                    result.SetSuccess(resultOfGettingDishes.Value.Dishes);
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(GetDishesAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public async Task<AOResult<IEnumerable<IngredientsCategoryModelDTO>>> GetIngredientCategoriesAsync()
        {
            var result = new AOResult<IEnumerable<IngredientsCategoryModelDTO>>();

            try
            {
                var query = $"{Constants.API.HOST_URL}/api/ingredients-categories";

                var resultOfGettingIngredientsCategories = await _restService.RequestAsync<GenericExecutionResult<GetIngredientsCategoriesListQueryResult>>(HttpMethod.Get, query);

                if (resultOfGettingIngredientsCategories.Success && resultOfGettingIngredientsCategories.Value?.IngredientsCategories is not null)
                {
                    result.SetSuccess(resultOfGettingIngredientsCategories.Value.IngredientsCategories);
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(GetIngredientCategoriesAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public async Task<AOResult<IEnumerable<IngredientModelDTO>>> GetIngredientsAsync()
        {
            var result = new AOResult<IEnumerable<IngredientModelDTO>>();

            try
            {
                var query = $"{Constants.API.HOST_URL}/api/ingredients";

                var resultOfGettingIngredients = await _restService.RequestAsync<GenericExecutionResult<GetIngredientsListQueryResult>>(HttpMethod.Get, query);

                if (resultOfGettingIngredients.Success && resultOfGettingIngredients.Value is not null)
                {
                    var ingredients = resultOfGettingIngredients.Value.Ingredients is null
                        ? Enumerable.Empty<IngredientModelDTO>()
                        : resultOfGettingIngredients.Value.Ingredients;

                    result.SetSuccess(ingredients);
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(GetIngredientsAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public async Task<AOResult<DishModelDTO>> GetDishByIdAsync(Guid dishId)
        {
            var result = new AOResult<DishModelDTO>();

            try
            {
                var query = $"{Constants.API.HOST_URL}/api/dishes/{dishId}";

                var resultOfGettingDish = await _restService.RequestAsync<GenericExecutionResult<GetDishByIdQueryResult>>(HttpMethod.Get, query);

                if (resultOfGettingDish.Success && resultOfGettingDish.Value?.Dish is not null)
                {
                    result.SetSuccess(resultOfGettingDish.Value.Dish);
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(GetDishByIdAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        #endregion
    }
}
