using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Models.Api.DTO
{
    public class IncomingSelectedDishModel
    {
        public double TotalPrice { get; set; }
        public double? DiscountPrice { get; set; }
        public Guid DishId { get; set; }
        public Guid SelectedDishProportionId { get; set; }
        public IEnumerable<IncomingSelectedDishModel> selectedProducts { get; set; }
    }
}
