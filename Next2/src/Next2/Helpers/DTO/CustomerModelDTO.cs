using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Helpers.DTO
{
    public class CustomerModelDTO
    {
        public string Id { get; set; } = string.Empty;

        public string? FullName { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public string Birthday { get; set; } = string.Empty;

        public string? MembershipId { get; set; }

        public List<string>? GiftCardsId { get; set; }
    }
}
