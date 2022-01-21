using Next2.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Next2.Services.Membership
{
    public interface IMembershipService
    {
        Task<IEnumerable<MemberModel>> GetAllAsync();

        Task<IEnumerable<T>> GetAllByQueryAsync<T>(Func<MemberModel, bool> func);

        Task EditMembershipStart(int memberId, DateTime startTime);

        Task EditMembershipEnd(int memberId, DateTime endTIme);
    }
}