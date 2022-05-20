using Next2.Interfaces;
using System;

namespace Next2.Models.Api.DTO
{
    public class GiftCardModelDTO : IBaseApiModel
    {
        public Guid Id { get; set; }
        public string? GiftCardNumber { get; set; }
        public DateTime Expire { get; set; }
        public double TotalBalance { get; set; }
        public Guid? CustomerId { get; set; }
    }
}
