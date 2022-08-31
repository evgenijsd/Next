using Next2.Models.API.DTO;
using Next2.Models.Bindables;

namespace Next2.Extensions
{
    public static class ProportionExtension
    {
        public static ProportionModelDTO ToProportionModelDTO(this SimpleDishProportionModelDTO proportion)
        {
            return new()
            {
                Id = proportion.Id,
                Name = proportion.ProportionName,
            };
        }

        public static ProportionModelDTO ToProportionModelDTO(this ProportionBindableModel proportion)
        {
            return new()
            {
                Id = proportion.Id,
                Name = proportion.ProportionName,
            };
        }

        public static DishProportionModelDTO ToDishProportionModelDTO(this SimpleDishProportionModelDTO proportion)
        {
            return new()
            {
                Id = proportion.Id,
                PriceRatio = proportion.PriceRatio,
                Proportion = proportion.ToProportionModelDTO(),
            };
        }

        public static ProportionBindableModel ToProportionBindableModel(this SimpleDishProportionModelDTO proportion)
        {
            return new()
            {
                Id = proportion.Id,
                ProportionId = proportion.ProportionId,
                PriceRatio = proportion.PriceRatio,
                ProportionName = proportion.ProportionName,
            };
        }

        public static DishProportionModelDTO ToDishProportionModelDTO(this ProportionBindableModel proportion)
        {
            return new()
            {
                Id = proportion.Id,
                PriceRatio = proportion.PriceRatio,
                Proportion = proportion.ToProportionModelDTO(),
            };
        }
    }
}
