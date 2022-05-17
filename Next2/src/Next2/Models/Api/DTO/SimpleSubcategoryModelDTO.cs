using Next2.Interfaces;
using System;

namespace Next2.Models.Api.DTO
{
    public class SimpleSubcategoryModelDTO : IBaseApiModel
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }
    }
}
