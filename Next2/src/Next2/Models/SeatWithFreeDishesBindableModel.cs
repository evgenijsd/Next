using Next2.Interfaces;
using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace Next2.Models
{
    public class SeatWithFreeDishesBindableModel : BindableBase, IBaseModel
    {
        public int Id { get; set; }

        public int SeatNumber { get; set; }

        public ObservableCollection<FreeDishBindableModel> Dishes { get; set; } = new ();
    }
}
