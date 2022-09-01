using Next2.Interfaces;
using System;

namespace Next2.Models.API.DTO
{
    public class SimpleTableModelDTO : IBaseApiModel, ICloneable
    {
        public Guid Id { get; set; }

        public int Number { get; set; }

        public int SeatNumbers { get; set; }

        public object Clone()
        {
            return new SimpleTableModelDTO()
            {
                Id = Id,
                Number = Number,
                SeatNumbers = SeatNumbers,
            };
        }
    }
}
