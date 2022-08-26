using Next2.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Next2.Models.API.DTO
{
    public class SimpleProductModelDTO : IBaseApiModel
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public decimal DefaultPrice { get; set; }

        public string? ImageSource { get; set; }

        public IEnumerable<OptionModelDTO>? Options { get; set; }

        public IEnumerable<SimpleIngredientModelDTO>? Ingredients { get; set; }

        public SimpleProductModelDTO Clone()
        {
            return new()
            {
                Id = Id,
                Name = Name,
                DefaultPrice = DefaultPrice,
                ImageSource = ImageSource,
                Options = Options?.Select(row => row.Clone()),
                Ingredients = Ingredients?.Select(row => row.Clone()),
            };
        }
    }
}
