using Next2.Enums;
using System;

namespace Next2.Helpers.DTO
{
    public class SimpleOrderModelDTO
    {
        public Guid Id { get; set; }

        public int Number { get; set; }

        public EOrderType OrderType { get; set; }

        public EOrderStatus OrderStatus { get; set; }

        public bool IsTab { get; set; }

        public double TotalPrice { get; set; }

        public int? TableNumber { get; set; }

        public CustomerNameModelDTO Customer { get; set; } = new ();
    }
}
