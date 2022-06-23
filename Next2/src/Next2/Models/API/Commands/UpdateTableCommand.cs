﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Models.API.Commands
{
    public class UpdateTableCommand
    {
        public Guid Id { get; set; }

        public int Number { get; set; }

        public int SeatNumbers { get; set; }

        public bool IsAvailable { get; set; }
    }
}
