using Next2.Models.API.DTO;
using Next2.Models.Bindables;
using System.Collections.ObjectModel;
using System.Linq;

namespace Next2.Extensions
{
    public static class SeatExtension
    {
        public static SeatBindableModel ToSeatBindableModel(this SeatModelDTO seat) =>
            new SeatBindableModel
            {
                SeatNumber = seat.Number,
                SelectedDishes = new ObservableCollection<DishBindableModel>(
                    seat.SelectedDishes.Select(y => new DishBindableModel()
                    {
                        TotalPrice = y.TotalPrice,
                        ImageSource = y.ImageSource,
                        Name = y.Name,
                    })),
            };
}
}
