﻿using Next2.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Helpers.DTO.Customers
{
    public class CustomerModelDTO : IBaseApiModel
    {
        public Guid Id { get; set; }

        public string? FullName { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public DateTime? Birthday { get; set; }

        public string? MembershipId { get; set; }

        public IEnumerable<string>? GiftCardsId { get; set; }
    }
}
