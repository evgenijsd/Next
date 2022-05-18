using Next2.Enums;
using Next2.Interfaces;
using Next2.Models.Api.DTO;
using System;
using System.Collections.Generic;

namespace Next2.Models.Api.Commands
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
        public double TaxCoefficient { get; set; }
        public double TotalPrice { get; set; }
        public double? DiscountPrice { get; set; }
        public double? SubTotalPrice { get; set; }
        public bool IsCashPayment { get; set; }
        public Guid? CouponId { get; set; }
        public Guid? DiscountId { get; set; }
        public Guid? CustomerId { get; set; }
        public string EmployeeId { get; set; }
        public IEnumerable<IncomingSeatModel>? Seats { get; set; }
    }
}
