using Next2.Interfaces;

namespace Next2.Models
{
    public class ProductModel : IBaseModel
    {
        public ProductModel()
        {
        }

        public ProductModel(ProductModel product)
        {
            Id = product.Id;
            SetId = product.SetId;
            DefaultProductId = product.DefaultProductId;
            DefaultOptionId = product.DefaultOptionId;
            Title = product.Title;
            ImagePath = product.ImagePath;
            ProductPrice = product.ProductPrice;
            IngredientsPrice = product.IngredientsPrice;
            TotalPrice = product.TotalPrice;
        }

        public int Id { get; set; }

        public int SetId { get; set; }

        public int DefaultProductId { get; set; }

        public int DefaultOptionId { get; set; }

        public string Title { get; set; }

        public string ImagePath { get; set; }

        public float ProductPrice { get; set; }

        public float IngredientsPrice { get; set; }

        public float TotalPrice { get; set; }

        public string? Comment { get; set; }
    }
}
