using Newtonsoft.Json;
using System;

namespace Next2.Models
{
    public class MemberModel : IEntityModelBase
    {
        [JsonProperty(nameof(Id))]
        public int Id { get; set; }

        [JsonProperty(nameof(CustomerName))]
        public string CustomerName { get; set; }

        [JsonProperty(nameof(Phone))]
        public string Phone { get; set; }

        [JsonProperty(nameof(MembershipStartTime))]
        public DateTime MembershipStartTime { get; set; }

        [JsonProperty(nameof(MembershipEndTime))]
        public DateTime MembershipEndTime { get; set; }
    }
}
