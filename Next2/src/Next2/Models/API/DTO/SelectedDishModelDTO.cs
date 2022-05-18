using Next2.Interfaces;
using System;
using System.Collections.Generic;

namespace Next2.Models.API.DTO
{
    public class SelectedDishModelDTO : IBaseApiModel
    {
        public Guid Id { get; set; }

        public decimal TotalPrice { get; set; }

        public decimal DiscountPrice { get; set; }

        public DishProportionModelDTO SelectedDishProportion { get; set; } = new();

        public DishModelDTO Dish { get; set; } = new();

        public IEnumerable<SelectedProductModelDTO>? SelectedProducts { get; set; }
    }
}
