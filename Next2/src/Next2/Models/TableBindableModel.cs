using Next2.Interfaces;
using Prism.Mvvm;

namespace Next2.Models
{
    public class TableBindableModel : BindableBase, IBaseModel
    {
        public int Id { get; set; }

        public int Number { get; set; }

        public int NumberOfSeats { get; set; }

        public int NumberOfAvailableSeats { get; set; }
    }
}
