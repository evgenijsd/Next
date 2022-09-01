using Next2.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Next2.Models.API.DTO
{
    public class SimpleProductModelDTO : IBaseApiModel, ICloneable
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public decimal DefaultPrice { get; set; }

        public string? ImageSource { get; set; }

        public IEnumerable<OptionModelDTO>? Options { get; set; }

        public IEnumerable<SimpleIngredientModelDTO>? Ingredients { get; set; }

        public object Clone()
        {
            return new SimpleProductModelDTO()
            {
                Id = Id,
                Name = new(Name),
                DefaultPrice = DefaultPrice,
                ImageSource = new(ImageSource),
                Options = Options?.Select(row => (OptionModelDTO)row.Clone()),
                Ingredients = Ingredients?.Select(row => (SimpleIngredientModelDTO)row.Clone()),
            };
        }
    }
}
