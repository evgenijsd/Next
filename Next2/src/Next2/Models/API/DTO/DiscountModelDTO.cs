using Next2.Interfaces;
using System;

namespace Next2.Models.API.DTO
{
    public class DiscountModelDTO : IBaseApiModel
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public int DiscountPercentage { get; set; }

        public bool IsActive { get; set; }
    }
}
