using Next2.Interfaces;
using System;

namespace Next2.Models
{
    public class GiftCardModel : IBaseApiModel
    {
        public Guid Id { get; set; }

        public string? GiftCardNumber { get; set; }

        public double TotalBalance { get; set; }

        public DateTime Expire { get; set; }

        public Guid CustomerId { get; set; }
    }
}
