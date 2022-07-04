using Next2.Helpers.ProcessHelpers;
using Next2.Models.API.Commands;
using Next2.Models.API.DTO;
using Next2.Models.API.Results;
using Next2.Resources.Strings;
using Next2.Services.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Next2.Services.Membership
{
    public class MembershipService : IMembershipService
    {
        private readonly IRestService _restService;

        public MembershipService(IRestService restService)
        {
            _restService = restService;
        }

        #region -- IMembershipService implementation --

        public async Task<AOResult<IEnumerable<MembershipModelDTO>>> GetAllMembersAsync(Func<MembershipModelDTO, bool>? condition = null)
        {
            var result = new AOResult<IEnumerable<MembershipModelDTO>>();

            try
            {
                var query = $"{Constants.API.HOST_URL}/api/memberships";

                var response = await _restService.RequestAsync<GenericExecutionResult<GetMembershipListQueryResult>>(HttpMethod.Get, query);

                if (response.Success && response.Value is not null)
                {
                    var members = response.Value.Memberships;

                    if (members is not null)
                    {
                        result.SetSuccess(condition == null
                            ? members
                            : members.Where(condition));
                    }
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(GetAllMembersAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public string ApplyNameFilter(string text)
        {
            var result = text;

            try
            {
                Regex regexName = new(Constants.Validators.NAME);
                Regex regexNumber = new(Constants.Validators.NUMBER);
                Regex regexText = new(Constants.Validators.TEXT);

                result = regexText.Replace(text, string.Empty);

                result = Regex.IsMatch(result, Constants.Validators.CHECK_NUMBER)
                    ? regexNumber.Replace(result, string.Empty)
                    : regexName.Replace(result, string.Empty);
            }
            catch (Exception)
            {
            }

            return result;
        }

        public async Task<AOResult> UpdateMemberAsync(MembershipModelDTO member)
        {
            var result = new AOResult();

            try
            {
                var membershipForUpdate = new UpdateMembershipCommand
                {
                    Id = member.Id,
                    StartDate = $"{member.StartDate:s}",
                    EndDate = $"{member.EndDate:s}",
                    CustomerId = member.Customer.Id,
                    IsActive = member.IsActive,
                };

                var query = $"{Constants.API.HOST_URL}/api/memberships";

                var updateResult = await _restService.RequestAsync<ExecutionResult>(HttpMethod.Put, query, membershipForUpdate);

                if (updateResult.Success)
                {
                    result.SetSuccess();
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(UpdateMemberAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        #endregion
    }
}