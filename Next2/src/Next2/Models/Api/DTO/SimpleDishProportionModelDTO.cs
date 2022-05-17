﻿using Next2.Interfaces;
using System;

namespace Next2.Models.Api.DTO
{
    public class SimpleDishProportionModelDTO : IBaseApiModel
    {
        public Guid Id { get; set; }

        public double PriceRatio { get; set; }

        public Guid ProportionId { get; set; }

        public string? ProportionName { get; set; }
    }
}