using Next2.Interfaces;
using System.Collections.Generic;

namespace Next2.Models
{
    public class SeatModel : IBaseModel
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int SeatNumber { get; set; }

        public IList<SetModel> Sets { get; set; } = new List<SetModel>();
    }
}