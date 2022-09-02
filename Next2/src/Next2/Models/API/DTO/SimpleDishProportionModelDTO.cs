using Next2.Interfaces;
using System;

namespace Next2.Models.API.DTO
{
    public class SimpleDishProportionModelDTO : IBaseApiModel, ICloneable
    {
        public Guid Id { get; set; }

        public decimal PriceRatio { get; set; }

        public Guid ProportionId { get; set; }

        public string? ProportionName { get; set; }

        public object Clone()
        {
            return new SimpleDishProportionModelDTO()
            {
                Id = Id,
                PriceRatio = PriceRatio,
                ProportionId = ProportionId,
                ProportionName = new(ProportionName),
            };
        }
    }
}
