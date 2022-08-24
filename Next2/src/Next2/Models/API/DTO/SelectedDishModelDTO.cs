using Next2.Interfaces;
using System;
using System.Collections.Generic;

namespace Next2.Models.API.DTO
{
    public class SelectedDishModelDTO : IBaseApiModel
    {
        public Guid Id { get; set; }

        public Guid DishId { get; set; }

        public string? Name { get; set; }

        public string? ImageSource { get; set; }

        public decimal TotalPrice { get; set; }

        public decimal DiscountPrice { get; set; }

        public decimal SplitPrice { get; set; }

        public bool IsSplitted { get; set; }

        public DateTime? HoldTime { get; set; }

        public DishProportionModelDTO SelectedDishProportion { get; set; } = new();

        public IEnumerable<SelectedProductModelDTO>? SelectedProducts { get; set; }
    }
}
