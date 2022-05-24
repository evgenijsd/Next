using Next2.Enums;
using Next2.Interfaces;
using System;

namespace Next2.Models.API.Commands
{
    public class CreateOrderCommand
    {
        public EOrderType OrderType { get; set; } = EOrderType.DineIn;

        public bool IsTab { get; set; } = true;

        public string EmployeeId { get; set; }

        public Guid? TableId { get; set; }

        public Guid? CustomerId { get; set; }
    }
}
