﻿using Next2.Helpers.ProcessHelpers;
using Next2.Interfaces;
using Next2.Models.API.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Next2.Services.Bonuses
{
    public interface IBonusesService
    {
        Task<AOResult<IEnumerable<T>>> GetAllBonusesAsync<T>(Func<T, bool>? condition = null)
            where T : IBaseApiModel, new();

        Task<AOResult<CouponModelDTO>> GetCouponById(Guid id);

        //Task<List<BonusModel>> GetActiveBonusesAsync(FullOrderBindableModel currentOrder);

        //Task<IEnumerable<BonusModel>> GetActiveCouponesAsync(List<BonusModel> bonuses);

        //Task<IEnumerable<BonusModel>> GetActiveDiscountsAsync(List<BonusModel> bonuses);

        //Task<FullOrderBindableModel> СalculationBonusAsync(FullOrderBindableModel currentOrder, BonusBindableModel bonus);
    }
}
