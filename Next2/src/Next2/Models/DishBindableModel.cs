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

        public DishBindableModel(DishBindableModel dish)
        {
            Id = dish.Id;
            Name = dish.Name;
            OriginalPrice = dish.OriginalPrice;
            ImageSource = dish.ImageSource;
            DefaultProductId = dish.DefaultProductId;
            Category = dish.Category;
            Subcategory = dish.Subcategory;
            Products = dish.Products;
            DishProportions = dish.DishProportions;
        }

        public Guid Id { get; set; }

        public string? Name { get; set; }

        public double OriginalPrice { get; set; }

        public string? ImageSource { get; set; }

        public Guid DefaultProductId { get; set; }

        public SimpleCategoryModelDTO Category { get; set; }

        public SimpleSubcategoryModelDTO Subcategory { get; set; }

        public API.PortionModel DishProportion { get; set; }

        public ObservableCollection<SimpleProductModelDTO>? Products { get; set; }

        public ObservableCollection<PortionModel>? DishProportions { get; set; }
    }
}
