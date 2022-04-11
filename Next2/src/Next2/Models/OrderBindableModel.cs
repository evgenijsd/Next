using Next2.Enums;
using Prism.Mvvm;

namespace Next2.Models
{
    public class OrderBindableModel : BindableBase
    {
        public int Id { get; set; }

        public int TableNumber { get; set; }

        public int OrderNumber { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? OrderStatus { get; set; }

        public EOrderType OrderType { get; set; }

        public BonusBindableModel Bonus { get; set; } = new();

        public string OrderNumberText { get; set; } = string.Empty;

        public double Total { get; set; }

        public double PriceTax { get; set; }
    }
}
