using Next2.Enums;
using Next2.Interfaces;
using Next2.Models.Api.DTO;
using System;
using System.Collections.Generic;

namespace Next2.Models.API.Commands
{
    public class UpdateOrderCommand : IBaseApiModel
    {
        public Guid Id { get; set; }

        public int Number { get; set; }

        public EOrderType OrderType { get; set; }

        public bool IsTab { get; set; }

        public Guid? TableId { get; set; }

        public DateTime Open { get; set; }

        public DateTime? Close { get; set; }

        public EOrderStatus OrderStatus { get; set; }

        public decimal TaxCoefficient { get; set; }

        public decimal TotalPrice { get; set; }

        public decimal? DiscountPrice { get; set; }

        public decimal? SubTotalPrice { get; set; }

        public bool IsCashPayment { get; set; }

        public Guid? CouponId { get; set; }

        public Guid? DiscountId { get; set; }

        public Guid? CustomerId { get; set; }

        public string EmployeeId { get; set; }

        public IEnumerable<IncomingSeatModel>? Seats { get; set; }
    }
}
