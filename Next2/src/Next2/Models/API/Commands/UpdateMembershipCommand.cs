using Next2.Interfaces;
using System;

namespace Next2.Models.API.Commands
{
    public class UpdateMembershipCommand : IBaseApiModel
    {
        public Guid Id { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public Guid CustomerId { get; set; }

        public bool IsActive { get; set; }
    }
}
