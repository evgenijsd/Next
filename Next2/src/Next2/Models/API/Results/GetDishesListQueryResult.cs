using Next2.Models.Api.DTO;
using System.Collections.Generic;

namespace Next2.Models.Api.Results
{
    public class GetDishesListQueryResult
    {
        public IEnumerable<DishModelDTO> Dishes { get; set; }
    }
}
