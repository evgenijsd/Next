using Next2.Interfaces;
using System;
using System.Collections.Generic;

namespace Next2.Models.API.DTO
{
    public class CouponModelDTO : DiscountModelDTO, IBaseApiModel
    {
        public IEnumerable<SimpleDishModelDTO>? Dishes { get; set; }
    }
}
