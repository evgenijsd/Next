using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Helpers.DTO.Categories
{
    public class CategoryModelDTO
    {
        public string Id { get; set; }

        public string? Name { get; set; }

        public List<SimpleSubcategoryModelDTO>? Subcategories { get; set; } = new ();
    }
}
