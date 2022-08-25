using Next2.Interfaces;
using Next2.Models.API.DTO;
using System;

namespace Next2.Models
{
    public class ReservationModel : IBaseModel
    {
        public int Id { get; set; }

        public EmployeeModelDTO? Employee { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public int GuestsAmount { get; set; }

        public TableModelDTO? Table { get; set; }

        public string? Comment { get; set; }

        public DateTime DateTime { get; set; }
    }
}
