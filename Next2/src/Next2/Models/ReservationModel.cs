﻿using Next2.Interfaces;
using Next2.Models.API.DTO;
using System;

namespace Next2.Models
{
    public class ReservationModel : IBaseModel
    {
        public int Id { get; set; }

        public EmployeeModelDTO Server { get; set; } = new();

        public string CustomerName { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public int GuestsAmount { get; set; }

        public TableModelDTO Table { get; set; } = new();

        public string? Comment { get; set; }

        public DateTime DateTime { get; set; }
    }
}
