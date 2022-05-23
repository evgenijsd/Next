using System;

namespace Next2.Helpers.DTO
{
    public class SimpleDishProportionModelDTO
    {
        public Guid Id { get; set; }

        public double PriceRatio { get; set; }

        public string ProportionId { get; set; } = string.Empty;

        public string? ProportionName { get; set; }
    }
}
