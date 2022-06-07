using Next2.Interfaces;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Next2.Models
{
    public class SeatBindableModel : BindableBase, IBaseApiModel
    {
        public SeatBindableModel()
        {
        }

        public SeatBindableModel(SeatBindableModel seat)
        {
            Id = seat.Id;
            SeatNumber = seat.SeatNumber;
            Checked = seat.Checked;
            SelectedItem = seat.SelectedItem;
            IsFirstSeat = seat.IsFirstSeat;
            SetSelectionCommand = seat.SetSelectionCommand;
            SeatSelectionCommand = seat.SeatSelectionCommand;
            SeatDeleteCommand = seat.SeatDeleteCommand;
            RemoveOrderCommand = seat.RemoveOrderCommand;
            SelectedDishes = new();

            if (seat.SelectedItem is not null)
            {
                foreach (var dish in seat.SelectedDishes)
                {
                    if (dish.Id == seat.SelectedItem.Id && dish.SelectedDishProportion == seat.SelectedItem.SelectedDishProportion && dish.TotalPrice == seat.SelectedItem.TotalPrice)
                    {
                        var tmpDish = seat.SelectedDishes.IndexOf(dish);
                        SelectedItem = SelectedDishes[tmpDish];
                    }
                }
            }
        }

        public Guid Id { get; set; }

        public int SeatNumber { get; set; }

        public bool Checked { get; set; }

        public DishBindableModel? SelectedItem { get; set; }

        public bool IsFirstSeat { get; set; }

        public ICommand SetSelectionCommand { get; set; }

        public ICommand SeatSelectionCommand { get; set; }

        public ICommand SeatDeleteCommand { get; set; }

        public ICommand RemoveOrderCommand { get; set; }

        public ObservableCollection<DishBindableModel> SelectedDishes { get; set; }
    }
}