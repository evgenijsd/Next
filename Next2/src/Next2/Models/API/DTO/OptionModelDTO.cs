using Next2.Interfaces;
using System;

namespace Next2.Models.API.DTO
{
    public class OptionModelDTO : IBaseApiModel
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public OptionModelDTO Clone()
        {
            return new()
            {
                Id = Id,
                Name = Name,
            };
        }
    }
}
