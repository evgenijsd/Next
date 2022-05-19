using Next2.Enums;
using Next2.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Models
{
    public class TaxModel : IBaseModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal Value { get; set; }
    }
}
