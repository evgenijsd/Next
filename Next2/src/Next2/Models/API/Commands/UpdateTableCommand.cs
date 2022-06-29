using Next2.Interfaces;
using System;

namespace Next2.Models.API.Commands
{
    public class UpdateTableCommand : IBaseApiModel
    {
        public Guid Id { get; set; }

        public int Number { get; set; }

        public int SeatNumbers { get; set; }

        public bool IsAvailable { get; set; }
    }
}
