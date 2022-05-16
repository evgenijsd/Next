using Next2.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Helpers.DTO.Categories.GetAllCategories
{
    public class GetCategoriesListQueryResult
    {
        public List<CategoryModel>? Categories { get; set; } = new ();
    }
}
