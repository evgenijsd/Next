using System.Collections.Generic;
using System.Windows.Input;

namespace Next2.Models.Bindables
{
    public class DishesGroupedBySeat : List<DishBindableModel>
    {
        public DishesGroupedBySeat(int seatNumber, List<DishBindableModel> dishes)
            : base(dishes)
        {
            SeatNumber = seatNumber;
        }

        public int SeatNumber { get; set; }

        public bool Checked { get; set; }

        public bool IsFirstSeat { get; set; }

        public ICommand? SelectSeatCommand { get; set; }

        public ICommand? DeleteSeatCommand { get; set; }

        public ICommand? RemoveOrderCommand { get; set; }
    }
}