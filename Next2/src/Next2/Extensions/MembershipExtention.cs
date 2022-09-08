using Next2.Models.API.Commands;
using Next2.Models.API.DTO;
using System;

namespace Next2.Extensions
{
    public static class MembershipExtention
    {
        public static UpdateMembershipCommand ToUpdateMembershipCommand(this MembershipModelDTO member) =>
            new UpdateMembershipCommand
            {
                Id = member.Id,
                StartDate = member.StartDate,
                EndDate = member.EndDate,
                CustomerId = member.Customer?.Id ?? Guid.Empty,
                IsActive = member.IsActive,
            };
    }
}
