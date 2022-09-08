using Next2.Enums;
using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using Next2.Models.API.Commands;
using Next2.Models.API.DTO;
using Next2.Models.Bindables;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Next2.Services.DishesHolding
{
    public interface IDishesHoldingService
    {
        Task<AOResult<IEnumerable<HoldItemModelDTO>>> GetHoldDishesAsync(Func<HoldItemModelDTO, bool>? condition = null);

        IEnumerable<HoldDishBindableModel> GetSortedHoldDishes(EHoldDishesSortingType typeSort, IEnumerable<HoldDishBindableModel> holdDihes);

        IEnumerable<HoldItemModelDTO> GetHoldDishesByTableNumber(int tableNumber);

        Task<AOResult> UpdateHoldItemsAsync(UpdateHoldItemsCommand holdItems);
    }
}
