using Next2.Models.API.DTO;
using System.Collections.Generic;

namespace Next2.Models.API.Results
{
    public class GetCouponsListQueryResult
    {
        public IEnumerable<CouponModelDTO>? Coupons { get; set; }
    }
}
