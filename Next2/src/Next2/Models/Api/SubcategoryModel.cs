using Next2.Interfaces;
using System;

namespace Next2.Api.Models.Category
{
    public class SubcategoryModel : IBaseApiModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}
