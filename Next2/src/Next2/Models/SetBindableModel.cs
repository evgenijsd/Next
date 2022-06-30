using Next2.Interfaces;
using Next2.Models.API.DTO;
using Next2.Models.Bindables;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Next2.Models
{
    public class SetBindableModel : BindableBase, IBaseModel
    {
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
