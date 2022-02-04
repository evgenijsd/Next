using Next2.Models;

namespace Next2.Extensions
{
    public static class MemberExtension
    {
        public static MemberBindableModel ToBindableModel(this MemberModel member)
        {
            return new MemberBindableModel
            {
                Id = member.Id,
                CustomerName = member.CustomerName,
                Phone = member.Phone,
                MembershipStartTime = member.MembershipStartTime,
                MembershipEndTime = member.MembershipEndTime,
            };
        }

        public static MemberModel ToModel(this MemberBindableModel member)
        {
            return new MemberModel
            {
                Id = member.Id,
                CustomerName = member.CustomerName,
                Phone = member.Phone,
                MembershipStartTime = member.MembershipStartTime,
                MembershipEndTime = member.MembershipEndTime,
            };
        }
    }
}
