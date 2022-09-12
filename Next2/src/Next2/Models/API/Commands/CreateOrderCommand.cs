using Next2.Enums;
using System;

namespace Next2.Models.API.Commands
{
    public class CreateOrderCommand
    {
        public EOrderType OrderType { get; set; } = EOrderType.DineIn;

        public bool IsTab { get; set; } = true;

        public string EmployeeId { get; set; } = string.Empty;

        public Guid? TableId { get; set; }

        public Guid? CustomerId { get; set; }

        public bool IsSplitBySeats { get; set; }
    }
}
