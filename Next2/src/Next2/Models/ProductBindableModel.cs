using Next2.Interfaces;
using System.Collections.ObjectModel;

namespace Next2.Models
{
    public class ProductBindableModel : IBaseModel
    {
        public ProductBindableModel()
        {
        }

        public ProductBindableModel(ProductBindableModel product)
        {
            Id = product.Id;
            SelectedOption = new();
            SelectedProduct = new();
            Options = new();
            ReplacementProducts = new();
            Title = product.Title;
            ImagePath = product.ImagePath;
            Price = product.Price;
        }

        public int Id { get; set; }

        public OptionModel SelectedOption { get; set; }

        public ProductModel SelectedProduct { get; set; }

        public ObservableCollection<OptionModel> Options { get; set; }

        public ObservableCollection<ProductModel> ReplacementProducts { get; set; }

        public string Title { get; set; }

        public string ImagePath { get; set; }

        public float Price { get; set; }
    }
}
