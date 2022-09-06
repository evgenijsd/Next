using Next2.Interfaces;
using Next2.Models.API.DTO;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Next2.Models.Bindables
{
    public class DishBindableModel : BindableBase, IBaseApiModel, ICloneable
    {
        public Guid Id { get; set; }

        public Guid DishId { get; set; }

        public string? Name { get; set; }

        public string? ImageSource { get; set; }

        public decimal TotalPrice { get; set; }

        public decimal DiscountPrice { get; set; }

        public decimal SplitPrice { get; set; }

        public bool IsSplitted { get; set; }

        public DateTime? HoldTime { get; set; }

        public DishProportionModelDTO? SelectedDishProportion { get; set; } = new();

        public ObservableCollection<ProductBindableModel>? SelectedProducts { get; set; }

        public IEnumerable<DishReplacementProductModelDTO>? ReplacementProducts { get; set; }

        public int SeatNumber { get; set; }

        public bool IsSeatSelected { get; set; }

        public IEnumerable<SimpleDishProportionModelDTO>? DishProportions { get; set; }

        public ICommand? SelectDishCommand { get; set; }

        public string? SelectedProductsNames => SelectedProducts.Select(x => x.Product.Name).Aggregate((i, j) => i + ", " + j);

        public object Clone()
        {
            return new DishBindableModel()
            {
                Id = Id,
                Name = new(Name),
                DishId = DishId,
                ImageSource = new(ImageSource),
                TotalPrice = TotalPrice,
                IsSplitted = IsSplitted,
                DiscountPrice = DiscountPrice,
                SplitPrice = SplitPrice,
                HoldTime = HoldTime,
                IsSeatSelected = IsSeatSelected,
                SeatNumber = SeatNumber,
                SelectDishCommand = SelectDishCommand,
                DishProportions = DishProportions is null
                    ? null
                    : DishProportions.Select(x => (SimpleDishProportionModelDTO)x.Clone()),
                SelectedDishProportion = SelectedDishProportion is null
                    ? null
                    : (DishProportionModelDTO)SelectedDishProportion.Clone(),
                SelectedProducts = SelectedProducts is null
                    ? null
                    : new(SelectedProducts.Select(row => (ProductBindableModel)row.Clone())),
                ReplacementProducts = ReplacementProducts is null
                    ? null
                    : ReplacementProducts.Select(row => (DishReplacementProductModelDTO)row.Clone()),
            };
        }
    }
}
