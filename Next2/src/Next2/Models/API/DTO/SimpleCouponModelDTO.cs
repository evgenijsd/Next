using Next2.Interfaces;
using System;

namespace Next2.Models.API.DTO
{
    public class SimpleCouponModelDTO : IBaseApiModel
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public int SeatNumbers { get; set; }
    }
}
