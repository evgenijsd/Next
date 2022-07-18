using Prism.Mvvm;
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

        public ICommand DishSelectionCommand { get; set; }

        public ICommand SeatSelectionCommand { get; set; }

        public ObservableCollection<DishBindableModel> SelectedDishes { get; set; } = new();
    }
}