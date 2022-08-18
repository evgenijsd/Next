using Next2.Interfaces;
using System;
using System.Collections.Generic;

namespace Next2.Models
{
    public class CategoryModel : IBaseApiModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public List<SubcategoryModel>? Subcategories { get; set; } = new();
    }
}
