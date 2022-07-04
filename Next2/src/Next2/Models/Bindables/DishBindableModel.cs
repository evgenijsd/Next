using Next2.Interfaces;
using Next2.Models.API.DTO;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Next2.Models.Bindables
{
    public class DishBindableModel : BindableBase, IBaseApiModel
    {
        public Guid Id { get; set; }

        public Guid DishId { get; set; }

        public string? Name { get; set; }

        public string? ImageSource { get; set; }

        public decimal TotalPrice { get; set; }

        public decimal DiscountPrice { get; set; }

        public IEnumerable<SimpleDishProportionModelDTO>? DishProportions { get; set; }

        public DishProportionModelDTO SelectedDishProportion { get; set; } = new();

        public ObservableCollection<SimpleProductModelDTO>? Products { get; set; } = new ();

        public ObservableCollection<ProductBindableModel>? SelectedProducts { get; set; }
    }
}
