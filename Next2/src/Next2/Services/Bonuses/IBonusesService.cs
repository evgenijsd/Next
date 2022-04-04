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

        Task<AOResult<IEnumerable<BonusConditionAndSetModel>>> GetConditionsAsync();

        Task<AOResult<IEnumerable<BonusConditionAndSetModel>>> GetBonusSetsAsync();

        ObservableCollection<SetBindableModel> GetSets(FullOrderBindableModel currentOrder);

        Task<FullOrderBindableModel> СalculationBonusAsync(FullOrderBindableModel currentOrder);

        ObservableCollection<BonusBindableModel> GetDiscounts(IEnumerable<BonusConditionAndSetModel> bonusConditions, ObservableCollection<BonusBindableModel> discounts, IEnumerable<SetBindableModel> sets);

        float GetPriceBonus(BonusBindableModel selectedBonus, SetBindableModel set);
    }
}
