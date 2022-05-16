using System.Collections.Generic;

namespace Next2.Models.API.Results
{
    public class GetCategoriesListQueryResult
    {
        public List<CategoryModel>? Categories { get; set; } = new ();
    }
}
