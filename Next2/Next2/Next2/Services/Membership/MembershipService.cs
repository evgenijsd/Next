using Next2.Models;
using Next2.Services.Rest;
using System;
using System.Collections.Generic;
using System.Net.Http;
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

        public Task EditMembershipEnd(int memberId, DateTime endTIme)
        {
            throw new NotImplementedException();
        }

        public Task EditMembershipStart(int memberId, DateTime startTime)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<MemberModel>> GetAllAsync()
        {
            string url = $"{Constants.API.DOMAIN}/{Constants.ApiServices.MEMBERSHIP}/{Constants.ApiMethods.GETALL}";

            IEnumerable<MemberModel> list = await _restService.RequestAsync<IEnumerable<MemberModel>>(HttpMethod.Get, url);

            return list;
        }

        public Task<IEnumerable<T>> GetAllByQueryAsync<T>(Func<MemberModel, bool> func)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}