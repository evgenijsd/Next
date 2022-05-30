using Next2.Interfaces;
using System;
using System.Collections.Generic;

namespace Next2.Models.API.DTO
{
    public class SubcategoryModelDTO : IBaseApiModel
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public IEnumerable<Guid>? categoriesId { get; set; }
    }
}
