using Next2.Interfaces;
using System;

namespace Next2.Models.API.DTO
{
    public class SimpleDishProportionModelDTO : IBaseApiModel
    {
        public Guid Id { get; set; }

        public decimal PriceRatio { get; set; }

        public Guid ProportionId { get; set; }

        public string? ProportionName { get; set; }

        public SimpleDishProportionModelDTO Clone()
        {
            return new()
            {
                Id = Id,
                PriceRatio = PriceRatio,
                ProportionId = ProportionId,
                ProportionName = ProportionName,
            };
        }
    }
}
