using Next2.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Models
{
    public class GiftCardModel : IBaseModel
    {
        public int Id { get; set; }

        public int GiftCardNumber { get; set; }

        public float GiftCardFounds { get; set; }

        public bool IsRegistered { get; set; }
    }
}
