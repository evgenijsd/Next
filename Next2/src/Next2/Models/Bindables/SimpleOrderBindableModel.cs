using Next2.Enums;
using Next2.Interfaces;
using Next2.Models.API.DTO;
using Prism.Mvvm;
using System;
using System.Collections.Generic;

namespace Next2.Models.Bindables
{
    public class SimpleOrderBindableModel : BindableBase, IBaseApiModel
    {
        public Guid Id { get; set; }

        public int Number { get; set; }

        public string? TableNumberOrName { get; set; }

        public EOrderType OrderType { get; set; }

        public bool IsTab { get; set; }

        public EOrderStatus OrderStatus { get; set; }

        public int TableNumber { get; set; }

        public DateTime Close { get; set; } = DateTime.Now;

        public decimal TotalPrice { get; set; }

        public List<SeatModelDTO>? Seats { get; set; }
    }
}
