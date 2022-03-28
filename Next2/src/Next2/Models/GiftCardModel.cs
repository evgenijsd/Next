using Next2.Enums;
using Next2.Interfaces;

namespace Next2.Models
{
    public class GiftCardModel : IBaseModel
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public EDiscountType DiscountType { get; set; }

        public double Amount { get; set; }
    }
}
