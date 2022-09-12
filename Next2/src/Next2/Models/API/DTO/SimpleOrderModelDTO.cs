using Next2.Enums;
using Next2.Interfaces;
using System;

namespace Next2.Models.API.DTO
{
    public class SimpleOrderModelDTO : IBaseApiModel
    {
        public Guid Id { get; set; }

        public int Number { get; set; }

        public EOrderType OrderType { get; set; }

        public EOrderStatus OrderStatus { get; set; }

        public bool IsTab { get; set; }

        public decimal TotalPrice { get; set; }

        public int? TableNumber { get; set; }

        public DateTime? Close { get; set; }

        public CustomerNameModelDTO Customer { get; set; } = new ();
    }
}
