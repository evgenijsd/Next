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
            Price = product.Price;
            PriceIngredients = product.PriceIngredients;
        }

        public int Id { get; set; }

        public int SetId { get; set; }

        public int DefaultProductId { get; set; }

        public int DefaultOptionId { get; set; }

        public string Title { get; set; }

        public string ImagePath { get; set; }

        public float Price { get; set; }

        public float PriceIngredients { get; set; }
    }
}
