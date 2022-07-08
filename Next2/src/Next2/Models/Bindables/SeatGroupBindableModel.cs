using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Next2.Models.Bindables
{
    public class SeatGroupBindableModel : List<DishBindableModel>
    {
        public SeatGroupBindableModel(int seatNumber, List<DishBindableModel> dishes)
            : base(dishes)
        {
            SeatNumber = seatNumber;
        }

        public int SeatNumber { get; set; }

        public bool Checked { get; set; }

        public bool IsFirstSeat { get; set; }

        public ICommand? DishSelectionCommand { get; set; }

        public ICommand? SeatSelectionCommand { get; set; }

        public ICommand? SeatDeleteCommand { get; set; }

        public ICommand? RemoveOrderCommand { get; set; }
    }
}