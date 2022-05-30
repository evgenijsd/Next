using Next2.Interfaces;
using System;
using System.Collections.Generic;

namespace Next2.Models.API.DTO
{
    public class SimpleSelectedDishModelDTO : IBaseApiModel
    {
        public Guid Id { get; set; }

        public string? Comment { get; set; }

        public decimal TotalPrice { get; set; }

        public string DishId { get; set; } = string.Empty;

        public SimpleDishProportionModelDTO SelectedProportion { get; set; } = new ();

        public IEnumerable<SimpleSelectedProductModelDTO>? SelectedProducts { get; set; }
    }
}
