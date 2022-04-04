using Next2.Enums;
using Prism.Mvvm;

namespace Next2.Models
{
    public class OrderBindableModel : BindableBase
    {
        public int Id { get; set; }

        public int TableNumber { get; set; }

        public int OrderNumber { get; set; }

        private string? _name;
        public string? Name
        {
            get => _name;
            set { _name = string.IsNullOrWhiteSpace(value) ? "- - - - - - - - -" : value; }
        }

        public string? OrderStatus { get; set; }

        public EOrderType OrderType { get; set; }

        public string OrderNumberText { get; set; }

        public double Total { get; set; }

        public double Tax { get; set; }
    }
}
