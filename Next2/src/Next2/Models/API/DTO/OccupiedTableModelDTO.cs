using Next2.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Models.API.DTO
{
    public class OccupiedTableModelDTO : IBaseApiModel
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        public int Number { get; set; }
    }
}
