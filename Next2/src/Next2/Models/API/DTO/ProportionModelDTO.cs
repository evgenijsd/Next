using Next2.Interfaces;
using System;

namespace Next2.Models.API.DTO
{
    public class ProportionModelDTO : IBaseApiModel, ICloneable
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public object Clone()
        {
            return new ProportionModelDTO()
            {
                Id = Id,
                Name = Name,
            };
        }
    }
}
