using Next2.Interfaces;
using System;
using System.Windows.Input;

namespace Next2.Models.Bindables
{
    public class ReservationBindableModel : IBaseModel, ITappable
    {
        public int Id { get; set; }

        public string CustomerName { get; set; }

        public string Phone { get; set; }

        public int GuestsAmount { get; set; }

        public int TableNumber { get; set; }

        public string? Comment { get; set; }

        public DateTime DateTime { get; set; }

        public ICommand? TapCommand { get; set; }
    }
}
