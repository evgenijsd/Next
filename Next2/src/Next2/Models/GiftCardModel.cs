using Next2.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Models
{
    public class GiftCardModel : IBaseApiModel
    {
        public Guid Id { get; set; }

        public int GiftCardNumber { get; set; } // string

        public double TotalBalance { get; set; }

        public DateTime Expire { get; set; }

        public Guid CustomerId { get; set; }

        public bool IsRegistered { get; set; } // will be deleted?
    }
}
