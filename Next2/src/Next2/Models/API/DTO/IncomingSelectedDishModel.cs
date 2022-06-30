using System;
using System.Collections.Generic;

namespace Next2.Models.API.DTO
{
    public class IncomingSelectedDishModel
    {
        public decimal TotalPrice { get; set; }

        public decimal? DiscountPrice { get; set; }

        public Guid DishId { get; set; }

        public Guid SelectedDishProportionId { get; set; }

        public IEnumerable<IncomingSelectedProductModel> SelectedProducts { get; set; }
    }
}
