using Next2.Models.API.Commands;
using Next2.Models.API.DTO;

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
                CustomerId = member.Customer.Id,
                IsActive = member.IsActive,
            };
    }
}
