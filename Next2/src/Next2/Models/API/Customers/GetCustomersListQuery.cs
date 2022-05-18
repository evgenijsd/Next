using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Helpers.DTO.Customers
{
    public class GetCustomersListQuery
    {
        public IEnumerable<CustomerModelDTO>? Customers { get; set; }
    }
}
