using System;
using System.Collections.Generic;
using System.Linq;

namespace Next2.Models.API.DTO
{
    public class IncomingSelectedDishModel
    {
        public Guid DishId { get; set; }

        public Guid SelectedDishProportionId { get; set; }

        public decimal TotalPrice { get; set; }

        public decimal? DiscountPrice { get; set; }

        public decimal SplitPrice { get; set; }

        public DateTime? HoldTime { get; set; }

        public IEnumerable<IncomingSelectedProductModel> SelectedProducts { get; set; } = Enumerable.Empty<IncomingSelectedProductModel>();
    }
}
