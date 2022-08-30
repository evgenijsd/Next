using Next2.Models.API.DTO;
using System.Collections.Generic;

namespace Next2.Models.API.Results
{
    public class GetHoldItemsListQueryResult
    {
        public IEnumerable<HoldItemModelDTO>? HoldItems { get; set; }
    }
}
