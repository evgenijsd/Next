using Next2.Models.API.DTO;
using System.Collections.Generic;

namespace Next2.Models.API.Results
{
    public class GetDiscountsListQueryResult
    {
        public IEnumerable<DiscountModelDTO>? Discounts { get; set; }
    }
}
