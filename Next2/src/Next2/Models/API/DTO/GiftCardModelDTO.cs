using Next2.Interfaces;
using System;

namespace Next2.Models.API.DTO
{
    public class GiftCardModelDTO : IBaseApiModel
    {
        public Guid Id { get; set; }

        public string? GiftCardNumber { get; set; }

        public DateTime Expire { get; set; }

        public decimal TotalBalance { get; set; }

        public Guid? CustomerId { get; set; }
    }
}
