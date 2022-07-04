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
                var response = await _restService.RequestAsync<GenericExecutionResult<GetCouponByIdQueryResult>>(HttpMethod.Get, $"{Constants.API.HOST_URL}/api/coupons/{id}");

                if (response.Success)
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
                var response = await _restService.RequestAsync<GenericExecutionResult<GetCouponsListQueryResult>>(HttpMethod.Get, $"{Constants.API.HOST_URL}/api/coupons");
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
                var response = await _restService.RequestAsync<GenericExecutionResult<GetDiscountsListQueryResult>>(HttpMethod.Get, $"{Constants.API.HOST_URL}/api/discounts");
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

        #endregion
    }
}
