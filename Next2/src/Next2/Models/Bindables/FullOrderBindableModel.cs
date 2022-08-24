using Next2.Enums;
using Next2.Interfaces;
using Next2.Models.API.DTO;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;

namespace Next2.Models.Bindables
{
    public class FullOrderBindableModel : BindableBase, IBaseApiModel
    {
        public Guid Id { get; set; }

        public int Number { get; set; }

        public EOrderType? OrderType { get; set; }

        public bool IsTab { get; set; }

        public SimpleTableModelDTO? Table { get; set; } = new();

        public DateTime Open { get; set; }

        public DateTime? Close { get; set; }

        public EOrderStatus? OrderStatus { get; set; }

        public decimal TaxCoefficient { get; set; }

        public decimal TotalPrice { get; set; }

        public decimal? DiscountPrice { get; set; }

        public decimal? SubTotalPrice { get; set; }

        public bool IsCashPayment { get; set; }

        public bool IsSplitBySeats { get; set; }

        public DiscountModelDTO? Discount { get; set; }

        public CouponModelDTO? Coupon { get; set; }

        public CustomerBindableModel? Customer { get; set; } = new();

        public string? EmployeeId { get; set; }

        public ObservableCollection<SeatBindableModel> Seats { get; set; } = new();

        public decimal PriceTax { get; set; }
    }
}