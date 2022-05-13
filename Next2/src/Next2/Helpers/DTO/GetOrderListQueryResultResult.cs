using System.Collections.Generic;

namespace Next2.Helpers.DTO
{
    public class GetOrderListQueryResultResult
    {
        public List<OrderModelDTO> Orders { get; set; } = new ();
    }
}
