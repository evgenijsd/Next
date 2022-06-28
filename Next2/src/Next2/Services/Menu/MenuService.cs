using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using Next2.Models.API.DTO;
using Next2.Models.API.Results;
using Next2.Models.Bindables;
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
        private readonly IOrderService _orderService;
        private IMockService _mockService;

        public MenuService(
            IMockService mockService,
            IRestService restService,
            IOrderService orderService,
            ISettingsManager settingsManager)
        {
            _mockService = mockService;
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

                    if (_orderService.CurrentOrder.Discount is not null || _orderService.CurrentOrder.Coupon is not null)
                    {
                        decimal bonusPercentage = 0;

                        bonusPercentage = _orderService.CurrentOrder.Coupon is not null
                            ? _orderService.CurrentOrder.Coupon.DiscountPercentage
                            : _orderService.CurrentOrder.Discount.DiscountPercentage;

                        decimal percentage = bonusPercentage / Convert.ToDecimal(100);

                        var dishes = resultGettingDishes.Value.Dishes;

                        if (_orderService.CurrentOrder.Coupon is not null)
                        {
                            var couponDishes = _orderService.CurrentOrder.Coupon.Dishes;

                            if (couponDishes is not null)
                            {
                                foreach (var dish in dishes)
                                {
                                    if (couponDishes.Any(x => x.Id == dish.Id))
                                    {
                                        dish.OriginalPrice -= dish.OriginalPrice * percentage;
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (var dish in dishes)
                            {
                                dish.OriginalPrice -= dish.OriginalPrice * percentage;
                            }
                        }
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

        public async Task<AOResult<DishModelDTO>> GetDishByIdAsync(Guid dishId)
        {
            var result = new AOResult<DishModelDTO>();

            try
            {
                var query = $"{Constants.API.HOST_URL}/api/dishes/{dishId}";
                var dishResult = await _restService.RequestAsync<GenericExecutionResult<GetDishByIdQueryResult>>(HttpMethod.Get, query);

                if (dishResult.Success && dishResult?.Value?.Dish is not null)
                {
                    result.SetSuccess(dishResult.Value.Dish);
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
