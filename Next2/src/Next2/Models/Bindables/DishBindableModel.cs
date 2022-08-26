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
                Name = Name,
                DishId = DishId,
                ImageSource = ImageSource,
                TotalPrice = TotalPrice,
                IsSplitted = IsSplitted,
                DiscountPrice = DiscountPrice,
                SplitPrice = SplitPrice,
                HoldTime = HoldTime,
                IsSeatSelected = IsSeatSelected,
                SeatNumber = SeatNumber,
                SelectDishCommand = SelectDishCommand,
                DishProportions = DishProportions?.Select(x => x.Clone()),
                SelectedDishProportion = SelectedDishProportion?.Clone(),
                SelectedProducts = SelectedProducts,
                ReplacementProducts = ReplacementProducts,
            };
        }
    }
}
