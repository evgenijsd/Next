﻿using Next2.Interfaces;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Next2.Models
{
    public class SeatBindableModel : BindableBase, IBaseModel
    {
        public SeatBindableModel()
        {
        }

        public SeatBindableModel(SeatBindableModel seat)
        {
            Id = seat.Id;
            SeatNumber = seat.SeatNumber;
            Checked = seat.Checked;
            SelectedItem = null;
            IsFirstSeat = seat.IsFirstSeat;
            SetSelectionCommand = seat.SetSelectionCommand;
            SeatSelectionCommand = seat.SeatSelectionCommand;
            SeatDeleteCommand = seat.SeatDeleteCommand;
            RemoveOrderCommand = seat.RemoveOrderCommand;
            Sets = new();

            foreach (var set in seat.Sets)
            {
                Sets.Add(new SetBindableModel(set));
            }

            if (seat.SelectedItem is not null)
            {
                var tmpSet = Sets.FirstOrDefault(row => row.Id == seat.SelectedItem.Id);

                SelectedItem = Sets[Sets.IndexOf(tmpSet)];
            }
        }

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