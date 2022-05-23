﻿using Next2.Interfaces;
using System;
using System.Collections.Generic;

namespace Next2.Models.API.DTO
{
    public class DishModelDTO : IBaseApiModel
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public double OriginalPrice { get; set; }

        public string? ImageSource { get; set; }

        public Guid DefaultProductId { get; set; }

        public SimpleCategoryModelDTO Category { get; set; }

        public SimpleSubcategoryModelDTO Subcategory { get; set; }

        public IEnumerable<SimpleProductModelDTO>? Products { get; set; }

        public IEnumerable<SimpleDishProportionModelDTO>? DishProportions { get; set; }
    }
}