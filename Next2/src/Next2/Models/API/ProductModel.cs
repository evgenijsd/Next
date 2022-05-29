using Next2.Interfaces;
using Next2.Models.API.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Models.API
{
    public class ProductModel : IBaseApiModel
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public double DefaultPrice { get; set; }

        public string? ImageSource { get; set; }

        public IEnumerable<SimpleIngredientModelDTO>? Ingridients { get; set; }

        public IEnumerable<OptionModelDTO>? Options { get; set; }
    }
}
