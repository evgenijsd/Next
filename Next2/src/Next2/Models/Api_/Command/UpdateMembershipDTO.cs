using Next2.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Helpers.API.Command
{
    public class UpdateMembershipDTO : IBaseApiModel
    {
        public Guid Id { get; set; }

        public string StartDate { get; set; } = string.Empty;

        public string EndDate { get; set; } = string.Empty;

        public Guid CustomerId { get; set; }

        public bool IsActive { get; set; }
    }
}
