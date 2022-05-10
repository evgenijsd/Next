namespace Next2.Helpers.DTO
{
    public class SimpleDishProportionModelDTO
    {
        public string Id { get; set; } = string.Empty;

        public double PriceRatio { get; set; }

        public string ProportionId { get; set; } = string.Empty;

        public string? ProportionName { get; set; }
    }
}
