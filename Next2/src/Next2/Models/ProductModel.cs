using Next2.Interfaces;

namespace Next2.Models
{
    public class ProductModel : IBaseModel
    {
        public int Id { get; set; }

        public int SetId { get; set; }

        public int DefaultProductId { get; set; }

        public int DefaultOptionId { get; set; }

        public string Title { get; set; }

        public string ImagePath { get; set; }

        public float Price { get; set; }
    }
}
