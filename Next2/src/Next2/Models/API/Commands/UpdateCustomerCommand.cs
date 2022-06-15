using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Models.API.Commands
{
    public class UpdateCustomerCommand
    {
        public Guid Id { get; set; }

        public string? FullName { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public DateTime? Birthday { get; set; }

        public IEnumerable<Guid>? GiftCardsId { get; set; }
    }
}
