﻿using AutoMapper;
using Next2.Enums;
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
        private readonly IMapper _mapper;

        public BonusesService(
            IMockService mockService,
            IMapper mapper)
        {
            _mockService = mockService;
            _mapper = mapper;
        }

        #region -- IBonusService implementation --

        public async Task<IEnumerable<BonusModel>> GetActiveCouponesAsync(List<BonusModel> bonuses)
        {
            IEnumerable<BonusModel> result = Enumerable.Empty<BonusModel>();

            IEnumerable<BonusConditionModel> bonusConditions = Enumerable.Empty<BonusConditionModel>();
            IEnumerable<BonusSetModel> bonusSets = Enumerable.Empty<BonusSetModel>();

            if (bonuses.Count > 0)
            {
                var resultConditions = await GetConditionsAsync();

                if (resultConditions.IsSuccess)
                {
                    bonusConditions = resultConditions.Result;
                }

                var resultBonusSets = await GetBonusSetsAsync();

                if (resultBonusSets.IsSuccess)
                {
                    bonusSets = resultBonusSets.Result;
                }

                result = bonuses.Where(x => !bonusConditions.Any(y => y.BonusId == x.Id));
            }

            return result;
        }

        public async Task<IEnumerable<BonusModel>> GetActiveDiscountsAsync(List<BonusModel> bonuses)
        {
            IEnumerable<BonusModel> result = Enumerable.Empty<BonusModel>();

            IEnumerable<BonusConditionModel> bonusConditions = Enumerable.Empty<BonusConditionModel>();
            IEnumerable<BonusSetModel> bonusSets = Enumerable.Empty<BonusSetModel>();

            if (bonuses.Count > 0)
            {
                var resultConditions = await GetConditionsAsync();

                if (resultConditions.IsSuccess)
                {
                    bonusConditions = resultConditions.Result;
                }

                var resultBonusSets = await GetBonusSetsAsync();

                if (resultBonusSets.IsSuccess)
                {
                    bonusSets = resultBonusSets.Result;
                }

                result = bonuses.Where(x => bonusConditions.Any(y => y.BonusId == x.Id));
            }

            return result;
        }

        public async Task<List<BonusModel>> GetActiveBonusesAsync(FullOrderBindableModel currentOrder)
        {
            List<BonusModel> result = new();

            IEnumerable<BonusConditionModel> bonusConditions = Enumerable.Empty<BonusConditionModel>();
            IEnumerable<BonusSetModel> bonusSets = Enumerable.Empty<BonusSetModel>();
            var resultBonuses = await GetBonusesAsync();

            if (resultBonuses.IsSuccess)
            {
                var resultConditions = await GetConditionsAsync();

                if (resultConditions.IsSuccess)
                {
                    bonusConditions = resultConditions.Result;
                }

                var resultBonusSets = await GetBonusSetsAsync();

                if (resultBonusSets.IsSuccess)
                {
                    bonusSets = resultBonusSets.Result;
                }

                foreach (var bonus in resultBonuses.Result)
                {
                    bool isBonus = true;
                    var sets = GetSets(currentOrder);
                    var conditions = bonusConditions.Where(x => x.BonusId == bonus.Id);
                    var setConditions = bonusSets.Where(x => x.BonusId == bonus.Id);

                    foreach (var condition in conditions)
                    {
                        var set = sets.FirstOrDefault(x => x.Id == condition.SetId);
                        if (set is null)
                        {
                            isBonus = false;
                        }
                        else
                        {
                            sets.Remove(set);
                        }
                    }

                    bool isSet = setConditions.Count() == 0;

                    foreach (var setCondition in setConditions)
                    {
                        var set = sets.FirstOrDefault(x => x.Id == setCondition.SetId);
                        if (set is not null)
                        {
                            isSet = true;
                            sets.Remove(set);
                        }
                    }

                    if (isBonus && isSet)
                    {
                        result.Add(bonus);
                    }
                }
            }

            return result;
        }

        public async Task<FullOrderBindableModel> СalculationBonusAsync(FullOrderBindableModel currentOrder)
        {
            if (currentOrder.BonusType != EBonusType.None)
            {
                currentOrder.PriceWithBonus = 0;

                IEnumerable<BonusConditionModel> bonusConditions = Enumerable.Empty<BonusConditionModel>();
                IEnumerable<BonusSetModel> bonusSets = Enumerable.Empty<BonusSetModel>();

                var resultConditions = await GetConditionsAsync();

                if (resultConditions.IsSuccess)
                {
                    bonusConditions = resultConditions.Result;
                }

                var resultBonusSets = await GetBonusSetsAsync();

                if (resultBonusSets.IsSuccess)
                {
                    bonusSets = resultBonusSets.Result;
                }

                var bonus = currentOrder.Bonus;
                var conditions = bonusConditions.Where(x => x.BonusId == bonus.Id);
                var setConditions = bonusSets.Where(x => x.BonusId == bonus.Id);
                bool isBonus = true;
                bool isSet = true;
                var sets = GetSets(currentOrder);
                List<SetModel> currentBonusSets = new();

                do
                {
                    isBonus = true;
                    int countSet = 0;

                    foreach (var condition in conditions)
                    {
                        var set = sets.FirstOrDefault(x => x.Id == condition.SetId);
                        if (set is not null)
                        {
                            isBonus = false;
                            sets.Remove(set);
                            countSet++;
                        }
                    }

                    isBonus = isBonus || countSet == conditions.Count();

                    if (isBonus)
                    {
                        isSet = true;

                        foreach (var setCondition in setConditions)
                        {
                            var set = sets.FirstOrDefault(x => x.Id == setCondition.SetId);
                            if (set is not null)
                            {
                                isSet = false;
                                currentBonusSets.Add(set);
                                sets.Remove(set);
                            }
                        }
                    }
                }
                while (!isBonus || !isSet);

                foreach (SeatBindableModel seat in currentOrder.Seats)
                {
                    foreach (SetBindableModel set in seat.Sets)
                    {
                        var currentBonusSet = currentBonusSets.FirstOrDefault(x => x.Id == set.Id);

                        if (setConditions.Count() == 0 || (currentBonusSet is not null))
                        {
                            set.PriceBonus = GetPriceBonus(currentOrder.Bonus, set);
                            currentBonusSets.Remove(currentBonusSet);
                        }
                        else
                        {
                            set.PriceBonus = set.Portion.Price;
                        }

                        currentOrder.PriceWithBonus += set.PriceBonus;
                    }

                    currentOrder.PriceTax = currentOrder.PriceWithBonus * currentOrder.Tax.Value;

                    currentOrder.Total = currentOrder.PriceWithBonus + currentOrder.PriceTax;
                }
            }

            return currentOrder;
        }

        #endregion

        #region -- Private helpers --

        private float GetPriceBonus(BonusBindableModel selectedBonus, SetBindableModel set)
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

        private List<SetModel> GetSets(FullOrderBindableModel currentOrder)
        {
            var result = new List<SetModel>();

            foreach (var seat in currentOrder.Seats)
            {
                foreach (var set in seat.Sets)
                {
                    result.Add(_mapper.Map<SetBindableModel, SetModel>(set));
                }
            }

            return result;
        }

        private async Task<AOResult<IEnumerable<BonusModel>>> GetBonusesAsync()
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

        private async Task<AOResult<IEnumerable<BonusConditionModel>>> GetConditionsAsync()
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

        private async Task<AOResult<IEnumerable<BonusSetModel>>> GetBonusSetsAsync()
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

        #endregion
    }
}