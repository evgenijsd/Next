using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Helpers.DTO.Categories
{
    public class GetCategoriesListQueryResult
    {
        public List<CategoryModelDTO>? Categories { get; set; } = new ();
    }
}
