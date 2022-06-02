using Next2.Interfaces;
using Next2.Models.API.DTO;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms.Internals;

namespace Next2.Models
{
    public class SetBindableModel : BindableBase, IBaseModel
    {
        public SetBindableModel()
        {
        }

        public SetBindableModel(SetBindableModel set)
        {
            Id = set.Id;
            SubcategoryId = set.SubcategoryId;
            Title = set.Title;
            Price = set.Price;
            ProductsPrice = set.ProductsPrice;
            IngredientsPrice = set.IngredientsPrice;
            TotalPrice = set.TotalPrice;
            PriceBonus = set.PriceBonus;
            ImagePath = set.ImagePath;
            Portion = new();
            Portions = new();
            Products = new();

            foreach (var portion in set.Portions)
            {
                Portions.Add(new PortionModel(portion));
            }

            var tmpPortion = Portions.FirstOrDefault(row => row.Id == set.Portion.Id);

            Portion = Portions[Portions.IndexOf(tmpPortion)];

            foreach (var product in set.Products)
            {
                Products.Add(new ProductBindableModel(product));
            }
        }

        public int Id { get; set; }

        public int SubcategoryId { get; set; }

        public string Title { get; set; }

        public decimal Price { get; set; }

        public decimal ProductsPrice { get; set; }

        public decimal IngredientsPrice { get; set; }

        public decimal TotalPrice { get; set; }

        public decimal PriceBonus { get; set; } = 0m;

        public string ImagePath { get; set; }

        public PortionModel Portion { get; set; }

        public IEnumerable<SimpleCouponModelDTO> Coupons { get; set; }

        public ObservableCollection<PortionModel> Portions { get; set; }

        public ObservableCollection<ProductBindableModel> Products { get; set; }
    }
}
