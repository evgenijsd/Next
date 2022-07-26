using Next2.Interfaces;
using System;

namespace Next2.Models
{
    public class HoldDishModel : IBaseModel
    {
        public int Id { get; set; }

        public int TableNumber { get; set; }

        public string? DishName { get; set; }

        public DateTime ReleaseTime { get; set; }
    }
}
