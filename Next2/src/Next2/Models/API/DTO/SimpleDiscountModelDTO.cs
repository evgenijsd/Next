using Next2.Interfaces;
using System;

namespace Next2.Models.API.DTO
{
    public class SimpleDiscountModelDTO : IBaseApiModel
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }
    }
}
