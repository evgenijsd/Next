using Next2.Enums;
using Next2.Interfaces;
using Next2.Models.API.DTO;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;

namespace Next2.Models
{
    public class FullOrderBindableModel : BindableBase, IBaseApiModel
    {
        public Guid Id { get; set; }

        public int Number { get; set; }

        public bool IsTab { get; set; }

        public SimpleTableModelDTO Table { get; set; } = new();

        public CustomerBindableModel Customer { get; set; } = new();

        public EOrderStatus? OrderStatus { get; set; }

        public EOrderType? OrderType { get; set; }

        public SimpleDiscountModelDTO Discount { get; set; } = new();

        public SimpleCouponModelDTO Coupon { get; set; } = new();

        public decimal TaxCoefficient { get; set; }

        public decimal? SubTotalPrice { get; set; }

        public decimal? DiscountPrice { get; set; }

        public decimal PriceTax { get; set; }

        public decimal TotalPrice { get; set; }

        public string? EmployeeId { get; set; }

        public ObservableCollection<SeatBindableModel> Seats { get; set; } = new();

        public void UpdateTotalSum()
        {
            SubTotalPrice = 0;

            foreach (var seat in Seats)
            {
                foreach (var set in seat.Sets)
                {
                    set.IngredientsPrice = 0;
                    set.ProductsPrice = 0;

                    foreach (var product in set.Products)
                    {
                        set.IngredientsPrice += product.IngredientsPrice;
                        set.ProductsPrice += product.SelectedProduct.ProductPrice;
                    }

                    set.TotalPrice = set.IngredientsPrice + set.Portion.Price;

                    SubTotalPrice += set.TotalPrice;
                }
            }

            PriceTax = (decimal)SubTotalPrice * TaxCoefficient;
            TotalPrice = (decimal)SubTotalPrice + PriceTax;
        }

        public int OrderNumber { get; set; }

        public string? CustomerName { get; set; }

        public EBonusType BonusType { get; set; } = EBonusType.None;

        public BonusBindableModel Bonus { get; set; } = new();

        public TaxModel Tax { get; set; } = new();

        public float SubTotal { get; set; }

        public float PriceWithBonus { get; set; } = 0f;

        public float Total { get; set; }

        public EOrderStatus? PaymentStatus;
    }
}