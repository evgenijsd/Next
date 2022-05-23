﻿using Next2.Interfaces;
using System;
using System.Collections.Generic;

namespace Next2.Models.API.DTO
{
    public class SimpleIngredientModelDTO : IBaseApiModel
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public double Price { get; set; }

        public string? ImageSource { get; set; }

        public IEnumerable<SimpleIngredientsCategoryModelDTO>? IngredientsCategory { get; set; }
    }
}