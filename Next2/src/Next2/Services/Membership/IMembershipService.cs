using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Next2.Services.Membership
{
    public interface IMembershipService
    {
        Task<AOResult<IEnumerable<MemberModel>>> GetAllMembersAsync(Func<MemberModel, bool>? condition = null);

        string ApplyNameFilter(string text);
    }
}