using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Helpers.DTO.Subcategories.GetSubcategoryById
{
    public class GetSubcategoryByIdQueryResult
    {
        public List<SubcategoryModelDTO> Subcategory { get; set; } = new();
    }
}
