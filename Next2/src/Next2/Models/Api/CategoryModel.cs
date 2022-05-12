using Next2.Interfaces;
using System;
using System.Collections.Generic;

namespace Next2.Api.Models.Category
{
    public class CategoryModel : IBaseApiModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public List<SubcategoryModel>? Subcategories { get; set; } = new();
    }
}
