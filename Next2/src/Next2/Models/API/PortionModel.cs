﻿using Next2.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Models.API
{
    public class PortionModel : IBaseApiModel
    {
        public Guid Id { get; set; }

        public string? Title { get; set; }

        public double Price { get; set; }
    }
}
