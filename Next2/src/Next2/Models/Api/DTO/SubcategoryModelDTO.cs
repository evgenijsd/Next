using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Helpers.API.DTO
{
    public class SubcategoryModelDTO
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public List<Guid>? categoriesId { get; set; } = new();
    }
}
