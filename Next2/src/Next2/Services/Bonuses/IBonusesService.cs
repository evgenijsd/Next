using Next2.Helpers.ProcessHelpers;
using Next2.Models.API.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Next2.Services.Bonuses
{
    public interface IBonusesService
    {
        Task<AOResult<IEnumerable<CouponModelDTO>>> GetCouponsAsync(Func<CouponModelDTO, bool>? condition = null);

        Task<AOResult<IEnumerable<DiscountModelDTO>>> GetDiscountsAsync(Func<DiscountModelDTO, bool>? condition = null);

        Task<AOResult<CouponModelDTO>> GetCouponById(Guid id);
    }
}
