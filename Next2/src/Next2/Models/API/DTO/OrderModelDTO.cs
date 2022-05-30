using Next2.Enums;
using Next2.Interfaces;
using System;
using System.Collections.Generic;

namespace Next2.Models.API.DTO
{
    public class OrderModelDTO : IBaseApiModel
    {
        public Guid Id { get; set; }

        public int Number { get; set; }

        public string? OrderType { get; set; }

        public bool IsTab { get; set; }

        public SimpleTableModelDTO Table { get; set; } = new ();

        public DateTime Open { get; set; }

        public DateTime? Close { get; set; }

        public EOrderStatus OrderStatus { get; set; }

        public decimal TaxCoefficient { get; set; }

        public decimal TotalPrice { get; set; }

        public decimal? DiscountPrice { get; set; }

        public decimal? SubTotalPrice { get; set; }

        public bool IsCashPayment { get; set; }

        public SimpleCouponModelDTO Coupon { get; set; } = new ();

        public SimpleDiscountModelDTO Discount { get; set; } = new ();

        public SimpleCustomerModelDTO Customer { get; set; } = new ();

        public string? EmployeeId { get; set; }

        public IEnumerable<SeatModelDTO>? Seats { get; set; }
    }
}
