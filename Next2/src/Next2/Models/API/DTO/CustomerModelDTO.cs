using Next2.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Models.API.DTO
{
    public class CustomerModelDTO : IBaseApiModel
    {
        public Guid Id { get; set; }

        public string? FullName { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public string Birthday { get; set; } = string.Empty;

        public string? MembershipId { get; set; }

        public List<string>? GiftCardsId { get; set; }
    }
}
