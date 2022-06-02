using Next2.Models.API.DTO;
using System.Collections.Generic;

namespace Next2.Models.API.Results
{
    public class GetIngredientsListQueryResult
    {
        public IEnumerable<IngredientModelDTO>? Ingredients { get; set; }
    }
}
