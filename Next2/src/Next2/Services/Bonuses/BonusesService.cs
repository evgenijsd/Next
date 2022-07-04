using AutoMapper;
using Next2.Helpers.ProcessHelpers;
using Next2.Models.API.DTO;
using Next2.Models.API.Results;
using Next2.Models.Bindables;
using Next2.Resources.Strings;
using Next2.Services.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Next2.Services.Bonuses
{
    public class BonusesService : IBonusesService
    {
        private readonly IMapper _mapper;
        private readonly IRestService _restService;

        public BonusesService(
            IRestService restService,
            IMapper mapper)
        {
            _mapper = mapper;
            _restService = restService;
        }

        #region -- IBonusService implementation --

        public async Task<AOResult<CouponModelDTO>> GetCouponById(Guid id)
        {
            var result = new AOResult<CouponModelDTO>();

            try
            {
                var query = $"{Constants.API.HOST_URL}/api/coupons/{id}";

                var response = await _restService.RequestAsync<GenericExecutionResult<GetCouponByIdQueryResult>>(HttpMethod.Get, query);

                if (response.Success && response.Value is not null)
                {
                    result.SetSuccess(response.Value.Coupon);
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(GetCouponById)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public async Task<AOResult<IEnumerable<CouponModelDTO>>> GetCouponsAsync(Func<CouponModelDTO, bool>? condition = null)
        {
            var result = new AOResult<IEnumerable<CouponModelDTO>>();

            try
            {
                var query = $"{Constants.API.HOST_URL}/api/coupons";

                var response = await _restService.RequestAsync<GenericExecutionResult<GetCouponsListQueryResult>>(HttpMethod.Get, query);
                var coupons = response?.Value?.Coupons;

                if (response.Success && coupons is not null)
                {
                    result.SetSuccess(condition == null
                        ? coupons
                        : coupons.Where(condition));
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(GetCouponsAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public async Task<AOResult<IEnumerable<DiscountModelDTO>>> GetDiscountsAsync(Func<DiscountModelDTO, bool>? condition = null)
        {
            var result = new AOResult<IEnumerable<DiscountModelDTO>>();

            try
            {
                var query = $"{Constants.API.HOST_URL}/api/discounts";

                var response = await _restService.RequestAsync<GenericExecutionResult<GetDiscountsListQueryResult>>(HttpMethod.Get, query);
                var discounts = response?.Value?.Discounts;

                if (response.Success && discounts is not null)
                {
                    result.SetSuccess(condition == null
                        ? discounts
                        : discounts.Where(condition));
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(GetDiscountsAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public void ResetСalculationBonus(FullOrderBindableModel currentOrder)
        {
            var dishes = currentOrder.Seats.SelectMany(x => x.SelectedDishes);

            if (currentOrder.Coupon is not null)
            {
                decimal percentage = currentOrder.Coupon.DiscountPercentage / Convert.ToDecimal(100);

                var couponDishes = currentOrder.Coupon.Dishes;

                if (couponDishes is not null)
                {
                    foreach (var dish in dishes)
                    {
                        dish.DiscountPrice = couponDishes.Any(x => x.Id == dish.DishId)
                            ? percentage == 1 ? 0 : dish.TotalPrice / (1 - percentage)
                            : dish.TotalPrice;

                        dish.TotalPrice = dish.DiscountPrice;
                    }
                }
            }
            else if (currentOrder.Discount is not null)
            {
                decimal percentage = currentOrder.Discount.DiscountPercentage / Convert.ToDecimal(100);

                foreach (var dish in dishes)
                {
                    dish.DiscountPrice = percentage == 1
                        ? 0
                        : dish.TotalPrice / percentage == 1 ? 0 : (1 - percentage);

                    dish.TotalPrice = dish.DiscountPrice;
                }
            }

            currentOrder.DiscountPrice = dishes.Sum(x => x.DiscountPrice);
            currentOrder.SubTotalPrice = currentOrder.DiscountPrice;
            currentOrder.PriceTax = (decimal)(currentOrder.DiscountPrice * currentOrder.TaxCoefficient);
            currentOrder.TotalPrice = (decimal)(currentOrder.PriceTax + currentOrder.DiscountPrice);
        }

        public void СalculationBonus(FullOrderBindableModel currentOrder)
        {
            var dishes = currentOrder.Seats.SelectMany(x => x.SelectedDishes);

            foreach (var dish in dishes)
            {
                dish.SelectedDishProportionPrice = dish.TotalPrice;
                dish.DiscountPrice = dish.TotalPrice;
            }

            if (currentOrder.Coupon is not null)
            {
                decimal percentage = currentOrder.Coupon.DiscountPercentage / Convert.ToDecimal(100);

                var couponDishes = currentOrder.Coupon.Dishes;

                if (couponDishes is not null)
                {
                    foreach (var dish in dishes)
                    {
                        dish.DiscountPrice = couponDishes.Any(x => x.Id == dish.DishId)
                            ? dish.TotalPrice - (dish.TotalPrice * percentage)
                            : dish.TotalPrice;

                        dish.TotalPrice = dish.DiscountPrice;
                    }
                }
            }
            else if (currentOrder.Discount is not null)
            {
                decimal percentage = currentOrder.Discount.DiscountPercentage / Convert.ToDecimal(100);

                foreach (var dish in dishes)
                {
                    dish.DiscountPrice = dish.TotalPrice - (dish.TotalPrice * percentage);
                    dish.TotalPrice = dish.DiscountPrice;
                }
            }

            currentOrder.DiscountPrice = dishes.Sum(x => x.DiscountPrice);
            currentOrder.SubTotalPrice = currentOrder.DiscountPrice;
            currentOrder.PriceTax = (decimal)(currentOrder.DiscountPrice * currentOrder.TaxCoefficient);
            currentOrder.TotalPrice = (decimal)(currentOrder.PriceTax + currentOrder.DiscountPrice);
        }

        #endregion
    }
}
