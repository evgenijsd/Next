using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Helpers.DTO.Subcategories
{
    public class GetSubcategoriesListQueryResult
    {
        public List<SubcategoryModelDTO>? Subcategories { get; set; } = new();
    }
}
