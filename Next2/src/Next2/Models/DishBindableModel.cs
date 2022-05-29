using Next2.Interfaces;
using Next2.Models.API.DTO;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;

namespace Next2.Models
{
    public class DishBindableModel : BindableBase, IBaseApiModel
    {
        public DishBindableModel()
        {
        }

        public DishBindableModel(DishBindableModel dish)
        {
            Id = dish.Id;
            TotalPrice = dish.TotalPrice;
            DiscountPrice = dish.DiscountPrice;
            SelectedDishProportion = dish.SelectedDishProportion;
            Dish = dish.Dish;
            SelectedProducts = new();

            foreach (var selectedProduct in dish?.SelectedProducts)
            {
                SelectedProducts.Add(new ProductBindableModel(selectedProduct));
            }
        }

        public Guid Id { get; set; }

        public decimal TotalPrice { get; set; }

        public decimal DiscountPrice { get; set; }

        public DishProportionModelDTO SelectedDishProportion { get; set; } = new();

        public decimal SelectedDishProportionPrice { get; set; }

        public DishModelDTO Dish { get; set; } = new();

        public ObservableCollection<ProductBindableModel>? SelectedProducts { get; set; }
    }
}
