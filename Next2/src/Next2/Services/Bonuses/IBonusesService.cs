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

        ObservableCollection<SetBindableModel> GetSets(FullOrderBindableModel currentOrder);

        ObservableCollection<BonusBindableModel> GetDiscounts(IEnumerable<BonusConditionModel> bonusConditions, ObservableCollection<BonusBindableModel> discounts, IEnumerable<SetBindableModel> sets);

        float GetPriceBonus(BonusBindableModel selectedBonus, SetBindableModel set);
    }
}
