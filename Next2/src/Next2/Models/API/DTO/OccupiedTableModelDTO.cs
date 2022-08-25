using Next2.Interfaces;
using System;
using System.Collections.Generic;

namespace Next2.Models.API.DTO
{
    public class OccupiedTableModelDTO : IBaseApiModel
    {
        public Guid Id { get; set; }

        public int Number { get; set; }

        public IEnumerable<Guid>? OrdersId { get; set; }
    }
}
