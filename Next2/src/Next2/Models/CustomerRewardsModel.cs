using Next2.Interfaces;
using System.Collections.Generic;

namespace Next2.Models
{
    public class CustomerRewardsModel : IBaseModel
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public List<int> RewardsId { get; set; } = new ();
    }
}