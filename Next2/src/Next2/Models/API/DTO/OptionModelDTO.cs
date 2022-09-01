using Next2.Interfaces;
using System;

namespace Next2.Models.API.DTO
{
    public class OptionModelDTO : IBaseApiModel, ICloneable
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public object Clone()
        {
            return new OptionModelDTO()
            {
                Id = Id,
                Name = Name,
            };
        }
    }
}
