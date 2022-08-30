using Next2.Interfaces;
using System;

namespace Next2.Models.API.DTO
{
    public class HoldItemModelDTO : IBaseApiModel
    {
        public Guid Id { get; set; }

        public int TableNumber { get; set; }

        public string? Name { get; set; }

        public DateTime ReleaseTime { get; set; }

        public Guid OrderId { get; set; }
    }
}
