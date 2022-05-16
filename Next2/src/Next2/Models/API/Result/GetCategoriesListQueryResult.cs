using Next2.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Models.API.Result
{
    public class GetCategoriesListQueryResult
    {
        public List<CategoryModel>? Categories { get; set; } = new ();
    }
}
