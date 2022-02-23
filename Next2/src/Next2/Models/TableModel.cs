using Next2.Interfaces;

namespace Next2.Models
{
    public class TableModel : IBaseModel
    {
        public int Id { get; set; }

        public int NumberOfSeats { get; set; }

        public int NumberOfAvailableSeats { get; set; }
    }
}
