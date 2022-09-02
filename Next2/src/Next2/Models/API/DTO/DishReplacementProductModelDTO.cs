using Next2.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Next2.Models.API.DTO
{
    public class DishReplacementProductModelDTO : IBaseApiModel, ICloneable
    {
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }

        public IEnumerable<SimpleProductModelDTO>? Products { get; set; }

        public object Clone()
        {
            return new DishReplacementProductModelDTO()
            {
                Id = Id,
                ProductId = ProductId,
                Products = Products?.Select(row => (SimpleProductModelDTO)row.Clone()),
            };
        }
    }
}
