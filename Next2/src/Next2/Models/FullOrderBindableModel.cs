using Next2.Enums;
using Next2.Interfaces;
using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace Next2.Models
{
    public class FullOrderBindableModel : BindableBase, IBaseModel
    {
        public int Id { get; set; }

        public int OrderNumber { get; set; }

        public TableBindableModel Table { get; set; }

        public CustomerModel Customer { get; set; } = new ();

        public string OrderStatus { get; set; }

        public EOrderType OrderType { get; set; }

        public double SubTotal { get; set; }

        public double Tax { get; set; }

        public double Total { get; set; }

        public ObservableCollection<SeatBindableModel> Seats { get; set; }
    }
}