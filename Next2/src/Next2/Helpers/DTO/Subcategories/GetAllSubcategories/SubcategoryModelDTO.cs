using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Helpers.DTO.Subcategories
{
    public class SubcategoryModelDTO
    {
        public string Id { get; set; }

        public string? Name { get; set; }

        public List<Guid>? categoriesId { get; set; } = new();
    }
}
