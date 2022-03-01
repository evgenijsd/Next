using Next2.Interfaces;
using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace Next2.Models
{
    public class SeatBindableModel : BindableBase, IBaseModel
    {
        public int Id { get; set; }

        public int SeatNumber { get; set; }

        public ObservableCollection<SetBindableModel> Sets { get; set; }
    }
}