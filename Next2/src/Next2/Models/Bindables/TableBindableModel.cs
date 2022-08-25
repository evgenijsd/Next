using Next2.Interfaces;
using Prism.Mvvm;
using System;

namespace Next2.Models.Bindables
{
    public class TableBindableModel : BindableBase, IBaseApiModel
    {
        public Guid Id { get; set; }

        public int TableNumber { get; set; }

        public int SeatNumbers { get; set; }

        public bool IsAvailable { get; set; }
    }
}
