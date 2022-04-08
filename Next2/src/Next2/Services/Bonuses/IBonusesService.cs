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
        Task<AOResult<IEnumerable<BonusModel>>> GetBonusesAsync();

        Task<AOResult<IEnumerable<BonusConditionModel>>> GetConditionsAsync();

        Task<AOResult<IEnumerable<BonusSetModel>>> GetBonusSetsAsync();

        Task<List<BonusModel>> GetActiveBonusesAsync(FullOrderBindableModel currentOrder);

        Task<IEnumerable<BonusModel>> GetActiveCouponesAsync(List<BonusModel> bonuses);

        Task<IEnumerable<BonusModel>> GetActiveDiscountsAsync(List<BonusModel> bonuses);

        Task<FullOrderBindableModel> СalculationBonusAsync(FullOrderBindableModel currentOrder);

        Task<List<SetModel>> GetConditionSetsAsync(FullOrderBindableModel currentOrder, EConditionSet eConditionSet);

        float GetPriceBonus(BonusBindableModel selectedBonus, SetBindableModel set);

        List<BonusModel> GetDiscounts(IEnumerable<BonusConditionModel>? bonusConditions, IEnumerable<BonusModel>? discounts, List<SetModel> sets);

        List<SetModel> GetSets(FullOrderBindableModel currentOrder);
    }
}
