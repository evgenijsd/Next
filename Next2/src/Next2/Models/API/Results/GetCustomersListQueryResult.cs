using Next2.Models.API.DTO;
using System.Collections.Generic;

namespace Next2.Models.API.Results
{
    public class GetCustomersListQueryResult
    {
        public IEnumerable<CustomerModelDTO>? Customers { get; set; }
    }
}
