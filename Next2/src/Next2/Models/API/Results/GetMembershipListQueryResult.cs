using Next2.Models.API.DTO;
using System.Collections.Generic;

namespace Next2.Helpers.API.Results
{
    public class GetMembershipListQueryResult
    {
        public IEnumerable<MembershipModelDTO>? Memberships { get; set; }
    }
}
