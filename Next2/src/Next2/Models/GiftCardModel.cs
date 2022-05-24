using Next2.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Models
{
    public class GiftCardModel : IBaseModel
    {
        public int Id { get; set; }

        public string UuId { get; set; }

        public int GiftCardNumber { get; set; }

        public decimal GiftCardFunds { get; set; }

        public bool IsRegistered { get; set; }
    }
}
