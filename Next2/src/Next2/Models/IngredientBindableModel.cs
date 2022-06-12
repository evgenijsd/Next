using Next2.Interfaces;
using Prism.Mvvm;
using System;
using System.Windows.Input;

namespace Next2.Models
{
    public class IngredientBindableModel : BindableBase, IBaseApiModel
    {
        public IngredientBindableModel()
        {
        }

        public IngredientBindableModel(IngredientBindableModel ingredientBindableModel)
        {
            Id = ingredientBindableModel.Id;
            IsToggled = ingredientBindableModel.IsToggled;
            Name = ingredientBindableModel.Name;
            Price = ingredientBindableModel.Price;
            ImageSource = ingredientBindableModel.ImageSource;
            IsDefault = ingredientBindableModel.IsDefault;
        }

        public Guid Id { get; set; }

        public int CategoryId { get; set; }

        public bool IsToggled { get; set; }

        public string? Name { get; set; }

        public decimal Price { get; set; }

        public string? ImageSource { get; set; }

        public bool IsDefault { get; set; }

        public ICommand ChangingToggle { get; set; }
    }
}
