using Next2.Interfaces;
using System;

namespace Next2.Helpers.DTO
{
    public class CustomerNameModelDTO : IBaseApiModel
    {
        public Guid Id { get; set; }

        public string? FullName { get; set; }
    }
}
