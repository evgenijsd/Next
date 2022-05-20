using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Models.Api.Commands
{
    public class UpdateGiftCardCommand
    {
        public string GiftCardNumber { get; set; }
        public DateTime Expire { get; set; }
        public double TotalBalance { get; set; }
        public Guid? CustomerId { get; set; }
    }
}
