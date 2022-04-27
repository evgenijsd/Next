using Next2.Enums;
using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace Next2.Services.Bonuses
{
    public interface IBonusesService
    {
        Task<List<BonusModel>> GetActiveBonusesAsync(FullOrderBindableModel currentOrder);

        Task<IEnumerable<BonusModel>> GetActiveCouponesAsync(List<BonusModel> bonuses);

        Task<IEnumerable<BonusModel>> GetActiveDiscountsAsync(List<BonusModel> bonuses);

        Task<FullOrderBindableModel> СalculationBonusAsync(FullOrderBindableModel currentOrder);
    }
}
