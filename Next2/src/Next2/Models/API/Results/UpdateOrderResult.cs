using Next2.Interfaces;
using Next2.Models.API.DTO;
using System;
using System.Collections.Generic;

namespace Next2.Models.API.Results
{
    public class UpdateOrderResult : IBaseApiModel
    {
        public Guid Id { get; set; }

        public IEnumerable<SimpleSeatModelDTO>? Seats { get; set; }
    }
}
