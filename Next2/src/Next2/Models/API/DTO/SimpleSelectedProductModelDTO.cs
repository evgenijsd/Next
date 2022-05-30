using System;
using System.Collections.Generic;

namespace Next2.Models.API.DTO
{
    public class SimpleSelectedProductModelDTO
    {
        public Guid Id { get; set; }

        public string? Comment { get; set; }

        public SimpleProductModelDTO Product { get; set; } = new ();

        public IEnumerable<OptionModelDTO>? SelectdOptions { get; set; }
    }
}
