using System.Collections.Generic;

namespace Next2.Helpers.DTO
{
    public class GetOrderListQueryResultResult
    {
        public List<SimpleOrderModelDTO> Orders { get; set; } = new ();
    }
}
