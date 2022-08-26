using Next2.Models.API.DTO;
using Next2.Models.Bindables;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
                SelectedDishes = new(seat.SelectedDishes.Select(row => row.ToDishBindableModel())),
            };
        }

        public static IEnumerable<SeatBindableModel> ToSeatsBindableModels(this IEnumerable<SeatModelDTO> seats)
        {
            return seats.Select(x => new SeatBindableModel()
            {
                IsFirstSeat = x.Id == seats.First().Id,
                SeatNumber = x.Number,
                SelectedDishes = new(x.SelectedDishes.Select(y => y.ToDishBindableModel())),
            });
        }

        public static IEnumerable<SeatModelDTO>? ToSeatsModelsDTO(this IEnumerable<SeatBindableModel> seats)
        {
            return seats.Select(x => x.ToSeatModelDTO());
        }

        public static SeatModelDTO ToSeatModelDTO(this SeatBindableModel seat)
        {
            return new()
            {
                Number = seat.SeatNumber,
                SelectedDishes = seat.SelectedDishes?.Select(y => new SelectedDishModelDTO
                {
                    Id = y.Id,
                    DiscountPrice = y.DiscountPrice,
                    DishId = y.DishId,
                    ImageSource = y.ImageSource,
                    Name = y.Name,
                    IsSplitted = y.IsSplitted,
                    SplitPrice = y.SplitPrice,
                    HoldTime = y.HoldTime,
                    TotalPrice = y.TotalPrice,
                    SelectedDishProportion = y.SelectedDishProportion.Clone(),
                    SelectedProducts = y.SelectedProducts?.Select(x => x.ToSelectedProductModelDTO()),
                }),
            };
        }
    }
}
