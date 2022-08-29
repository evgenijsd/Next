using Next2.Interfaces;
using System;

namespace Next2.Models.API.DTO
{
    public class DishProportionModelDTO : IBaseApiModel
    {
        public Guid Id { get; set; }

        public decimal PriceRatio { get; set; }

        public ProportionModelDTO Proportion { get; set; } = new();

        public DishProportionModelDTO Clone()
        {
            return new()
            {
                Id = Id,
                PriceRatio = PriceRatio,
                Proportion = Proportion?.Clone(),
            };
        }
    }
}
