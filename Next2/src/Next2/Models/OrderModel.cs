using Next2.Enums;
using Next2.Interfaces;

namespace Next2.Models
{
    public class OrderModel : IBaseModel
    {
        public int Id { get; set; }

        public CustomerModel Customer { get; set; } = new CustomerModel();

        public int TableNumber { get; set; }

        public string OrderStatus { get; set; }

        public EOrderType OrderType { get; set; }

        public int OrderNumber { get; set; }

        public EBonusType BonusType { get; set; } = EBonusType.None;

        public double Total { get; set; }

        public double PriceTax { get; set; }

        public EOrderPaymentStatus? PaymentStatus { get; set; }
    }
}
