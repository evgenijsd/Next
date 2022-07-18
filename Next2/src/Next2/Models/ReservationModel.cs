using Next2.Interfaces;
using System;

namespace Next2.Models
{
    public class ReservationModel : IBaseModel
    {
        public int Id { get; set; }

        public string CustomerName { get; set; }

        public string Phone { get; set; }

        public int GuestsAmount { get; set; }

        public int TableNumber { get; set; }

        public string? Comment { get; set; }

        public DateTime DateTime { get; set; }
    }
}
