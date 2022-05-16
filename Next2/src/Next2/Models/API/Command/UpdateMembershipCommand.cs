using Next2.Interfaces;
using System;

namespace Next2.Models.API.Command
{
    public class UpdateMembershipCommand : IBaseApiModel
    {
        public Guid Id { get; set; }

        public string StartDate { get; set; } = string.Empty;

        public string EndDate { get; set; } = string.Empty;

        public Guid CustomerId { get; set; }

        public bool IsActive { get; set; }
    }
}
