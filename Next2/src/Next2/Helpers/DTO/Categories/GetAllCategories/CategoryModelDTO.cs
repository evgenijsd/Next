using Next2.Api.Models.Category;
using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Helpers.DTO.Categories.GetAllCategories
{
    public class CategoryModelDTO
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public List<SubcategoryModel>? Subcategories { get; set; } = new ();
    }
}
