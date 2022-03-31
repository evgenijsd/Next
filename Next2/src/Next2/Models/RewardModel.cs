﻿using Next2.Enums;
using Next2.Interfaces;
using System.Collections.Generic;

namespace Next2.Models
{
    public class RewardModel : IBaseModel
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public int SetId { get; set; }

        public string SetTitle { get; set; } = string.Empty;
    }
}
