using System;
using System.ComponentModel;

namespace Next2.Models
{
    public class MemberBindableModel : IBaseBindableModel
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public int Id { get; set; }

        public string CustomerName { get; set; }

        public string Phone { get; set; }

        public DateTime MembershipStartTime { get; set; }

        public DateTime MembershipEndTime { get; set; }
    }
}
