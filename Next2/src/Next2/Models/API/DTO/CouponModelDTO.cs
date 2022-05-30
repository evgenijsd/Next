using Next2.Interfaces;
using System;
using System.Collections.Generic;

namespace Next2.Models.API.DTO
{
    public class CouponModelDTO : SimpleCouponModelDTO
    {
        public int DiscountPercentage { get; set; }

        public bool IsActive { get; set; }

        public IEnumerable<SimpleDishModelDTO>? Dishes { get; set; }
    }
}
