using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Helpers.DTO
{
    public class GetMembershipsListQueryResult
    {
        public IEnumerable<MembershipModelDTO>? Memberships { get; set; }
    }
}
