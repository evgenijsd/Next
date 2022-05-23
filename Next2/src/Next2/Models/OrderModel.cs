using Next2.Enums;
using Next2.Interfaces;
using Next2.Models.API.DTO;

namespace Next2.Models
{
    public class OrderModel : IBaseModel
    {
        public int Id { get; set; }

        public CustomerModelDTO? Customer { get; set; }

        public int TableNumber { get; set; }

        public EOrderStatus OrderStatus { get; set; }

        public EOrderType OrderType { get; set; }

        public int OrderNumber { get; set; }

        public bool IsTab { get; set; }

        public EBonusType BonusType { get; set; } = EBonusType.None;

        public float Total { get; set; }

        public float PriceTax { get; set; }
    }
}
