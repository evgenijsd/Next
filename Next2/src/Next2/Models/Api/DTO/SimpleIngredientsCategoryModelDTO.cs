using Next2.Interfaces;
using System;

namespace Next2.Models.Api.DTO
{
    public class SimpleIngredientsCategoryModelDTO : IBaseApiModel
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }
    }
}
