using Next2.Interfaces;
using System;

namespace Next2.Models
{
    public class RewardModel : IBaseModel
    {
        public int Id { get; set; }

        public Guid CustomerId { get; set; }

        public int DishId { get; set; }

        public string DishTitle { get; set; } = string.Empty;
    }
}