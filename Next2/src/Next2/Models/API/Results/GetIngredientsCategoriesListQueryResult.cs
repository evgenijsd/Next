using Next2.Models.API.DTO;
using System.Collections.Generic;

namespace Next2.Models.API.Results
{
    public class GetIngredientsCategoriesListQueryResult
    {
        public IEnumerable<IngredientsCategoryModelDTO>? IngredientsCategories { get; set; }
    }
}
