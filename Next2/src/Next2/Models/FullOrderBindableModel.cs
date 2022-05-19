using Next2.Enums;
using Next2.Interfaces;
using Next2.Models.API.DTO;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;

namespace Next2.Models
{
    public class FullOrderBindableModelDTO : BindableBase, IBaseApiModel
    {
        public Guid Id { get; set; } //public int Id { get; set; }

        public int Number { get; set; } //public int OrderNumber { get; set; }

        public SimpleTableModelDTO Table { get; set; } = new();  //public TableBindableModel Table { get; set; } = new();

        public SimpleCustomerModelDTO Customer { get; set; } = new(); //public CustomerModel? Customer { get; set; }

        //public string? CustomerName { get; set; }
        public EOrderStatus? OrderStatus { get; set; } //public string OrderStatus { get; set; } = string.Empty; public EOrderStatus? PaymentStatus;

        public EOrderType? OrderType { get; set; } //public EOrderType OrderType { get; set; }

        //public EBonusType BonusType { get; set; } = EBonusType.None;
        public SimpleDiscountModelDTO Discount { get; set; } = new(); //public BonusBindableModel Bonus { get; set; } = new();
        public SimpleCouponModelDTO Coupon { get; set; } = new();
        public decimal TaxCoefficient { get; set; } //public TaxModel Tax { get; set; } = new();

        public decimal? SubTotalPrice { get; set; } // public decimal SubTotal { get; set; }

        public decimal? DiscountPrice { get; set; } //public decimal PriceWithBonus { get; set; } = 0m;

        public decimal PriceTax { get; set; }

        public decimal TotalPrice { get; set; } //public decimal Total { get; set; }

        public string? EmployeeId { get; set; }

        public ObservableCollection<SeatBindableModel> Seats { get; set; } = new();

        /* public Guid Id { get; set; }

        public int Number { get; set; }

        public string? OrderType { get; set; }

        public bool IsTab { get; set; }

        public SimpleTableModelDTO Table { get; set; } = new();

        public DateTime Open { get; set; }

        public DateTime? Close { get; set; }

        public string? OrderStatus { get; set; }

        public decimal TaxCoefficient { get; set; }

        public decimal TotalPrice { get; set; }

        public decimal? DiscountPrice { get; set; }

        public decimal? SubTotalPrice { get; set; }

        public bool IsCashPayment { get; set; }

        public SimpleDiscountModelDTO Discount { get; set; } = new();

        public SimpleCouponModelDTO Coupon { get; set; } = new();

        public SimpleCustomerModelDTO Customer { get; set; } = new();

        public string? EmployeeId { get; set; }

        public IEnumerable<SeatModelDTO>? Seats { get; set; } */
    }
}