using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using Next2.Resources.Strings;
using Next2.Services.Mock;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Next2.Services.Bonuses
{
    public class BonusesService : IBonusesService
    {
        private readonly IMockService _mockService;

        public BonusesService(IMockService mockService)
        {
            _mockService = mockService;
        }

        public async Task<AOResult<IEnumerable<BonusModel>>> GetBonusesAsync()
        {
            var result = new AOResult<IEnumerable<BonusModel>>();

            try
            {
                var bonuses = await _mockService.GetAsync<BonusModel>(x => x.Id != 0);

                if (bonuses is not null)
                {
                    result.SetSuccess(bonuses);
                }
                else
                {
                    result.SetFailure(Strings.NotFoundOrders);
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(GetBonusesAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }
    }
}
