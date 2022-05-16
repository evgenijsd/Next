using System;
using System.Collections.Generic;

namespace Next2.Helpers.DTO
{
    public class SeatModelDTO
    {
        public Guid Id { get; set; }

        public int Number { get; set; }

        public List<SimpleSelectedDishModelDTO>? SelectedDishes { get; set; }
    }
}
