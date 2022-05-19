using Next2.Interfaces;
using System;

namespace Next2.Models
{
    public class RewardModel : IBaseModel
    {
        public int Id { get; set; }

        public Guid CustomerId { get; set; }

        public int SetId { get; set; }

        public string SetTitle { get; set; } = string.Empty;
    }
}
