using Next2.Helpers.API.Results;
using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using Next2.Models.API;
using Next2.Models.API.Commands;
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

         #region -- IMembership implementation --

        public async Task<AOResult<IEnumerable<MemberModel>>> GetAllMembersAsync(Func<MemberModel, bool>? condition = null)
        {
            var result = new AOResult<IEnumerable<MemberModel>>();

            try
            {
                var membersDTO = await _restService.RequestAsync<GenericGetExecutionResult<GetMembershipListQueryResult>>(HttpMethod.Get, $"{Constants.API.HOST_URL}/api/memberships");

                var members = membersDTO?.Value?.Memberships?.Select(x =>
                    new MemberModel
                    {
                        Id = x.Id,
                        CustomerName = x.Customer.FullName,
                        Phone = x.Customer.Phone,
                        MembershipStartTime = x.StartDate,
                        MembershipEndTime = x.EndDate,
                        IsActive = x.IsActive,
                        CustomerId = x.Customer.Id,
                    });

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
                result.SetError($"{nameof(GetAllMembersAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public string ApplyNameFilter(string text)
        {
            Regex regexName = new(Constants.Validators.NAME);
            Regex regexNumber = new(Constants.Validators.NUMBER);
            Regex regexText = new(Constants.Validators.TEXT);

            var result = regexText.Replace(text, string.Empty);
            result = Regex.IsMatch(result, Constants.Validators.CHECK_NUMBER)
                ? regexNumber.Replace(result, string.Empty)
                : regexName.Replace(result, string.Empty);

            return result;
        }

        public async Task<AOResult> UpdateMemberAsync(MemberModel member)
        {
            var result = new AOResult();

            try
            {
                var update = new UpdateMembershipCommand
                {
                    Id = member.Id,
                    StartDate = member.MembershipStartTime,
                    EndDate = member.MembershipEndTime,
                    CustomerId = member.CustomerId,
                    IsActive = member.IsActive,
                };

                var resultUpdate = await _restService.RequestAsync<ExecutionResult>(HttpMethod.Put, $"{Constants.API.HOST_URL}/api/memberships", update);

                if (resultUpdate.Success)
                {
                    result.SetSuccess();
                }
                else
                {
                    result.SetFailure();
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