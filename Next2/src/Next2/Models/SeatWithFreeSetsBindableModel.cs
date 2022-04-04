using Next2.Interfaces;
using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace Next2.Models
{
    public class SeatWithFreeSetsBindableModel : BindableBase, IBaseModel
    {
        public int Id { get; set; }

        public int SeatNumber { get; set; }

        public ObservableCollection<FreeSetBindableModel> Sets { get; set; } = new ();
    }
}
