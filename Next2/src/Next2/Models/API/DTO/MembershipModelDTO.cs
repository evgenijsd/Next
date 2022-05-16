using Next2.Interfaces;
using System;

namespace Next2.Models.API.DTO
{
    public class MembershipModelDTO : IBaseApiModel
    {
        public Guid Id { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; }

        public SimpleCustomerModelDTO Customer { get; set; } = new();
    }
}
