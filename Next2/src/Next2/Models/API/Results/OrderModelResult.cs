using Next2.Interfaces;
using System;

namespace Next2.Models.API.Results
{
    public class OrderModelResult : IBaseApiModel
    {
        public Guid Id { get; set; }

        public int OrderNumber { get; set; }

        public DateTime Open { get; set; }

        public decimal TaxCoefficient { get; set; }
    }
}
