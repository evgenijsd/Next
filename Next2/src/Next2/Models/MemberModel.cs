using Next2.Interfaces;
using System;

namespace Next2.Models
{
    public class MemberModel : IBaseModel
    {
        public int Id { get; set; }

        public string CustomerName { get; set; }

        public string Phone { get; set; }

        public DateTime MembershipStartTime { get; set; }

        public DateTime MembershipEndTime { get; set; }
    }
}
