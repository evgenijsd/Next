﻿using System;
using System.Collections.Generic;

namespace Next2.Models.API.DTO
{
    public class SubcategoryModelDTO
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public List<Guid>? categoriesId { get; set; } = new();
    }
}
