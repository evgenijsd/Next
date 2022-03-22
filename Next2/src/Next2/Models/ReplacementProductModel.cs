using Next2.Interfaces;

namespace Next2.Models
{
    public class ReplacementProductModel : IBaseModel
    {
        public int Id { get; set; }

        public int ReplacementProductId { get; set; }

        public int ProductId { get; set; }
    }
}
