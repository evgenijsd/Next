using Next2.Models.API.DTO;
using Next2.Models.Bindables;
using System.Linq;

namespace Next2.Extensions
{
    public static class SeatExtension
    {
        public static IncomingSeatModel ToIncomingSeatModel(this SeatBindableModel seat)
        {
            return new()
            {
                Number = seat.SeatNumber,
                SelectedDishes = seat.SelectedDishes.Select(x => x.ToIncomingSelectedDishModel()),
            };
        }

        public static IncomingSeatModel ToIncomingSeatModel(this SeatModelDTO seat)
        {
            return new()
            {
                Number = seat.Number,
                SelectedDishes = seat.SelectedDishes.Select(x => x.ToIncomingSelectedDishModel()),
            };
        }

        public static SeatBindableModel ToSeatBindableModel(this SeatModelDTO seat)
        {
            return new()
            {
                SeatNumber = seat.Number,
                Checked = seat.Number == 1,
                IsFirstSeat = seat.Number == 1,
                SelectedDishes = new(seat.SelectedDishes.Select(row => row.ToDishBindableModel())),
            };
        }

        public static SeatModelDTO ToSeatModelDTO(this SeatBindableModel seat)
        {
            return new()
            {
                Number = seat.SeatNumber,
                SelectedDishes = seat.SelectedDishes?.Select(row => row.ToSelectedDishModelDTO()),
            };
        }
    }
}
