using Next2.Models.API.DTO;
using System.Collections.Generic;

namespace Next2.Helpers.DTO.Customers
{
    public class GetCustomersListQueryResult
    {
        public IEnumerable<CustomerModelDTO>? Customers { get; set; }
    }
}
