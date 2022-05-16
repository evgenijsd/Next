using Next2.Enums;
using System;
using System.Collections.Generic;

namespace Next2.Helpers.DTO
{
    public class OrderModelDTO
    {
        public Guid Id { get; set; }

        public int Number { get; set; }

        public string? OrderType { get; set; }

        public bool IsTab { get; set; }

        public SimpleTableModelDTO Table { get; set; } = new ();

        public DateTime Open { get; set; }

        public DateTime? Close { get; set; }

        public EOrderStatus OrderStatus { get; set; }

        public double TaxCoefficient { get; set; }

        public double TotalPrice { get; set; }

        public double? DiscountPrice { get; set; }

        public double? SubTotalPrice { get; set; }

        public bool IsCashPayment { get; set; }

        public SimpleCouponModelDTO Coupon { get; set; } = new ();

        public SimpleDiscountModelDTO Discount { get; set; } = new ();

        public SimpleCustomerModelDTO Customer { get; set; } = new ();

        public string? EmployeeId { get; set; }

        public List<SeatModelDTO>? Seats { get; set; }
    }
}
