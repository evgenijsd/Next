using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Helpers.DTO.Subcategories.GetAllSubcategories
{
    public class GetSubcategoriesListQueryResult
    {
        public List<SubcategoryModelDTO>? Subcategories { get; set; } = new();
    }
}
