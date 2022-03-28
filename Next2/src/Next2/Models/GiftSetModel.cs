using Next2.Enums;
using Next2.Interfaces;

namespace Next2.Models
{
    public class GiftSetModel : IBaseModel
    {
        public int Id { get; set; }

        public int GiftCardId { get; set; }

        public int SetId { get; set; }
    }
}
