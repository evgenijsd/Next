using Next2.Interfaces;
using Prism.Mvvm;

namespace Next2.Models
{
    public class SeatWIthBonusesBindableModel : BindableBase, IBaseModel
    {
        public int Id { get; set; }

        public string ImagePath { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public float OldPrice { get; set; }

        public float NewPrice { get; set; }
    }
}
