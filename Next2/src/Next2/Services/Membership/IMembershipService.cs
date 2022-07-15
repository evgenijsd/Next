using Next2.Helpers.ProcessHelpers;
using Next2.Models.API.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Next2.Services.Membership
{
    public interface IMembershipService
    {
        Task<AOResult<IEnumerable<MembershipModelDTO>>> GetAllMembersAsync(Func<MembershipModelDTO, bool>? condition = null);

        Task<AOResult> UpdateMemberAsync(MembershipModelDTO member);
    }
}