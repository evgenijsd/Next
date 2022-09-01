using Next2.Interfaces;
using System;

namespace Next2.Models.API.DTO
{
    public class DishProportionModelDTO : IBaseApiModel, ICloneable
    {
        public Guid Id { get; set; }

        public decimal PriceRatio { get; set; }

        public ProportionModelDTO Proportion { get; set; } = new();

        public object Clone()
        {
            return new DishProportionModelDTO()
            {
                Id = Id,
                PriceRatio = PriceRatio,
                Proportion = (ProportionModelDTO)Proportion?.Clone(),
            };
        }
    }
}
