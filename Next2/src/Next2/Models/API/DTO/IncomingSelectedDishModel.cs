﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Models.Api.DTO
{
    public class IncomingSelectedDishModel
    {
        public decimal TotalPrice { get; set; }
        public decimal? DiscountPrice { get; set; }
        public Guid DishId { get; set; }
        public Guid SelectedDishProportionId { get; set; }
        public IEnumerable<IncomingSelectedProductModel> selectedProducts { get; set; }
    }
}
