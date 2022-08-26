using Next2.Interfaces;
using System;

namespace Next2.Models.API.DTO
{
    public class ProportionModelDTO : IBaseApiModel
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public ProportionModelDTO Clone()
        {
            return new()
            {
                Id = Id,
                Name = Name,
            };
        }
    }
}
