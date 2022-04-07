using Next2.Interfaces;
using System.Collections.ObjectModel;
using System.Linq;

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
            SelectedIngredients = new();
            Title = product.Title;
            ImagePath = product.ImagePath;
            ProductPrice = product.ProductPrice;
            IngredientsPrice = product.IngredientsPrice;
            TotalPrice = product.TotalPrice;
            Comment = product.Comment;

            foreach (var option in product.Options)
            {
                Options.Add(new OptionModel(option));
            }

            foreach (var replacementProducts in product.ReplacementProducts)
            {
                ReplacementProducts.Add(new ProductModel(replacementProducts));
            }

            foreach (var ingredient in product.SelectedIngredients)
            {
                SelectedIngredients.Add(new IngredientOfProductModel(ingredient));
            }

            if (product.SelectedOption is not null)
            {
                var tmpOption = Options.FirstOrDefault(row => row.Id == product.SelectedOption.Id);

                SelectedOption = Options[Options.IndexOf(tmpOption)];
            }

            var tmpSelectedProduct = ReplacementProducts.FirstOrDefault(row => row.Id == product.SelectedProduct.Id);

            if (tmpSelectedProduct is not null)
            {
                SelectedProduct = ReplacementProducts[ReplacementProducts.IndexOf(tmpSelectedProduct)];
            }
            else
            {
                SelectedProduct = new(product.SelectedProduct);
            }
        }

        public int Id { get; set; }

        public OptionModel SelectedOption { get; set; }

        public ProductModel SelectedProduct { get; set; }

        public ObservableCollection<OptionModel> Options { get; set; }

        public ObservableCollection<ProductModel> ReplacementProducts { get; set; } = new();

        public ObservableCollection<IngredientOfProductModel> SelectedIngredients { get; set; } = new();

        public string Title { get; set; }

        public string ImagePath { get; set; }

        public float ProductPrice { get; set; }

        public float IngredientsPrice { get; set; }

        public float TotalPrice { get; set; }

        public string? Comment { get; set; }
    }
}
