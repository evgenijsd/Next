using Next2.Enums;
using Next2.Interfaces;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Linq;

namespace Next2.Models
{
    public class FullOrderBindableModel : BindableBase, IBaseModel
    {
        public FullOrderBindableModel()
        {
        }

        public FullOrderBindableModel(FullOrderBindableModel order)
        {
            Id = order.Id;
            OrderNumber = order.OrderNumber;
            Table = order.Table;
            Customer = order.Customer;
            CustomerName = order.CustomerName;
            OrderStatus = order.OrderStatus;
            OrderType = order.OrderType;
            Bonus = order.Bonus;
            BonusType = order.BonusType;
            SubTotal = order.SubTotal;
            Tax = order.Tax;
            PriceWithBonus = order.PriceWithBonus;
            PriceTax = order.PriceTax;
            Total = order.Total;
            Seats = new();

            foreach (var seat in order.Seats)
            {
                Seats.Add(new SeatBindableModel(seat));
            }
        }

        public void UpdateTotalSum()
        {
            SubTotal = 0;

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

                    SubTotal += set.TotalPrice;
                }
            }

            PriceTax = SubTotal * Tax.Value;
            Total = SubTotal + PriceTax;
        }

        public int Id { get; set; }

        public int OrderNumber { get; set; }

        public TableBindableModel Table { get; set; } = new();

        public CustomerModel? Customer { get; set; }

        public string? CustomerName { get; set; }

        public string OrderStatus { get; set; } = string.Empty;

        public EOrderType OrderType { get; set; }

        public EBonusType BonusType { get; set; } = EBonusType.None;

        public BonusBindableModel Bonus { get; set; } = new();

        public TaxModel Tax { get; set; } = new();

        public decimal SubTotal { get; set; }

        public decimal PriceWithBonus { get; set; } = 0m;

        public decimal PriceTax { get; set; }

        public decimal Total { get; set; }

        public ObservableCollection<SeatBindableModel> Seats { get; set; } = new();

        public EOrderStatus? PaymentStatus;
    }
}