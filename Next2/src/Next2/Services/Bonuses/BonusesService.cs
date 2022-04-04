﻿using Next2.Enums;
using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using Next2.Resources.Strings;
using Next2.Services.Mock;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
                    result.SetFailure(Strings.NotFoundData);
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(GetBonusesAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public async Task<AOResult<IEnumerable<BonusConditionAndSetModel>>> GetConditionsAsync()
        {
            var result = new AOResult<IEnumerable<BonusConditionAndSetModel>>();

            try
            {
                var conditions = await _mockService.GetAsync<BonusConditionAndSetModel>(x => x.Id != 0);

                if (conditions is not null)
                {
                    result.SetSuccess(conditions);
                }
                else
                {
                    result.SetFailure(Strings.NotFoundData);
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(GetBonusesAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public async Task<AOResult<IEnumerable<BonusConditionAndSetModel>>> GetBonusSetsAsync()
        {
            var result = new AOResult<IEnumerable<BonusConditionAndSetModel>>();

            try
            {
                var bonusSet = await _mockService.GetAsync<BonusConditionAndSetModel>(x => x.Id != 0);

                if (bonusSet is not null)
                {
                    result.SetSuccess(bonusSet);
                }
                else
                {
                    result.SetFailure(Strings.NotFoundData);
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(GetBonusesAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public ObservableCollection<SetBindableModel> GetSets(FullOrderBindableModel currentOrder)
        {
            var result = new ObservableCollection<SetBindableModel>();

            foreach (var seat in currentOrder.Seats)
            {
                foreach (var set in seat.Sets)
                {
                    result.Add(set);
                }
            }

            return result;
        }

        public float GetPriceBonus(BonusBindableModel selectedBonus, SetBindableModel set)
        {
            float result = 0;

            switch (selectedBonus.Type)
            {
                case EBonusValueType.Value:
                    result = set.Portion.Price - selectedBonus.Value;
                    break;
                case EBonusValueType.Percent:
                    result = set.Portion.Price - (selectedBonus.Value * set.Portion.Price);
                    break;
                case EBonusValueType.AbsoluteValue:
                    result = selectedBonus.Value;
                    break;
                default:
                    break;
            }

            if (result < 0)
            {
                result = 0;
            }

            return result;
        }

        public ObservableCollection<BonusBindableModel> GetDiscounts(IEnumerable<BonusConditionAndSetModel> bonusConditions, ObservableCollection<BonusBindableModel> discounts, IEnumerable<SetBindableModel> sets)
        {
            var result = new ObservableCollection<BonusBindableModel>();

            foreach (var discount in discounts)
            {
                var conditions = bonusConditions.Where(x => x.BonusId == discount.Id);
                int count = conditions.Count();

                foreach (var condition in conditions)
                {
                    if (sets.Any(x => x.Id == condition.SetId))
                    {
                        count -= 1;
                    }
                }

                if (count == 0)
                {
                    result.Add(discount);
                }
            }

            return result;
        }

        public async Task<FullOrderBindableModel> СalculationBonusAsync(FullOrderBindableModel currentOrder)
        {
            if (currentOrder.BonusType != EBonusType.None)
            {
                var resultConditions = await GetConditionsAsync();

                var resultBonusSets = await GetBonusSetsAsync();

                currentOrder.PriceWithBonus = 0;

                var bonusConditions = resultConditions.Result?.Where(x => x.BonusId == currentOrder.Bonus.Id).ToList() ?? Enumerable.Empty<BonusConditionAndSetModel>().ToList();
                var bonusSets = resultBonusSets.Result?.Where(x => x.BonusId == currentOrder.Bonus.Id).ToList() ?? Enumerable.Empty<BonusConditionAndSetModel>().ToList();
                List<SetBindableModel> setConditions = new();
                List<SetBindableModel> setBonus = new();
                int countCondition;
                int countBonusSet;
                var sets = GetSets(currentOrder).ToList();

                do
                {
                    countCondition = 0;

                    foreach (var condition in bonusConditions)
                    {
                        var set = sets?.FirstOrDefault(x => x.Id == condition.SetId);

                        if (set is not null)
                        {
                            setConditions.Add(set);
                            sets?.Remove(set);
                            countCondition++;
                        }
                    }

                    countBonusSet = 0;

                    foreach (var bonusSet in bonusSets)
                    {
                        var set = sets?.FirstOrDefault(x => x.Id == bonusSet.SetId);

                        if (set is not null)
                        {
                            setBonus.Add(set);
                            sets?.Remove(set);
                            countBonusSet++;
                        }
                    }
                }
                while ((countCondition == bonusConditions.Count) && (countBonusSet == bonusSets.Count) && !(countCondition == 0 && countBonusSet == 0));

                while (countBonusSet > 0 && countBonusSet < bonusSets.Count)
                {
                    setBonus.Remove(setBonus[^1]);
                    countBonusSet--;
                }

                while (countCondition > 0 && countCondition < bonusConditions.Count)
                {
                    setConditions.Remove(setConditions[^1]);
                    countCondition--;
                }

                foreach (SeatBindableModel seat in currentOrder.Seats)
                {
                    foreach (SetBindableModel set in seat.Sets)
                    {
                        if ((setConditions.Count == 0 && bonusConditions.Count > 0) || !setConditions.Any(x => x.Id == set.Id))
                        {
                            if ((setBonus.Count == 0 && bonusSets.Count == 0) || setBonus.Any(x => x.Id == set.Id))
                            {
                                set.PriceBonus = GetPriceBonus(currentOrder.Bonus, set);
                                setBonus.Remove(set);
                            }
                            else
                            {
                                set.PriceBonus = set.Portion.Price;
                            }
                        }
                        else
                        {
                            set.PriceBonus = set.Portion.Price;
                            setConditions?.Remove(set);
                        }

                        currentOrder.PriceWithBonus += set.PriceBonus;
                    }

                    currentOrder.PriceTax = currentOrder.PriceWithBonus * currentOrder.Tax.Value;

                    currentOrder.Total = currentOrder.PriceWithBonus + currentOrder.PriceTax;
                }

                return currentOrder;
            }

            return currentOrder;
        }
    }
}
