using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using Next2.Resources.Strings;
using Next2.Services.Mock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Next2.Services.Rewards
{
    public class RewardsService : IRewardsService
    {
        private IMockService _mockService;

        public RewardsService(IMockService mockService)
        {
            _mockService = mockService;
        }

        #region -- IReardsService --

        public async Task<AOResult<IEnumerable<RewardModel>>> GetCustomersRewards(int customerId)
        {
            var result = new AOResult<IEnumerable<RewardModel>>();

            try
            {
                var rewards = await _mockService.GetAsync<RewardModel>(x => x.CustomerId == customerId);

                if (rewards is not null && rewards.Any())
                {
                    result.SetSuccess(rewards);
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(GetCustomersRewards)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        #endregion
    }
}
