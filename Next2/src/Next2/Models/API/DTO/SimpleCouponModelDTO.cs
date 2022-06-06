using Next2.Interfaces;
using System;

namespace Next2.Models.API.DTO
{
    public class SimpleCouponModelDTO : SimpleDiscountModelDTO
    {
        public int DiscountPercentage { get; set; }

        public bool IsActive { get; set; }
    }
}
