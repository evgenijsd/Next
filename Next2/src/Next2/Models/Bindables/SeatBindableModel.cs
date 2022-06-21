using Next2.Interfaces;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Next2.Models.Bindables
{
    public class SeatBindableModel : BindableBase
    {
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