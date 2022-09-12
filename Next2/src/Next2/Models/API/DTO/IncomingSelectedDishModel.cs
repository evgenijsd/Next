using System;
using System.Collections.Generic;
using System.Linq;

namespace Next2.Models.API.DTO
{
    public class IncomingSelectedDishModel
    {
        public decimal TotalPrice { get; set; }

        public decimal? DiscountPrice { get; set; }

        public decimal SplitPrice { get; set; }

        public Guid? RewardId { get; set; }

        public int? Points { get; set; }

        public DateTime? HoldTime { get; set; }

        public Guid DishId { get; set; }

        public Guid SelectedDishProportionId { get; set; }

        public IEnumerable<IncomingSelectedProductModel> SelectedProducts { get; set; } = Enumerable.Empty<IncomingSelectedProductModel>();
    }
}
