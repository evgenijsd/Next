using Next2.Interfaces;
using Next2.Models.API.DTO;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Next2.Models
{
    public class DishBindableModel : BindableBase, IBaseApiModel
    {
        public DishBindableModel()
        {
        }

        public Guid Id { get; set; }

        public decimal TotalPrice { get; set; }

        public decimal DiscountPrice { get; set; }

        public DishProportionModelDTO SelectedDishProportion { get; set; } = new();

        public decimal DishPriceBaseOnSelectedProportion { get; set; }

        public DishModelDTO Dish { get; set; } = new();

        public ObservableCollection<ProductBindableModel>? SelectedProducts { get; set; }
    }
}
