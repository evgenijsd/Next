using System.Collections.Generic;

namespace Next2.Helpers.DTO
{
    public class SeatModelDTO
    {
        public string Id { get; set; } = string.Empty;

        public int Number { get; set; }

        public List<SimpleSelectedDishModelDTO>? SelectedDishes { get; set; }
    }
}
