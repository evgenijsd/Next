using Next2.Interfaces;
using System;
using System.Collections.Generic;

namespace Next2.Models.API.DTO
{
    public class CouponModelDTO : IBaseApiModel
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public int SeatNumber { get; set; }

        public int DiscountPercentage { get; set; }

        public bool IsActive { get; set; }

        public IEnumerable<SimpleDishModelDTO>? Dishes { get; set; }
    }
}
