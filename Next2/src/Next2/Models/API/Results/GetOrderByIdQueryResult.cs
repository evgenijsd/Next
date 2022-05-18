using Next2.Models.API.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Models.API.Results
{
    public class GetOrderByIdQueryResult
    {
        public OrderModelDTO Order { get; set; } = new();
    }
}
