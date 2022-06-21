using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using Next2.Models.API.DTO;
using Next2.Models.API.Results;
using Next2.Resources.Strings;
using Next2.Services.Mock;
using Next2.Services.Order;
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
                var categories = await _restService.RequestAsync<GenericExecutionResult<GetCategoriesListQueryResult>>(HttpMethod.Get, $"{Constants.API.HOST_URL}/api/categories");

                if (categories.Success && categories.Value.Categories is not null)
                {
                    result.SetSuccess(categories.Value.Categories);
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

                var resultGettingDishes = await _restService.RequestAsync<GenericExecutionResult<GetDishesListQueryResult>>(HttpMethod.Get, query);

                if (resultGettingDishes.Success)
                {
                    result.SetSuccess(resultGettingDishes.Value.Dishes);

                    var orderService = App.Resolve<IOrderService>();
                    decimal bonusPercentage = 0;

                    if (orderService.CurrentOrder.Discount is not null || orderService.CurrentOrder.Coupon is not null)
                    {
                        bonusPercentage = orderService.CurrentOrder.Coupon is not null
                            ? orderService.CurrentOrder.Coupon.DiscountPercentage
                            : orderService.CurrentOrder.Discount.DiscountPercentage;
                    }

                    foreach (var dish in resultGettingDishes.Value.Dishes)
                    {
                        dish.OriginalPrice = dish.OriginalPrice - (dish.OriginalPrice * bonusPercentage / 100);
                    }
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

                var ingredientsCategories = await _restService.RequestAsync<GenericExecutionResult<GetIngredientsCategoriesListQueryResult>>(HttpMethod.Get, query);

                if (ingredientsCategories.Success && ingredientsCategories.Value?.IngredientsCategories is not null)
                {
                    result.SetSuccess(ingredientsCategories.Value.IngredientsCategories);
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
                var ingredients = await _restService.RequestAsync<GenericExecutionResult<GetIngredientsListQueryResult>>(HttpMethod.Get, query);

                if (ingredients.Success && ingredients.Value is not null)
                {
                    result.SetSuccess(ingredients.Value.Ingredients);
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
