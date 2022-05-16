using Next2.Interfaces;
using System;

namespace Next2.Models.API.DTO
{
    public class SimpleCustomerModelDTO : IBaseApiModel
    {
        public Guid Id { get; set; }

        public string? FullName { get; set; }

        public string? Phone { get; set; }
    }
}
