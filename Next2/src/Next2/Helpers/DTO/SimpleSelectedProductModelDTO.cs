using System;
using System.Collections.Generic;

namespace Next2.Helpers.DTO
{
    public class SimpleSelectedProductModelDTO
    {
        public Guid Id { get; set; }

        public string? Comment { get; set; }

        public SimpleProductModelDTO Product { get; set; } = new ();

        public List<OptionModelDTO>? SelectdOptions { get; set; }
    }
}
