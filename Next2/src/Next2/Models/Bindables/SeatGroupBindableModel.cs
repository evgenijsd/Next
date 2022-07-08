using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Next2.Models.Bindables
{
    public class SeatGroupBindableModel : List<DishBindableModel>
    {
        public SeatGroupBindableModel(string seatNumber, List<DishBindableModel> dishes)
            : base(dishes)
        {
            SeatNumber = seatNumber;
        }

        public string SeatNumber { get; set; }

        public bool Checked { get; set; }
    }
}