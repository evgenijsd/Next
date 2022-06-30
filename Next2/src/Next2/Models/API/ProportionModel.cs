using Next2.Interfaces;
using System;

namespace Next2.Models.API
{
    public class ProportionModel : IBaseApiModel
    {
        public Guid Id { get; set; }

        public Guid ProportionId { get; set; }

        public decimal PriceRatio { get; set; }

        public string? ProportionName { get; set; }

        public decimal Price { get; set; }
    }
}
