using Next2.Interfaces;
using Prism.Mvvm;

namespace Next2.Models
{
    public class SeatListItemBindableModel : BindableBase, IBaseModel
    {
        public int Id { get; set; }

        public int SeatNumber { get; set; }
    }
}
