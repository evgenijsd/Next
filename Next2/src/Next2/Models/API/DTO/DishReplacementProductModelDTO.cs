using Next2.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Next2.Models.API.DTO
{
    public class DishReplacementProductModelDTO : IBaseApiModel
    {
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }

        public IEnumerable<SimpleProductModelDTO>? Products { get; set; }

        public DishReplacementProductModelDTO Clone()
        {
            return new()
            {
                Id = Id,
                ProductId = ProductId,
                Products = Products.Select(row => row.Clone()),
            };
        }
    }
}
