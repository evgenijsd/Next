using System;
using System.Collections.Generic;

namespace Next2.Helpers.DTO
{
    public class OrderModelDTO
    {
        public string Id { get; set; } = string.Empty;

        public string? OrderType { get; set; }

        public SimpleTableModelDTO Table { get; set; } = new ();

        public DateTime Open { get; set; }

        public DateTime? Close { get; set; }

        public string? OrderStatus { get; set; }

        public double TaxCoefficient { get; set; }

        public double TotalPrice { get; set; }

        public double? DiscountPrice { get; set; }

        public double? SubTotalPrice { get; set; }

        public bool IsCashPayment { get; set; }

        public SimpleCouponModelDTO Coupon { get; set; } = new ();

        public SimpleDiscountModelDTO Discount { get; set; } = new ();

        public string? CustomerId { get; set; }

        public string? EmployeeId { get; set; }

        public List<SeatModelDTO>? Seats { get; set; }
    }
}
