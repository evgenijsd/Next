using Next2.Interfaces;
using System;
using System.Collections.Generic;

namespace Next2.Models.API.DTO
{
    public class SimpleSeatModelDTO : IBaseApiModel
    {
        public Guid Id { get; set; }

        public IEnumerable<Guid>? SelectedDishesId { get; set; }
    }
}
