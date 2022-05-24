using Next2.Interfaces;
using Prism.Mvvm;
using System.Windows.Input;

namespace Next2.Models
{
    public class IngredientBindableModel : BindableBase, IBaseModel
    {
        public IngredientBindableModel()
        {
        }

        public IngredientBindableModel(IngredientBindableModel ingredientBindableModel)
        {
            Id = ingredientBindableModel.Id;
            IsToggled = ingredientBindableModel.IsToggled;
            Title = ingredientBindableModel.Title;
            Price = ingredientBindableModel.Price;
            ImagePath = ingredientBindableModel.ImagePath;
            IsDefault = ingredientBindableModel.IsDefault;
        }

        public int Id { get; set; }

        public int CategoryId { get; set; }

        public bool IsToggled { get; set; }

        public string Title { get; set; }

        public decimal Price { get; set; }

        public string ImagePath { get; set; }

        public bool IsDefault { get; set; }

        public ICommand ChangingToggle { get; set; }
    }
}
