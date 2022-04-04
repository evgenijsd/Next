using Next2.Interfaces;
using Prism.Mvvm;

namespace Next2.Models
{
    public class FreeSetBindableModel : BindableBase, IBaseModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string ImagePath { get; set; }

        public float Price { get; set; }

        public float OldPrice { get; set; }

        public bool IsFree { get; set; }
    }
}
