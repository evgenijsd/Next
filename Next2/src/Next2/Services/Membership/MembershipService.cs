using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Next2.Helpers.DTO;
using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using Next2.Resources.Strings;
using Next2.Services.Mock;
using Next2.Services.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
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
                var membersObj = await _restService.RequestAsync<JObject>(HttpMethod.Get, $"{Constants.API.HOST_URL}/api/memberships");
                var membersDTO = membersObj.SelectToken("value")?.SelectToken("memberships")?.ToObject<IEnumerable<MembershipModelDTO>>();

                var members = membersDTO?.Select(x =>
                              new MemberModel
                              {
                                  Id = x.Id,
                                  CustomerName = x.Customer.FullName,
                                  Phone = x.Customer.Phone,
                                  MembershipStartTime = DateTime.Parse(x.StartDate),
                                  MembershipEndTime = DateTime.Parse(x.EndDate),
                                  IsActive = x.IsActive,
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

        /*public async Task<AOResult<bool>> DisableMemberAsync(MemberModel member)
        {
            var result = new AOResult<bool>();

            try
            {
                var remove = await _mockService.RemoveAsync(member);

                if (remove)
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
                result.SetError($"{nameof(DisableMemberAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }*/

        /*public async Task<AOResult<MemberModel>> UpdateMemberAsync(MemberModel member)
        {
            var result = new AOResult<MemberModel>();

            try
            {
                var update = await _mockService.UpdateAsync(member);

                if (update is not null)
                {
                    result.SetSuccess(update);
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
        }*/

        #endregion

    }
}