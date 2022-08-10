using System;

namespace Next2.Models.API.Commands
{
    public class UpdateGiftCardCommand
    {
        public string GiftCardNumber { get; set; } = string.Empty;

        public DateTime Expire { get; set; }

        public double TotalBalance { get; set; }

        public Guid? CustomerId { get; set; }
    }
}
