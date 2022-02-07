using Next2.Enums;
using Next2.Interfaces;

namespace Next2.Models
{
    public class OrderModel : IBaseModel
    {
        public int Id { get; set; }

        public EOrderType Ordertype { get; set; }

        public float Total { get; set; }
    }
}
