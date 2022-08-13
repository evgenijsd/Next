using Next2.Interfaces;
using System;

namespace Next2.Models.API.DTO
{
    public class OccupiedTableModelDTO : IBaseApiModel
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        public int Number { get; set; }
    }
}
