using System.Collections.Generic;

namespace Next2.Helpers.DTO
{
    public class SimpleSelectedDishModelDTO
    {
        public string Id { get; set; } = string.Empty;

        public string? Comment { get; set; }

        public double TotalPrice { get; set; }

        public string DishId { get; set; } = string.Empty;

        public SimpleDishProportionModelDTO SelectedProportion { get; set; } = new ();

        public List<SimpleSelectedProductModelDTO>? SelectedProducts { get; set; }
    }
}
