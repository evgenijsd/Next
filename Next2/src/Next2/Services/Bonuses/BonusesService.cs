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

        public async Task<AOResult<IEnumerable<BonusConditionModel>>> GetConditionsAsync()
        {
            var result = new AOResult<IEnumerable<BonusConditionModel>>();

            try
            {
                var conditions = await _mockService.GetAsync<BonusConditionModel>(x => x.Id != 0);

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

        public async Task<AOResult<IEnumerable<BonusSetModel>>> GetBonusSetsAsync()
        {
            var result = new AOResult<IEnumerable<BonusSetModel>>();

            try
            {
                var bonusSet = await _mockService.GetAsync<BonusSetModel>(x => x.Id != 0);

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

        public ObservableCollection<BonusBindableModel> GetDiscounts(IEnumerable<BonusConditionModel> bonusConditions, ObservableCollection<BonusBindableModel> discounts, IEnumerable<SetBindableModel> sets)
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
    }
}
