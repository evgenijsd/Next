using Next2.Interfaces;
using Prism.Mvvm;

namespace Next2.Models
{
    public class FreeDishBindableModel : BindableBase, IBaseModel
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string ImagePath { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public bool IsFree { get; set; }

        public string ProductNames { get; set; } = string.Empty;
    }
}
