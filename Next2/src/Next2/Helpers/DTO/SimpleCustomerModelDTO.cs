using System;

namespace Next2.Helpers.DTO
{
    public class SimpleCustomerModelDTO
    {
        public Guid Id { get; set; }

        public string? FullName { get; set; }

        public string? Phone { get; set; }
    }
}
