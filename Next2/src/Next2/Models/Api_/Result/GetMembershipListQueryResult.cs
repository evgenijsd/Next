using Next2.Helpers.API.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Helpers.API.Result
{
    public class GetMembershipListQueryResult
    {
        public IEnumerable<MembershipModelDTO>? Memberships { get; set; }
    }
}
