using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Next2.Services.Bonuses
{
    public interface IBonusesService
    {
        Task<AOResult<IEnumerable<BonusModel>>> GetBonusesAsync();

        Task<AOResult<IEnumerable<SetModel>>> GetSetsAsync();
    }
}
