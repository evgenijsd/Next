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

        public TableBindableModel Table { get; set; } = new();

        public CustomerModel Customer { get; set; } = new();

        public string OrderStatus { get; set; } = string.Empty;

        public EOrderType OrderType { get; set; }

        public EBonusType BonusType { get; set; } = EBonusType.None;

        public BonusBindableModel Bonus { get; set; } = new();

        public TaxModel Tax = new();

        public double SubTotal { get; set; }

        public double PriceWithBonus { get; set; } = 0f;

        public double PriceTax { get; set; }

        public double Total { get; set; }

        public ObservableCollection<SeatBindableModel> Seats { get; set; } = new();
    }
}