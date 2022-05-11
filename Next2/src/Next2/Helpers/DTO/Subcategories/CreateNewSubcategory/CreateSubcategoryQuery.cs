using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Helpers.DTO.Subcategories.CreateNewSubcategory
{
    public class CreateSubcategoryQuery
    {
        public string Name { get; set; } = string.Empty;

        public List<Guid> CategoriesId { get; set; } = new ();
    }
}
