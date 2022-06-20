using Next2.Models.API.DTO;
using System.Collections.Generic;

namespace Next2.Models.API.Results
{
    public class GetOrdersListQueryResult
    {
        public List<SimpleOrderModelDTO> Orders { get; set; } = new ();
    }
}
