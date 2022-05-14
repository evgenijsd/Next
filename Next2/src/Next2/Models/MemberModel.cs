using Next2.Interfaces;
using System;

namespace Next2.Models
{
    public class MemberModel : IBaseApiModel
    {
        public Guid Id { get; set; }

        public string? CustomerName { get; set; }

        public string? Phone { get; set; }

        public DateTime MembershipStartTime { get; set; }

        public DateTime MembershipEndTime { get; set; }

        public bool IsActive { get; set; }

        public Guid CustomerId { get; set; }
    }
}
