using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using Next2.Services.Mock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Next2.Services.Membership
{
    public class MembershipService : IMembershipService
    {
        private readonly IMockService _mockService;

        public MembershipService(IMockService mockService)
        {
            _mockService = mockService;
        }

         #region -- IMembership implementation --

        public async Task<AOResult<IEnumerable<MemberModel>>> GetAllMembersAsync(Func<MemberModel, bool>? condition = null)
        {
            var result = new AOResult<IEnumerable<MemberModel>>();

            try
            {
                var members = await _mockService.GetAllAsync<MemberModel>();

                if (members != null)
                {
                    result.SetSuccess(condition == null
                        ? members
                        : members.Where(condition));
                }
                else
                {
                    result.SetFailure();
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(GetAllMembersAsync)}: exception", "Some issues", ex);
            }

            return result;
        }

        #endregion
    }
}