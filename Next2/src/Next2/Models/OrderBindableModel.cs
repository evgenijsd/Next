using Next2.Enums;
using Prism.Mvvm;

namespace Next2.Models
{
    public class OrderBindableModel : BindableBase
    {
        public int TableNumber { get; set; }

        public int OrderNumber { get; set; }

        public string Name { get; set; }

        public string? OrderStatus { get; set; }

        public EOrderType OrderType { get; set; }

        public string OrderNumberText { get; set; }

        public double Total { get; set; }
    }
}
