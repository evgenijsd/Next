using Next2.Interfaces;
using System;

namespace Next2.Models.API.DTO
{
    public class SimpleCouponModelDTO : SimpleDiscountModelDTO, IBaseApiModel
    {
        public int SeatNumbers { get; set; }
    }
}
