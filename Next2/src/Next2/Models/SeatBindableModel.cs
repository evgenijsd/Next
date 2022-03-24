﻿using Next2.Interfaces;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Next2.Models
{
    public class SeatBindableModel : BindableBase, IBaseModel
    {
        public int Id { get; set; }

        public int SeatNumber { get; set; }

        public bool Checked { get; set; }

        public SetBindableModel? SelectedItem { get; set; }

        public bool IsFirstSeat { get; set; }

        public ICommand SetSelectionCommand { get; set; }

        public ICommand SeatSelectionCommand { get; set; }

        public ICommand SeatDeleteCommand { get; set; }

        public ICommand RemoveOrderCommand { get; set; }

        public ObservableCollection<SetBindableModel> Sets { get; set; }
    }
}