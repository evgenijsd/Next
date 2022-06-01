using Next2.Interfaces;
using Prism.Mvvm;
using System;
using System.Windows.Input;

namespace Next2.Models
{
    public class IngredientBindableModel : BindableBase, IBaseApiModel
    {
        public Guid Id { get; set; }

        public Guid CategoryId { get; set; }

        public bool IsToggled { get; set; }

        public string? Title { get; set; }

        public decimal Price { get; set; }

        public string? ImagePath { get; set; }

        public bool IsDefault { get; set; }

        public ICommand ChangingToggle { get; set; }
    }
}
