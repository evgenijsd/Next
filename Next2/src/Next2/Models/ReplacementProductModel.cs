using Next2.Interfaces;

namespace Next2.Models
{
    public class ReplacementProductModel : IBaseModel
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public string Title { get; set; }

        public string ImagePath { get; set; }

        public float Price { get; set; }
    }
}
