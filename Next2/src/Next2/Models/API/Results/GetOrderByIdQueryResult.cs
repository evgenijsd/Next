using Next2.Models.API.DTO;

namespace Next2.Models.API.Results
{
    public class GetOrderByIdQueryResult
    {
        public OrderModelDTO Order { get; set; } = new();
    }
}
