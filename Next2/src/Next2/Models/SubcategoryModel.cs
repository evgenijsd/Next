using Next2.Interfaces;
using System;

namespace Next2.Models
{
    public class SubcategoryModel : IBaseApiModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}