using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Next2.Services.Rewards
{
    public interface IRewardsService
    {
        Task<AOResult<IEnumerable<RewardModel>>> GetCustomersRewards(int customerId);
    }
}
