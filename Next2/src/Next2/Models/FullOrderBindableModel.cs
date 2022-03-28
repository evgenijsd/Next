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

        public string? CustomerName { get; set; }

        public string OrderStatus { get; set; }

        public EOrderType OrderType { get; set; }

        public EBonusType BonusType { get; set; } = EBonusType.None;

        public string BonusName { get; set; } = string.Empty;

        public double SubTotal { get; set; }

        public double Bonus { get; set; } = 0f;

        public double Tax { get; set; }

        public double Total { get; set; }

        public ObservableCollection<SeatBindableModel> Seats { get; set; }
    }
}