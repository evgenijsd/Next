using Next2.Interfaces;
using System;
using System.Collections.Generic;

namespace Next2.Models.API.DTO
{
    public class DishReplacementProductModelDTO : IBaseApiModel
    {
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }

        public IEnumerable<SimpleProductModelDTO>? Products { get; set; }
    }
}
