using System.Collections.Generic;

namespace Next2.Helpers.DTO
{
    public class SimpleSelectedProductModelDTO
    {
        public string Id { get; set; } = string.Empty;

        public string? Comment { get; set; }

        public SimpleProductModelDTO Product { get; set; } = new ();

        public List<OptionModelDTO>? SelectdOptions { get; set; }
    }
}
