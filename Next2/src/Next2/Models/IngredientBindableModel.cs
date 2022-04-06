using Next2.Interfaces;
using Prism.Mvvm;
using System.Windows.Input;

namespace Next2.Models
{
    public class IngredientBindableModel : BindableBase, IBaseModel
    {
        public int Id { get; set; }

        public int CategoryId { get; set; }

        public bool IsToggled { get; set; }

        public string Title { get; set; }

        public float Price { get; set; }

        public string ImagePath { get; set; }

        public ICommand ChangingToggle { get; set; }
    }
}
