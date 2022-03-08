using Next2.ENums;
using Next2.Interfaces;

namespace Next2.Models
{
    public class DiscountModel : IBaseModel
    {
        public int Id { get; set; }

        public double Value { get; set; }

        public EDiscountType Type { get; set; }
    }
}
