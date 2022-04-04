﻿using Next2.ENums;
using Next2.Interfaces;
using System.Collections.Generic;

namespace Next2.Models
{
    public class SeatModel : IBaseModel
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int SeatNumber { get; set; }

        public List<SetModel> Sets { get; set; } = new ();
    }
}