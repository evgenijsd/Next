using Next2.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Helpers.API.DTO
{
    public class MembershipModelDTO : IBaseApiModel
    {
        public Guid Id { get; set; }

        public string StartDate { get; set; } = string.Empty;

        public string EndDate { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        public SimpleCustomerModelDTO Customer { get; set; } = new();
    }
}
