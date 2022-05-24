using Next2.Interfaces;
using System;
using System.Collections.Generic;

namespace Next2.Models.API.DTO
{
    public class SeatModelDTO : IBaseApiModel
    {
        public Guid Id { get; set; }

        public int Number { get; set; }

        public IEnumerable<SelectedDishModelDTO>? SelectedDishes { get; set; }
    }
}
