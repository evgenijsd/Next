using Next2.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Helpers.DTO
{
    public class SimpleCustomerModelDTO : IBaseApiModel
    {
        public Guid Id { get; set; }

        public string? FullName { get; set; }

        public string? Phone { get; set; }
    }
}
