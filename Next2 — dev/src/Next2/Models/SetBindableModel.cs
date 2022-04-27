﻿using Next2.Interfaces;
using Prism.Mvvm;
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

        public float Price { get; set; }

        public float ProductsPrice { get; set; }

        public float IngredientsPrice { get; set; }

        public float TotalPrice { get; set; }

        public float PriceBonus { get; set; } = 0f;

        public string ImagePath { get; set; }

        public PortionModel Portion { get; set; }

        public ObservableCollection<PortionModel> Portions { get; set; }

        public ObservableCollection<ProductBindableModel> Products { get; set; }
    }
}
